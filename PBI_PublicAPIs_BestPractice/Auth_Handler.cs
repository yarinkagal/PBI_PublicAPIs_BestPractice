using Azure.Core;
using Microsoft.Identity.Client;
using Microsoft.Win32.SafeHandles;

namespace PBI_PublicAPIs_BestPractice
{
    class Auth_Handler
    {   
        private static Auth_Handler instance = null;
        public static Auth_Handler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Auth_Handler();
                }
                return instance;
            }
        }
        public string apiName = "auth";
        public string accessToken {  get; set; }  


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

                Auth_Handler.Instance.accessToken = result.AccessToken;
                return accessToken;
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
