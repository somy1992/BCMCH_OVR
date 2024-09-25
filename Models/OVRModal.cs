using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BCMCHOVR.Models
{
    public class OVRViewModal
    {
        public List<SelectListItem> CategoryList { get; set; }
        public OVRViewModal()
        {

        }
        public OVRViewModal(string userName)
        {
            LoggedUser = userName;
            User = new UserModal().Get(userName);
        }
        public UserModal User { get; set; }
        public int OvrId { get; set; }
        public DataTable OvrDetails { get; set; }
        public DataTable WitnessDetails { get; set; }
        public DataTable ActionDetails { get; set; }
        public DataTable ActionRequest { get; set; }
        public DataTable EmployeeDetails { get; set; }
        public string OvrViewScope { get; set; }
        public string LoggedUser { get; set; }
        public bool EnableActionPanel { get; set; }
        public List<UserModal> Witness { get; set; }
        public UserModal NextActionBy { get; set; }
        public List<string> OVRCategory {get;set; }
        public int OVRCartegoryCode { get; set; }
        public string IncidentType { get; set; }
        public List<OVRFiles> Files { get; set; }
    }

    public class OVRListViewModal
    {
     
        public DataTable MyRequests { get; set; }
        public DataTable PendingActions { get; set; } 
        public DataTable MyActions { get; set; }
      


    }
    public class OVRCategoryModal
    { 
        public string Header { get; set; }
        public Dictionary<string, string> Values { get; set; }
    }
    public class SelectCategory
    {
        public int CatID { get; set; }
        public string Category { get; set; }
        public Dictionary<string, string> Values { get; set; }


        //public static DataTable GetDepartments(string typeId)
        //{
        //    string sql = "SELECT Id, Name from Departments where Active = 1 order by Name";
        //    var db = new SqlDbHelper();
        //    return db.ExecuteAsDataTable(string.Format(sql, typeId));
        //}
    }



    public class OVRFiles
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
    }
}
