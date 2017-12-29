using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
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
    }
}
