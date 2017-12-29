using System;
using EFFC.Frame.Net.Global;

namespace EFFC.Frame.Net.Business.Logic
{
    public partial class BaseLogic<P, D>
    {
        private FrameCache _cache = new FrameCache();
        /// <summary>
        /// cache操作
        /// </summary>
        public virtual FrameCache CacheHelper
        {
            get { return _cache; }
        }

        public class FrameCache
        {
            /// <summary>
            /// 写入缓存,默认30分钟后超时
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public void SetCache(string key, object value)
            {
                SetCache(key, value, 30);
            }
            /// <summary>
            /// 写入缓存
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <param name="expirationMinites"></param>
            public void SetCache(string key, object value, int expirationMinites)
            {
                GlobalCommon.ApplicationCache.Set(key,value, DateTime.Now.AddMinutes(expirationMinites));
            }
            /// <summary>
            /// 写入缓存
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <param name="expirationtime"></param>
            public void SetCache(string key, object value, DateTime expirationtime)
            {
                GlobalCommon.ApplicationCache.Set(key, value, expirationtime);
            }
            /// <summary>
            /// 写入缓存（超过指定时间没用则过期）
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <param name="slide"></param>
            public void SetCache(string key, object value, TimeSpan slide)
            {
                GlobalCommon.ApplicationCache.Set(key, value, slide);
            }
            /// <summary>
            /// 从缓存中获取数据
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public object GetCache(string key)
            {
                return GlobalCommon.ApplicationCache.Get(key);
            }
            /// <summary>
            /// 移除缓存
            /// </summary>
            /// <param name="key"></param>
            public void RemoveCache(string key)
            {
                GlobalCommon.ApplicationCache.Remove(key);
            }
        }
    }
}
