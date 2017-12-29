using System;
using System.Collections.Generic;
using System.Linq;
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
    /// 通过Assembly反射方式加载单实例Logic并运行
    /// 默认过期时间为12小时
    /// </summary>
    /// <typeparam name="P"></typeparam>
    /// <typeparam name="D"></typeparam>
    /// <typeparam name="L"></typeparam>
    public abstract class AssemblyLoadBusinessSingletonModule<P, D, L> : BaseModule<P, D>
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

        Dictionary<string, L> _logics = null;

        ILogic GetLogicByName(string resourceName)
        {
            lock (lockobj)
            {
                if (_logics == null)
                {

                    if (GlobalCommon.ApplicationCache.Get(this.GetType().FullName + ".SingletonLogicList") != null)
                    {
                        _logics = (Dictionary<string, L>)GlobalCommon.ApplicationCache.Get(this.GetType().FullName + ".SingletonLogicList");
                    }
                    else
                    {

                        _logics = new Dictionary<string, L>();
                        Assembly asm = Assembly.Load(new AssemblyName(LogicAssemblyPath));
                        Type[] ts = asm.GetTypes();
                        foreach (Type t in ts)
                        {
                            if (t.GetTypeInfo().IsSubclassOf(typeof(L)) && !t.GetTypeInfo().IsAbstract && !t.GetTypeInfo().IsInterface)
                            {
                                L l = (L)Activator.CreateInstance(t, true);
                                L outl = default(L);
                                if (_logics.ContainsKey(l.Name.ToUpper()))
                                {
                                    throw new DuplicateLogicException("Duplicate Logic's Name,please make sure Logic:[" + l.Name + "] unique!");
                                }
                                else
                                {

                                    _logics.Add(l.Name.ToUpper(), l);
                                }
                            }
                        }
                        //写入缓存
                        GlobalCommon.ApplicationCache.Set(this.GetType().FullName + ".SingletonLogicList", _logics, TimeExpiration);
                    }
                }
            }
            if (_logics.ContainsKey(resourceName.ToUpper()))
            {
                return _logics[resourceName.ToUpper()];
            }
            else
            {
                throw new Exception("未找到名为" + resourceName + "的Logic!");
            }
        }
        /// <summary>
        /// 设置固定的过期时间,默认12小时
        /// 两种过期时间设了其中一种，另一种要设为0,用NoAbsolute(Sliding)Expiration枚举
        /// </summary>
        public virtual DateTime TimeExpiration
        {
            get
            {
                return DateTime.Now.AddHours(12);
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
