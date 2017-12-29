using EFFC.Frame.Net.Module.Extend.EWRA.Constants;
using EFFC.Frame.Net.Module.Web.Datas;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EWRA.DataCollections
{
    /// <summary>
    /// EWRA的专用数据集
    /// </summary>
    public class EWRAData:RestDataCollection
    {
        public EWRAData()
        {
            StatusCode = RestStatusCode.NONE;
            ContentType = RestContentType.JSON;
            RefreshCacheList = new List<string>();
            IsCache = true;
        }
        /// <summary>
        /// 返回的结果状态码
        /// </summary>
        public RestStatusCode StatusCode
        {
            get;
            set;
        }
        /// <summary>
        /// 返回的contenttype
        /// </summary>
        public RestContentType ContentType
        {
            get;
            set;
        }
        /// <summary>
        /// 通过route搜索出来待执行的Method
        /// </summary>
        internal MethodInfo InvokeMethod
        {
            get;
            set;
        }
        /// <summary>
        /// 执行用的参数集
        /// </summary>
        internal object[] InvokeParameters
        {
            get;
            set;
        }
        /// <summary>
        /// 当出现4xx的错误时的错误信息内容
        /// </summary>
        public string Error
        {
            get;
            set;
        }
        /// <summary>
        /// 当为2xx时的返回的代码值
        /// </summary>
        public string Code
        {
            get;
            set;
        }
        /// <summary>
        /// 待处理缓存的列表
        /// </summary>
        public List<string> RefreshCacheList
        {
            get;
            internal protected set;
        }
        /// <summary>
        /// 本次get是否做cache,默认为true
        /// </summary>
        public bool IsCache
        {
            get;
            set;
        }
    }
}
