using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BCMCHOVR.Helpers
{
  
        public class Authentication
        {
            public static ClaimsPrincipal GetIdentityClaims(UserIdentity identity)
            {
                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, identity.UserName),
                        new Claim(ClaimTypes.Role, identity.Role),
                        new Claim(ClaimTypes.NameIdentifier, identity.FullName)
                    };
                var userIdentity = new ClaimsIdentity(claims, "bc-login");
                var principal = new ClaimsPrincipal(userIdentity);
                return principal;
            }
        }

        public class UserAuthentication 
        {
            public string EmployeeCode { get; set; }
            public bool IsValidate { get; set; }
            public string FullName { get; set; }
            public string Role { get; set; }
            public string UserName { get; set; }
            public string Message { get; set; }

            public UserAuthentication()
            {
                IsValidate = false;
            }

            public static UserAuthentication ValidateEmployee(string userName, string password, string scope)
            {
            string encryptedPassword = Helpers.Cryptography.MD5Cryptography.Encrypt(password, true);

            using var dataClient = new DataClient("UserAuthentication");
            var result = 
            dataClient.AddParameter("VarChar", "@UserName", userName)
            .AddParameter("VarChar", "@Password", encryptedPassword)
            .ExecuteQuery();

            if (result == null || string.IsNullOrEmpty(result))
            {
                return new UserAuthentication()
                {
                    IsValidate = false,
                    Message = "Wrong password"
                };
            }
            var user = new UserAuthentication()
            {
                IsValidate = true,
                EmployeeCode = result,
                UserName = userName,
                FullName = userName,
                Role = "EMP"
            };
            return user;
        }
    }   
}
