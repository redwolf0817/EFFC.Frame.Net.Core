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
    internal class DefaultRestRouteContext:ARestRouteContext
    {
        Dictionary<string, List<EntryPointEntity>> _d_entry_ = new Dictionary<string, List<EntryPointEntity>>();

        public DefaultRestRouteContext()
        {
        }
        /// <summary>
        /// 当前主版本号
        /// </summary>
        public override string MainVersion
        {
            get;
            protected set;
        }
        /// <summary>
        /// 总路由表描述
        /// </summary>
        public override FrameDLRObject RouteDesc
        {
            get;
            protected set;
        }
        /// <summary>
        /// 主版本访问路由
        /// </summary>
        public override FrameDLRObject MainRouteDesc
        {
            get;
            protected set;
        }

        /// <summary>
        /// 根据版本号获取RestAPI调用说明
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public override FrameDLRObject GetRouteDesc(string version)
        {
            return RouteDesc.GetValue(version) == null ? null : (FrameDLRObject)RouteDesc.GetValue(version);
        }
        
        public override List<RouteInvokeEntity> FindByRoute(string route, string verb, ref object[] parsedParams)
        {
            var sary = route.Replace("\\", "/").Split('/').Where(p => p != "").ToList();
            if (sary.Count <= 0) return null;

            //搜索版本号
            var apiversion = MainVersion;
            var reg = new Regex("v[1-9.]+");
            if (reg.IsMatch(sary[0]))
            {
                apiversion = sary[0];
                sary.RemoveAt(0);
            }

            var entry = this[apiversion];
            if (entry == null) return null;

            var rtn = new List<RouteInvokeEntity>();
            foreach (var item in entry)
            {
                var findroute = item.FindByRoute(sary.ToArray(), verb, ref parsedParams);
                if (findroute != null)
                    rtn.AddRange(findroute);
            }
            return rtn;
        }
        /// <summary>
        /// 根据版本号和入口名称获取
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public List<EntryPointEntity> this[string version]
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
        public override void Load(string assemblyName, string mainversion,Type logicBaseType)
        {
            MainVersion = mainversion;
            Assembly asm = Assembly.Load(new AssemblyName(assemblyName));
            Type[] ts = asm.GetTypes();
            var reg = new Regex(@"(?<=.)V\d+._\d+", RegexOptions.IgnoreCase);
            var list = ts.Where(p => p.GetTypeInfo().IsSubclassOf(logicBaseType)).ToList();
            foreach (var t in list.Where(p => p.GetTypeInfo().BaseType == logicBaseType))
            {
                var ple = EntryPointEntity.CreateFrom(t, list, logicBaseType);
                //如果为null，则不处理
                if (ple == null) continue;
                //在版本号下构建多入口节点
                if (!_d_entry_.ContainsKey(ple.APIVersion))
                {
                    var entrylist = new List<EntryPointEntity>();
                    entrylist.Add(ple);
                    _d_entry_.Add(ple.APIVersion, entrylist);
                }
                else
                {
                    _d_entry_[ple.APIVersion].Add(ple);
                }
            }
            RouteDesc = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            MainRouteDesc = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            foreach (var litem in _d_entry_)
            {
                foreach(var item in litem.Value)
                {
                    if (!RouteDesc.Keys.Contains(item.APIVersion))
                    {
                        RouteDesc.SetValue(item.APIVersion, item.RouteDesc);
                    }
                    else
                    {
                        var rd = (FrameDLRObject)RouteDesc.GetValue(item.APIVersion);
                        var index = rd.Keys.Count;
                        foreach(var v in item.RouteDesc.Items)
                        {
                            rd.SetValue($"NO.{index+1}", v.Value);
                            index++;
                        }
                    }
                   
                }
            }
            if(MainVersion == "")
            {
                var ver = "v0.0";
                foreach(KeyValuePair<string, object> item in RouteDesc.Items)
                {
                    var v = double.Parse(item.Key.Replace("v", ""));
                    if(v > double.Parse(ver.Replace("v", "")))
                    {
                        ver = item.Key;
                    }
                }
                MainVersion = ver;
            }
            if(RouteDesc.GetValue(MainVersion) != null)
            {
                MainRouteDesc = (FrameDLRObject)RouteDesc.GetValue(MainVersion);
            }
            else
            {
                MainRouteDesc.SetValue("warning","无可调用API");
            }
        }
    }
}
