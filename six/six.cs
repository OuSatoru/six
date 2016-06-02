﻿using System;
using System.Collections.Generic;
using System.Text;
using Fiddler;
using System.Diagnostics;
using System.Threading;

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
            if (oSession.fullUrl.Contains("www.baidu.com"))
            {
                oSession.utilDecodeResponse();
                string requestText = System.Text.Encoding.UTF8.GetString(oSession.ResponseBody);
                if (requestText.Contains("hao123"))
                {
                    requestText = requestText.Replace("hao123", "BBBBB");
                    oSession.utilSetResponseBody(requestText);
                    Console.WriteLine(requestText);
                }
            }
        }
    }
}
