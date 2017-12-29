using EFFC.Frame.Net.Base.Interfaces.System;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Base.Cache
{
    /// <summary>
    /// 框架默认的Cache-内存级的
    /// </summary>
    public class DefaultCache : IFrameCache
    {
        static object lockobj = new object();
        MemoryCache _cache = null;
        public DefaultCache()
        {
            _cache = new MemoryCache(new MemoryCacheOptions()
            {

            });
        }
        public object Get(string key)
        {
            object rtn = null;
            if (_cache.TryGetValue(key, out rtn))
            {
                return rtn;
            }
            else
            {
                return null;
            }
        }

        public void Remove(string key)
        {
            lock (lockobj)
            {
                _cache.Remove(key);
            }
            
        }

        public void Set(string key, object obj, DateTime expira)
        {
            lock (lockobj)
            {
                _cache.Set(key, obj, expira);
            }
        }

        public void Set(string key, object obj, TimeSpan slide)
        {
            lock (lockobj)
            {
                _cache.Set(key, obj, slide);
            }
        }
    }
}
