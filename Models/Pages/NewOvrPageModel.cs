using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BCMCHOVR.Models.Pages
{
    public class NewOvrPageModel
    {
        public List<OvrCategory> Categories { get; set; } 

        public void GetCategory()
        {

        }
//        {
//<<<<<<< HEAD
//            using SqlConnection sqlConnection = new("");
//=======
//            using SqlConnection sqlConnection = new(Connections.SqlConnection);
//>>>>>>> 76f9b13784c14827e9cca90f0f4b7d3b7c70518f
//            using SqlCommand sqlCommand = new("SelectOVRCategoryWithOptions", sqlConnection);            
//            var sqlDap = new SqlDataAdapter()
//            { 
//            SelectCommand = sqlCommand
//            };
//            var dSet = new DataSet();
//            sqlDap.Fill(dSet);
//            if(dSet.Tables.Count> 0)
//            {
//                Categories = new List<OvrCategory>();
//                DataView view = new(dSet.Tables[0]);
//                DataTable disTable = view.ToTable(true, "TypeID", "TypeText");
//                foreach (DataRow row in disTable.Rows)
//                {
//                    var category = new OvrCategory()
//                    {
//                        CategoryName = row["TypeText"].ToString(),
//                        CategoryOptions = new List<OvrCategoryOptions>()
//                    };
//                    var catTypeId = row[0].ToString();
//                    DataRow[] rowsFiltered = dSet.Tables[0].Select("TypeID=" + catTypeId);
//                    foreach(DataRow irow in rowsFiltered)
//                    {
//                        var options = new OvrCategoryOptions()
//                        {
//                            OptionID = Convert.ToInt32(irow["OptionID"]),
//                            OptionText = irow["OptionText"].ToString()
//                        };
//                        category.CategoryOptions.Add(options);
//                    }
//                    Categories.Add(category);
//                }

//            }
//        }

    }

    public class OvrCategory
    {
        public string CategoryName { get; set; }
        public List<OvrCategoryOptions> CategoryOptions { get; set; }
    }
    public class OvrCategoryOptions
    {
        public string OptionText { get; set; }
        public int OptionID { get; set; }

    }

}
