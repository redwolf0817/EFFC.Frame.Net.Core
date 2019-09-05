using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Module.Extend.EConsole;
using EFFC.Frame.Net.Resource.Loggers;
using System;

namespace BatchApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var dt = DateTime.Now;
            GlobalCommon.Logger = new Log4Net();
            var options = FrameDLRObject.CreateInstance();
            options.BusinessAssemblyName = "BatchApplication";
            var starter = new ConsoleStarter(options);

            if(args == null || args.Length <= 0)
            {
                args = new string[]{ "so", "load" };
            }
            starter.Start(args);
            GlobalCommon.Logger.WriteLog(EFFC.Frame.Net.Base.Constants.LoggerLevel.INFO, $"处理完成，耗时：{(DateTime.Now - dt).TotalMilliseconds}ms");
            Console.Read();
        }
    }
}
