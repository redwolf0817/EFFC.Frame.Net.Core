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
using EFFC.Frame.Net.Base.Common;

namespace WeixinTest
{
    public class Program
    {
        static string defaulturl = "http://*:85";
        public static void Main(string[] args)
        {
            GlobalCommon.Logger = new Log4Net();
            GlobalCommon.ExceptionProcessor = new WeixinExceptionProcessor();
            Console.OutputEncoding = Encoding.UTF8;

            //设定服务端参数
            //设定监听端口
            if (!String.IsNullOrEmpty(MyConfig.GetConfiguration("Server", "BindUrl")))
            {
                defaulturl = MyConfig.GetConfiguration("Server", "BindUrl");
            }
            else if (args != null)
            {
                if (args.Length > 0 && ComFunc.nvl(args[0]) != "")
                    defaulturl = args[0];
            }
            Console.WriteLine($"服务器启动绑定URL为{defaulturl},如果要调整，请在启动应用时，后面接端口参数，或者在appsettings.json中的Server下设定BindUrl参数（参数优先）");

            var host = new WebHostBuilder()
               .UseKestrel()
               .UseContentRoot(Directory.GetCurrentDirectory())
               .UseUrls(defaulturl)
               .UseIISIntegration()
               .UseStartup<Startup>()
               .UseApplicationInsights()
               .Build();

            host.Run();
        }
    }
}
