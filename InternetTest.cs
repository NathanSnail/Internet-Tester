using System;
using System.Net;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace InternetTest
{
    class InternetTest
    {
        static void Main(string[] args)
        {
            string lockPID = File.ReadAllLines("lock.txt")[0];

            int curPID = Process.GetCurrentProcess().Id;
            try
            {
                Process old = Process.GetProcessById(int.Parse(lockPID));
                if (old.ProcessName == File.ReadAllLines("lock.txt")[1])
                {
                    throw new Exception();
                }
                return;
            }
            catch
            {
                File.WriteAllText("lock.txt", curPID.ToString() + "\n" + Process.GetCurrentProcess().ToString());
            }
            string strURL;
            WebRequest webURL;
            WebResponse response;
            long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            // Display the number of command line arguments.
            strURL = "https://www.example.com";
            while (true)
            {
                try
                {
                    webURL = WebRequest.Create(strURL);
                    startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    response = webURL.GetResponse();
                    long timeTaken = DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime;
                    if (timeTaken > 1000)
                    {
                        Console.WriteLine("Warn: {0} took {1}ms (Bad Ping)", DateTimeOffset.FromUnixTimeMilliseconds(startTime), timeTaken);
                        LogToFile($"Warn: {DateTimeOffset.FromUnixTimeMilliseconds(startTime)} took {timeTaken}ms (Bad Ping)");
                    }
                    else
                    {
                        Console.WriteLine("Info: {0} took {1}ms", DateTimeOffset.FromUnixTimeMilliseconds(startTime), timeTaken);
                        LogToFile($"Info: {DateTimeOffset.FromUnixTimeMilliseconds(startTime)} took {timeTaken}ms");
                    }
                }
                catch
                {
                    Console.WriteLine("Error at {0}", DateTimeOffset.FromUnixTimeMilliseconds(startTime));
                    LogToFile($"Error at {DateTimeOffset.FromUnixTimeMilliseconds(startTime)}");
                }
                Thread.Sleep(Math.Max(0, 1000));
            }

        }
        public static async Task LogToFile(string info)
        {
            File.AppendAllTextAsync("log.txt", info + "\n");
        }
    }
}