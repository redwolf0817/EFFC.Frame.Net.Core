using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Module.Extend.WebGo;
using EFFC.Frame.Net.Module.Extend.WebGo.Log;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Test
{
    public class WebGoTest
    {
        public static void Test()
        {
            GlobalCommon.Logger = new Log4Net();
            GlobalCommon.ExceptionProcessor = new WebGoExceptionProcessor();
            Console.OutputEncoding = Encoding.UTF8;

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseUrls("http://*:5001")
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }
    }
}
