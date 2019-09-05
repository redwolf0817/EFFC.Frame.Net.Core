using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Module.Business.Logic;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Base.Module.Proxy;
using EFFC.Frame.Net.Global;
using System.Reflection;

namespace EFFC.Frame.Net.Module.Business
{
    /// <summary>
    /// 业务模块的基本流程定义
    /// </summary>
    /// <typeparam name="TLogic"></typeparam>
    /// <typeparam name="TParameter"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public abstract class BaseBusiModule<TLogic,TParameter,TData> : BaseModule
        where TLogic:BaseLogic<TParameter,TData>
        where TParameter:ParameterStd
        where TData:DataCollection
    {
        protected override void OnUsed(ProxyManager ma, dynamic options)
        {
            var busiassemblyname = "";
            if (options == null)
            {
                busiassemblyname = "";
            }
            else
            {
                if (options.BusinessAssemblyName != null)
                {
                    busiassemblyname = options.BusinessAssemblyName;
                }
            }

            if (busiassemblyname == "")
            {
                GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.INFO, $@"{this.GetType().Name}初始化缺少Logic工程的Assembly命名空间参数（BusinessAssemblyName），这可能会引发调用Logic时的异常，
请在调用ProxyManager.UseProxy中的options中设定该参数（BusinessAssemblyName格式为Logic dll的namespace如：xxx.Business）");
                return;
            }
            Assembly asm = Assembly.Load(new AssemblyName(busiassemblyname));
            Type[] ts = asm.GetTypes();
            var list = new List<Type>();
            foreach (Type t in ts)
            {
                if (t.GetTypeInfo().IsSubclassOf(typeof(TLogic)) && !t.GetTypeInfo().IsAbstract && !t.GetTypeInfo().IsInterface)
                {
                    list.Add(t);
                }
            }
            OnLoadAssembly(ma, options, list);
            GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.INFO, string.Format(@"BusiModule执行初始化作业，加载{0}下的所有{1}的Logic完成", busiassemblyname, typeof(TLogic).Name));
        }
        public override bool CheckParametersAndConfig(ParameterStd p, DataCollection d)
        {
            if (!(p is TParameter)) return false;
            if (!(d is TData)) return false;

            var tp = (TParameter)p;
            var td = (TData)d;
            return CheckMyParametersAndConfig(tp, td);
        }

        protected override void OnError(Exception ex, ParameterStd p, DataCollection d)
        {
            var tp = (TParameter)p;
            var td = (TData)d;

            tp.Resources.RollbackTransaction(tp.CurrentTransToken);

            throw new Exception($"{this.GetType().Name}处理出错：" + ex.Message, ex.InnerException == null?ex:ex.InnerException);
        }

        protected override void Run(ParameterStd p, DataCollection d)
        {
            var tp = (TParameter)p;
            var td = (TData)d;

            InvokeBusiness(tp, td);
            AfterProcess(tp, td);

            tp.Resources.CommitTransaction(tp.CurrentTransToken);
        }
        #region Abstract
        
        /// <summary>
        /// 执行参数检查
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        protected abstract bool CheckMyParametersAndConfig(TParameter p, TData d);
        /// <summary>
        /// 根据参数集进行逻辑执行
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        protected abstract void InvokeBusiness(TParameter p, TData d);
        #endregion

        #region virtual
        /// <summary>
        /// 执行后的参数和结果集处理
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="d"></param>
        protected virtual void AfterProcess(TParameter p, TData d)
        {
            //do nothing
        }
        /// <summary>
        /// 系统启动时加载业务逻辑模块和其他自定义加载作业
        /// </summary>
        /// <param name="ma"></param>
        /// <param name="options"></param>
        /// <param name="logics"></param>
        protected virtual void OnLoadAssembly(ProxyManager ma, dynamic options, List<Type> logics)
        {
            //do nothing
        }
        #endregion
    }
}
