using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Exceptions;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Parameter;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Base.Module.Proxy
{
    /// <summary>
    /// 模块的代理
    /// </summary>
    public abstract class ModuleProxy
    {
        /// <summary>
        /// 当proxy被加载到ProxyManager的时候调用，主要是application层面进行初始化的时候调用
        /// </summary>
        /// <param name="ma"></param>
        /// <param name="options"></param>
        protected void OnUsed(ProxyManager ma, dynamic options)
        {
            MyUsed(ma, options);
            var tmp = CreateModuleInstance();
            FrameExposedObject.From(tmp).OnUsed(ma, options);
        }
        /// <summary>
        /// 自定义OnUsed，可以在application层面进行初始化的时候调用
        /// </summary>
        /// <param name="ma"></param>
        /// <param name="options"></param>
        protected virtual void MyUsed(ProxyManager ma, dynamic options)
        {
            //Customer define
        }
        #region Async Call
        /// <summary>
        /// 异步呼叫指定的模块，并返回期望的结果集
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="version"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<TResult> CallModule<TResult>(params object[] obj)
        {
            var t = typeof(TResult);
            object result = null;
            
            if (!t.GetTypeInfo().IsClass)
            {
                if (t.FullName == typeof(int).FullName)
                {
                    result = int.MinValue;
                }
                else if (t.FullName == typeof(float).FullName)
                {
                    result = float.MinValue;
                }
                else if (t.FullName == typeof(double).FullName)
                {
                    result = double.MinValue;
                }
                else if (t.FullName == typeof(decimal).FullName)
                {
                    result = decimal.MinValue;
                }
                else if (t.FullName == typeof(DateTime).FullName)
                {
                    result = DateTime.MinValue;
                }
            }
            else
            {
                if (t.FullName == typeof(string).FullName)
                {
                    result = "";
                }
                else
                {
                    result = Activator.CreateInstance<TResult>();
                }
            }
            CallModule(ref result, obj);

            return (TResult)result;
        }
        #endregion
        #region Call
        /// <summary>
        /// 调用模块，并输出期望的结果对象
        /// </summary>
        /// <param name="version">模块版本号，默认版本号为master，即模块的最新版本号</param>
        /// <param name="result">期望的结果对象</param>
        /// <param name="obj">参数集</param>
        /// <returns></returns>
        public bool CallModule(ref object result, params object[] obj)
        {
            var module = CreateModuleInstance();
            var module_p = ConvertParameters(obj);
            var module_d = ConvertDataCollection(ref result);
            if (!module.CheckParametersAndConfig(module_p, module_d))
            {
                throw new ModuleFailedException($"{module.GetType().Name} check parameters failed!Please read the module's instruction,and fix it");
            }
            try
            {
                module.Start(module_p, module_d);
                ParseDataCollection2Result(module_d, ref result);
            }
            catch (Exception ex)
            {
                OnError(ex, module_p, module_d);
                return false;
            }
            finally
            {
                Dispose(module_p,module_d);
            }
            return true;
        }
        #endregion
        /// <summary>
        /// 将传入的参数集转换成模块指定的参数类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected abstract ParameterStd ConvertParameters(object[] obj);
        /// <summary>
        /// 将传入的期望结果转换成模块指定的数据集类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected abstract DataCollection ConvertDataCollection(ref object obj);
        /// <summary>
        /// 将模块计算完成后的数据集转化成调用者期望的结果集类
        /// </summary>
        /// <param name="d"></param>
        /// <param name="obj"></param>
        protected abstract void ParseDataCollection2Result(DataCollection d, ref object obj);
        /// <summary>
        /// 加载指定版本的模块
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        protected abstract BaseModule CreateModuleInstance();
        /// <summary>
        /// 出错处理
        /// </summary>
        /// <param name="ex"></param>
        protected abstract void OnError(Exception ex, ParameterStd p, DataCollection d);
        /// <summary>
        /// 代理对本次调用进行资源释放
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        protected abstract void Dispose(ParameterStd p, DataCollection d);
    }
}
