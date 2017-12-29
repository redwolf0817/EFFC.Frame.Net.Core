using System;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Base.Module.Proxy;

namespace EFFC.Frame.Net.Base.Module
{
    /// <summary>
    /// Module定义
    /// </summary>
    /// <typeparam name="ParameterStd">约定的参数类型</typeparam>
    /// <typeparam name="DataCollection">约定的数据类型</typeparam>
    public abstract class BaseModule : IModular, IDriver,IDisposable
    {
        /// <summary>
        /// ProxyManager在UseProxy的时候进行初始化调用，用于application启动时进行初始化调用
        /// 模块可以在此进行module使用前的设定处理
        /// </summary>
        /// <param name="ma"></param>
        /// <param name="options"></param>
        protected virtual void OnUsed(ProxyManager ma, dynamic options)
        {
        }
        #region Define by Instance
        /// <summary>
        /// 每个模块实例的运行逻辑实现
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        protected abstract void Run(ParameterStd p, DataCollection d);
        /// <summary>
        /// 出错处理，该方法由Module自己调用
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="p"></param>
        /// <param name="d"></param>
        protected abstract void OnError(Exception ex, ParameterStd p, DataCollection d);

        /// <summary>
        /// 模块运行逻辑定义，该方法由proxy调用
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        public void Start(ParameterStd p, DataCollection d)
        {
            try
            {
                Run(p, d);
            }
            catch (Exception ex)
            {
                OnError(ex, p, d);
            }
            finally
            {
                Dispose();
            }
        }
        /// <summary>
        /// 模块内部资源释放定义，不对ParameterStd中的资源进行释放
        /// </summary>
        public abstract void Dispose();
        /// <summary>
        /// 每个模块实例都需要检核自己所需要的Parameter（相关的Config配置、资源管理器也会包含其中）是否齐全并且状态完好并做调整处理，如果不通过，则proxy会中断模块调用并抛出异常
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public abstract bool CheckParametersAndConfig(ParameterStd p,DataCollection d);
        #endregion

        #region Define by Class
        /// <summary>
        /// 方法的名称，每类模块为一个名称，为Class级调用
        /// </summary>
        public abstract string Name
        {
            get;
        }
        /// <summary>
        /// 模块描述，为Class级调用
        /// </summary>
        public abstract string Description
        {
            get;
        }
        #endregion
       
        
    }
}
