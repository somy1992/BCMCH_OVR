using BCMCHOVR.Helpers;
using BCMCHOVR.Models;
using BCMCHOVR.Models.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;

namespace BCMCHOVR.APIController
{

    [ApiController]
    public class OVR : ControllerBase
    {
        private readonly string _connectionString;
        public OVR(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        [Route("~/ovr/request/save")]
        public IActionResult NewOVRRequest(OVRRequest request)
        {
            var username = User.Identity.Name;
            if (string.IsNullOrEmpty(username))
            {
                return NotFound("Request terminated");
            }

            var details = request.Details;
            var witness = JsonSerializer.Serialize(request.Witness);
            string ovrSubject = details["subject"];
            string ovrLocation = details["location"];
            string ovrDescription = details["description"];
            string ImmediateCorrection = details["ImmediateCorrection"];
            string userclassification = details["userclassification"];
            string Subcategory = details["Subcategory"];
            string ovrpatientinfo = details["patintcomplaint"];

            string dateString = details["date"];
            string timeString = details["time"];

            if (!DateTime.TryParse(dateString, out DateTime eventDate))
            {
                return BadRequest("Invalid date format.");
            }

            if (!TimeSpan.TryParse(timeString, out TimeSpan eventTime))
            {
                return BadRequest("Invalid time format.");
            }

            try
            {
                using var dataClient = new DataClient("INSERTOVRDETAILS");
                var result = dataClient
                    .AddParameter("VarChar", "@OVRSubject", ovrSubject)
                    .AddParameter("VarChar", "@EventLocation", ovrLocation)
                    .AddParameter("Date", "@EventDate", eventDate)
                    .AddParameter("Time", "@EventTime", eventTime)
                    .AddParameter("VarChar", "@DescriptionOfTheEvent", ovrDescription)
                    .AddParameter("VarChar", "@ImmediateCorrection", ImmediateCorrection)
                    .AddParameter("VarChar", "@UserCategory", userclassification)
                    .AddParameter("VarChar", "@Subcategory", Subcategory)
                    .AddParameter("VarChar", "@PatientInfo", ovrpatientinfo)
                    .AddParameter("VarChar", "@UserId", username)
                    .ExecuteQuery();

                if (result != null)
                {

                    Dictionary<string, string> encKey = new()
                {
                    { "id", result },
                    { "type", "OVREVENT" },
                    { "user", username },
                    { "reference", "" }
                };

                    string jsonEnc = JsonSerializer.Serialize(encKey);

                    var apiActionModal = new ApiActionModal<string>()
                    {
                        Status = !string.IsNullOrEmpty(result),
                        Data = result,
                        TKey = Cryptography.MD5Cryptography.Encrypt(jsonEnc, true),
                        Message = "Request submitted successfully"
                    };

                    SendOvrNotification(Convert.ToInt64(result));

                    return Ok(apiActionModal);
                }
                else
                {
                    return StatusCode(500, "Failed to submit OVR request");
                }
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Route("/ovr/categories")]
        public IActionResult GetCategories()
        {
            var categories = new List<Dictionary<string, object>>();

            using (var dataClient = new DataClient("DbCategory_Select"))
            {
                var result = dataClient.ExecuteAsTable();

                if (result == null || result.Rows.Count == 0)
                {
                    return NotFound("No categories found");
                }

                foreach (DataRow row in result.Rows)
                {
                    var categoryDict = new Dictionary<string, object>
            {
                { "CatID", Convert.ToInt32(row["CatID"].ToString()) },
                { "Category", row["Category"].ToString() }
            };
                    categories.Add(categoryDict);
                }
            }

            
           

            return Ok(categories);
        }

        [Route("/ovr/subcategories")]
        public IActionResult GetSubCategories([FromBody] IDictionary<string, string> values)
        {

            int catId = Convert.ToInt32(values["CatID"]);
            var subcategories = new List<Dictionary<string, object>>();

            using (var dataClient = new DataClient("Category_Select"))
            {
                dataClient.AddParameter("Int", "@CatID", catId);

                var result = dataClient.ExecuteAsTable();

                if (result == null || result.Rows.Count == 0)
                {
                    return NotFound("No subcategories found for the selected category");
                }

                foreach (DataRow row in result.Rows)
                {
                    var subcategoryDict = new Dictionary<string, object>
            {
                { "CatID", Convert.ToInt32(row["SubID"]) },
                { "Subcategory", row["Subcategory"].ToString() }
            };
                    subcategories.Add(subcategoryDict);

                }
            }

            return Ok(subcategories);
        }


        [Route("~/ovr/action/witnesscomnt")]
        public IActionResult UpdateWitnessComment(IDictionary<string, string> values)
        {
            var username = User.Identity.Name;
            if (string.IsNullOrEmpty(username))
            {
                return NotFound("Request terminated");
            }

            string ovrId = values["ovrid"];
            string comment = values["comment"];

            using var dataClient = new DataClient("UpdateWitnessAction"); // Need to Change this
            var result = dataClient
                    .AddParameter("VarChar", "@Comment", comment)
                    .AddParameter("VarChar", "@UserID", username)
                    .AddParameter("BigInt", "@OVRID", ovrId)
                    .ExecuteNonQuery();

            string status = result > 0 ? "updated" : "failed";

            var apiActionModal = new ApiActionModal<string>()
            {
                Data = status
            };

            return Ok(apiActionModal);
        }

        [Route("~/ovr/category/get")]
        public IActionResult GetOVRCategoryWithOptions()
        {
            var username = User.Identity.Name;
            if (string.IsNullOrEmpty(username))
            {
                return NotFound("Request terminated");
            }


            using var dataClient = new DataClient("SelectOVRCategoryWithOptions"); // Need to Change this
            var result = dataClient.ExecuteAsTable();

            var value = new List<OVRCategoryModal>();

            DataView view = new(result);
            DataTable distinctValues = view.ToTable(true, "TypeID", "TypeText");

            foreach (DataRow row in distinctValues.Rows)
            {
                var id = row[0].ToString();
                var oName = row[1].ToString();
                var dic = new Dictionary<string, string>();
                foreach (DataRow _r in result.Select("TypeID  =" + id))
                {
                    dic.Add(_r["OptionID"].ToString(),
                        _r["OptionText"].ToString());
                }

                value.Add(new OVRCategoryModal()
                {
                    Header = oName,
                    Values = dic
                });
            }

            var apiActionModal = new ApiActionModal<List<OVRCategoryModal>>()
            {
                Data = value
            };

            return Ok(apiActionModal);
        }

      
       


        [Route("~/ovr/actionr/request")]
        public IActionResult OVRActionAuthoritySave(OVRRequest request)
        {
            var username = User.Identity.Name;
            if (string.IsNullOrEmpty(username))
            {
                return NotFound("Request terminated");
            }
            var details = request.Details;
            string ovrID = details["ovrid"];
            string authorityId = details["auth"];
            string remark = details["comnt"];

            var witness = JsonSerializer.Serialize(request.Witness);

            using var dataClient = new DataClient("OVRActionAuthorityInsert"); // Need to Change this
            var result = dataClient
                    .AddParameter("BigInt", "@OvrID", ovrID)
                    .AddParameter("VarChar", "@AuthorityId", authorityId)
                    .AddParameter("VarChar", "@ActionType", witness)
                    .AddParameter("VarChar", "@Remarks", remark)
                    .AddParameter("VarChar", "@UserID", username)
                    .ExecuteNonQuery();
            if (result > 0)
            {
                var apiActionModal = new ApiActionModal<string>()
                {
                    Data = "saved"
                };
                return Ok(apiActionModal);
            }
            else
            {
                var apiActionModal = new ApiActionModal<string>()
                {
                    Status = false,
                    Message = "Error"
                };
                return Ok(apiActionModal);
            }

        }

        [Route("~/ovr/actionp/updtc")]
        [HttpPost]
        public IActionResult OVRActionUpateType(Dictionary<string, string> value)
        {
            var username = User.Identity.Name;
            if (string.IsNullOrEmpty(username))
            {
                return NotFound("Request terminated");
            }
           
            bool result=false;
            if (value["type"] == "T")
            {
                string sql = "";
                var listp = new Dictionary<string, string>();
                sql = "update OVRDetails set IncidentType=@Value where id=@OvrId";
                listp.Add("@Value", value["value"]);
                listp.Add("@OvrId", value["id"]);
                result = UpdateTable(sql, listp);


            }
            else if (value["type"] == "C")
            {

                using var sqlCommand = new DataClient().GetCommand("OVRRequestCategoryInsert");
                sqlCommand.Parameters.AddWithValue("@OVRId", value["id"]);
                var catValue = "[" + value["value"] + "]";
                sqlCommand.Parameters.AddWithValue("@CategoryId", catValue);
                sqlCommand.Parameters.AddWithValue("@UserId", username);
                sqlCommand.Connection.Open();
                var response = sqlCommand.ExecuteNonQuery();
                result = response > 0;
            }

            var apiActionModal = new ApiActionModal<bool>()
            {
                Status = result,
                Message = result ? "Success" : "Error"
            };
            return Ok(apiActionModal);

        }


        [Route("~/ovr/actionr/comnt")]
        [HttpPost]
        public IActionResult OVRActionUpateRequestComment(Dictionary<string, string> value)
        {
            var username = User.Identity.Name;
            if (string.IsNullOrEmpty(username))
            {
                return NotFound("Request terminated");
            }

            var listp = new Dictionary<string, string>();
            var ovrId = value["ovrid"];
            var type = value["type"];
            string sql =
            "update OVRActionAuthority set ActionDate = GETDATE(), " +
            "ActionComment= @Comment" +
            " where ID = @ID and AuthorityId = @EmpCode";

            listp.Add("@Comment", value["comment"]);
            listp.Add("@ID", value["id"]);
            listp.Add("@EmpCode", username);
            var response = UpdateTable(sql, listp);


            var rettype = SelectSingle("select ActionType from OVRActionAuthority where id = @ID",
                 new Dictionary<string, string>()
                 {
                     {"@Id",value["id"]}

                 });

            Dictionary<string, string> encKey = new();
            encKey.Add("id", ovrId);
            encKey.Add("type", rettype);
            encKey.Add("user", username);
            encKey.Add("reference", value["id"]);


            string jsonEnc = JsonSerializer.Serialize(encKey);

            var apiActionModal = new ApiActionModal<string>()
            {
                Data = response ? "success" :"error",
                TKey = Cryptography.MD5Cryptography.Encrypt(jsonEnc, true)
            };
            return Ok(apiActionModal);

        }


        [Route("~/ovr/get/test/{id}")]
        [HttpGet]
        public IActionResult OVRTest(int id)
        {
            SendOvrNotification(id);
            return Ok("Done");
        }

        private static bool UpdateTable(string sql, Dictionary<string, string> values)
        {
            using var sqlCommand = new DataClient().GetCommand(sql, true);

            foreach (var item in values)
            {
                sqlCommand.Parameters.AddWithValue(item.Key, item.Value);
            }

            sqlCommand.Connection.Open();
            var reuslt = sqlCommand.ExecuteNonQuery();
            return reuslt > 0;
        }
        private static string SelectSingle(string sql, Dictionary<string, string> values)
        {
            using var sqlCommand = new DataClient().GetCommand(sql, true);

            foreach (var item in values)
            {
                sqlCommand.Parameters.AddWithValue(item.Key, item.Value);
            }
             
            sqlCommand.Connection.Open();
            var reuslt = sqlCommand.ExecuteScalar();
            return reuslt.ToString();
        }


        private static void SendOvrNotification(long id)
        {
            return;
            Task.Run(() =>
            {

                using var dc = new DataClient("SELECTOVREMAILCONTENT");
                var data = dc.AddParameter("BigInt", "@OVRID", id).ExecuteAsReader();
                var list = data.ToStringList();

                using var dc1 = new DataClient("SELECTOVRADMINEMAILS");
                var data1 = dc1.ExecuteAsTable();
                List<string> emailList = new List<string>();
                foreach(System.Data.DataRow item in data1.Rows)
                {
                    var email = item[1].ToString();
                    if (email.Length > 3)
                    {
                        emailList.Add(email);
                    }
                }

                var eMailBody = Shared.GetEmailTemplate("notificationTemplate.html");
                eMailBody = eMailBody.Replace("{BODY}", list.ToHtml(true));


                //new NotificationService().SendCloudEmail
                //(
                //    emailList,
                //    "New OVR Registered",
                //    eMailBody,
                //    "BCMCH OVR - New "
                //);
                
            });
        }


    }
}
