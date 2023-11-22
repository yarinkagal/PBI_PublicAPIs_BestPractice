using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using PBI_PublicAPIs_BestPractice;
using static System.Net.Mime.MediaTypeNames;

class Program
{
    static async Task Main(string[] args)
    {
        Configuration_Handler configuration_handler = Configuration_Handler.Instance;
        await configuration_handler.addAccessToken();

        ModifiedAPI_Handler modifiedAPI = new ModifiedAPI_Handler();
        await modifiedAPI.run();










    }

    private static string getClientSecret()
    {
        return "Quw8Q~w1CqnvS_uc6IKbr~mj32mbn4QaAOupqcfJ";
    }
}


