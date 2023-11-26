using System;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using PBI_PublicAPIs_BestPractice;
using PBI_PublicAPIs_BestPractice.API_Handlers;

class Program
{
    private static string worspacesFilePath {get;set;}
    private static Semaphore threadPool;
    private static WorkspaceInfoAPI_Handler workspaceInfoAPI;
    private static ScanStatusAPI_Handler scanStatusAPI;
    private static ScanResultAPI_Handler scanResultAPI;

    static async Task Main(string[] args)
    {
        //Checks tst = new Checks();
        //tst.seekCheck();
        //Configuration_Handler.Instance.setConfig("modified", "modifiedSince", "");
        try
        {
            
            Auth_Handler authHandler = new Auth_Handler();
            Configuration_Handler configuration_handler = Configuration_Handler.Instance;
            ModifiedAPI_Handler modifiedAPI = new ModifiedAPI_Handler();
            

            int threadsCount = (int)Configuration_Handler.Instance.getConfig("general", "threadsCount");
            threadPool = new Semaphore(threadsCount, threadsCount); ;


            string accessToken = await authHandler.authenticate();
            configuration_handler.setConfig("auth", "accessToken", accessToken);


            worspacesFilePath = (string) await modifiedAPI.run();
            
            workspaceInfoAPI = new WorkspaceInfoAPI_Handler(worspacesFilePath);

            

            await runWithoutThreads();

            //for (int i = 0; i < threadsCount; i++)
            //{
            //    Thread thread = new Thread(new ParameterizedThreadStart(runAPIs));
            //    thread.Start();
            //}

        }
        catch (Exception ex)
        {

            Console.WriteLine(ex.Message);
        }


    }

    static async void runAPIs(object? obj)
    {
        threadPool.WaitOne();

        // run post (B) => save scan id
        string scanId = (string) await workspaceInfoAPI.run();

        // run getStatus for each scan id

        scanStatusAPI = new ScanStatusAPI_Handler(scanId);

        while (!Equals(await scanStatusAPI.run(),"Succeeded"))
        {
            await Task.Delay(5000);
        }
        // run result and save 


        threadPool.Release();
    }

    public static async Task<string> runWithoutThreads()
    {
        string scanId =(string) await workspaceInfoAPI.run();

        if (scanId != null)
        {
            scanStatusAPI = new ScanStatusAPI_Handler(scanId);

            while (!Equals(await scanStatusAPI.run(), "Succeeded"))
            {
                await Task.Delay(5000);
            }
            scanResultAPI = new ScanResultAPI_Handler(scanId);

            bool x = (bool)await scanResultAPI.run();
        }




        return scanId;
    }


}


