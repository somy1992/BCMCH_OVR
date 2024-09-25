using BCMCHOVR.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCMCHOVR.Models
{
    public class UserModal
    {
        public string EmpCode { get; set; }
        public string EmpID { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public bool Admin { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string EmployeeName { get; set; }
         
        public UserModal Get(string userName) 
        {
            using var dataClient = new DataClient("SelectUserDetails"); // Need to Change this
            var result = dataClient
                    .AddParameter("VarChar", "@UserID", userName)
                    .ExecuteAsTable();
            if (result != null && result.Rows.Count > 0)
            {
                var item = result.Rows[0];
                EmpID = item["EmpCode"].ToString();
                EmpCode = item["EmpId"].ToString();
                UserName = item["EmployeeName"].ToString();
                EmployeeName = item["EmployeeName"].ToString();
                Department = item["Department"].ToString();
                Designation = item["Designation"].ToString();

                Role = item["Role"].ToString();

                if (Role == "QLT" || Role == "MSO")
                {
                    Admin = true;
                }

                return this;
            }
            return null;
        }
    }
}
