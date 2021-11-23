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
            // Display the number of command line arguments.

            while (true)
            {
                long startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                HeartBeat(startTime);
                CheckWeb(startTime);
                //wait
                Thread.Sleep(60000);
                //     try
                //     {
                //         
                //         if (timeTaken > 1000)
                //         {
                //             Console.WriteLine("Warn: {0} took {1}ms (Bad Ping)", DateTimeOffset.FromUnixTimeMilliseconds(startTime), timeTaken);
                //             LogToFile($"Warn: {DateTimeOffset.FromUnixTimeMilliseconds(startTime)} took {timeTaken}ms (Bad Ping)");
                //         }
                //         else
                //         {
                //             Console.WriteLine("Info: {0} took {1}ms", DateTimeOffset.FromUnixTimeMilliseconds(startTime), timeTaken);
                //             LogToFile($"Info: {DateTimeOffset.FromUnixTimeMilliseconds(startTime)} took {timeTaken}ms");
                //         }
                //     }
                //     catch
                //     {
                //         Console.WriteLine("Error at {0}", DateTimeOffset.FromUnixTimeMilliseconds(startTime));
                //         LogToFile($"Error at {DateTimeOffset.FromUnixTimeMilliseconds(startTime)}");
                //     }
                //     Thread.Sleep(Math.Max(0, 1000));
            }
        }

        public static async Task HeartBeat(long startTime)
        {
            LogToFile("Heart Beat", startTime, 0);
        }

        public static async Task CheckWeb(long startTime)
        {
            string strURL = "https://www.orcon.net.nz/terms/broadband/";
            long duration = -1;
            try
            {
                HttpWebRequest webURL = (HttpWebRequest)WebRequest.Create(strURL);
                webURL.Timeout = 30000;
                WebResponse response = webURL.GetResponse();
                duration = DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime;
            }
            catch
            {

            }
            LogToFile("Web", startTime, duration);
        }
        public static async Task LogToFile(string name, long startTime, long duration)
        {
            File.AppendAllTextAsync("log.txt", $"{name}, {startTime}, {duration}" + "\n");
        }
    }
}