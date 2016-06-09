using System;
using System.Collections.Generic;
using System.Text;
using Fiddler;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;

namespace six
{
    class six
    {
        //[STAThread]
        static void Main(string[] args)
        {
            Fiddler.FiddlerApplication.BeforeRequest += delegate (Fiddler.Session oSession)
            {
                Console.WriteLine("Before request for:\t" + oSession.fullUrl);
                Debug.WriteLine(oSession.RequestBody);
                oSession.bBufferResponse = true;
            };
            Fiddler.FiddlerApplication.BeforeResponse += delegate (Fiddler.Session oSession)
            {
                if (oSession.fullUrl.Contains("66.185.64.100:8080"))//66.185.64.100:8080
                {
                    oSession.utilDecodeResponse();
                    Console.WriteLine("Before response for:\t{0} response code:\t{1}", oSession.fullUrl, oSession.responseCode);
                    string requestText = oSession.GetResponseBodyAsString();
                    Console.WriteLine(requestText);
                    if (requestText.Contains("invigilate.js"))
                    {
                        requestText = requestText.Replace("<script type=\"text/javascript\" src=\"../../../../resources/scripts/proj/invigilate.js\"></script>", "");
                        oSession.utilSetResponseBody(requestText);
                        Console.WriteLine("Changed to: " + requestText);
                    }
                    if (requestText.Contains("[{\"id\":null"))
                    {
                        Console.WriteLine("Catching question json...");
                        oSession.utilSetResponseBody(requestText);
                        //Clipboard.SetText(requestText);
                        Write(requestText);
                        Console.WriteLine("Done.");
                    }
                }
            };
            CleanIE6();   //ie6 can't use
            Fiddler.FiddlerApplication.Startup(8877, FiddlerCoreStartupFlags.Default);
            Console.ReadKey();
            Fiddler.FiddlerApplication.Shutdown();
            Thread.Sleep(500);
        }
        static void RunCmd(string cmd)
        {
            ProcessStartInfo p = new ProcessStartInfo();
            p.FileName = "cmd.exe";
            p.Arguments = "/c " + cmd;
            p.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(p);
        }
        static void CleanIE()
        {
            /*
            1.History 2.Cookies 8.Temporary Internet Files
            16.Form Data 32.Passwords 255.Delete All
            4351.Delete All - "Also delete files and settings stored by add-ons"
            */
            RunCmd("RunDll32.exe InetCpl.cpl,ClearMyTracksByProcess 8");
        }
        static void CleanIE6()
        {
            string cachePath = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
            DirectoryInfo di = new DirectoryInfo(cachePath);
            foreach (FileInfo fi in di.GetFiles("*.*", SearchOption.AllDirectories))
            {
                Console.WriteLine(fi.Name);
                try
                {
                    fi.Delete();
                }
                catch { }
            }
        }
        static void Write(string str)
        {
            FileStream fs = new FileStream("json.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(str);
            sw.Close();
            fs.Close();
        }
    }
}
