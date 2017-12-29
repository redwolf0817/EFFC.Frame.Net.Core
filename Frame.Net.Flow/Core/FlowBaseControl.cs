using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Base.Token;
using EFFC.Frame.Net.Data.FlowData;
using EFFC.Frame.Net.Data.Parameters.Flow;
using EFFC.Frame.Net.Flow.Exceptions;
using EFFC.Frame.Net.Flow.Interfaces;
using EFFC.Frame.Net.Global;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace EFFC.Frame.Net.Flow.Core
{
    /// <summary>
    /// 流程控制器
    /// 根据flow name和实例版本号加载对应的流程实例，如果未找到对应的流程实例则抛出异常InvalidFlowException
    /// </summary>
    public abstract partial class FlowBaseControl : BaseModule<FlowParameter, FlowData>
    {
        protected FlowParameter _p;
        protected FlowData _d;
        ConcurrentDictionary<string, Type> _dic = null;
        ConcurrentDictionary<string, FlowBaseDefine> _dicinstance = null;
        protected static object lockobj = new object();
        /// <summary>
        /// 流程实例持久化
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        /// <param name="flowinstance"></param>
        protected abstract void SaveInstance(FlowParameter p, FlowData d);
        /// <summary>
        /// 获取流程实例
        /// 每个instanceid的实例只有一个对应
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        protected abstract void LoadInstance(FlowParameter p, FlowData d);

        /// <summary>
        /// 初始化作业最优先执行
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        protected virtual void Init(FlowParameter p, FlowData d)
        {
            _p = p;
            _d = d;

            lock (lockobj)
            {
                if (_dic == null)
                {
                    if (HttpRuntime.Cache["flow.flowcontrols"] != null)
                    {
                        _dic = (ConcurrentDictionary<string, Type>)HttpRuntime.Cache["flow.flowcontrols"];
                    }
                    else
                    {
                        _dic = new ConcurrentDictionary<string, Type>();
                        Assembly asm = Assembly.Load(GlobalCommon.FlowCommon.FlowDefineAssemblyPath);
                        Type[] ts = asm.GetTypes();
                        foreach (Type t in ts)
                        {
                            if (t.IsSubclassOf(typeof(FlowBaseDefine)) && !t.IsAbstract && !t.IsInterface)
                            {
                                FlowBaseDefine l = (FlowBaseDefine)Activator.CreateInstance(t, true);
                                string key = l.FlowName.ToUpper() + "." + l.Version.ToString();
                                if (_dic.ContainsKey(key))
                                {
                                    throw new DuplicateFlowInstanceException(string.Format("Duplicate flow-instance's Name-Version,pleas ensure flow-instance:[{0}-{1}] unique!", l.FlowName.ToUpper(), l.Version.ToString()));
                                }
                                else
                                {
                                    _dic.TryAdd(key, t);
                                }
                            }
                        }
                        //写入缓存
                        HttpRuntime.Cache.Insert("flow.flowcontrols", _dic, null, Cache.NoAbsoluteExpiration, TimeSpan.FromHours(12));
                    }
                }
                if (_dicinstance == null)
                {
                    if (HttpRuntime.Cache["flow.flowinstances"] != null)
                    {
                        _dicinstance = (ConcurrentDictionary<string, FlowBaseDefine>)HttpRuntime.Cache["flow.flowinstances"];
                    }
                    else
                    {
                        _dicinstance = new ConcurrentDictionary<string, FlowBaseDefine>();
                        HttpRuntime.Cache.Insert("flow.flowinstances", _dicinstance, null, Cache.NoAbsoluteExpiration, TimeSpan.FromHours(12));
                    }
                }
            }
        }
        private FlowBaseDefine LoadFlowDefine(FlowParameter p, FlowData d)
        {
            FlowBaseDefine rtn = null;
            var key = "";
            if (d.CurrentInfo != null && d.CurrentInfo.FlowState != FlowStateType.None)
            {
                key = p.FlowName.ToUpper() + "." + d.CurrentInfo.FlowVersion.ToString();
            }
            else
            {
                var latestver =  GetLatestVersionBy(p.FlowName);
                key = p.FlowName.ToUpper() + "." +latestver;
                if (d.CurrentInfo == null)
                {
                    d.CurrentInfo = new FlowInstanceInfo();
                }
                d.CurrentInfo.FlowName = p.FlowName;
                d.CurrentInfo.FlowVersion = latestver;
                d.CurrentInfo.InstanceID = p.FlowInstanceID;
                d.CurrentInfo.FlowState = FlowStateType.Ready;
            }
            //获取的实例不为空或无效状态，则继续流程运行，否则创建最新的流程
            if (_dic.ContainsKey(key))
            {
                var typekey = p.FlowName.ToUpper() + "." + d.CurrentInfo.FlowVersion.ToString();
                var instancekey = p.FlowName.ToUpper() + "." + d.CurrentInfo.FlowVersion.ToString() + "." + p.FlowInstanceID;
                if (_dicinstance.ContainsKey(instancekey))
                {
                    rtn = _dicinstance[instancekey];
                }
                else
                {
                    rtn = (FlowBaseDefine)Activator.CreateInstance(_dic[typekey], true);
                    if (!_dicinstance.TryAdd(instancekey, rtn))
                    {
                        return LoadFlowDefine(p, d);
                    }
                }

            }
            else
            {
                throw new InvalidFlowException(string.Format("Not found  the flow-instance named [{0}-{1}]!", p.FlowName.ToUpper(), d.CurrentInfo.FlowVersion.ToString()));
            }
            return rtn;
        }
        /// <summary>
        /// 通过流程名称获取已经注册的流程实例最新版本号
        /// </summary>
        /// <param name="flowname"></param>
        /// <returns></returns>
        protected FlowVersion GetLatestVersionBy(string flowname)
        {
            var strver = _dic.Where(c => c.Key.StartsWith(flowname.ToUpper() + ".")).Select(c => c.Key.Replace(flowname.ToUpper() + ".", "")).Max();
            return FlowVersion.Parse(strver);
        }
        /// <summary>
        /// 收尾处理，最后执行
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        protected virtual void AfterProcess(FlowParameter p, FlowData d)
        {
            //do noting

        }
        protected override void Run(FlowParameter p, FlowData d)
        {
            Init(p, d);
            //流程参数不齐全则报错处理
            if (ComFunc.nvl(p.FlowName) == "" || ComFunc.nvl(p.FlowInstanceID) == "")
            {
                throw new InvalidFlowException("Invalid flow:flow or flow instance is empty");
            }

            //获取流程实例
            LoadInstance(p, d);
            //流程已经结束
            if (d.CurrentInfo.FlowState == FlowStateType.End)
            {
                d.ResultContent = FrameDLRObject.CreateInstance(@"
{

issuccess:false,
message:""Flow " + p.FlowInstanceID + @" is end"",
code:1
}");
            }
            else
            {

                var fd = LoadFlowDefine(p, d);

                var driver = fd.GetDriver(p, d);
                var step = driver.NextStep(p, d);
                var state = step.Run(p, d);
                if (p.IsContinue)
                {
                    d.CurrentInfo.StepID = step.StepUID;
                    d.CurrentInfo.FlowState = state;
                    SaveInstance(p, d);

                    d.IsSuccess = true;
                    if (d.ResultContent == null)
                    {
                        d.ResultContent = FrameDLRObject.CreateInstance(@"{

issuccess:true,
message:""执行成功"",
code:0
}");
                    }
                }
                else
                {
                    d.ResultContent = FrameDLRObject.CreateInstance(@"{
issuccess:false,
message:""Flow " + p.FlowInstanceID + @" is suspend"",
code:2
}");

                }

                AfterProcess(p, d);

            }
            foreach (var k in p.TransTokenList.Keys)
            {
                CommitTrans(p.TransTokenList[k]);
            }
            _p.Resources.ReleaseAll();

        }


        /// <summary>
        /// 創建一個普通的資源對象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T NewResourceEntity<T>() where T : IResourceEntity
        {
            return _p.Resources.CreateInstance<T>(this.GetHashCode().ToString());
        }
        /// <summary>
        /// 創建一個事務資源對象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T NewTransResourceEntity<T>() where T : IResourceEntity, ITransaction
        {
            return _p.Resources.CreateInstance<T>(_p.CurrentTransToken);
        }

        /// <summary>
        /// 開啟事務
        /// </summary>
        public void BeginTrans()
        {
            _p.Resources.BeginTransaction(_p.CurrentTransToken);
        }
        public TransactionToken BeginTrans(FrameIsolationLevel level)
        {
            TransactionToken newtoken = TransactionToken.NewToken();
            newtoken.Begin();
            _p.TransTokenList.Add(newtoken);
            return newtoken;
        }
        /// <summary>
        /// 提交事務
        /// </summary>
        public void CommitTrans()
        {
            _p.Resources.CommitTransaction(_p.CurrentTransToken);
        }
        public void CommitTrans(TransactionToken token)
        {
            _p.Resources.CommitTransaction(token);
        }
        /// <summary>
        /// 回滾事務
        /// </summary>
        public void RollBack()
        {
            _p.Resources.RollbackTransaction(_p.CurrentTransToken);
        }

        public void RollBack(TransactionToken token)
        {
            _p.Resources.RollbackTransaction(token);
        }
    }
}
