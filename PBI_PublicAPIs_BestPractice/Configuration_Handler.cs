using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
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


        public JObject getApiSettings(string apiName)
        {
            return (JObject)_configurationSettings[apiName];
        }

        public JToken getConfig(string apiName, string parameterName)
        {

            if(_configurationSettings.TryGetValue(apiName, out var apiSettings) &&
                ((JObject)apiSettings).TryGetValue(parameterName, out JToken parameterValue))

            {
                if (parameterName.Equals("modifiedSince") && DateTime.TryParse((string)parameterValue, out DateTime dateTime))
                {
                    string iso8601Time = dateTime.ToString("O");
                    return iso8601Time;
                }

                return parameterValue;
            }
            else
            {
                return null;
            }
        }

        public bool setConfig(string apiName, string parameterName, JToken value)
        {
            try { 
                _configurationSettings.TryGetValue(apiName, out object apiSettings);
                ((JObject)apiSettings)[parameterName] = value;
            
                string jsonString = JsonConvert.SerializeObject(_configurationSettings, Formatting.Indented);
                File.WriteAllText(_configurationFilePath, jsonString);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }

   




}
