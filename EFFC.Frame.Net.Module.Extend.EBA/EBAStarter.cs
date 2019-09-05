using EFFC.Frame.Net.Base.Module.Proxy;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Module.Extend.EConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Module.Extend.EBA
{
    /// <summary>
    /// 定时器
    /// </summary>
    public class EBAStarter
    {
        static ProxyManager pa = new ProxyManager();
        static ManualResetEventSlim _stopEvent = new ManualResetEventSlim(false);
        /// <summary>
        /// Console 启动器初始化
        /// </summary>
        /// <param name="options"></param>
        public EBAStarter(dynamic options)
        {
            var sbmsg = new StringBuilder();
            sbmsg.AppendLine($"欢迎使用EFFC.BatchApplication,你可以使用“-help/-h”来查看执行命令的选项");
            GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.INFO, sbmsg.ToString());
            if (options != null)
            {
                if(options.args != null && options.args is string[])
                {
                    var args = (string[])options.args;
                    ParseOptions(args,options);
                }
            }
            GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.INFO, sbmsg.ToString());
            pa.UseProxy<ScheduleProxy>("schedule", options);
        }
        
        /// <summary>
        /// 启动
        /// </summary>
        public virtual void Start()
        {
            Task.Run(() =>
             {
                object result = null;
                pa["schedule"].CallModule(ref result);
             });
            Console.CancelKeyPress += Console_CancelKeyPress;
            //进程退出事件
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            //卸载事件
            AssemblyLoadContext.Default.Unloading += Default_Unloading;
            _stopEvent.Wait();
        }
        private void ParseOptions(string[] args,dynamic options)
        {
            var firstoption = args.Length > 0 ? (args[0].StartsWith("-") ? args[0].Substring(1) : "") : "";
            firstoption = firstoption.ToLower();
            var sb = new StringBuilder();
            sb.AppendLine();
            
            if (firstoption == "help" || firstoption == "h")
            {
                sb.AppendLine("**********************************Schedule Help List****************************************");
                sb.AppendLine($"-immediately/-i:立即执行，并且只执行一次");
                sb.AppendLine($"-logic/-l:执行指定的logic，如果后面没有指定action，则指定默认的load方法");
                sb.AppendLine($"-action/-a:执行指定的logic下的action，如果没有指定action，则指定默认的load方法");
                sb.AppendLine($"-group/-g:执行指定group组的批次，如果没有指定group，则执行所有的批次");
                sb.AppendLine($"特别说明:如果同时指定了logic、action和group，则所有的条件都满足才会执行，否则不执行");
                sb.AppendLine("**********************************Schedule Help List end************************************");
                GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.INFO, sb.ToString());
                options.isnotexcute = true;
            }
            else
            {
                var dic = new Dictionary<string, string>();
                foreach(var s in args.Where(p=>p.StartsWith("-")).Select(p=>p.Substring(1).ToLower()))
                {
                    var v = "";
                    if(s == "immediately" || s == "i")
                    {
                        dic.Add("immediately", "");
                    }
                    else if(s == "logic" || s == "l")
                    {
                        if(args.ToList().IndexOf($"-{s}") >= 0)
                        {
                            v = args[args.ToList().IndexOf($"-{s}") + 1];
                            dic.Add("logic", v);
                        }
                    }
                    else if (s == "action" || s == "a")
                    {
                        if (args.ToList().IndexOf($"-{s}") >= 0)
                        {
                            v = args[args.ToList().IndexOf($"-{s}") + 1];
                            dic.Add("action", v);
                        }
                    }
                    else if (s == "group" || s == "g")
                    {
                        if (args.ToList().IndexOf($"-{s}") >= 0)
                        {
                            v = args[args.ToList().IndexOf($"-{s}") + 1];
                            dic.Add("group", v);
                        }
                    }
                }
                if (dic.ContainsKey("immediately"))
                {
                    options.immediately = true;
                }
                if (dic.ContainsKey("logic"))
                {
                    options.calllogic = dic["logic"];
                }
                if (dic.ContainsKey("action"))
                {
                    options.callaction = dic["action"];
                }
                if (dic.ContainsKey("group"))
                {
                    options.callgroup = dic["group"];
                }
            }
        }
        private void Default_Unloading(AssemblyLoadContext obj)
        {
            Stop();
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Stop();
        }

        private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Stop();
        }
        private void Stop()
        {
            if (!_stopEvent.IsSet)
            {
                _stopEvent.Set();
                object result = null;
                pa["schedule"].CallModule(ref result, "stop");
                _stopEvent.Dispose();
            }
        }
    }
}
