using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBI_PublicAPIs_BestPractice
{
    public abstract class API_Handler
    {
        public string apiName { get; set; }
        public string apiUrl { get; set; }
        public object[] parameters { get; set; }
        public string[] headers { get; set; }
        public string output_folder { get; set; }
        public int requestLimit { get; set; }
        public int limitDurationInMinutes { get; set; }

        // 200 requests per hour -> requestLimit = 200 , limitDurationInMinutes = 60


        public API_Handler(string apiName)
        {
            this.apiName = apiName;
            apiUrl = $"https://api.powerbi.com/v1.0/myorg/admin/workspaces/{apiName}";

        }

        public abstract Task<bool> run();

        public abstract Task saveOutput(HttpContent content);

        


    }
}
