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
            //create files
            if (!(File.Exists("log.txt")))
            {
                File.Create("log.txt");
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
                Task.Factory.StartNew(() => CheckLAN(startTime));
                //wait
                Thread.Sleep(1000);
            }
        }

        public static void HeartBeat(long startTime)
        {
            LogToFile("Heart Beat", startTime, 0);
        }

        public static void CheckWeb(long startTime)
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
        public static void CheckLAN(long startTime)
        {
            string strURL = "http://10.0.0.1/";
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
            LogToFile("LAN", startTime, duration);
        }
        public static async void LogToFile(string name, long startTime, long duration)
        {
            File.AppendAllTextAsync("log.txt", $"{name}, {startTime}, {duration}" + "\n");
        }
    }
}
//http://10.0.0.1/