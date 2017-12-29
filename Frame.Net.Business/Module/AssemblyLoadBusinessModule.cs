using System;
using System.Collections.Generic;
using System.Reflection;
using EFFC.Frame.Net.Base.Exceptions;
using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Business.Logic;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Global;

namespace EFFC.Frame.Net.Business.Module
{
    /// <summary>
    /// 通过Assembly反射方式加载Logic并运行
    /// </summary>
    /// <typeparam name="P"></typeparam>
    /// <typeparam name="D"></typeparam>
    /// <typeparam name="L"></typeparam>
    public abstract class AssemblyLoadBusinessModule<P, D, L> : BaseModule<P, D>
        where P : ParameterStd
        where D : DataCollection
        where L : ILogic
    {
        P _p;
        D _d;
        protected static object lockobj = new object();
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
                GetLogicByName(LogicName).process(p, d);
            }
        }

        Dictionary<string, Type> _logics = null;
        ILogic GetLogicByName(string resourceName)
        {
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
                                ILogic l = (ILogic)Activator.CreateInstance(t, true);
                                if (!_logics.ContainsKey(l.Name.ToUpper()))
                                {
                                    _logics.Add(l.Name.ToUpper(), t);
                                }
                                else
                                {
                                    throw new DuplicateLogicException("Duplicate Logic's Name,please make sure Logic:[" + l.Name + "] unique!");
                                }
                            }
                        }
                        //写入缓存
                        GlobalCommon.ApplicationCache.Set(this.GetType().FullName + ".LogicList", _logics, DateTime.MaxValue);
                    }
                }
            }
            if (_logics.ContainsKey(resourceName.ToUpper()))
            {
                return (ILogic)Activator.CreateInstance(_logics[resourceName.ToUpper()], true);
            }
            else
            {
                throw new Exception("未找到名为" + resourceName + "的Logic!");
            }

        }

        public abstract string LogicAssemblyPath
        {
            get;
        }

        public abstract string LogicName
        {
            get;
        }
    }
}
