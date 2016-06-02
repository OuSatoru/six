using System;
using System.Collections.Generic;
using System.Text;
using Fiddler;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;

namespace six
{
    class six
    {
        static void Main(string[] args)
        {
            Fiddler.FiddlerApplication.BeforeRequest += new SessionStateHandler(FiddlerApplication_BeforeRequest);
            Fiddler.FiddlerApplication.BeforeResponse += new Fiddler.SessionStateHandler(FiddlerApplication_BeforeResponse);
            Fiddler.FiddlerApplication.Startup(8877, FiddlerCoreStartupFlags.Default);
            Console.ReadKey();
            Fiddler.FiddlerApplication.Shutdown();
            Thread.Sleep(500);
        }

        private static void FiddlerApplication_BeforeRequest(Session oSession)
        {
            Console.WriteLine("Before request for:\t" + oSession.fullUrl);
            Debug.WriteLine(oSession.RequestBody);
            oSession.bBufferResponse = true;
        }

        static void FiddlerApplication_BeforeResponse(Session oSession)
        {
            if (oSession.fullUrl.Contains("tobeconfirm"))
            {
                oSession.utilDecodeResponse();
                string requestText = System.Text.Encoding.UTF8.GetString(oSession.ResponseBody);
                if (requestText.Contains("invigilate.js"))
                {
                    requestText = requestText.Replace("<SCRIPT src=\"../../../../resources/scripts/proj/invigilate.js\" type=text/javascript></SCRIPT>", "");
                    oSession.utilSetResponseBody(requestText);
                    //Console.WriteLine(requestText);
                }
            }
        }
    }
}
