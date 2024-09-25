using BCMCHOVR.Helpers;
using BCMCHOVR.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCMCHOVR.APIController
{

    [ApiController]
    public class EmployeeRepo : ControllerBase
    {
        [Route("~/ovr/emp/search")]
        public IActionResult EmployeeSearch(IDictionary<string, string> value)
        {
            var username = User.Identity.Name;
            if (string.IsNullOrEmpty(username))
            {
                return NotFound("Request terminated");
            }

            string text = value["text"];
            using var dataClient = new DataClient("ESSPSearchEmployee"); // Need to Change this
            var result = dataClient
                    .AddParameter("VarChar", "@SearchText", text)
                    .ExecuteAsJsonDataGrid();

            var apiActionModal = new ApiActionModal<JsonDataGrid>()
            {
                Data = result
            };

            return Ok(apiActionModal);
        }
    }
}
