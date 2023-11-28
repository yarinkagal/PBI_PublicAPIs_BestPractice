using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace PBI_PublicAPIs_BestPractice.API_Handlers
{
    public class WorkspaceInfoAPI_Handler : API_Handler
    {
        private int chunkMaxSize;
        private int nextIndexToCheck;
        private readonly object lockObject = new object();
        private string[] worspacesIds;

        public WorkspaceInfoAPI_Handler(string worspacesFilePath) : base("getInfo")
        {
            worspacesIds = File.ReadAllLines(worspacesFilePath);
            chunkMaxSize = (int)Configuration_Handler.Instance.getConfig(apiName, "chunkMaxSize");
            nextIndexToCheck = 0;

            JObject apiSettings = Configuration_Handler.Instance.getApiSettings(apiName);
            foreach (var property in apiSettings.Properties() )
            {
                if(property.Name != "chunkMaxSize")
                {
                    parameters.Add(property.Name, property.Value);
                }
            }

            if ((bool)parameters["datasetExpressions"] && (bool)parameters["datasetSchema"])
            {
                Console.WriteLine("datasetSchema can not set to false while datasetExpressions is set to true");
                throw new Exception($"Please check console - {apiName}");
            }
           
            setParameters();
        }

        public override async Task<object> run()
        {
            int start;
            int length;
            lock (lockObject)
            {
                start = nextIndexToCheck;
                length =  Math.Min(start + chunkMaxSize, worspacesIds.Length)-start;
                nextIndexToCheck = start + length ;
            }
            string[] workspacesToScan = worspacesIds.Skip(start).Take(length).ToArray();

            if(workspacesToScan.Length == 0)
            {
                return null;
            }

            using (HttpClient httpClient = new HttpClient())
            {
                var requestBody = new
                {
                    workspaces = workspacesToScan
                };

                string requestJsonString = JsonConvert.SerializeObject(requestBody);
                HttpContent content = new StringContent(requestJsonString, Encoding.UTF8, "application/json");

                string accessToken = Auth_Handler.Instance.accessToken;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                HttpResponseMessage response = await httpClient.PostAsync(apiUriBuilder.Uri, content);

                verifySuccess(response);

                if (response.Content != null)
                {
                    var scanDetailsString = await response.Content.ReadAsStringAsync();

                    JObject scanDetails = JObject.Parse(scanDetailsString);
                    JToken token = scanDetails["id"];

                    string scanId = token.Value<string>();

                    return scanId;

                }
                return null;
            }
        }
    }
}
