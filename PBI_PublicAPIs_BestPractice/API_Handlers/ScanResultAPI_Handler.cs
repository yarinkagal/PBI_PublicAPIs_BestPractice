using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace PBI_PublicAPIs_BestPractice.API_Handlers
{
    public class ScanResultAPI_Handler : API_Handler
    {
        
        public string baseOutputFolder;
        public string scanId;

        public ScanResultAPI_Handler(string scanId) : base("scanResult")
        {
            this.scanId = scanId;
            apiUriBuilder.Path += $"/{scanId}";

            baseOutputFolder = (string)Configuration_Handler.Instance.getConfig(apiName, "baseOutputFolder");
            if (!Directory.Exists(baseOutputFolder))
            {
                Directory.CreateDirectory(baseOutputFolder);
            }
        }

        public override async Task<object> run()
        {

            HttpResponseMessage response = await sendGetRequest();

            string jsonResponse = await response.Content.ReadAsStringAsync();
            JObject resultObject = JObject.Parse(jsonResponse);
            JArray workspacesArray = (JArray)resultObject["workspaces"];

            // Now you can work with the "workspaces" array
            foreach (var workspace in workspacesArray)
            {
                string outputFolder = $"{baseOutputFolder}\\{workspace["id"]}";

                if (!Directory.Exists(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                }

                DateTime currentTime = DateTime.Now;
                string currentTimeString = currentTime.ToString("yyyy-MM-dd-HHmmss");
                string outputFilePath = $"{outputFolder}\\{scanId}_{currentTimeString}.json";

                string workspaceJson = JsonConvert.SerializeObject(workspace, Formatting.Indented);
                File.WriteAllText(outputFilePath, workspaceJson);
            }
            return true;
        }


    }
}
