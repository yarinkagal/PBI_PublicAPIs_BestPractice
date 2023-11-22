using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PBI_PublicAPIs_BestPractice
{

    public sealed class Configuration_Handler
    {
        private static Configuration_Handler instance = null;

        public string _configurationFilePath = "../../../configurationsFile.json";
        public Dictionary<string, object> _configurationSettings { get; }

        private Configuration_Handler() 
        {
            string jsonString = File.ReadAllText(this._configurationFilePath);
            _configurationSettings = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);

        }
        
        public static Configuration_Handler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Configuration_Handler();
                }
                return instance;
                
            }
        }

        public async Task<string> addAccessToken()
        {
            Auth_Handler authHandler = new Auth_Handler();
            string accessToken = await authHandler.authenticate();
            Configuration_Handler.Instance.addConfig("auth", "accessToken", accessToken);
            return accessToken;
        }

        public JToken getConfig(string apiName, string parameterName)
        {
            Configuration_Handler configuration_Handler = Configuration_Handler.Instance;



            if(configuration_Handler._configurationSettings.TryGetValue(apiName, out var apiSettings) &&
                ((JObject)apiSettings).TryGetValue(parameterName, out JToken parameterValue))

            {
                return parameterValue;
            }
            else
            {
                return null;
            }
        }

        public bool addConfig(string apiName, string parameterName, JToken value)
        {
            Configuration_Handler configuration_Handler = Configuration_Handler.Instance;
            configuration_Handler._configurationSettings.TryGetValue(apiName, out object apiSettings);
            bool result =((JObject)apiSettings).TryAdd(parameterName ,value);
            
            return result;
        }

    }

   




}
