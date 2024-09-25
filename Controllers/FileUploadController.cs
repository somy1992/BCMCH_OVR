using BCMCHOVR.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;

namespace BCMCHOVR.Controllers
{

    public class FileUploadController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FileUploadController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        [Route("/file/{path}")]
        public FileStreamResult Download(string path)
        {
            string filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Files", path);
            var stream = System.IO.File.OpenRead(filePath);

            System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
            {
                FileName = path,
                Inline = true  // false = prompt the user for downloading;  true = browser to try to show the file inline
            };
            Response.Headers.Add("Content-Disposition", cd.ToString());
            Response.Headers.Add("X-Content-Type-Options", "nosniff");
            // return new FileStreamResult(stream, "application/octet-stream");

            return File(stream, "image/jpeg");
        }

        [HttpPost]
        [Route("/ovr/savfiles")]
        public async Task<ActionResult> UploadFiles(IList<IFormFile> files)
        {
            string authorization = HttpContext.Request.Headers["Authorization"];
            var authorizationKey = Cryptography.MD5Cryptography.Decrypt(authorization, true);
            var dicJsonValues = JsonSerializer.Deserialize<Dictionary<string,string>>(authorizationKey);
             
            var username = User.Identity.Name;
            if(username == null)
            {
                return BadRequest();
            }

            string ovrID = dicJsonValues["id"];
            string type = dicJsonValues["type"];
            string user = dicJsonValues["user"];
            var res = dicJsonValues.TryGetValue("reference", out string reference);

            if(string.IsNullOrEmpty(reference))
            {
                reference = "";
            }



            var dfileList = new Dictionary<string, string>();
            foreach (IFormFile source in files)
            {
                string filename = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.Trim('"');
                filename = EnsureCorrectFilename(filename);
                string actualFileName = Path.GetFileName(filename);
                string filePathNname = this.GetPathAndFilename(filename);
                string actualFileNewName = Path.GetFileName(filePathNname);

                using FileStream output =
                    System.IO.File.Create(filePathNname);
                await source.CopyToAsync(output);
                dfileList.Add(actualFileNewName,actualFileName);
            }

            foreach (var item in dfileList)
            {

                using var dataClient = new DataClient("OVRFilesInsert");
                var result = dataClient
                            .AddParameter("VarChar", "@OvrID", ovrID)
                            .AddParameter("VarChar", "@FileName", item.Value)
                            .AddParameter("VarChar", "@FilePath", item.Key)
                            .AddParameter("VarChar", "@FileType", type)
                            .AddParameter("VarChar", "@ReferenceID", reference)
                            .AddParameter("VarChar", "@UserName", user)
                        .ExecuteNonQuery();
            }
            return Json("Done");
        }
        private static string EnsureCorrectFilename(string filename)
        {
            if (filename.Contains("\\"))
                filename = filename[(filename.LastIndexOf("\\") + 1)..];

            return filename;
        }

        private string GetPathAndFilename(string filename)
        {
            var extension = Path.GetExtension(filename);

            filename = (System.Guid.NewGuid().ToString()).Replace("-", "").ToUpper();
            filename += extension;
            string filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Files", filename);
            return filePath;
        }
    }
}
