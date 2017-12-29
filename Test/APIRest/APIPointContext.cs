using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using static Test.APIRest.EntryPointEntity;

namespace Test.APIRest
{
    public class APIPointContext
    {
        Dictionary<string,EntryPointEntity> _d_entry_ = new Dictionary<string, EntryPointEntity>();

        private APIPointContext()
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

        public FrameDLRObject GetRouteDesc(string version)
        {
            return RouteDesc.GetValue(version) == null ? null : (FrameDLRObject)RouteDesc.GetValue(version);
        }
        public object Invoke(string route,string verb)
        {
            var sary = route.ToLower().Replace("\\", "/").Split('/').Where(p => p != "").ToList();
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
            return entry.Invoke(sary.ToArray(), verb);
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

        public static APIPointContext Create(string assemblyName, string mainversion)
        {
            var rtn = new APIPointContext();
            rtn.MainVersion = mainversion;
            Assembly asm = Assembly.Load(new AssemblyName(assemblyName));
            Type[] ts = asm.GetTypes();
            var reg = new Regex(@"(?<=.)V\d+._\d+", RegexOptions.IgnoreCase);
            var list = ts.Where(p => p.GetTypeInfo().IsSubclassOf(typeof(PointLogic))).ToList();
            foreach (var t in list.Where(p => p.GetTypeInfo().BaseType == typeof(PointLogic)))
            {
                var ple = EntryPointEntity.CreateFrom(t, list);
                if (!rtn._d_entry_.ContainsKey(ple.APIVersion))
                {
                    rtn._d_entry_.Add(ple.APIVersion, ple);
                }
            }
            rtn.RouteDesc = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            rtn.MainRouteDesc = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            foreach(var item in rtn._d_entry_)
            {
                rtn.RouteDesc.SetValue(item.Value.APIVersion, item.Value.RouteDesc);
            }
            rtn.MainRouteDesc = (FrameDLRObject)rtn.RouteDesc.GetValue(rtn.MainVersion);
            
            return rtn;
        }
    }
}
