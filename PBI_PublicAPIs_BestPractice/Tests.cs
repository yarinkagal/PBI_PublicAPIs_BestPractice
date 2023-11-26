using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PBI_PublicAPIs_BestPractice
{
    public class Checks
    {
        Semaphore semaphore = new Semaphore(initialCount: 3, maximumCount: 3);
        public void SemaphoreCheck()
        {

             

            // Create and start five threads
            for (int i = 0; i < 5; i++)
            {
                Thread thread = new Thread(DoWork);
                thread.Start(i);
            }

            

        }

        internal void seekCheck()
        {
            string filePath = "C:\\Users\\yarinkagal\\OneDrive - Microsoft\\Desktop\\Work\\PBI_PublicAPIs_BestPractice\\PBI_PublicAPIs_BestPractice\\outputs\\ModifiedWorkspaces\\2023-11-23-095404.txt";
            int offset = 40; // Replace with the desired offset

              using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                
                // Move the cursor to the specified offset
                byte[] buffer = new byte[36];
                fileStream.Read(buffer, 0, 36);
                string result1 = Encoding.UTF8.GetString(buffer);
                fileStream.Seek(2, SeekOrigin.Current);

                byte[] buffer2 = new byte[36];
                fileStream.Read(buffer, 0, 36);
                string result2 = Encoding.UTF8.GetString(buffer);

                var t = 5;
            }
        }

        void DoWork(object threadId)
        {
            Console.WriteLine($"Thread {threadId} is waiting to enter the semaphore.");

            // Wait to enter the semaphore
            semaphore.WaitOne();

            Console.WriteLine($"Thread {threadId} has entered the semaphore.");

            // Simulate some work
            Thread.Sleep(2000);

            Console.WriteLine($"Thread {threadId} is releasing the semaphore.");

            // Release the semaphore
            semaphore.Release();
        }
    }
    
}
