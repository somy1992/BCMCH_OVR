using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;

namespace BCMCHOVR.Helpers
{
    public class ESSPAuthentication
    {
        public static async Task<Dictionary<string, object>> Authenticate(IDictionary<string, string> value)
        {
            try
            {
                string myJson = JsonConvert.SerializeObject(value);
                using (var client = new HttpClient())
                {
                    var content = new StringContent(myJson, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("https://sso.bcmch.org:4430/validateusr", content);
                    var contents = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var dicResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(contents);
                        string status = dicResponse["status"].ToString();
                        if (!Convert.ToBoolean(status))
                        {
                            return null;
                        }
                        return dicResponse;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
