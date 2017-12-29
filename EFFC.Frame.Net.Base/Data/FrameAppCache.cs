using EFFC.Frame.Net.Base.Interfaces.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Base.Data
{
    /// <summary>
    /// 框架默认的Cache,Application级别,介质为内存
    /// </summary>
    public class FrameAppCache:IFrameCache
    {
        static Dictionary<string, CacheEntity> _d = new Dictionary<string, CacheEntity>();
        static object _cache_lockobj = new object();
        /// <summary>
        /// 下次清理缓存的时间
        /// </summary>
        static DateTime nextcleardatetime = DateTime.Now;
        /// <summary>
        /// 每个多少分钟清理一次缓存
        /// </summary>
        static int clearminutes = 60;
        /// <summary>
        /// 新增数据，如果存在则更新,线程安全，到指定超时
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expira"></param>
        public void Set(string key, object obj, DateTime expira)
        {
            if (key == null) return;
            lock (_cache_lockobj)
            {
                if (!_d.ContainsKey(key))
                {
                    _d.Add(key, new CacheEntity(obj, expira, CacheExpiraType.DateTime, TimeSpan.Zero));
                }
                else
                {
                    _d[key].Value = obj;
                }
            }
        }
        /// <summary>
        /// 新增数据，如果存在则更新,线程安全,多长时间不用则超时
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="slide"></param>
        public void Set(string key, object obj, TimeSpan slide)
        {
            if (key == null) return;
            lock (_cache_lockobj)
            {
                if (!_d.ContainsKey(key))
                {
                    _d.Add(key, new CacheEntity(obj, DateTime.Now.AddTicks(slide.Ticks), CacheExpiraType.Slide, slide));
                }
                else
                {
                    _d[key].Value = obj;
                }
            }
        }
        /// <summary>
        /// 根据key获取数据，如果超时则返回null,线程安全
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(string key)
        {
            if (key == null) return null;
            lock (_cache_lockobj)
            {
                if (_d.ContainsKey(key))
                {
                    var e = _d[key];
                    if (e.Expira > DateTime.Now)
                    {
                        if (e.ExpiraType == CacheExpiraType.Slide)
                        {
                            e.Expira = DateTime.Now.AddTicks(e.Slide.Ticks);
                        }
                        AutoRemoveValue();
                        return e.Value;
                    }
                    else
                    {
                        _d.Remove(key);
                        AutoRemoveValue();
                        return null;
                    }
                }
                else
                {
                    AutoRemoveValue();
                    return null;
                }
            }
        }
        /// <summary>
        /// 超时自动清理
        /// </summary>
        private void AutoRemoveValue()
        {
            
            var t = Task.Run(() =>
            {
                lock (_cache_lockobj)
                {
                    if (nextcleardatetime <= DateTime.Now)
                    {
                        var strarr = new string[_d.Keys.Count];
                        _d.Keys.CopyTo(strarr, 0);
                        foreach (var key in strarr)
                        {
                            if (_d[key].Expira > DateTime.Now)
                            {
                                _d.Remove(key);
                            }
                        }

                        nextcleardatetime = nextcleardatetime.AddMinutes(clearminutes);
                    }
                }
            });
        }

        /// <summary>
        /// 移除缓存,线程安全
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            lock (_cache_lockobj)
            {
                _d.Remove(key);
            }
        }
        private enum CacheExpiraType
        {
            /// <summary>
            /// 绝对超时方式
            /// </summary>
            DateTime,
            /// <summary>
            /// 过多长时间不用则超时
            /// </summary>
            Slide
        }
        private class CacheEntity
        {
            public CacheEntity(object value, DateTime expira, CacheExpiraType type, TimeSpan slide)
            {
                Value = value;
                Expira = expira;
                ExpiraType = type;
                Slide = slide;
            }
            public object Value
            {
                get;
                set;
            }
            public DateTime Expira
            {
                get;
                set;
            }

            public CacheExpiraType ExpiraType
            {
                get;
                set;
            }
            public TimeSpan Slide
            {
                get;
                set;
            }

        }


    }
}
