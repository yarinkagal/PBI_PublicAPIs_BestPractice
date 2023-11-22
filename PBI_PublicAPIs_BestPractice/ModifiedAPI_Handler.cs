using Azure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PBI_PublicAPIs_BestPractice
{
    internal class ModifiedAPI_Handler : API_Handler
    {

        public ModifiedAPI_Handler() : base("modified") 
        {
            string modifiedSince = (string)Configuration_Handler.Instance.getConfig(apiName, "modifiedSince");
            bool excludeInActiveWorkspaces = (bool)Configuration_Handler.Instance.getConfig(apiName, "excludeInActiveWorkspaces");
            bool excludePersonalWorkspaces = (bool)Configuration_Handler.Instance.getConfig(apiName, "excludePersonalWorkspaces");

            this.parameters = new object[] { modifiedSince, excludeInActiveWorkspaces, excludePersonalWorkspaces };
            this.output_folder = "..\\..\\..\\outputs\\ModifiedWorkspaces";
        }


        public override async Task<bool> run()
        {
            using (HttpClient client = new HttpClient())
            {
                // Set the authorization header with the token
                string accessToken = (string)Configuration_Handler.Instance.getConfig("auth", "accessToken");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                try
                {
                    // Send the GET request
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    
                    bool succeeded = await verifySuccess(response);
                    if (succeeded)
                    {
                        await saveOutput(response.Content);
                        return true;
                    }
                    return false;

                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                    return false;
                }
            }

        }

        public override async Task saveOutput(HttpContent content)
        {
            DateTime currentTime = DateTime.Now;

            string currentTimeString = currentTime.ToString("yyyy-MM-dd-HHmmss");
            
            using (FileStream outputFile = File.Create($"{output_folder}\\{currentTimeString}.txt"))

            //using (StreamWriter outputFile = new StreamWriter(Path.Combine("C:\\Users\\yarinkagal\\OneDrive - Microsoft\\Desktop\\Work\\PBI_PublicAPIs_BestPractice\\PBI_PublicAPIs_BestPractice\\outputs\\", output_file_path)))
            {
                string output = "";
                if (content != null)
                {
                    var ModifiedWorkspacesString = await content.ReadAsStringAsync();
                    var objects = JsonConvert.DeserializeObject<object[]>(ModifiedWorkspacesString);
                    foreach (JObject modifiedWorkspace in objects)
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes((string)modifiedWorkspace["id"]+"\n");
                        // Add some information to the file.
                        outputFile.Write(info, 0, info.Length);
                    }
                }
            }
        }

        public async Task<bool> verifySuccess(HttpResponseMessage response)
        {
            // Check if the request was successful (status code 200-299)
            if (response.IsSuccessStatusCode)
            {
                // Read and print the response content
                string content = await response.Content.ReadAsStringAsync();
                return true;
            }
            else
            {
                throw new Exception(this.apiName);
            }
        }
    }
}
