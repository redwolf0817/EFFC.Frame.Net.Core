using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using EFFC.Frame.Net.Base.Interfaces;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Parameter;


namespace EFFC.Frame.Net.Base.Module
{
    /// <summary>
    /// 通过动态方式加载module
    /// CalledModuleName：待呼叫的module的name
    /// CalledModuleVersion：待呼叫的module的version
    /// </summary>
    public abstract class AssemblyModuleProxy:IModularProxy
    {
        /// <summary>
        /// Assembly的路径描述
        /// </summary>
        protected abstract string AssemblyPath { get; }
        /// <summary>
        /// 待呼叫的module的name
        /// </summary>
        protected abstract string CalledModuleName { get; }
        /// <summary>
        /// 待呼叫的module的version
        /// </summary>
        protected abstract string CalledModuleVersion { get; }

        private BaseModule _module = null;

        public bool CallModule(ParameterStd p, DataCollection data)
        {
            if (_module == null)
            {
                LoadModule();
            }

            _module.StepStart(p, data);

            return true;
        }

        private void LoadModule()
        {
            Assembly asm = Assembly.Load(AssemblyPath);
            Type[] ts = asm.GetTypes();
            foreach (Type c in ts)
            {
                if (c.IsInterface || c.IsAbstract)
                {
                    continue;
                }
                //判断是否为IModular的实现
                if (c.GetInterface(typeof(IModular).FullName) != null)
                {
                    IModular m = (IModular)Activator.CreateInstance(c, true);
                    if (m.Name == CalledModuleName && m.Version == CalledModuleVersion)
                    {
                        //判断是否为BaseModule的子类
                        if (m is BaseModule)
                        {
                            _module = (BaseModule)m;
                            break;
                        }
                    }
                }
            }
        }

        public virtual void OnError(Exception ex, ParameterStd p, DataCollection data)
        {
            throw ex;
        }
    }

    /// <summary>
    /// 通过动态方式加载module
    /// CalledModuleName：待呼叫的module的name
    /// CalledModuleVersion：待呼叫的module的version
    /// </summary>
    public abstract class AssemblyModuleProxy<P,D> : IModularProxy<P,D>
        where P : ParameterStd
        where D : DataCollection
    {
        /// <summary>
        /// Assembly的路径描述
        /// </summary>
        protected abstract string AssemblyPath { get; }
        /// <summary>
        /// 待呼叫的module的name
        /// </summary>
        protected abstract string CalledModuleName { get; }
        /// <summary>
        /// 待呼叫的module的version
        /// </summary>
        protected abstract string CalledModuleVersion { get; }

        private BaseModule<P,D> _module = null;

        public bool CallModule(P p, D data)
        {
            LoadModule().StepStart(p, data);

            return true;
        }

        protected abstract BaseModule<P, D> LoadModule();
        

        public virtual void OnError(Exception ex, P p, D data)
        {
            throw ex;
        }
    }
}
