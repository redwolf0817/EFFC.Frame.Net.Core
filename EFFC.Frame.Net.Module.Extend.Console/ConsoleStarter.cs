using EFFC.Frame.Net.Base.Module.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EConsole
{
    /// <summary>
    /// Console的启动入口
    /// </summary>
    public class ConsoleStarter
    {
        static ProxyManager pa = new ProxyManager();
        /// <summary>
        /// Console 启动器初始化
        /// </summary>
        /// <param name="options"></param>
        public ConsoleStarter(dynamic options)
        {
            pa.UseProxy<ConsoleBusiProxy>("console", options);
        }
        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="args"></param>
        public virtual async void Start(string[] args)
        {
            object result = null;
            pa["console"].CallModule(ref result, args.ToArray<object>());
        }
    }
}
