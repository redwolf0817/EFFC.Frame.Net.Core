using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using EFFC.Frame.Net.Module.Extend.WebGo.Log;
using EFFC.Frame.Net.Module.Extend.WebGo;
using System.Text;
using EFFC.Frame.Net.Global;

namespace WebTest
{
    public class Program
    {
        public static void Main(string[] args)
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
