using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BCMCHOVR.Helpers
{
    public class CookieIdentity
    {
    }
    public class CookieAuthentication 
    {
        public static ClaimsPrincipal GetIdentityClaims(UserIdentity identity)
        {
            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, identity.UserName),
                        new Claim(ClaimTypes.Role, identity.Role),
                        new Claim(ClaimTypes.NameIdentifier, identity.FullName),
                        new Claim(ClaimTypes.UserData,identity.EmployeeCode)

                    };
            var userIdentity = new ClaimsIdentity(claims, "bc-ovr-login");
            var principal = new ClaimsPrincipal(userIdentity);
            return principal;
        }
    }

    public class UserIdentity
    {
        public bool IsValidate { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public string EmployeeCode { get; set; }
        public UserIdentity()
        {
            IsValidate = false;
        }
    }
}
