using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Base.Interfaces.System
{
    public interface IFrameCache
    {
        /// <summary>
        /// 设置绝对超时的时间的缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="expira"></param>
        void Set(string key, object obj, DateTime expira);
        /// <summary>
        /// 设置过多长时间不用则超时
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="ts"></param>
        void Set(string key, object obj, TimeSpan slide);
        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object Get(string key);
        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);


    }
}
