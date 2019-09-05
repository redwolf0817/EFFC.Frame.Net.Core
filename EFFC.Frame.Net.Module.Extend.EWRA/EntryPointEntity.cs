using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Module.Extend.EWRA.Attributes;
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
    /// RESTAPI入口节点定义
    /// </summary>
    internal class EntryPointEntity
    {
        static Regex _reg_version_ = new Regex(@"(?<=.)v\d+(._\d+){0,1}", RegexOptions.IgnoreCase);
        static Regex _reg_brace_p_ = new Regex(@"\{[A-Za-z0-9_]+\}", RegexOptions.IgnoreCase);
        static string[] _method_verbs_ = new string[] { "get", "post", "put", "patch", "delete" };
        static string _parameter_reg_express ="[^/]+";
        /// <summary>
        /// Logic的基类，为RestLogic的子类，用于建构时的扩展识别
        /// </summary>
        static Type LType = null;

        Dictionary<string, List<RouteInvokeEntity>> _d_invoke_ = new Dictionary<string, List<RouteInvokeEntity>>();
        /// <summary>
        /// verb+route资源个数做key映射一个索引,该索引可以用于通过url进行执行列表的查找功能
        /// </summary>
        Dictionary<string, string> _d_route_index_ = new Dictionary<string, string>();
        /// <summary>
        /// 无效的关键字，用于路由搜索
        /// 覆盖范围：logic的name，{p}
        /// </summary>
        List<string> _invalid_keys_ = new List<string>();
        private EntryPointEntity() { }
        /// <summary>
        /// 入口节点的API版本号
        /// </summary>
        public string APIVersion
        {
            get;
            internal protected set;
        }
        /// <summary>
        /// 入口节点名称
        /// </summary>
        public string Name
        {
            get;
            internal protected set;
        }
        /// <summary>
        /// API调用结构说明
        /// </summary>
        public FrameDLRObject RouteDesc
        {
            get;
            internal protected set;
        }

        /// <summary>
        /// 根据请求的url资源和verb抓取执行列表
        /// </summary>
        /// <param name="sarray"></param>
        /// <param name="verb"></param>
        /// <param name="parsedParams"></param>
        /// <returns></returns>
        internal List<RouteInvokeEntity> FindByRoute(string[] sarray, string verb,ref object[] parsedParams)
        {
            var paramValues = new List<object>();
            var key = $"{verb.ToLower()}:{sarray.Length}:";
            for (var i = 0; i < sarray.Length; i++)
            {
                //if (_invalid_keys_.Contains(sarray[i].ToLower()))
                if (_invalid_keys_.Contains(sarray[i]))
                {
                    //key += $"{sarray[i].ToLower()}@{i}|";
                    key += $"{sarray[i]}@{i}|";
                }
                else
                {
                    paramValues.Add(sarray[i]);
                }
            }

            if (!_d_route_index_.ContainsKey(key)) return null;

            parsedParams = paramValues.ToArray();
            return _d_invoke_[_d_route_index_[key]];
        }
        /// <summary>
        /// 根据Type类型和Type列表创建入口节点
        /// </summary>
        /// <param name="t"></param>
        /// <param name="pointTypeList"></param>
        /// <param name="baseType">为RestLogic的子类，如果为空或不为RestLogic子类，则自动识别为RestLogic</param>
        /// <returns></returns>
        internal static EntryPointEntity CreateFrom(Type t, List<Type> pointTypeList, Type baseType)
        {
            if(LType == null)
            {
                if(baseType != null && baseType.GetTypeInfo().IsSubclassOf(typeof(RestLogic)))
                {
                    LType = baseType;
                }
                else
                {
                    LType = typeof(RestLogic);
                }
            }
           
            if (t.GetTypeInfo().BaseType != baseType) return null;

            var rtn = new EntryPointEntity();
            var tmp = (RestLogic)Activator.CreateInstance(t, true);
            //版本号
            var ns = t.Namespace;
            if (_reg_version_.IsMatch(ns))
            {
                rtn.APIVersion = _reg_version_.Match(ns).Value.ToLower().Replace("_", "");
            }
            rtn.Name = tmp.Name;
            //建立树形调用链式结构
            rtn.BuildEntryRouteInvokeLink(t, pointTypeList);
            //建立基于RouteAttribute描述的独立入口
            rtn.BuildSingleEntryInvokeLink(t, pointTypeList);

            //建立API Description Doc
            rtn.RouteDesc = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            var index = 1;
            foreach (var item in rtn._d_invoke_)
            {
                var key = $"NO.{index}";
                var v = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                v.Desc = item.Value.Last().RouteDesc;
                v.Verb = item.Key.Split(':')[0];
                v.Route = item.Key.Split(':')[1];
                v.Inputs =  (from tt in item.Value.Last().InputItems
                            select new
                            {
                                tt.Name,
                                tt.Desc,
                                tt.ValueType,
                                tt.DefaultValue,
                                tt.Position,
                                tt.IsAllowEmpty
                            }).ToList();
                var outputitem = item.Value.Last().OutputDesc;
                v.Output = new { outputitem.Desc, outputitem.FormatDesc, outputitem.ReturnType };
                if (item.Value.Last().IsVisible)
                {
                    rtn.RouteDesc.SetValue(key, v);
                }

                index++;
            }
            //通过{verb+route+keyname@keyname所在位置}做key映射一个索引
            var list = new List<string>();
            foreach (var item in rtn._d_invoke_)
            {
                var verb = item.Key.Split(':')[0];
                var url = item.Key.Split(':')[1];
                var sary = url.Split('/').Where(p => p != "");
                var express = _reg_brace_p_.Replace(url, @"{p}").Substring(1);

                //关键字
                var keynames = express.Split('/',StringSplitOptions.RemoveEmptyEntries);
                list.AddRange(keynames);

                var key = $"{verb}:{sary.Count()}:";
                for (var i = 0; i < keynames.Count(); i++)
                {
                    if (keynames[i] != "{p}")
                    {
                        key += $"{keynames[i]}@{i}|";
                    }
                }
                if (!rtn._d_route_index_.ContainsKey(key))
                {
                    rtn._d_route_index_.Add(key, item.Key);
                }
            }

            rtn._invalid_keys_ = list.Distinct().ToList();

            return rtn;
        }
        /// <summary>
        /// 构建入口路由
        /// </summary>
        /// <param name="t"></param>
        /// <param name="pointTypeList"></param>
        private void BuildEntryRouteInvokeLink(Type t, List<Type> pointTypeList)
        {
            var methods = t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(p => _method_verbs_.Contains(p.Name.ToLower())
                                    && (p.DeclaringType == t || p.DeclaringType == LType) && p.GetCustomAttribute(typeof(EWRARouteAttribute)) == null);
            var subtypes = pointTypeList.Where(p => p.GetTypeInfo().BaseType == t).ToList();
            var instance = (RestLogic)Activator.CreateInstance(t, true);
            var tmpdic = new Dictionary<string, List<RouteInvokeEntity>>();

            foreach (var item in methods)
            {
                //入口logic不可拥有名称开头为parent_的参数
                if (item.GetParameters().Where(p => p.Name.StartsWith("parent_")).Count() > 0) continue;
                //参数类型只能为string
                if (item.GetParameters().Where(p => p.ParameterType != typeof(string)).Count() > 0) continue;
                //判断该method是否对外开放，无EWRAIsOpenAttribute属性或IsOpen==false的才开放
                var isopenattr = item.GetCustomAttribute<EWRAIsOpenAttribute>(false);
                if(isopenattr !=null && !isopenattr.IsOpen)
                {
                    continue;
                }


                var entity = new RouteInvokeEntity();
                entity.InstanceType = t;
                entity.InvokeMethod = item;
                entity.InvokeName = item.Name.ToLower();

                entity.Route = $"/{instance.Name}";
                entity.ParameterCountWithOutParent = 0;
                entity.HasParentParameter = false;
                foreach (var p in entity.InvokeMethod.GetParameters())
                {
                    entity.Route += $"/{{{p.Name}}}";
                    entity.ParameterCountWithOutParent++;
                }
                var attrdesc = entity.InvokeMethod.GetCustomAttribute<EWRARouteDescAttribute>();
                entity.RouteDesc = attrdesc == null ? "" : attrdesc.Desc;
                entity.RouteRegExpress = _reg_brace_p_.Replace(entity.Route, _parameter_reg_express);
                //是否可见
                var attrvisible = entity.InvokeMethod.GetCustomAttribute<EWRAVisibleAttribute>();
                if (attrvisible != null)
                {
                    entity.IsVisible = attrvisible.IsVisible;
                }
                else
                {
                    entity.IsVisible = true;
                }
                //添加参数描述
                var attrinputs = entity.InvokeMethod.GetCustomAttributes<EWRAAddInputAttribute>();
                if (attrinputs != null)
                {
                    foreach (var attr in attrinputs)
                    {
                        entity.AddInputItem(attr);
                    }
                }
                //添加返回数据的描述
                var attroutput = entity.InvokeMethod.GetCustomAttribute<EWRAOutputDescAttribute>();
                if (attroutput != null)
                {
                    entity.OutputDesc.Desc = attroutput.Desc;
                    entity.OutputDesc.FormatDesc = attroutput.FormatDesc;
                    entity.OutputDesc.ReturnType = attroutput.ReturnType;
                }



                var key = $"{entity.InvokeName}:{entity.Route}";
                if (_d_invoke_.ContainsKey(key))
                {
                    if (entity.InvokeMethod.DeclaringType == t && _d_invoke_[key][0].InvokeMethod.DeclaringType != t)
                    {
                        _d_invoke_[key][0] = entity;
                        tmpdic[key][0] = entity;
                    }
                }
                else
                {
                    var list = new List<RouteInvokeEntity>();
                    list.Add(entity);
                    _d_invoke_.Add(key, list);
                    tmpdic.Add(key, list);
                }

            }
            foreach (var item in tmpdic)
            {
                //搜索子类
                foreach (var subt in subtypes)
                {
                    BuildNextRouteInvokeLink(subt, item.Key, item.Value, pointTypeList);
                }
            }
            tmpdic.Clear();
            //子类独立入口路径建立
            foreach (var subt in subtypes)
            {
                BuildEntryRouteInvokeLink(subt, pointTypeList);
            }
        }
        /// <summary>
        /// 构建next执行链接
        /// </summary>
        /// <param name="t"></param>
        /// <param name="parentKey"></param>
        /// <param name="entryRIE"></param>
        /// <param name="pointTypeList"></param>
        private void BuildNextRouteInvokeLink(Type t, string parentKey, List<RouteInvokeEntity> entryRIE, List<Type> pointTypeList)
        {
            var methods = t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(p => _method_verbs_.Contains(p.Name.ToLower())
                                    && (p.DeclaringType == t || p.DeclaringType == LType) && p.GetCustomAttribute(typeof(EWRARouteAttribute)) == null);
            var subtypes = pointTypeList.Where(p => p.GetTypeInfo().BaseType == t).ToList();

            RouteInvokeEntity parentRIE = entryRIE[entryRIE.Count - 1];
            var instance = (RestLogic)Activator.CreateInstance(t, true);

            foreach (var item in methods)
            {
                //parent_开头的参数只能有一个
                if (item.GetParameters().Where(p => p.Name.StartsWith("parent_")).Count() != 1) continue;
                if (parentRIE.InvokeName != "get") continue;
                var parent_p = item.GetParameters().Where(p => p.Name.StartsWith("parent_")).First();
                if (parentRIE.InvokeMethod.ReturnType != parent_p.ParameterType) continue;
                //参数类型只能为string
                if (item.GetParameters().Where(p => p.ParameterType != typeof(string) && !p.Name.StartsWith("parent_")).Count() > 0) continue;

                //判断该method是否对外开放，无EWRAIsOpenAttribute属性或IsOpen==false的才开放
                var isopenattr = item.GetCustomAttribute<EWRAIsOpenAttribute>(false);
                if (isopenattr != null && !isopenattr.IsOpen)
                {
                    continue;
                }

                var list = new List<RouteInvokeEntity>();

                list.AddRange(entryRIE);

                var entity = new RouteInvokeEntity();
                entity.InstanceType = t;
                entity.InvokeMethod = item;
                entity.InvokeName = item.Name.ToLower();
                entity.Route = $"/{instance.Name}";
                entity.ParameterCountWithOutParent = 0;
                entity.HasParentParameter = true;
                foreach (var p in entity.InvokeMethod.GetParameters().Where(p => !p.Name.StartsWith("parent_")))
                {
                    entity.Route += $"/{{{p.Name}}}";
                    entity.ParameterCountWithOutParent++;
                }
                var attrdesc = entity.InvokeMethod.GetCustomAttribute<EWRARouteDescAttribute>();
                entity.RouteDesc = attrdesc == null ? "" : attrdesc.Desc;
                entity.RouteRegExpress = _reg_brace_p_.Replace(entity.Route, _parameter_reg_express);
                //是否可见
                var attrvisible = entity.InvokeMethod.GetCustomAttribute<EWRAVisibleAttribute>();
                if (attrvisible != null)
                {
                    entity.IsVisible = attrvisible.IsVisible;
                }
                else
                {
                    entity.IsVisible = true;
                }
                //添加参数描述
                var attrinputs = entity.InvokeMethod.GetCustomAttributes<EWRAAddInputAttribute>();
                if(attrinputs != null)
                {
                    foreach(var attr in attrinputs)
                    {
                        entity.AddInputItem(attr);
                    }
                }
                //添加返回数据的描述
                var attroutput = entity.InvokeMethod.GetCustomAttribute<EWRAOutputDescAttribute>();
                if (attroutput != null)
                {
                    entity.OutputDesc.Desc = attroutput.Desc;
                    entity.OutputDesc.FormatDesc = attroutput.FormatDesc;
                    entity.OutputDesc.ReturnType = attroutput.ReturnType;
                }

                list.Add(entity);
                var key = $"{entity.InvokeName}:{parentKey.Split(':')[1]}{entity.Route}";
                _d_invoke_.Add(key, list);

                //搜索子类
                foreach (var subt in subtypes)
                {
                    BuildNextRouteInvokeLink(subt, key, list, pointTypeList);
                }

            }
        }
        /// <summary>
        /// 基于RouteAttribute描述构建自定义独立API入口
        /// </summary>
        /// <param name="t"></param>
        /// <param name="pointTypeList"></param>
        private void BuildSingleEntryInvokeLink(Type entryType, List<Type> pointTypeList)
        {
            var types = pointTypeList.Where(p => p == entryType || p.GetTypeInfo().IsSubclassOf(entryType));
            foreach (Type item in types)
            {
                var methods = item.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(p => p.GetCustomAttribute<EWRARouteAttribute>() != null);
                foreach (var m in methods)
                {
                    //参数类型只能为string
                    if (m.GetParameters().Where(p => p.ParameterType != typeof(string)).Count() > 0) continue;
                    //声明的类不是item的则不处理
                    if (m.DeclaringType != item) continue;

                    var attri = m.GetCustomAttribute<EWRARouteAttribute>();
                    if (attri.MethodVerb == "" || !_method_verbs_.Contains(attri.MethodVerb)) continue;

                    //判断该method是否对外开放，无EWRAIsOpenAttribute属性或IsOpen==false的才开放
                    var isopenattr = m.GetCustomAttribute<EWRAIsOpenAttribute>(false);
                    if (isopenattr != null && !isopenattr.IsOpen)
                    {
                        continue;
                    }

                    var list = new List<RouteInvokeEntity>();

                    var entity = new RouteInvokeEntity();
                    entity.InstanceType = item;
                    entity.InvokeMethod = m;
                    entity.InvokeName = attri.MethodVerb;
                    entity.Route = $"/{attri.Route}";
                    entity.ParameterCountWithOutParent = 0;
                    entity.HasParentParameter = false;
                    foreach (var p in entity.InvokeMethod.GetParameters())
                    {
                        //如果route中已经含有对应的参数定义，则不自动添加
                        if (!attri.Route.Contains($"/{{{p.Name}}}"))
                        {
                            entity.Route += $"/{{{p.Name}}}";
                        }
                        entity.ParameterCountWithOutParent++;
                    }
                    var attrdesc = entity.InvokeMethod.GetCustomAttribute<EWRARouteDescAttribute>();
                    entity.RouteDesc = attrdesc == null ? "" : attrdesc.Desc;
                    entity.RouteRegExpress = _reg_brace_p_.Replace(entity.Route, _parameter_reg_express);
                    //是否可见
                    var attrvisible = entity.InvokeMethod.GetCustomAttribute<EWRAVisibleAttribute>();
                    if(attrvisible != null)
                    {
                        entity.IsVisible = attrvisible.IsVisible;
                    }
                    else
                    {
                        entity.IsVisible = true;
                    }
                    //添加参数描述
                    var attrinputs = entity.InvokeMethod.GetCustomAttributes<EWRAAddInputAttribute>();
                    if (attrinputs != null)
                    {
                        foreach (var attr in attrinputs)
                        {
                            entity.AddInputItem(attr);
                        }
                    }
                    //添加返回数据的描述
                    var attroutput = entity.InvokeMethod.GetCustomAttribute<EWRAOutputDescAttribute>();
                    if (attroutput != null)
                    {
                        entity.OutputDesc.Desc = attroutput.Desc;
                        entity.OutputDesc.FormatDesc = attroutput.FormatDesc;
                        entity.OutputDesc.ReturnType = attroutput.ReturnType;
                    }

                    list.Add(entity);
                    var key = $"{attri.MethodVerb}:{attri.Route}";
                    if (!_d_invoke_.ContainsKey(key))
                        _d_invoke_.Add(key, list);
                    else
                    {
                        Console.WriteLine($"\n路由路径:{key}已经存在;相关信息:类名-{item.Name},方法名称-{m.Name},参数-{entity.Route.Replace($"/{entity.Route}", "").Replace("/", ",")}");
                        continue;
                    }
                }
            }
        }
        
    }
}
