using Newtonsoft.Json.Linq;
using PBI_PublicAPIs_BestPractice;
using PBI_PublicAPIs_BestPractice.API_Handlers;
using System.Runtime.CompilerServices;

class Program
{
    private static string workspacesFilePath {get;set;}
    private static SemaphoreSlim threadPool;
    private static WorkspaceInfoAPI_Handler workspaceInfoAPI;
    private static ScanStatusAPI_Handler scanStatusAPI;
    private static ScanResultAPI_Handler scanResultAPI;


    static async Task Main(string[] args)
    {

        try
        {
            Configuration_Handler configuration_handler = Configuration_Handler.Instance;
            configuration_handler.setConfigurationsFile(args[0]);

            Auth_Handler authHandler = new Auth_Handler();
            ModifiedAPI_Handler modifiedAPI = new ModifiedAPI_Handler();



            int threadsCount = Configuration_Handler.Instance.getConfig("shared", "threadsCount").Value<int>();
            threadPool = new SemaphoreSlim(threadsCount, threadsCount); ;


            string accessToken = await authHandler.authenticate();

            scanResultAPI = new ScanResultAPI_Handler("d39a93dd-42a9-4166-b2df-dff536beaf5d");
            for (int i = 0; i < 600; i++)
            {
                scanResultAPI.run();
            }


            workspacesFilePath = (string)await modifiedAPI.run();

            workspaceInfoAPI = new WorkspaceInfoAPI_Handler(workspacesFilePath);

            // Start 16 tasks, each trying to acquire a permit from the semaphore
            Task[] tasks = new Task[threadsCount];
            for (int i = 0; i < threadsCount; i++)
            {
                tasks[i] = Task.Run(() => runAPIs(i));
            }

            // Wait for all tasks to complete
            await Task.WhenAll(tasks);

            Console.WriteLine("All tasks completed.");

        }
        catch (ScannerAPIException ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine($"You can use docs here: {ex.HelpLink}");
            Console.WriteLine($"And the ReadMe file of this module: {ex.ReadMeLink}");
        }

        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        static async Task runAPIs(object? num)
        {
            threadPool.WaitAsync();
            string scanId;
            while (!Equals(scanId = (string)await workspaceInfoAPI.run(), "Done"))
            {
                while (scanId == null)
                {
                    // Waiting for response
                    Console.WriteLine($"Thread number {num} is going to sleep .....");
                    Thread.Sleep(500);
                    Console.WriteLine($"Thread number {num} awake .....");
                }

                scanStatusAPI = new ScanStatusAPI_Handler(scanId);

                while (!Equals(await scanStatusAPI.run(), "Succeeded"))
                {
                    Console.WriteLine($"Thread number {num} is going to sleep (waiting for status) ...");
                    await Task.Delay(2500); // change waiting increasly
                    Console.WriteLine($"Thread number {num} is awake from waiting for status ...");
                }

                scanResultAPI = new ScanResultAPI_Handler(scanId);
                scanResultAPI.run().Wait();

                try
                {
                    threadPool.Release();
                }
                catch
                {

                }
            }

        }

    }

}


