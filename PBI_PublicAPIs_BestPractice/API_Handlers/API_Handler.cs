using Azure;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PBI_PublicAPIs_BestPractice.API_Handlers
{
    public abstract class API_Handler
    {
        public string apiName { get; set; }
        public UriBuilder apiUriBuilder { get; set; }
        public Dictionary<string,object> parameters { get; set; }

        

        public API_Handler(string apiName)
        {
            this.apiName = apiName;
            this.parameters =  new Dictionary<string, object>();
            apiUriBuilder = new UriBuilder($"https://api.powerbi.com/v1.0/myorg/admin/workspaces/{apiName}");
        }
        
        public async Task<HttpResponseMessage> sendGetRequest()
        {
            using (HttpClient client = new HttpClient())
            {
                // Set the authorization header with the token
                string accessToken = (string)Configuration_Handler.Instance.getConfig("auth", "accessToken");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Send the GET request
                HttpResponseMessage response = await client.GetAsync(apiUriBuilder.Uri);

                verifySuccess(response);
                return response;
            }
        }

        public abstract Task<object> run();
           

        public void setParameters()
        {

            string parametersString = "";
            foreach (KeyValuePair<string, object> kvp in parameters)
            {
                parametersString += $"{kvp.Key}={kvp.Value}&";
            }
            apiUriBuilder.Query= parametersString;

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
