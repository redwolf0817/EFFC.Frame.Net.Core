using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Module.Extend.EBA;
using System;

namespace EBAApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var dt = DateTime.Now;
            GlobalCommon.Logger = new Log4Net();
            var options = FrameDLRObject.CreateInstance();
            options.BusinessAssemblyName = "EBAApplication";
            options.args = args;
            var starter = new EBAStarter(options);

            starter.Start();
            
        }
    }
}
