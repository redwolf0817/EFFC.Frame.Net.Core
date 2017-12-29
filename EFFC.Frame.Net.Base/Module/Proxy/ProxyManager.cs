using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EFFC.Frame.Net.Base.Module.Proxy
{
    /// <summary>
    /// 模块代理管理器
    /// </summary>
    public class ProxyManager
    {
        Dictionary<string, ProxyEntity> _proxys = new Dictionary<string, ProxyEntity>();
        /// <summary>
        /// 添加一个代理模型
        /// </summary>
        /// <typeparam name="TProxy"></typeparam>
        /// <param name="name">代理名称</param>
        /// <param name="options">代理配置选项</param>
        /// <returns></returns>
        public ProxyManager UseProxy<TProxy>(string name="",dynamic options=null)
        {
            if (options == null) options = FrameDLRObject.CreateInstance();

            var t = typeof(TProxy);
            if (ComFunc.nvl(name) == "") name = t.Name.ToLower().Replace("proxy", "").Replace("module", "");
            if (!_proxys.ContainsKey(name))
            {
                _proxys.Add(name, new ProxyEntity(t));
                //模块执行初始化作业
                FrameExposedObject.From(this[name]).OnUsed(this, options);
            }

            return this;
        }
        /// <summary>
        /// 判断是否已存在指定名称的proxy
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasProxy(string name)
        {
            return _proxys.ContainsKey(name.ToLower());
        }
        /// <summary>
        /// 根据名称获取proxy实例
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ModuleProxy this[string name]
        {
            get
            {
                if (_proxys.ContainsKey(name.ToLower()))
                {
                    return (ModuleProxy)Activator.CreateInstance(_proxys[name.ToLower()].TProxy);
                }
                else
                {
                    throw new Exception("Not find proxy named " + name);
                }
            }
        }

        private class ProxyEntity
        {
            public ProxyEntity(Type t)
            {
                TProxy = t;
                var tmp = (ModuleProxy)Activator.CreateInstance(t, true);
            }
            /// <summary>
            /// 代理的类型
            /// </summary>
            public Type TProxy
            {
                get;
                set;
            }
        }
    }
}
