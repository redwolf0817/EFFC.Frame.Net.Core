using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Interfaces;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Parameter;
using System.Web;
using System.Web.Caching;

namespace EFFC.Frame.Net.Base.Module
{
    public class ModuleProxyManager
    {
        /// <summary>
        /// 同步调用一个模块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool Call<T>(ParameterStd p, DataCollection data) where T : IModularProxy
        {
            bool rtn = false;
            T t = default(T);
            try
            {
                t = (T)Activator.CreateInstance(typeof(T), true);
                rtn = t.CallModule(p, data);
            }
            catch (Exception ex)
            {
                t.OnError(ex, p, data);
            }
            return rtn;
        }
        /// <summary>
        /// 同步调用一个模块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <typeparam name="D"></typeparam>
        /// <param name="p"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool Call<T, P, D>(P p, D data) where T : IModularProxy<P, D>
            where P:ParameterStd
            where D:DataCollection
        {
            bool rtn = false;
            T t = default(T);
            try
            {
                t = (T)Activator.CreateInstance(typeof(T), true);
                rtn = t.CallModule(p, data);
            }
            catch (Exception ex)
            {
                t.OnError(ex, p, data);
            }
            return rtn;
        }
        /// <summary>
        /// 异步调用一个模块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <typeparam name="D"></typeparam>
        /// <param name="p"></param>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static T BeginCall<T, P, D>(P p, D data, Action<P,D> callback)
            where T : IModularAsyncProxy<P, D>
            where P : ParameterStd
            where D : DataCollection
        {
            bool rtn = false;
            T t = default(T);
            try
            {
                t = (T)Activator.CreateInstance(typeof(T), true);
                rtn = t.BeginCallModule(p, data, callback);
            }
            catch (Exception ex)
            {
                t.OnError(ex, p, data);
            }
            return t;
        }
        /// <summary>
        /// 结束异步呼叫
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <typeparam name="D"></typeparam>
        /// <param name="t"></param>
        /// <param name="p"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool EndCall<T, P, D>(T t, P p, D d)
            where T : IModularAsyncProxy<P, D>
            where P : ParameterStd
            where D : DataCollection
        {
            if (t != null)
            {
                return t.EndCallModule(p, d);
            }
            else
            {
                return false;
            }
        }

        public static void WaitCall<T, P, D>(T t, P p, D d)
            where T : IModularAsyncProxy<P, D>
            where P : ParameterStd
            where D : DataCollection
        {
            if (t != null)
            {
                t.WaitMe(p, d);
            }
        }
    }

    public class ModuleProxyManager<P, D>
        where P : ParameterStd
        where D : DataCollection
    {
        public static bool Call<T>(P p, D data) where T : IModularProxy<P, D>
        {
            bool rtn = false;
            T t = default(T);
            try
            {
                t = (T)Activator.CreateInstance(typeof(T), true);
                rtn = t.CallModule(p, data);
            }
            catch (Exception ex)
            {
                t.OnError(ex, p, data);
            }
            return rtn;
        }
    }
}
