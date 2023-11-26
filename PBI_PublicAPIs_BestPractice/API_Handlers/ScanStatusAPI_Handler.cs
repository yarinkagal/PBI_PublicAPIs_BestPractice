using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBI_PublicAPIs_BestPractice.API_Handlers
{
    public class ScanStatusAPI_Handler : API_Handler
    {
        public ScanStatusAPI_Handler(string scanId) : base("scanStatus")
        {
            apiUriBuilder.Path += $"/{scanId}";
        }

        public override async Task<object> run()
        {
            HttpResponseMessage response = await sendGetRequest();

            string jsonResponse = await response.Content.ReadAsStringAsync();

            JObject statusObject = JObject.Parse(jsonResponse);

            string statusValue = (string)statusObject["status"];

            return statusValue;
        }
    }
}
