using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using static System.Net.Mime.MediaTypeNames;


namespace PBI_PublicAPIs_BestPractice
{
    class Auth_Handler
    {   
        public string apiName = "auth";
        private string email { get; }
        //public string /*accessToken*/ { get; set; }
        public bool isConnected { get; set; }


        public Auth_Handler()
        {
            isConnected = false;
        }

        public async Task<string> authenticate()
        {
            string clientId = (string)Configuration_Handler.Instance.getConfig(apiName, "clientId");
            string tenantId = (string)Configuration_Handler.Instance.getConfig(apiName, "tenantId");

            string[] scopes = { "https://analysis.windows.net/powerbi/api/Tenant.Read.All", 
                                "https://analysis.windows.net/powerbi/api/Tenant.ReadWrite.All" 
                              }; // Use the appropriate scope for Power BI

            Uri tenantAuthority = new Uri($"https://login.microsoftonline.com/{tenantId}");

            string redirectUri = "http://localhost";

            try
            {
                IPublicClientApplication app = PublicClientApplicationBuilder
                    .Create(clientId)
                    .WithAuthority(tenantAuthority)
                    .WithRedirectUri(redirectUri)
                    .Build();

                var result = await app.AcquireTokenInteractive(scopes)
                                    .WithUseEmbeddedWebView(false)
                                    .ExecuteAsync();

                string accessToken = result.AccessToken;

                isConnected = true;
                return accessToken;
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }




    }

        ////string clientId = "your-client-id";
        //string clientId = "849efc11-1579-4345-88e1-baa6ea935748";

        ////string tenantId = "your-tenant-id";
        //string tenantId = "82e09e3a-99a6-4ec0-a928-e8a90bcd1515";

        //string authority = $"https://login.microsoftonline.com/common/oauth2/v2.0/authorize";

        //string[] scopes = { "user.read", "https://analysis.windows.net/powerbi/api/Tenant.Read.All" }; // Use the appropriate scope for Power BI


        //IPublicClientApplication app = PublicClientApplicationBuilder
        //        .Create(clientId)
        //        //.WithAuthority(authority)
        //        .WithAuthority(new Uri($"https://login.microsoftonline.com/{tenantId}"))
        //        .WithRedirectUri("http://localhost")
        //        .Build();


        //var result = await app.AcquireTokenInteractive(scopes)
        //                    .WithUseEmbeddedWebView(false)
        //                    .WithAuthority(authority)
        //                    .ExecuteAsync();

        //var token = result.AccessToken;
    

    
}
