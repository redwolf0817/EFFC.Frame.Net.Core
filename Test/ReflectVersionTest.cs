using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Test.APIRest;

namespace Test
{
    public class ReflectVersionTest
    {
        static APIPointContext _context = null;
        public static void Test()
        {
            Console.OutputEncoding = Encoding.UTF8;
            var dt = DateTime.Now;
            //_context = PointContext.Create(Assembly.GetEntryAssembly().FullName,"v1.1");
            var mc = new MemoryCache(new MemoryCacheOptions()
            {
                
            });
            var url = "/my/logicb/T1";
            //var requestRes = url.Split('/');
            _context = APIPointContext.Create(Assembly.GetEntryAssembly().FullName, "v1.1");
            File.WriteAllText("d:/RestAPI.json" ,_context.RouteDesc.ToJSONString(),Encoding.UTF8);
            Console.WriteLine($"cast:{(DateTime.Now - dt).TotalMilliseconds}ms");dt = DateTime.Now;
            var er = _context.Invoke("/my/001/logicb/T1", "get");
            Console.WriteLine($"cast:{(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            var er2 = _context.Invoke("/my/dsasd/dsafs/logicb", "put");
            Console.WriteLine($"cast:{(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            var er3 = _context.Invoke("/my/dsasd/dsafs/logicb/he/logicc/ych", "get");
            Console.WriteLine($"cast:{(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            var er4 = _context.Invoke("//school/天津一小/student/小明", "get");
            Console.WriteLine($"cast:{(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;
            var er5 = _context.Invoke("/fsad/school/天津一小/student/小明", "get");
            Console.WriteLine($"cast:{(DateTime.Now - dt).TotalMilliseconds}ms"); dt = DateTime.Now;

            Console.Read();
        }

        internal class PointContext
        {
            Dictionary<string, Dictionary<string, PointLogicEntity>> _d_entry_ = new Dictionary<string, Dictionary<string, PointLogicEntity>>();

            private PointContext()
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

            public static PointContext Create(string assemblyName,string mainversion)
            {
                var rtn = new PointContext();
                rtn.MainVersion = mainversion;
                Assembly asm = Assembly.Load(new AssemblyName(assemblyName));
                Type[] ts = asm.GetTypes();
                var reg = new Regex(@"(?<=.)V\d+._\d+", RegexOptions.IgnoreCase);
                var list = ts.Where(p => p.GetTypeInfo().IsSubclassOf(typeof(EndPoint))).ToList();
                foreach (var t in list.Where(p => p.GetTypeInfo().BaseType == typeof(EndPoint)))
                {
                    var ple = PointLogicEntity.LoadSubpoint(t, list);
                    var key = $"{ple.APIVersion}-{ple.LogicName}";
                    if (!rtn._d_entry_.ContainsKey(ple.APIVersion))
                    {
                        rtn._d_entry_.Add(ple.APIVersion, new Dictionary<string, PointLogicEntity>());
                    }
                    rtn._d_entry_[ple.APIVersion].Add(ple.LogicName, ple);
                }
                rtn.RouteDesc = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                rtn.MainRouteDesc = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                foreach (var ver in rtn._d_entry_.Keys)
                {
                    rtn.RouteDesc.SetValue(ver, FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase));
                    dynamic vroute = rtn.RouteDesc.GetValue(ver);

                    foreach (var item in rtn._d_entry_[ver])
                    {
                        var index = 0;
                        foreach (var rm in item.Value.RouteTable)
                        {
                            index++;
                            var info = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                            info.Desc = rm.Value.RouteDesc;
                            info.Url =  rm.Key;
                            vroute.SetValue(ver + "-" + index, info);
                            if(ver == rtn.MainVersion)
                            {
                                var mvinfo = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                                mvinfo.Desc = rm.Value.RouteDesc;
                                mvinfo.Url = rm.Key.Replace($"/{ver}","");
                                rtn.MainRouteDesc.SetValue(ver + "-" + index, mvinfo);
                            }
                        }
                    }
                }

                return rtn;
            }
        }

        internal class PointLogicEntity
        {
            static Regex _reg_version_ = new Regex(@"(?<=.)v\d+._\d+", RegexOptions.IgnoreCase);
            static string[] _method_verbs_ = new string[] { "get", "post", "put", "patch","delete" };
            public PointLogicEntity(Type t)
            {
                LogicType = t;
                var tmp = (EndPoint)Activator.CreateInstance(t, true);
                LogicName = tmp.Name;
                Route = "/" + LogicName;
                var ns = LogicType.Namespace;
                if (_reg_version_.IsMatch(ns))
                {
                    APIVersion = _reg_version_.Match(ns).Value.ToLower().Replace("_", "");
                }
                var methods = t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(p => _method_verbs_.Contains(p.Name.ToLower()) 
                                    && (p.DeclaringType == t || p.DeclaringType == typeof(EndPoint)));
                RouteMethods = new Dictionary<string, RouteMethod>();
                foreach (var m in methods)
                {
                    
                    var rm = RouteMethod.Create(m);
                    if (rm == null) continue;
                    var key = rm.Key;
                    //优先获取本类型定义的同名方法，如果没有再才登记EndPoint的方法
                    if (RouteMethods.ContainsKey(key))
                    {
                        if(RouteMethods[key].Method.DeclaringType != t && m.DeclaringType == t)
                        {
                            RouteMethods[key] = rm;
                        }
                    }
                    else
                    {
                        RouteMethods.Add(key, rm);
                    }
                }

                tmp = null;
            }

            public static PointLogicEntity LoadSubpoint(Type t, List<Type> logics)
            {
                //递归获取subpoint，构建entrylogic
                var rtn = new PointLogicEntity(t);
                var subs = logics.Where(p => p.GetTypeInfo().BaseType == t);
                var list = new Dictionary<string, PointLogicEntity>();
                foreach (var sub in subs)
                {
                    var point = LoadSubpoint(sub, logics);
                    var key = point.LogicName;
                    list.Add(key, point);
                }
                rtn.SubPoints = list;

                //构建路由表-路由表只在入口logic中建立
                if (rtn.LogicType.GetTypeInfo().BaseType == typeof(EndPoint))
                {
                    var rlist = new Dictionary<string,RouteMethod>();
                    rtn.BuildRouteTable(rlist);
                    rtn.RouteTable = rlist;
                }

                return rtn;
            }
            protected void BuildRouteTable(Dictionary<string, RouteMethod> table, string parentname = "", string parentroute = "", Type parentMethodReturnType = null)
            {
                foreach (var m in RouteMethods)
                {
                    var routepath = "";
                    if (string.IsNullOrEmpty(parentroute))
                    {
                        routepath = Route + m.Value.Route;
                    }
                    else
                    {
                        if (m.Value.Route.StartsWith("#parent#"))
                        {
                            if (parentMethodReturnType != null && parentMethodReturnType == m.Value.ParameterType4Parent)
                            {
                                routepath = parentroute + Route + m.Value.Route.Replace("#parent#", "");
                            }
                        }
                        else
                        {
                            if (parentroute.IndexOf("{") < 0 || parentroute.IndexOf("}") < 0)
                                //routepath = parentroute + Route + m.Value.Route4Desc;
                                routepath = Route + m.Value.Route;
                        }

                    }
                    if (routepath != "")
                    {
                        var routekey = m.Value.Name + ":/" + this.APIVersion + routepath;
                        if (!table.ContainsKey(routekey))
                            table.Add(m.Value.Name + ":/" + this.APIVersion + routepath, m.Value);
                    }


                    //只有get方法才向下延伸
                    if (m.Value.Name == "get" && SubPoints != null && routepath != "")
                    {
                        foreach (var point in SubPoints)
                        {
                            point.Value.BuildRouteTable(table, parentname + Route, routepath, m.Value.Method.ReturnType);
                        }
                    }
                }

            }
            /// <summary>
            /// 判定是否含有subpoint
            /// </summary>
            public bool HasSubPoint
            {
                get
                {
                    return (SubPoints != null && SubPoints.Count > 0) ? true : false;
                }
            }

            public string LogicName
            {
                get;
                internal protected set;
            }
            public string Route
            {
                get;
                internal protected set;
            }
            public Dictionary<string,RouteMethod> RouteTable
            {
                get;
                internal protected set;
            }
            public Type LogicType
            {
                get;
                internal protected set;
            }

            public Dictionary<string, PointLogicEntity> SubPoints
            {
                get;
                internal protected set;
            }
            public Dictionary<string, RouteMethod> RouteMethods
            {
                get;
                internal protected set;
            }

            public string APIVersion
            {
                get;
                internal protected set;
            }
        }

        internal class RouteMethod
        {
            private RouteMethod()
            {                 
            }

            public static RouteMethod Create(MethodInfo mi)
            {
                //参数名称中含有parent_开头的表示该接口需要父类的资料，路由中则父类需要带参数的形式
                if (mi.GetParameters().Where(p => p.ParameterType != typeof(string)
                        && (p.Name.ToLower().StartsWith("parent_") == false)).Count() > 0) return null;
                //parent_开头的参数只允许有一个
                if (mi.GetParameters().Where(p => p.Name.ToLower().StartsWith("parent_")).Count() > 1) return null;

                var rtn = new RouteMethod();
                rtn.Name = mi.Name.ToLower();
                rtn.Method = mi;
                var r = new StringBuilder();
                foreach (var p in mi.GetParameters())
                {
                    if (p.ParameterType == typeof(string))
                    {
                        r.Append("/{" + p.Name + "}");
                    }
                }
                var pparam = mi.GetParameters().Where(p => p.Name.ToLower().StartsWith("parent_"));
                if (pparam.Count() > 0)
                {
                    r.Insert(0, "#parent#");

                    rtn.ParameterType4Parent = pparam.First().ParameterType;
                }
                rtn.Route = r.ToString();
                
                rtn.Key = GetRouteMethodKey(mi);
                var rattr = rtn.Method.GetCustomAttribute<RouteDescAttribute>();
                if (rattr == null)
                {
                    rtn.RouteDesc = rtn.Key;
                }
                else
                {
                    rtn.RouteDesc = rattr.Desc;
                }

                return rtn;
            }
            protected static string GetRouteMethodKey(MethodInfo mi)
            {
                var sb = new StringBuilder();
                foreach (var p in mi.GetParameters())
                {
                    sb.Append($"[{p.ParameterType.Name.ToLower()},{p.Name}]|");
                }
                var key = $"{mi.Name.ToLower()}|{mi.GetParameters().Count()}|{(sb.Length > 0 ? sb.ToString().Substring(0, sb.Length - 1) : "")}";
                return key;
            }
            public MethodInfo Method
            {
                get;
                internal protected set;
            }

            public string Name
            {
                get;
                internal protected set;
            }
            public string Route
            {
                get;
                internal protected set;
            }
            public string RouteDesc
            {
                get;
                internal protected set;
            }
            public string Key
            {
                get;
                internal protected set;
            }

            public Type ParameterType4Parent
            {
                get;
                internal protected set;
            }

        }
    }
}
