using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using EFFC.Frame.Net.Global;
using System.Text;
using EFFC.Frame.Net.Module.Extend.WebGo.Log;
using EFFC.Frame.Net.Module.Extend.WeixinWeb;
using Microsoft.AspNetCore;

namespace WeixinTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            GlobalCommon.Logger = new Log4Net();
            GlobalCommon.ExceptionProcessor = new WeixinExceptionProcessor();
            Console.OutputEncoding = Encoding.UTF8;

            //var host = WebHost.CreateDefaultBuilder(args)
            //    .UseStartup<Startup>()
            //    .UseUrls("http://*:5001")
            //    .UseIISIntegration()
            //    .UseApplicationInsights()
            //    .UseContentRoot(Directory.GetCurrentDirectory())
            //    .Build();
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseUrls("http://localhost:5001")
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }
    }
}
