using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Module.Extend.EWRA.Log;
using System.Text;
using EFFC.Frame.Net.Module.Extend.EWRA;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Common;

namespace RestAPISample
{
    public class Program
    {
        static string defaultport = "5001";
        public static void Main(string[] args)
        {
            Console.WriteLine(ComFunc.GetApplicationRoot());
            //准备框架环境参数
            GlobalCommon.Logger = new Log4Net();
            GlobalCommon.ExceptionProcessor = new EWRAExceptionProcessor();
            Console.OutputEncoding = Encoding.UTF8;
            
            //设定服务端参数
            //设定监听端口
            if (!String.IsNullOrEmpty(MyConfig.GetConfiguration("Server", "Port")))
            {
                defaultport = MyConfig.GetConfiguration("Server", "Port");
            }
            else if (args != null)
            {
                if (args.Length > 0 && IntStd.IsInt(args[0]))
                    defaultport = args[0];
            }
            Console.WriteLine($"服务器启动监听端口为{defaultport},如果要调整，请在启动应用时，后面接端口参数，或者在appsettings.json中的Server下设定Port参数（配置档优先）");
            var host = new WebHostBuilder()
               .UseKestrel()
               .UseContentRoot(Directory.GetCurrentDirectory())
               .UseUrls("http://*:" + defaultport)
               .UseIISIntegration()
               .UseStartup<Startup>()
               .UseApplicationInsights()
               .Build();

            host.Run();
        }
    }
}
