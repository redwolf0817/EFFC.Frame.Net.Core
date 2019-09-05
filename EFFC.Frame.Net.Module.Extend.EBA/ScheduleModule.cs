using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Module.Extend.EConsole.DataCollections;
using EFFC.Frame.Net.Module.Extend.EConsole.Parameters;
using EFFC.Frame.Net.Module.Business;
using EFFC.Frame.Net.Base.Module.Proxy;
using System.Reflection;
using System.Linq;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Module.Extend.EConsole;
using System.Threading.Tasks;
using EFFC.Frame.Net.Module.Extend.EConsole.Logic;
using System.Threading;
using EFFC.Frame.Net.Module.Extend.EBA.Attributes;
using EFFC.Frame.Net.Base.Constants;

namespace EFFC.Frame.Net.Module.Extend.EBA
{
    /// <summary>
    /// 计划任务执行的逻辑模块
    /// </summary>
    public class ScheduleModule : ConsoleBusiModule
    {
        static Dictionary<string, ScheduleMethod> _call_dic = new Dictionary<string, ScheduleMethod>();
        static Dictionary<string, DateTime> _next_run_schedule = new Dictionary<string, DateTime>();
        static Dictionary<string, Task> _running_task = new Dictionary<string, Task>();
        static Dictionary<string, bool> _is_run = new Dictionary<string, bool>();
        static ScheduleExcuteOptions _excute_options = new ScheduleExcuteOptions();
        static object lockobj = new object();
        public override string Name => "ScheduleModule";
        public override string Description => "任务计划管理模块";

