using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Module.Extend.EWRA.DataCollections;
using EFFC.Frame.Net.Module.Extend.EWRA.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EWRA.Logic
{
    public partial class RestLogic
    {
       /// <summary>
       /// 设置需要刷新的缓存列表，只有post，put，patch才有效
       /// </summary>
       /// <param name="route">待缓存的get方法对应的路由路径，格式为：/xxxx/{参数名称}</param>
        public void SetRefreshCacheRoute(params string[] route)
        {
            if (route != null)
                CallContext_DataCollection.RefreshCacheList.AddRange(route);
        }
        /// <summary>
        /// 使用自定义方法进行校验，系统自动设定返回状态码
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public bool IsValidBy(string msg, Func<bool> f)
        {
            if (f.Invoke())
            {
                return true;
            }
            else
            {
                CallContext_DataCollection.StatusCode = Constants.RestStatusCode.INVALID_REQUEST;
                CallContext_DataCollection.Error = msg;
                return false;
            }
        }
        /// <summary>
        /// 使用自定义方法进行校验，系统自动设定返回状态码
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public bool IsValidBy(Func<StringBuilder,bool> f)
        {
            StringBuilder msg = new StringBuilder();
            if (f.Invoke(msg))
            {
                return true;
            }
            else
            {
                CallContext_DataCollection.StatusCode = Constants.RestStatusCode.INVALID_REQUEST;
                CallContext_DataCollection.Error = msg.ToString();
                return false;
            }
        }
        /// <summary>
        /// 设定是否做缓存，默认开启
        /// </summary>
        /// <param name="isEnable"></param>
        public void SetCacheEnable(bool isEnable)
        {
            CallContext_DataCollection.IsCache = isEnable;
        }

        /// <summary>
        /// 呼叫本地logic
        /// </summary>
        /// <param name="url">请求的路由，格式：/xxxx/yyyy</param>
        /// <param name="method">请求的method,post,get,patch等</param>
        /// <param name="header">额外附加的header信息</param>
        /// <param name="query_string">额外附加的query_string信息</param>
        /// <param name="post_data">额外附加的post_data信息</param>
        /// <returns></returns>
        public object CallLocalLogic(string url,string method, FrameDLRObject header = null,FrameDLRObject query_string = null ,FrameDLRObject post_data = null)
        {
            object obj = new EWRAData();
            var dp = CallContext_Parameter.DeepCopy<EWRAParameter>();

            dp.RequestUri = new Uri(ServerInfo.SiteHostUrl + url);
            dp.RequestRoute = url;
            dp.RestResourcesArray = url.Split('/');
            dp.MethodName = method;

            if (header != null)
            {
                foreach (var item in header.Items)
                {
                    dp.SetValue(DomainKey.HEADER, item.Key, item.Value);
                }
            }
            if (query_string != null)
            {
                foreach (var item in query_string.Items)
                {
                    dp.SetValue(DomainKey.QUERY_STRING, item.Key, item.Value);
                }
            }
            if (post_data != null)
            {
                foreach (var item in post_data.Items)
                {
                    dp.SetValue(DomainKey.POST_DATA, item.Key, item.Value);
                }
            }

            GlobalCommon.Proxys["busi"].CallModule(ref obj, new object[] { dp });
            return ((EWRAData)obj).Result;
        }
    }
}
