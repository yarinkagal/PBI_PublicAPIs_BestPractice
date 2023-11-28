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
    private static bool finished = false;

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

            worspacesFilePath = (string) await modifiedAPI.run();
            workspaceInfoAPI = new WorkspaceInfoAPI_Handler(worspacesFilePath);

            

            //await runWithoutThreads();

            for (int i = 0; i < threadsCount; i++)
            {
                //Thread thread = new Thread(new ParameterizedThreadStart(runAPIs));
                Thread thread = new Thread(async () => await runAPIs(i));
                thread.Start();
            }

        }
        catch (Exception ex)
        {

            Console.WriteLine(ex.Message);
        }


    }

    static async Task runAPIs(object? obj)
    {
        threadPool.WaitOne();
        Console.WriteLine($"Thread number {obj} is running .....");

        string scanId = (string)await workspaceInfoAPI.run();
        while (scanId == null && !finished)
        { 
            Console.WriteLine($"Thread number {obj} is going to sleep .....");
            Thread.Sleep(500);
            Console.WriteLine($"Thread number {obj} awake .....");
        }
        //string scanId = scanIdTask.Result;
        Console.WriteLine($"Thread number {obj} in line 70 .....");
        Console.WriteLine($"scanId = {scanId}");

        scanStatusAPI = new ScanStatusAPI_Handler(scanId);

        while (!Equals(await scanStatusAPI.run(), "Succeeded"))
        {
            await Task.Delay(500);
        }

        scanResultAPI = new ScanResultAPI_Handler(scanId);
        scanResultAPI.run();
        finished = true;
        threadPool.Release();
    }

    public static async Task<string> runWithoutThreads()
    {
        string scanId;

        while ((string) (scanId = (string)await workspaceInfoAPI.run())!=null ){ 
            scanStatusAPI = new ScanStatusAPI_Handler(scanId);

            while (!Equals(await scanStatusAPI.run(), "Succeeded"))
            {
                await Task.Delay(5000);
            }
            scanResultAPI = new ScanResultAPI_Handler(scanId);

            await scanResultAPI.run();
        }


        return scanId;
    }


}


