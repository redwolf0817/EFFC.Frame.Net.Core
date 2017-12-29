using EFFC.Frame.Net.Base.Cache;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Interfaces.System;
using EFFC.Frame.Net.Base.Module.Proxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Global
{
    /// <summary>
    /// 框架全局应用参数和API设置
    /// </summary>
    public class GlobalCommon
    {
        static ILogger logger = null;
        static IExceptionProcess _ep = null;
        static IFrameCache _cache = new DefaultCache();
        static ProxyManager _proxys = new ProxyManager();

        /// <summary>
        /// Logger
        /// </summary>
        public static ILogger Logger
        {
            get
            {
                return logger;
            }

            set
            {
                logger = value;
            }
        }

        /// <summary>
        /// 異常處理
        /// </summary>
        public static IExceptionProcess ExceptionProcessor
        {
            get
            {
                return _ep;
            }
            set
            {
                _ep = value;
            }
        }
        /// <summary>
        /// 框架cache
        /// </summary>
        public static IFrameCache ApplicationCache
        {
            get
            {
                if (_cache == null)
                {
                    _cache = new FrameAppCache();
                }
                return _cache;
            }
            set
            {
                _cache = value;
            }
        }
        /// <summary>
        /// 模块代理管理器
        /// </summary>
        public static ProxyManager Proxys
        {
            get
            {
                return _proxys;
            }
        }
    }
}
