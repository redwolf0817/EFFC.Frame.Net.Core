using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.Interfaces;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Parameter;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Base.Module
{
    public abstract class LocalModuleProxy : IModularProxy,IModularAsyncProxy
    {
        abstract protected BaseModule GetModule(ParameterStd p, DataCollection data);
        protected virtual void AfterProcess(ParameterStd p, DataCollection data)
        {
            //do nothing
        }
        public bool CallModule(ParameterStd p, DataCollection data)
        {
            GetModule(p, data).StepStart(p, data);
            AfterProcess(p, data);
            return true;
        }

        public virtual void OnError(Exception ex, ParameterStd p, DataCollection data)
        {
            throw new Exception(ex.Message, ex);
        }
        /// <summary>
        /// 执行异步操作，在该模式下，参数集会复制使其线程独占，数据集则采用线程共享模式
        /// 异步操作处理时，不提供中断操作
        /// </summary>
        /// <param name="p"></param>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public bool BeginCallModule(ParameterStd p, DataCollection data, Action<ParameterStd, DataCollection> callback)
        {
            var task = Task.Run(() =>
            {
                var m = GetModule(p, data);
                m.StepStart(p, data);
                if (callback != null)
                {
                    callback.Invoke(p, data);
                }
            });
            p.ExtentionObj.async = task;
            return true;
        }
        /// <summary>
        /// 执行异步操作结束操作
        /// 等待作业处理完成，如果执行的过程中有异常，则会抛出异常
        /// </summary>
        /// <param name="p"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool EndCallModule(ParameterStd p, DataCollection data)
        {
            if (p.ExtentionObj.async != null && p.ExtentionObj.async is Task)
            {
                ((Task)p.ExtentionObj.async).Wait();
                if (((Task)p.ExtentionObj.async).Exception != null)
                {
                    throw ((Task)p.ExtentionObj.async).Exception;
                }
            }

            AfterProcess(p, data);
            return true;
        }

        /// <summary>
        /// 等待异步操作完成，执行过程中有异常则会引发异常的抛出
        /// </summary>
        /// <param name="p"></param>
        /// <param name="data"></param>
        public void WaitMe(ParameterStd p, DataCollection data)
        {
            if (p.ExtentionObj.async != null && p.ExtentionObj.async is Task)
            {
                ((Task)p.ExtentionObj.async).Wait();
            }
        }
    }

    public abstract class LocalModuleProxy<P, D> : IModularProxy<P, D>, IModularAsyncProxy<P,D>
        where P : ParameterStd
        where D : DataCollection
    {
        abstract protected BaseModule<P, D> GetModule(P p, D data);
        protected virtual void AfterProcess(P p, D data)
        {
            //do nothing
        }
        public bool CallModule(P p, D data)
        {
            GetModule(p, data).StepStart(p, data);
            AfterProcess(p, data);
            return true;
        }

        public abstract void OnError(Exception ex, P p, D data);
        /// <summary>
        /// 执行异步操作，在该模式下，参数集会复制使其线程独占，数据集则采用线程共享模式
        /// 异步操作处理时，不提供中断操作
        /// </summary>
        /// <param name="p"></param>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public bool BeginCallModule(P p, D data, Action<P, D> callback)
        {
            var task = Task.Run(() =>
            {
                try
                {
                    var m = GetModule(p, data);
                    m.StepStart(p, data);
                    if (callback != null)
                    {
                        callback.Invoke(p, data);
                    }
                    foreach (var k in p.TransTokenList.Keys)
                    {
                        p.Resources.CommitTransaction(p.TransTokenList[k]);
                    }
                }
                catch (Exception ex)
                {
                    foreach (var k in p.TransTokenList.Keys)
                    {
                        p.Resources.RollbackTransaction(p.TransTokenList[k]);
                    }
                    AsyncOnError(ex, p, data);
                }
                finally
                {
                    p.Resources.ReleaseAll();
                }
            });
            p.ExtentionObj.async = task;
            return true;
        }
        /// <summary>
        /// 执行异步操作结束操作
        /// 等待作业处理完成，如果执行的过程中有异常，则会抛出异常
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public bool EndCallModule(P p, D d)
        {
            if (p.ExtentionObj.async != null && p.ExtentionObj.async is Task)
            {
                ((Task)p.ExtentionObj.async).Wait();
                if (((Task)p.ExtentionObj.async).Exception != null)
                {
                    throw ((Task)p.ExtentionObj.async).Exception;
                }
            }
            AfterProcess(p, d);
            return true;
        }
        protected virtual void AsyncOnError(Exception ex, P p, D data)
        {
            //
        }
        /// <summary>
        /// 等待异步操作完成，执行过程中有异常则会引发异常的抛出
        /// </summary>
        /// <param name="p"></param>
        /// <param name="data"></param>
        public void WaitMe(P p, D data)
        {
            if (p.ExtentionObj.async != null && p.ExtentionObj.async is Task)
            {
                ((Task)p.ExtentionObj.async).Wait();
            }
        }
    }
}