        protected override void OnLoadAssembly(ProxyManager ma, dynamic options, List<Type> logics)
        {
            var sbmsg = new StringBuilder();
            if (options != null)
            {
                if (options.immediately != null && options.immediately == true)
                {
                    _excute_options.IsImmediately = true;
                }
                if (options.isnotexcute != null && options.isnotexcute == true)
                {
                    _excute_options.IsNotExcute = true;
                }
                _excute_options.CallLogic = ComFunc.nvl(options.calllogic);
                _excute_options.CallAction = ComFunc.nvl(options.callaction);
                _excute_options.CallGroup = ComFunc.nvl(options.callgroup);
            }
            sbmsg.Clear();
            sbmsg.AppendLine($"当前设定为{(_excute_options.IsNotExcute ? "不执行任务" : "执行任务")},请在调用EBAStarter初始化的时候使用的options中设定该参数（isnotexcute格式为bool类型）");
            sbmsg.AppendLine($"当前执行方式为{(_excute_options.IsImmediately ? "立即执行并只执行一次" : "循环执行")},请在调用EBAStarter初始化的时候使用的options中设定该参数（immediately格式为bool类型），或者使用指令的-help来查看相关选项设定");
            sbmsg.AppendLine($"当前执行Logic为{(_excute_options.CallLogic == "" ? "空" : _excute_options.CallLogic)},如果为空则表示会依照执行计划表执行所有的批次，否则只执行指定的批次逻辑，如需调整请在调用EBAStarter初始化的时候使用的options中设定该参数（calllogic格式为待执行logic的名称），或者使用指令的-help来查看相关选项设定");
            sbmsg.AppendLine($"当前执行Logic为{(_excute_options.CallAction == "" ? "空" : _excute_options.CallAction)},如果为空则表示会依照执行计划表执行默认的action-load方法，否则只执行指定的action方法（如果方法存在则执行），如需调整请在调用EBAStarter初始化的时候使用的options中设定该参数（callaction格式为待执行logic的名称），或者使用指令的-help来查看相关选项设定");
            sbmsg.AppendLine($"当前执行的组为{(_excute_options.CallGroup == "" ? "空" : _excute_options.CallGroup)},如果为空则表示会依照执行计划表执行执行所有的批次，否则只执行指定的group，如需调整请在调用EBAStarter初始化的时候使用的options中设定该参数（callgroup格式为待执行组的名称），或者使用指令的-help来查看相关选项设定");
            GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.INFO, sbmsg.ToString());
            sbmsg.Clear();
            sbmsg.AppendLine();
            sbmsg.AppendLine("**********************************Schedule List****************************************");
            foreach (var l in logics)
            {
                var methods = l.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(t => t.GetCustomAttribute<ScheduleAttribute>(true) != null);


                foreach (var m in methods)
                {
                    var attr = m.GetCustomAttribute<ScheduleAttribute>();
                    var attrdesc = m.GetCustomAttribute<EBADescAttribute>();
                    var groupdesc = m.GetCustomAttribute<EBAGroupAttribute>();
                    var isopenattr = m.GetCustomAttribute<EBAIsOpenAttribute>();
                    var repeat = m.GetCustomAttribute<EBARepeatWhenExceptionAttribute>();
                    var lname = l.Name.ToLower();
                    var action = m.Name.ToLower();
                    var key = $"{lname}.{action}";
                    if (_call_dic.ContainsKey(key))
                    {
                        GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.ERROR, $"{this.GetType().Name}加载Logic失败，原因:Command为{key}的存在重复的Function，请排除后再启动");
                        return;
                    }
                    var sm = new ScheduleMethod(l, attr, attrdesc, groupdesc, isopenattr, repeat);
                    var isaddrun = true;
                    if (_excute_options.CallLogic != "" && lname != _excute_options.CallLogic)
                    {
                        isaddrun = false;
                    }
                    if (_excute_options.CallAction != "" && action != _excute_options.CallAction )
                    {
                        isaddrun = false;
                    }
                    if (_excute_options.CallGroup != "" && sm.Group != _excute_options.CallGroup)
                    {
                        isaddrun = false;
                    }
                    if (!sm.IsOpen)
                    {
                        isaddrun = false;
                    }
                    if (isaddrun)
                    {
                        _call_dic.Add(key, sm);
                        _next_run_schedule.Add(key, sm.GetNextRunTime(DateTime.Now));
                        var logmsg = $"{(string.IsNullOrEmpty(sm.Name) ? key : sm.Name)}:执行指令为{key},首次执行时间{_next_run_schedule[key].ToString("yyyy-MM-dd HH:mm:ss")}";
                        sbmsg.AppendLine($"{logmsg}");
                    }
                }

            }
            sbmsg.AppendLine("**********************************Schedule List end************************************");
            GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.INFO, sbmsg.ToString());
            
        }
        protected override bool CheckMyParametersAndConfig(ConsoleParameter p, ConsoleData d)
        {
            foreach (var s in MyConfig.GetConfigurationList("Mail"))
            {
                p[DomainKey.CONFIG, $"Mail_{s.Key}"] = s.Value;
            }
            return true;
        }
        static bool isrun = true;
        static List<Task> running_task = new List<Task>();
        static object taskobj = new object();
        protected override void InvokeBusiness(ConsoleParameter p, ConsoleData d)
        {
            if (p.ExtentionObj.stop != null && p.ExtentionObj.stop)
            {
                isrun = false;
                GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.INFO, $"接收到stop指令，准备结束运行，请等待...");
            }
            if (_excute_options.IsNotExcute)
            {
                GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.INFO, $"程式不执行");
            }
            else if (_excute_options.IsImmediately)
            {
                //立即执行并且只执行一次
                //如果没指定要调用的逻辑，则默认都执行一次
                foreach (var k in _call_dic.Keys)
                {
                    var isrepeat = false;
                    var repeatcount = 0;
                    var repeattimes = 0;
                    do
                    {
                        if (isrepeat)
                        {
                            repeatcount++;
                            GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.INFO, $"当前为任务{k}第{repeatcount}次重复执行");
                        }
                        try
                        {
                            using (var cp = p.DeepCopy<ConsoleParameter>())
                            {
                                using (var cd = d.DeepCopy<ConsoleData>())
                                {
                                    GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.INFO, $"开始执行任务{k}");
                                    var lname = k.Split('.')[0];
                                    var action = k.Split('.')[1];

                                    cp.CallLogicName = lname;
                                    cp.CallAction = action;
                                    if (!string.IsNullOrEmpty(cp.CallLogicName) && !string.IsNullOrEmpty(cp.CallAction))
                                    {
                                        var logic = _call_dic[k].NewInstance();
                                        logic.process(cp, cd);
                                        GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.INFO, $"结束任务{k}");
                                    }
                                    else
                                    {
                                        GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.INFO, $"任务{k}不符合任务执行选项标准，故不执行");
                                    }
                                }
                            }

                            isrepeat = false;
                            repeatcount = 0;
                            repeattimes = 0;
                        }
                        catch (Exception ex)
                        {
                            var errormsg = new StringBuilder();
                            errormsg.AppendLine();
                            errormsg.AppendLine($"任务{k}执行出错，错误信息为{ex.Message}");
                            errormsg.AppendLine(ex.StackTrace);
                            if (ex.InnerException != null)
                            {
                                errormsg.AppendLine(ex.InnerException.Message);
                                errormsg.AppendLine(ex.StackTrace);
                            }
                            GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.ERROR, errormsg.ToString());
                            if (_call_dic[k].RepeatWhenExceptionType != null
                                                && (ex.GetType().FullName == _call_dic[k].RepeatWhenExceptionType.FullName || ex.GetType().IsSubclassOf(_call_dic[k].RepeatWhenExceptionType)
                                                || (ex.InnerException != null && (ex.InnerException.GetType().FullName == _call_dic[k].RepeatWhenExceptionType.FullName || ex.InnerException.GetType().IsSubclassOf(_call_dic[k].RepeatWhenExceptionType)))))
                            {
                                if (!isrepeat)
                                {
                                    isrepeat = true;
                                    repeattimes = _call_dic[k].RepeatTimes;
                                    GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.INFO, $"任务{k}执行出现异常，因设定重复执行任务，因此进入重复执行序列，直到执行完成或达到重复次数，设定的重复次数为{repeattimes}");
                                }
                            }
                            else
                            {
                                isrepeat = false;
                                repeatcount = 0;
                                repeattimes = 0;
                            }
                        }
                        finally
                        {
                            GC.Collect();
                        }
                    } while (isrepeat && repeatcount < repeattimes);
                }
            }
            else
            {
                while (isrun)
                {
                    var dt = DateTime.Now;
                    var rs = _next_run_schedule.Where(t => 1 == 1).ToDictionary(k => k.Key, v => v.Value);
                    foreach (var item in rs)
                    {
                        if (dt >= item.Value)
                        {
                            if (!IsRun(item.Key))
                            {
                                SetRunFlag(item.Key);
                                var task = Task.Run(() =>
                                {
                                    //等待task添加到dictionary中
                                    Task.Delay(100).Wait();
                                    var isrepeat = false;
                                    var repeatcount = 0;
                                    var repeattimes = 0;
                                    do
                                    {
                                        if (isrepeat)
                                        {
                                            repeatcount++;
                                            GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.INFO, $"当前为任务{item.Key}第{repeatcount}次重复执行");
                                        }
                                        try
                                        {
                                            using (var cp = p.DeepCopy<ConsoleParameter>())
                                            {
                                                using (var cd = d.DeepCopy<ConsoleData>())
                                                {
                                                    GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.INFO, $"开始执行任务{item.Key}");
                                                    var lname = item.Key.Split('.')[0];
                                                    var action = item.Key.Split('.')[1];
                                                    cp.CallLogicName = lname;
                                                    cp.CallAction = action;

                                                    if (!string.IsNullOrEmpty(cp.CallLogicName) && !string.IsNullOrEmpty(cp.CallAction))
                                                    {
                                                        var logic = _call_dic[item.Key].NewInstance();
                                                        //提前写入下次执行的时间，便于logic执行善后处理
                                                        UpdateNextRunTime(item.Key, dt);
                                                        cp.ExtentionObj.Next_Excute_Time = _next_run_schedule[item.Key];

                                                        cp.ExtentionObj.ExcuteName = _call_dic[item.Key].Name;
                                                        cp.ExtentionObj.ExcuteDescription = _call_dic[item.Key].Desc;
                                                        logic.process(cp, cd);
                                                        GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.INFO, $"结束任务{item.Key},下次执行时间点为{_next_run_schedule[item.Key].ToString("yyyy-MM-dd HH:mm:ss")}");
                                                    }
                                                    else
                                                    {
                                                        GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.INFO, $"任务{item.Key}不符合任务执行选项标准，故不执行,后续也不再执行");
                                                    }
                                                }
                                            }

                                            isrepeat = false;
                                            repeatcount = 0;
                                            repeattimes = 0;
                                        }
                                        catch (Exception ex)
                                        {
                                            var errormsg = new StringBuilder();
                                            errormsg.AppendLine();
                                            errormsg.AppendLine($"任务{item.Key}执行出错，错误信息为{ex.Message}");
                                            errormsg.AppendLine(ex.StackTrace);
                                            if (ex.InnerException != null)
                                            {
                                                errormsg.AppendLine(ex.InnerException.Message);
                                                errormsg.AppendLine(ex.StackTrace);
                                            }
                                            GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.ERROR, errormsg.ToString());
                                            if(_call_dic[item.Key].RepeatWhenExceptionType != null 
                                                && (ex.GetType().FullName == _call_dic[item.Key].RepeatWhenExceptionType.FullName || ex.GetType().IsSubclassOf(_call_dic[item.Key].RepeatWhenExceptionType)
                                                || (ex.InnerException != null && (ex.InnerException.GetType().FullName == _call_dic[item.Key].RepeatWhenExceptionType.FullName || ex.InnerException.GetType().IsSubclassOf(_call_dic[item.Key].RepeatWhenExceptionType)))))
                                            {
                                                if (!isrepeat)
                                                {
                                                    isrepeat = true;
                                                    repeattimes = _call_dic[item.Key].RepeatTimes;
                                                    GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.INFO, $"任务{item.Key}执行出现异常，因设定重复执行任务，因此进入重复执行序列，直到执行完成或达到重复次数，设定的重复次数为{repeattimes}");
                                                }
                                            }
                                            else
                                            {
                                                isrepeat = false;
                                                repeatcount = 0;
                                                repeattimes = 0;
                                            }
                                            UpdateNextRunTime(item.Key, dt);
                                        }
                                        finally
                                        {
                                            SetStopFlag(item.Key);
                                            GC.Collect();
                                        }
                                    } while (isrepeat && repeatcount < repeattimes);

                                });
                                if (_running_task.ContainsKey(item.Key))
                                {
                                    _running_task[item.Key].Wait();
                                    _running_task.Remove(item.Key);
                                    _running_task.Add(item.Key, task);
                                }
                                else
                                {
                                    _running_task.Add(item.Key, task);
                                }

                            }
                        }
                        //else
                        //{
                        //    GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.INFO, $"任务{item.Key}未到执行时间点，无须执行");
                        //}
                    }
                    Task.Delay(1000).Wait();
                }
                //结束之前需要等待所有的正在运行的线程执行完成
                Task.WaitAll(_running_task.Values.ToArray());
                GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.INFO, $"所有任务执行完成，结束运行");
            }
        }
        private void UpdateNextRunTime(string key, DateTime pretime)
        {
            lock (lockobj)
            {
                _next_run_schedule[key] = _call_dic[key].GetNextRunTime(pretime);
            }
        }
        private bool IsRun(string key)
        {
            if (_is_run.ContainsKey(key))
            {
                return _is_run[key];
            }
            else
            {
                return false;
            }
        }
        private void SetRunFlag(string key)
        {
            lock (lockobj)
            {
                if (!_is_run.ContainsKey(key))
                {
                    _is_run.Add(key, true);
                }
                else
                {
                    _is_run[key] = true;
                }
            }
        }
        private void SetStopFlag(string key)
        {
            lock (lockobj)
            {
                if (!_is_run.ContainsKey(key))
                {
                    _is_run.Add(key, false);
                }
                else
                {
                    _is_run[key] = false;
                }
                if (!_running_task.ContainsKey(key))
                {
                    _running_task.Remove(key);
                }
            }
        }

        protected override void AfterProcess(ConsoleParameter p, ConsoleData d)
        {
            p.Resources.CommitTransaction(p.CurrentTransToken);
        }

        protected class ScheduleMethod
        {
            public ScheduleMethod(Type l, ScheduleAttribute schedule, 
                EBADescAttribute desc, 
                EBAGroupAttribute group,
                EBAIsOpenAttribute isopen,
                EBARepeatWhenExceptionAttribute repeat) 
            {
                LogicType = l;
                Schedule = schedule;
                Name = desc==null?"": desc.Name;
                Desc = desc == null ? "" : desc.Description;
                Group = group == null ? "" : group.GroupName;
                IsOpen = isopen == null ? true : isopen.IsOpen;
                RepeatWhenExceptionType = repeat == null ? null : repeat.ExceptionType;
                RepeatTimes = repeat == null ? 0 : repeat.RepeatTimes;
            }
            public ScheduleAttribute Schedule
            {
                get;
                protected set;
            }
            public Type LogicType
            {
                get;
                protected set;
            }
            public DateTime GetNextRunTime(DateTime pretime)
            {
                return Schedule.GetNextStartTime(pretime);
            }
            public ScheduleLogic NewInstance()
            {
                return (ScheduleLogic)Activator.CreateInstance(LogicType, true);
            }
            public string Name
            {
                get;
                protected set;
            }
            public string Desc
            {
                get;
                protected set;
            }
            public string Group
            {
                get;
                protected set;
            }
            public bool IsOpen
            {
                get;
                protected set;
            }

            /// <summary>
            /// 遇到异常的类型才执行重复
            /// </summary>
            public Type RepeatWhenExceptionType
            {
                get;
                protected set;
            }
            /// <summary>
            /// 重复次数
            /// </summary>
            public int RepeatTimes { get; protected set; }
        }


    }
}
