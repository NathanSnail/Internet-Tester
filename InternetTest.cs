using System;
using System.Net;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace InternetTest
{
    class InternetTest
    {
        public static int cycleTime = 1000;
        static void Main(string[] args)
        {
            //create files
            if (!(File.Exists("log.txt")))
            {
                File.Create("log.txt").Close();
            }
            if (!(File.Exists("lock.txt")))
            {
                File.Create("lock.txt").Close();
                File.AppendAllLines("lock.txt", new string[2] { "1", "2" });
            }

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
                Task.Factory.StartNew(() => HeartBeat(startTime));
                Task.Factory.StartNew(() => CheckWeb(startTime));
                // Task.Factory.StartNew(() => CheckRouter(startTime));
                Task.Factory.StartNew(() => CheckPing(startTime));
                //wait
                Thread.Sleep(cycleTime);
            }
        }

        public static void HeartBeat(long startTime)
        {
            LogToFile("Heart Beat", startTime, 0);
        }

        public static void CheckWeb(long startTime)
        {
            string strURL = "https://www.orcon.net.nz/terms/broadband/";
            long duration = cycleTime / 2;
            try
            {
                HttpWebRequest webURL = (HttpWebRequest)WebRequest.Create(strURL);
                webURL.Timeout = cycleTime / 2;
                WebResponse response = webURL.GetResponse();
                duration = DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime;
            }
            catch
            {
                duration = cycleTime / 2;
            }
            LogToFile("Web", startTime, duration);
        }
        // public static void CheckRouter(long startTime)
        // {
        //     string strURL = "http://10.0.0.1/";
        //     long duration = cycleTime / 2;
        //     try
        //     {
        //         HttpWebRequest webURL = (HttpWebRequest)WebRequest.Create(strURL);
        //         webURL.Timeout = cycleTime / 2;
        //         WebResponse response = webURL.GetResponse();
        //         duration = DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime;
        //     }
        //     catch
        //     {
        //         duration = cycleTime / 2;
        //     }
        //     LogToFile("Router", startTime, duration);
        // }
        public static void CheckPing(long startTime)
        {
            try
            {
                new Ping().SendPingAsync(IPAddress.Parse("10.0.0.1"), cycleTime / 2);
                LogToFile("Ping", startTime, DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime);
            }
            catch
            {
                LogToFile("Ping", startTime, cycleTime / 2);
            }
        }
        public static async void LogToFile(string name, long startTime, long duration)
        {
            try
            {
                await File.AppendAllTextAsync("log.txt", $"{name}, {startTime}, {duration}" + "\n");
            }
            catch { }
        }
    }
}