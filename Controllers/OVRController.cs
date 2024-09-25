using BCMCHOVR.Helpers;
using BCMCHOVR.Models;
using BCMCHOVR.Models.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;

namespace BCMCHOVR.Controllers
{

    public class OVRController : Controller
    {
        private readonly IConfiguration _configuration;

        public OVRController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private string userName { get; set; }
        private string employeeCode { get; set; }
        private string fullName { get; set; }


       
        private void GetUserInfo()
        {
            userName = User.Identity.Name;
            employeeCode = User.Claims.FirstOrDefault(
               c => c.Type == ClaimTypes.UserData)?.Value;
            //fullName = User.Claims.FirstOrDefault(
            //    c => c.Type == ClaimTypes.Name)?.Value;

        }

        [Route("~/page")]
        public IActionResult Index()
        {
            GetUserInfo();

            var ovrModal = new OVRViewModal(userName)
            {

            };

            return View(ovrModal);
        }

        [Route("~/ovrs")]
        public IActionResult ShowAllOvrs()
        {
            var name = User.Identity.Name;

            using var dataClient = new DataClient("SelectOVRReports"); // Need to Change this
            var result = dataClient
                    .ExecuteAsTable();

            var ovrModal = new OVRViewModal(name)
            {
                OvrDetails = result
            };

            return View(ovrModal);
        }

        [Route("~/ovr/view/{ovrid}")]
        public IActionResult OvrView(string ovrid)
        {
            var username = User.Identity.Name;
            int _ovrId=0;
            if (string.IsNullOrEmpty(username))
            {
                return NotFound("Request terminated");
            }

           
            using var dataClient = new DataClient("SelectOvrDetails"); 
            var result = dataClient
                    .AddParameter("VarChar", "@UserId", username)
                    .AddParameter("VarChar", "@UniqueKey", ovrid)
                    .ExecuteAsTableSet();

            if(result.Tables.Count>0 && result.Tables[0].Rows.Count>0)
            {
                var value = result.Tables[0].Rows[0][0];
                _ovrId = Convert.ToInt32(value);
            }
            else
            {
                return NotFound();
            }

            var ovrModal = new OVRViewModal(username)
            {
                OvrId = _ovrId
            };

            if (result != null)
            {
                foreach (System.Data.DataTable item in result.Tables)
                {
                    if (item.Rows.Count == 0)
                    {
                        continue;
                    }
                    var columnName = item.Columns[0].ColumnName;
                    if (columnName == "id")
                    {
                        ovrModal.OvrDetails = item;
                    }
                    else if (columnName == "EmpCode")
                    {
                        ovrModal.EmployeeDetails = item;
                    }
                    else if (columnName == "Employee")
                    {
                        ovrModal.WitnessDetails = item;
                    }
                }
                string nextAction =
                    ovrModal.OvrDetails.Rows[0]["NextActionBy"].ToString();
                // ovrModal.NextActionBy = new UserModal().Get(nextAction);

                if (Convert.ToInt16(nextAction) > 0)
                {
                    ovrModal.EnableActionPanel = true;
                    ovrModal.ActionRequest = GetActionRequest(_ovrId, username);
                }
                ovrModal.ActionDetails = GetActionDetails(_ovrId);
                ovrModal.Files = GetAttachments(_ovrId, "evidence");
                ovrModal.OVRCategory = GetCategory(_ovrId);
            }



            return View(ovrModal);
        }
        [Route("~/ovr/getlist")]
        public IActionResult OVRList()
        {
            var username = User.Identity.Name;
            if (string.IsNullOrEmpty(username))
            {
                return NotFound("Request terminated");
            }


            using var dataClient = new DataClient("SelectOvrDetailsByUser"); // Need to Change this
            var result = dataClient
                    .AddParameter("VarChar", "@UserId", username)
                    .ExecuteAsTableSet();
            var modal = new OVRListViewModal();
            foreach (System.Data.DataTable item in result.Tables)
            {
                if (item.Rows.Count == 0)
                {
                    continue;
                }
                var columnName = item.Columns[0].ColumnName;
                if (columnName == "id")
                {
                    modal.MyRequests = item;
                }
                else if (columnName == "Action")
                {
                    modal.PendingActions = item;
                }

            }


            return PartialView("_OverListView", modal);
        }
        
        [Route("/newovr")]
        public ActionResult NewOvr()
        {
            var pageModel = new NewOvrPageModel();
            pageModel.GetCategory();
            return View(pageModel);
        }

        #region "Privates"
        private static System.Data.DataTable GetActionDetails(int ovrId)
        {
            using var dataClient = new DataClient("SELECTOVRActionAuthority"); // Need to Change this
            var result = dataClient
                    // .AddParameter("VarChar", "@UserId", username)
                    .AddParameter("BigInt", "@OvrID", ovrId)
                    .ExecuteAsTable();
            return result;
        }
        private static System.Data.DataTable GetActionRequest(int ovrId, string username)
        {
            using var dataClient = new DataClient("SelectOVRActionByRequest"); // Need to Change this
            var result = dataClient
                    .AddParameter("VarChar", "@UserId", username)
                    .AddParameter("BigInt", "@OvrID", ovrId)
                    .ExecuteAsTable();
            return result;
        }
        private static List<OVRFiles> GetAttachments(int ovrId, string fileType)
        {
            using var dataClient = new DataClient("OVRFilesSelect"); // Need to Change this
            var result = dataClient
                    .AddParameter("BigInt", "@OvrID", ovrId)
                    //.AddParameter("VarChar", "@ReferenceID", ovrId)
                    .ExecuteAsTable();
            var list = new List<OVRFiles>();

            foreach(DataRow row in result.Rows)
            {
                list.Add(new OVRFiles()
                {
                    FileName = row[0].ToString(),
                    FilePath = row[1].ToString(),
                    FileType = row[2].ToString(),
                });
            }

            return list;
        }
   
        private static List<string> GetCategory(int ovrId)
        {
            using var dataClient = new DataClient("OVRRequestCategorySelect"); // Need to Change this
            var result = dataClient
                    .AddParameter("BigInt", "@OvrID", ovrId)
                    .ExecuteAsTable();
            var list = new List<string>();
            foreach (DataRow row in result.Rows)
            {
                list.Add(row[0].ToString());
            }
            return list;
        }
        #endregion
    }
}
