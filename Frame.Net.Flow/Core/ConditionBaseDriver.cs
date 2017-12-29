using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Exceptions;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Data.FlowData;
using EFFC.Frame.Net.Data.LogicData;
using EFFC.Frame.Net.Data.Parameters;
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
    public abstract partial class ConditionBaseDriver:IConditionDriver<FlowParameter,FlowData>
    {
        protected FlowParameter _p;
        protected FlowData _d;
        ConcurrentDictionary<string, FlowBaseStep> _dic = null;
        static object lockobj = new object();
        public IStep<FlowParameter, FlowData> NextStep(FlowParameter p, FlowData d)
        {
            _p = p;
            _d = d;

            return LoadStep(GetNextStepName(p, d));
        }

        private FlowBaseStep LoadStep(string uid)
        {
            lock (lockobj)
            {
                if (_dic == null)
                {
                    if (HttpRuntime.Cache["flow.steps"] != null)
                    {
                        _dic = (ConcurrentDictionary<string, FlowBaseStep>)HttpRuntime.Cache["flow.steps"];
                    }
                    else
                    {
                        _dic = new ConcurrentDictionary<string, FlowBaseStep>();
                        Assembly asm = Assembly.Load(GlobalCommon.FlowCommon.StepAssemblyPath);
                        Type[] ts = asm.GetTypes();
                        foreach (Type t in ts)
                        {
                            if (t.IsSubclassOf(typeof(FlowBaseStep)) && !t.IsAbstract && !t.IsInterface)
                            {
                                FlowBaseStep l = (FlowBaseStep)Activator.CreateInstance(t, true);
                                string key = l.StepUID.ToUpper();
                                if (_dic.ContainsKey(key))
                                {
                                    throw new DuplicateException(string.Format("Duplicate step's UID,pleas ensure step:[{0}] unique!", l.StepUID.ToUpper()));
                                }
                                else
                                {
                                    _dic.TryAdd(key, l);
                                }
                            }
                        }
                        //写入缓存
                        HttpRuntime.Cache.Insert("flow.steps", _dic, null, Cache.NoAbsoluteExpiration, TimeSpan.FromHours(12));
                    }
                }
            }
            var stepkey = uid.ToUpper();
            if (_dic.ContainsKey(stepkey))
            {
                return _dic[stepkey];
            }
            else
            {
                throw new InvalidFlowException(string.Format("Not found  the step named [{0}]!", uid));
            }
        }

        protected abstract string GetNextStepName(FlowParameter p,FlowData d);
        /// <summary>
        /// 当前实例信息
        /// </summary>
        protected FlowInstanceInfo CurrentInstance
        {
            get
            {
                return _d.CurrentInfo;
            }
        }
    }
}
