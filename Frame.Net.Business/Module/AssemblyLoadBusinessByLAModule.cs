using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Exceptions;
using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Business.Logic;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EFFC.Frame.Net.Business.Module
{
    public abstract class AssemblyLoadBusinessByLAModule<P, D, L> : BaseModule<P, D>
        where P : WebParameter
        where D : DataCollection
        where L : ILogic
    {
        P _p;
        D _d;
        private static object lockobj = new object();
        private static object lockobj_l = new object();
        protected P Parameter
        {
            get { return _p; }
        }

        protected D DataCollection
        {
            get { return _d; }
        }

        protected override void Run(P p, D d)
        {
            _p = p;
            _d = d;
            if (!string.IsNullOrEmpty(LogicAssemblyPath))
            {
                var l = GetLogicByName(p.RequestResourceName, p.Action);
                var key = (p.RequestResourceName + "." + p.Action).ToUpper();
                var lockl = _lockobj_instance[key];
                lock (lockl)
                {
                    l.process(p, d);
                }
            }
        }

        Dictionary<string, Type> _logics = null;
        Dictionary<string, L> _instance = null;
        Dictionary<string, object> _lockobj_instance = null;
        ILogic GetLogicByName(string resourceName,string action)
        {
            string reqlakey = (resourceName + "." + action).ToUpper();
            lock (lockobj)
            {
                if (_logics == null)
                {
                    if (GlobalCommon.ApplicationCache.Get(this.GetType().FullName + ".LogicList") != null)
                    {
                        _logics = (Dictionary<string, Type>)GlobalCommon.ApplicationCache.Get(this.GetType().FullName + ".LogicList");
                    }
                    else
                    {

                        _logics = new Dictionary<string, Type>();
                        Assembly asm = Assembly.Load(new AssemblyName(LogicAssemblyPath));
                        Type[] ts = asm.GetTypes();
                        foreach (Type t in ts)
                        {
                            if (t.GetTypeInfo().IsSubclassOf(typeof(L)) && !t.GetTypeInfo().IsAbstract && !t.GetTypeInfo().IsInterface)
                            {
                                L l = (L)Activator.CreateInstance(t, true);
                                var lkey = (l.Name).ToUpper();
                                if (!_logics.ContainsKey(lkey))
                                {
                                    _logics.Add(lkey, t);
                                }
                                else
                                {
                                    if (_logics.ContainsKey(lkey))
                                    {
                                        throw new DuplicateLogicException("Duplicate Logic's Name,please make sure Logic:[" + l.Name + "] unique!");
                                    }
                                }
                            }
                        }
                        //写入缓存
                        GlobalCommon.ApplicationCache.Set(this.GetType().FullName + ".LogicList", _logics,DateTime.MaxValue);
                    }
                }

                if (_instance == null)
                {
                    if (GlobalCommon.ApplicationCache.Get(this.GetType().FullName + ".LogicInstanceList") != null)
                    {
                        _instance = (Dictionary<string, L>)GlobalCommon.ApplicationCache.Get(this.GetType().FullName + ".LogicInstanceList");
                    }
                    else
                    {
                        _instance = new Dictionary<string, L>();
                        //写入缓存
                        GlobalCommon.ApplicationCache.Set(this.GetType().FullName + ".LogicInstanceList", _instance, DateTime.MaxValue);
                    }

                    if (GlobalCommon.ApplicationCache.Get(this.GetType().FullName + ".LogicInstancelockobj") != null)
                    {
                        _lockobj_instance = (Dictionary<string, object>)GlobalCommon.ApplicationCache.Get(this.GetType().FullName + ".LogicInstancelockobj");

                    }
                    else
                    {
                        _lockobj_instance = new Dictionary<string, object>();
                        //写入缓存
                        GlobalCommon.ApplicationCache.Set(this.GetType().FullName + ".LogicInstancelockobj", _lockobj_instance, DateTime.MaxValue);
                    }
                }

                if (!_lockobj_instance.ContainsKey(reqlakey))
                {
                    _lockobj_instance.Add(reqlakey, new object());
                }

                if (_instance.ContainsKey(reqlakey))
                {
                    return (L)_instance[reqlakey];
                }
                else
                {
                    if (_logics.ContainsKey(resourceName.ToUpper()))
                    {
                        L l = (L)Activator.CreateInstance(_logics[resourceName.ToUpper()], true);

                        _instance.Add(reqlakey, l);
                        return l;
                    }
                    else
                    {
                        throw new Exception("未找到名为" + resourceName + "的Logic!");
                    }
                }
            }
        }

        public abstract string LogicAssemblyPath
        {
            get;
        }
    }
}
