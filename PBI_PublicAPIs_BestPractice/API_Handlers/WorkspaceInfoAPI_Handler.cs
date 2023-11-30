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
            chunkMaxSize = Configuration_Handler.Instance.getConfig(apiName, "chunkMaxSize").Value<int>();
            nextIndexToCheck = 0;

            JObject apiSettings = Configuration_Handler.Instance.getApiSettings(apiName);
            foreach (var property in apiSettings.Properties() )
            {
                if(property.Name != "chunkMaxSize")
                {
                    parameters.Add(property.Name, property.Value);
                }
            }
           
            setParameters();
        }

        public override async Task<object> run()
        {
            int start;
            int length;
            string[] workspacesToScan;
            lock (lockObject)
            {
                start = nextIndexToCheck;
                length =  Math.Min(start + chunkMaxSize, worspacesIds.Length)-start;
                nextIndexToCheck = start + length ;
                //Console.WriteLine($"Start: {start}");
                //Console.WriteLine($"End: {start+length-1}");
                workspacesToScan = worspacesIds.Skip(start).Take(length).ToArray();
            }

            if (workspacesToScan.Length == 0)
            {
                return "Done";
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
                HttpResponseMessage response;
                try
                {
                    response = await httpClient.PostAsync(apiUriBuilder.Uri, content);
                }
                catch (Exception ex)
                {
                    throw new ScannerAPIException(apiName, "Can't send request");
                    
                }
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
