using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace PBI_PublicAPIs_BestPractice.API_Handlers
{
    public abstract class API_Handler
    {
        public string apiName { get; set; }
        public UriBuilder apiUriBuilder { get; set; }
        public JObject parameters { get; set; }

        public API_Handler(string apiName)
        {
            this.apiName = apiName;
            parameters = new JObject();
            apiUriBuilder = new UriBuilder($"https://api.powerbi.com/v1.0/myorg/admin/workspaces/{apiName}");
        }
        
        public abstract Task<object> run();

        public async Task<HttpResponseMessage> sendGetRequest()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                // Set the authorization header with the token
                string accessToken = Auth_Handler.Instance.accessToken;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Send the GET request
                HttpResponseMessage response = await httpClient.GetAsync(apiUriBuilder.Uri);

                verifySuccess(response);
                return response;
            }
        }

        public void setParameters()
        {
            //string parametersString = "";
            StringBuilder parametersString = new StringBuilder();
            foreach (JProperty apiProperty in parameters.Properties())
            {
                JToken token = apiProperty.Value;
                parametersString.Append($"{apiProperty.Name}={token.Value<object>()}&");
            }

            apiUriBuilder.Query = parametersString.ToString();
        }

        public void verifySuccess(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: {response.RequestMessage}");
                throw new Exception($"{apiName} API has an error, please check it's properties. You can use this WIKI - " +
                    $"(TODO ADD LINK) ");
            }
        }

    }
}
