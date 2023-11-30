using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PBI_PublicAPIs_BestPractice
{
    public class ScannerAPIException : Exception
    {
        public string ReadMeLink = "TODO ADD README FILE";

        public ScannerAPIException(string apiName, string errorMessage) :
            base($"{apiName} has an error: {errorMessage}. Please check it's properties.")
        {}
    }
}
