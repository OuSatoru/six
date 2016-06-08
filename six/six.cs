using System;
using System.Collections.Generic;
using System.Text;
using Fiddler;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace six
{
    class six
    {
        [STAThreadAttribute]
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
                    Console.WriteLine("Before response for:\t" + oSession.fullUrl);
                    string requestText = oSession.GetResponseBodyAsString();
                    Console.WriteLine(requestText);
                    if (requestText.Contains("invigilate.js"))
                    {
                        requestText = requestText.Replace("<script type=\"text/javascript\" src=\"../../../../resources/scripts/proj/invigilate.js\"></script>", "");
                        oSession.utilSetResponseBody(requestText);
                        Console.WriteLine("Changed to: " + requestText);
                    }
                    if (requestText.StartsWith("[{\"id\":null"))
                    {
                        string res = requestText;
                        Clipboard.SetText(res);
                        oSession.utilSetResponseBody(requestText);
                        Console.WriteLine("Catching question json: " + res);
                    }
                }
            };
            Fiddler.FiddlerApplication.Startup(8877, FiddlerCoreStartupFlags.Default);
            Console.ReadKey();
            Fiddler.FiddlerApplication.Shutdown();
            Thread.Sleep(500);
        }
    }
}
