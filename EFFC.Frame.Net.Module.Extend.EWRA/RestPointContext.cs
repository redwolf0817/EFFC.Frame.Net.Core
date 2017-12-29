using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Module.Extend.EWRA.DataCollections;
using EFFC.Frame.Net.Module.Extend.EWRA.Logic;
using EFFC.Frame.Net.Module.Extend.EWRA.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace EFFC.Frame.Net.Module.Extend.EWRA
{
    /// <summary>
    /// RestAPI入口节点上下文结构，用于解析路由
    /// </summary>
    internal class RestPointContext
    {
        Dictionary<string, EntryPointEntity> _d_entry_ = new Dictionary<string, EntryPointEntity>();

        private RestPointContext()
        {
        }
        /// <summary>
        /// 当前主版本号
        /// </summary>
        public string MainVersion
        {
            get;
            internal protected set;
        }
        /// <summary>
        /// 总路由表描述
        /// </summary>
        public FrameDLRObject RouteDesc
        {
            get;
            internal protected set;
        }
        /// <summary>
        /// 主版本访问路由
        /// </summary>
        public FrameDLRObject MainRouteDesc
        {
            get;
            internal protected set;
        }
        /// <summary>
        /// 根据版本号获取RestAPI调用说明
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public FrameDLRObject GetRouteDesc(string version)
        {
            return RouteDesc.GetValue(version) == null ? null : (FrameDLRObject)RouteDesc.GetValue(version);
        }
        
        public List<RouteInvokeEntity> FindByRoute(string route, string verb, ref object[] parsedParams)
        {
            var sary = route.Replace("\\", "/").Split('/').Where(p => p != "").ToList();
            if (sary.Count <= 0) return null;

            //搜索版本号
            var apiversion = MainVersion;
            if (sary[0].StartsWith("v"))
            {
                apiversion = sary[0];
                sary.RemoveAt(0);
            }

            var entry = this[apiversion];
            if (entry == null) return null;

            return entry.FindByRoute(sary.ToArray(), verb,ref parsedParams);
        }
        /// <summary>
        /// 根据版本号和入口名称获取
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public EntryPointEntity this[string version]
        {
            get
            {
                var key = version.ToLower();
                return _d_entry_.ContainsKey(key) ? _d_entry_[key] : null;
            }
        }
        /// <summary>
        /// 创建Rest入口节点上下文
        /// </summary>
        /// <param name="assemblyName">logic所在的Assembly名称</param>
        /// <param name="mainversion">主版本号</param>
        /// <returns></returns>
        public static RestPointContext Create(string assemblyName, string mainversion,Type logicBaseType)
        {
            var rtn = new RestPointContext();
            rtn.MainVersion = mainversion;
            Assembly asm = Assembly.Load(new AssemblyName(assemblyName));
            Type[] ts = asm.GetTypes();
            var reg = new Regex(@"(?<=.)V\d+._\d+", RegexOptions.IgnoreCase);
            var list = ts.Where(p => p.GetTypeInfo().IsSubclassOf(logicBaseType)).ToList();
            foreach (var t in list.Where(p => p.GetTypeInfo().BaseType == logicBaseType))
            {
                var ple = EntryPointEntity.CreateFrom(t, list, logicBaseType);
                //如果为null，则不处理
                if (ple == null) continue;
                if (!rtn._d_entry_.ContainsKey(ple.APIVersion))
                {
                    rtn._d_entry_.Add(ple.APIVersion, ple);
                }
            }
            rtn.RouteDesc = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            rtn.MainRouteDesc = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            foreach (var item in rtn._d_entry_)
            {
                rtn.RouteDesc.SetValue(item.Value.APIVersion, item.Value.RouteDesc);
            }
            if(rtn.MainVersion == "")
            {
                var ver = "v0.0";
                foreach(KeyValuePair<string, object> item in rtn.RouteDesc.Items)
                {
                    var v = double.Parse(item.Key.Replace("v", ""));
                    if(v > double.Parse(ver.Replace("v", "")))
                    {
                        ver = item.Key;
                    }
                }
                rtn.MainVersion = ver;
            }
            if(rtn.RouteDesc.GetValue(rtn.MainVersion) != null)
            {
                rtn.MainRouteDesc = (FrameDLRObject)rtn.RouteDesc.GetValue(rtn.MainVersion);
            }
            else
            {
                rtn.MainRouteDesc.SetValue("warning","无可调用API");
            }
            

            return rtn;
        }
    }
}
