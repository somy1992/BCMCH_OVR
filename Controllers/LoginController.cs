using BCMCHOVR.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace BCMCHOVR.Controllers
{
    public class LoginController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LoginController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public ActionResult Index()
        {


            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Index(string txtLoginName, string txtPassword)
        {

            var userData = new Dictionary<string, string>
                {
                    { "username", txtLoginName },
                    { "password", txtPassword }
                };
            var authResult = ESSPAuthentication.Authenticate(userData).Result;
            if (authResult == null)
            {
                ViewBag.ErrorMessage = "Invalid UserName or Password";
                return View();
            }

            var profile = authResult["profile"];

            JObject jsonObject = JObject.Parse(JsonConvert.SerializeObject(profile));

            var employee = jsonObject["Employee"].ToString();


            var userIdentity = new UserIdentity()
                {
                    FullName = employee,
                    UserName = txtLoginName,
                    EmployeeCode = txtLoginName,
                    Role = ""
                };
                var principal = CookieAuthentication.GetIdentityClaims(userIdentity);
                await HttpContext.SignInAsync(principal);
                return Redirect("~/page");
            
        }
        [Route("/Site/UnAuthorised")]
        public ActionResult UnAuthorised()
        {
            return Content("Hello");
        }

        [Route("/test")]
        public ActionResult Test()
        {
            return View();
        }
    [Route("/logout")]
        public async Task<ActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

    }
}


/*
            var eMailBody = Shared.GetEmailTemplate("notificationTemplate.html");
            eMailBody = eMailBody.Replace("{BODY}", "Hello there");
            new NotificationService().SendCloudEmail("godly@bcmch.org", "Hi Godly", eMailBody, "Killer Home");
 */