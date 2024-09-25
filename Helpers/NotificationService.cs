using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
namespace BCMCHOVR.Helpers
{
    public class NotificationService
    {
        public void SendCloudEmail(string recipient, string subject, string emailBody, string fromText)
        {
            SendCloudEmail(new List<string>() { recipient }, subject, emailBody, fromText);
        }
        public void SendCloudEmail(List<string> recipient, string subject, string emailBody, string fromText)
        {
            Task.Run(() =>
            {

                var value = new
                {
                    Recipient = recipient,
                    FromText = fromText,
                    Subject = subject,
                    EmailBody = emailBody
                };

                using var client = new HttpClient();
                client.BaseAddress = new Uri("http://apps.bcmch.org:99/send/");
                client.DefaultRequestHeaders.Authorization =
                           new System.Net.Http.Headers.AuthenticationHeaderValue("Authorization",
                           "eHBTZTVhRHFyNWhmZitGZlRnZnZpdz09");
                //HTTP POST                

                var postTask = client.PostAsJsonAsync("email", value);
                postTask.Wait();
                var result = postTask.Result;
                //if (result.IsSuccessStatusCode)
                // {

                // }

            });
        }

    }
}
