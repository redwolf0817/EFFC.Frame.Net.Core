/*
 * Used for EFFC.Frame 1.0 & EFFC.Frame 2.5
 * Added by chuan.yin in 2014/11/13
 */
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Interfaces.DataConvert;
using Frame.Net.Base.Interfaces.DataConvert;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EFFC.Frame.Net.Base.Data.Base
{
    /// <summary>
    /// 动态数据对象，对象属性的为忽略大小的方式
    /// </summary>
    [Serializable]
    public class FrameDLRObject : MyDynamicMetaProvider, IJSONable, IDisposable, ICloneable
    {
        static object lockobj = new object();
        static int jseexpire_min = 30;
        static DateTime createTime = DateTime.Now;
        string tabflag = "  ";
        bool ignorecase = true;
        string ori_jsongstring = "";
        string _cuid = "";
        string _duid = "";
        Dictionary<string, object> _valuelist = new Dictionary<string, object>();
        /// <summary>
        /// 设定或读取是否忽略大小写
        /// </summary>
        public bool IgnoreCase
        {
            get
            {
                return ignorecase;
            }
        }
        /// <summary>
        /// 获取创建时的标签设定
        /// </summary>
        public FrameDLRFlags Flags
        {
            get
            {
                return IgnoreCase ? FrameDLRFlags.None : FrameDLRFlags.SensitiveCase;
            }
        }
        /// <summary>
        /// 获取规范格式化的json定义串
        /// </summary>
        [Obsolete("暂时取消，不再使用", false)]
        public string OriJsonString
        {
            get
            {
                //if (ori_jsongstring == "")
                //{
                //    ori_jsongstring = ToJSON_Structure();
                //}
                return ori_jsongstring;
            }
        }
        /// <summary>
        /// 获取OriJsonString时生成的数据集（为非结构化数据总集）
        /// </summary>
        public Dictionary<string, object> SummaryValueList
        {
            get
            {
                return _valuelist;
            }
        }

        /// <summary>
        /// 对象定义唯一标示码，该码源自定义时使用的json串，用于标示本对象的最原始定义的唯一性
        /// </summary>
        [Obsolete("暂时取消，不再使用", false)]
        public string DUID
        {
            get
            {
                //if (_duid == "")
                //{
                //    _duid = ComFunc.getMD5_String(OriJsonString, Encoding.UTF8);
                //}
                return _duid;
            }
        }
        /// <summary>
        /// 获取结构唯一标示码，该码源自结构化定义json，用于标示本对象的结构定义唯一性
        /// </summary>
        [Obsolete("暂时取消，不再使用", false)]
        public string CUID
        {
            get
            {
                //if (_cuid == "")
                //{
                //    var s = ToJSON_Structure();
                //    _cuid = ComFunc.getMD5_String(s, Encoding.UTF8);
                //}
                return _cuid;
            }
        }
        protected Dictionary<string, object> _d = new Dictionary<string, object>();

        /// <summary>
        /// 根据json创建数组对象
        /// </summary>
        /// <param name="arrjsonstring"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static object[] CreateArray(string arrjsonstring, FrameDLRFlags flag = FrameDLRFlags.None, params KeyValuePair<string, object>[] context)
        {
            if (!string.IsNullOrEmpty(arrjsonstring))
            {
                //采用js引擎存在跨平台环境兼容的问题，暂时屏蔽，改采用newtonjson的技术
                //lock (lockobj)
                //{
                //    CreateJSE();
                //    try
                //    {

                //        string js = "var out=" + arrjsonstring;
                //        if (context != null)
                //        {
                //            jse.Evaluate(js, context);
                //        }
                //        else
                //        {
                //            jse.Evaluate(js);
                //        }
                //        var d = jse.GetOutObject("out");
                //        if (d is object[])
                //        {
                //            return (object[])d;
                //        }
                //        else
                //        {
                //            return null;
                //        }

                //    }
                //    finally
                //    {
                //        ReleaseJSE();
                //    }
                //}
                var newjson = arrjsonstring;
                if (context != null)
                {
                    foreach (var item in context)
                    {
                        if (item.Value is string)
                        {
                            newjson = newjson.Replace(item.Key, "\"" + item.Value + "\"");
                        }
                        else
                        {
                            newjson = newjson.Replace(item.Key, "\"$context:" + item.Key + "\"");
                        }
                    }
                }

                var d = JsonConvert.DeserializeObject(newjson);
                if (d is JArray)
                {
                    var ja = (JArray)d;
                    var list = ja.ToObject<List<object>>();
                    var rtn = new List<object>();
                    foreach (var item in list)
                    {
                        rtn.Add(ConvertObjectFrom(item, flag));
                    }
                    return rtn.ToArray();
                }
                else
                {
                    return null;
                }

            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 根据json创建数组对象
        /// </summary>
        /// <param name="arrjsonstring"></param>
        /// <returns></returns>
        public static object[] CreateArray(string arrjsonstring, FrameDLRFlags flag = FrameDLRFlags.None)
        {
            return CreateArray(arrjsonstring, flag, null);
        }
        /// <summary>
        /// 根据格式化的json串创建数组对象
        /// </summary>
        /// <param name="formatjson"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static object[] CreateArrayFormat(string formatjson, FrameDLRFlags flag = FrameDLRFlags.None, params object[] values)
        {
            var context = new List<KeyValuePair<string, object>>();
            var json = formatjson;
            var validlist = new Dictionary<string, object>();
            for (int i = 0; i < values.Length; i++)
            {
                var pkey = "{" + i + "}";
                var vkey = "fdo_p_" + i;
                json = json.Replace(pkey, vkey);
                context.Add(new KeyValuePair<string, object>("fdo_p_" + i, values[i]));
            }

            return CreateArray(json, flag, context.ToArray());
        }
        /// <summary>
        /// 根据格式化的json串创建动态对象
        /// </summary>
        /// <param name="fromatjsonstring">格式化的json串，如下：
        /// {
        ///     key:{0},
        ///     key2:{1}
        /// }
        /// </param>
        ///  <param name="flag"></param>
        /// <param name="values">对象数组，下标对应json串中的{index}</param>
        /// <returns></returns>
        public static dynamic CreateInstanceFromat(string fromatjsonstring, FrameDLRFlags flag, params object[] values)
        {
            var context = new List<KeyValuePair<string, object>>();
            var json = fromatjsonstring;
            var validlist = new Dictionary<string, object>();
            var i = 0;

            foreach (var item in values)
            {
                var pkey = "{" + i + "}";
                validlist.Add(pkey, item);
                var vkey = "fdo_p_" + i;
                json = json.Replace(pkey, vkey);
                context.Add(new KeyValuePair<string, object>("fdo_p_" + i, values[i]));
                i++;
            }

            FrameDLRObject rtn = CreateInstance(json, flag, context.ToArray());

            rtn._valuelist = validlist;




            return rtn;
        }
        /// <summary>
        /// 根据格式化的json串创建动态对象
        /// </summary>
        /// <param name="fromatjsonstring">格式化的json串，如下：
        /// {
        ///     key:{0},
        ///     key2:{1}
        /// }
        /// </param>
        /// <param name="values">对象数组，下标对应json串中的{index}</param>
        /// <returns></returns>
        public static dynamic CreateInstanceFromat(string fromatjsonstring, params object[] values)
        {
            return CreateInstanceFromat(fromatjsonstring, FrameDLRFlags.None, values);
        }
        /// <summary>
        /// 创建一个DLR的新实例对象
        /// </summary>
        /// <param name="d"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static dynamic CreateInstance(Dictionary<string, object> d, FrameDLRFlags flags)
        {
            FrameDLRObject rtn = null;

            if (d != null)
            {
                rtn = BuildLoopDics(d, flags);
            }
            else
            {
                rtn = new FrameDLRObject();
                rtn._d = new Dictionary<string, object>();
            }

            return rtn;
        }
        /// <summary>
        /// 创建一个DLR的新实例对象,默认忽略大小写
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static dynamic CreateInstance(Dictionary<string, object> d)
        {
            return CreateInstance(d, FrameDLRFlags.None);
        }
        /// <summary>
        /// 创建一个DLR的新实例对象
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static dynamic CreateInstance(FrameDLRFlags flags)
        {
            FrameDLRObject rtn = new FrameDLRObject();
            if (rtn._d == null)
            {
                rtn._d = new Dictionary<string, object>();
            }

            if ((flags & FrameDLRFlags.SensitiveCase) != 0)
            {
                rtn.ignorecase = false;
            }
            else
            {
                rtn.ignorecase = true;
            }

            return rtn;
        }
        /// <summary>
        /// 创建一个DLR的新实例对象,默认忽略大小写
        /// </summary>
        /// <returns></returns>
        public static dynamic CreateInstance()
        {
            return CreateInstance(FrameDLRFlags.None);
        }
        /// <summary>
        /// 根据json串创建动态对象,默认忽略大小写
        /// </summary>
        /// <param name="model"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static dynamic CreateInstance(object model, params KeyValuePair<string, object>[] context)
        {
            return CreateInstance(model, FrameDLRFlags.None, context);
        }
        /// <summary>
        /// 创建动态对象，通过反射将model的property转成动态对象。对效能有影响，不建议使用
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static dynamic CreateInstance(object model)
        {
            return CreateInstance(model, FrameDLRFlags.None);
        }

        /// <summary>
        /// 创建动态对象,预设好成功失败及错误讯息
        /// </summary>
        /// <param name="issuccess"></param>
        /// <param name="msg"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static dynamic CreateInstance(bool issuccess, string msg, object model = null)
        {
            var rtn = CreateInstance(model);
            rtn.issuccess = issuccess;
            rtn.msg = msg;
            return rtn;
        }

        /// <summary>
        /// 创建动态对象
        /// </summary>
        /// <param name="model"></param>
        /// <param name="flags"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static dynamic CreateInstance(object model, FrameDLRFlags flags, params KeyValuePair<string, object>[] context)
        {
            FrameDLRObject rtn = CreateInstance(flags);
            if (model != null)
            {
                if (model is KeyValuePair<string, object>[])
                {
                    var arr = (KeyValuePair<string, object>[])model;
                    foreach (var item in arr)
                    {
                        rtn.SetValue(item.Key, item.Value);
                    }
                }
                else if (model is IDictionary<string, object>)
                {
                    rtn = BuildLoopDics((IDictionary<string, object>)model, flags);
                }
                else if (model is IDictionary<string, string>)
                {
                    rtn = BuildLoopDics(((IDictionary<string, string>)model).ToDictionary(k => k.Key, v => (object)v.Value), flags);
                }
                else if (model is IDictionary<string, FrameDLRObject>)
                {
                    rtn = BuildLoopDics((IDictionary<string, FrameDLRObject>)model, flags);
                }
                else if (model is XmlDocument)
                {
                    BuildLoopXml(((XmlDocument)model).ChildNodes, rtn);
                }
                else if (model is FrameDLRObject)
                {
                    rtn = (FrameDLRObject)model;
                }
                else if (model is string)
                {
                    var str = ComFunc.nvl(model);
                    if (str != "")
                    {
                        XmlDocument xd = null;
                        if (str.ToLower().Trim().StartsWith("<?xml") || str.ToLower().Trim().StartsWith("<xml"))
                        {
                            if (TryParseXml(ComFunc.nvl(str), out xd))
                            {
                                BuildLoopXml(xd.ChildNodes, rtn);
                            }
                            else
                            {
                                rtn = null;
                            }
                        }
                        else
                        {
                            //采用js引擎存在跨平台环境兼容的问题，暂时屏蔽，改采用newtonjson的技术
                            //lock (lockobj)
                            //{
                            //    try
                            //    {
                            //        CreateJSE();
                            //        string js = "var out=" + str;
                            //        if (context != null)
                            //        {
                            //            jse.Evaluate(js, context);
                            //        }
                            //        else
                            //        {
                            //            jse.Evaluate(js);
                            //        }
                            //        var d = (Dictionary<string, object>)jse.GetOutObject("out");
                            //        rtn = BuildLoopDics(d, flags);
                            //    }
                            //    finally
                            //    {
                            //        ReleaseJSE();
                            //    }
                            //}
                            var newjson = str;
                            if (context != null)
                            {
                                foreach (var item in context)
                                {
                                    if (item.Value is string)
                                    {
                                        newjson = newjson.Replace(item.Key, "\"" + item.Value + "\"");
                                    }
                                    else
                                    {
                                        newjson = newjson.Replace(item.Key, "\"$context:" + item.Key + "\"");
                                    }
                                }
                            }

                            var d = JsonConvert.DeserializeObject<Dictionary<string, object>>(newjson);
                            rtn = BuildLoopDics(d, flags, new Dictionary<string, object>(context));
                        }
                    }
                }
                else
                {
                    //var t = model.GetType();
                    //rtn.ignorecase = flags == FrameDLRFlags.SensitiveCase ? false : true;
                    //foreach (var p in t.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    //{
                    //    rtn.SetValue(p.Name, p.GetValue(model));
                    //}
                    //foreach (var p in t.GetFields(BindingFlags.Instance | BindingFlags.Public))
                    //{
                    //    rtn.SetValue(p.Name, p.GetValue(model));
                    //}
                    BuildAnonymousType(model, rtn);
                }
            }
            return rtn;
        }
        private static void BuildAnonymousType(object model, FrameDLRObject parent)
        {
            var t = model.GetType();
            if (t.Name.StartsWith("<>f__AnonymousType"))
            {
                var dobj = FrameDLRObject.CreateInstance(parent.Flags);
                foreach (var p in t.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    var v = p.GetValue(model);
                    if (v == null || v == DBNull.Value)
                    {
                        parent.SetValue(p.Name, v);
                    }
                    else if (v.GetType().Name.StartsWith("<>f__AnonymousType"))
                    {
                        BuildAnonymousType(v, dobj);
                        parent.SetValue(p.Name, dobj);
                    }
                    else if (v is IEnumerable<string>)
                    {
                        parent.SetValue(p.Name, v);
                    }
                    else if (v is IEnumerable<object>)
                    {
                        var l = ((IEnumerable<object>)v);
                        var list = new List<object>();
                        foreach (var item in l)
                        {
                            if (item.GetType().IsValueType)
                            {
                                list.Add(item);
                            }
                            else if (item is string)
                            {
                                list.Add(item);
                            }
                            else
                            {
                                list.Add(CreateInstance(item, parent.Flags));
                            }
                        }
                        parent.SetValue(p.Name, list);
                    }
                    else
                    {
                        parent.SetValue(p.Name, v);
                    }
                }
            }
            else
            {
                foreach (var p in t.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    parent.SetValue(p.Name, p.GetValue(model));
                }
                foreach (var p in t.GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    parent.SetValue(p.Name, p.GetValue(model));
                }
            }
        }
        /// <summary>
        /// 通过xml建立动态对象
        /// </summary>
        /// <param name="nodelist"></param>
        /// <param name="parent"></param>
        private static void BuildLoopXml(XmlNodeList nodelist, FrameDLRObject parent)
        {
            //根据每个节点的名称进行计数
            var namecount = new Dictionary<string, int>();
            foreach (XmlNode sub in nodelist)
            {
                if (sub.NodeType != XmlNodeType.Element) continue;
                if (namecount.ContainsKey(sub.Name))
                {
                    namecount[sub.Name] += 1;
                }
                else
                {
                    namecount.Add(sub.Name, 1);
                }
            }
            foreach (XmlNode node in nodelist)
            {
                if (!namecount.ContainsKey(node.Name)) continue;

                if (namecount[node.Name] > 1)
                {
                    if (parent.GetValue(node.Name) == null) parent.SetValue(node.Name, new List<object>());

                    var list = (List<object>)parent.GetValue(node.Name);
                    BuildLoopXmlNode(node, list, parent.Flags);
                }
                else
                {
                    BuildLoopXmlNode(node, parent, parent.Flags);
                }
            }
        }
        /// <summary>
        /// 通过xml建立动态对象
        /// </summary>
        /// <param name="node"></param>
        /// <param name="parent"></param>
        private static void BuildLoopXmlNode(XmlNode node, object parent, FrameDLRFlags flags)
        {
            if (node.NodeType != XmlNodeType.Element) return;

            if (parent is List<object>)
            {
                var list = (List<object>)parent;
                FrameDLRObject obj = FrameDLRObject.CreateInstance(flags);
                foreach (XmlAttribute attr in node.Attributes)
                {
                    obj.SetValue(attr.Name, ConvertObjectFromXml(attr.Value));
                }
                foreach (XmlNode sub in node.ChildNodes)
                {
                    if (sub.HasChildNodes && sub.FirstChild.NodeType == XmlNodeType.Element)
                    {
                        //BuildLoopXml(sub.ChildNodes, obj);
                        var pitem = FrameDLRObject.CreateInstance(flags);
                        obj.SetValue(sub.Name, pitem);
                        BuildLoopXml(sub.ChildNodes, pitem);
                    }
                    else
                    {
                        obj.SetValue(sub.Name, ConvertObjectFromXml(sub.InnerText));
                    }
                }

                list.Add(obj);
            }
            else
            {
                var dparent = (FrameDLRObject)parent;
                if (node.HasChildNodes && node.FirstChild.NodeType == XmlNodeType.Element)
                {
                    FrameDLRObject obj = FrameDLRObject.CreateInstance(flags);
                    foreach (XmlAttribute attr in node.Attributes)
                    {
                        obj.SetValue(attr.Name, ConvertObjectFromXml(attr.Value));
                    }
                    BuildLoopXml(node.ChildNodes, obj);

                    dparent.SetValue(node.Name, obj);
                }
                else
                {
                    if (node.Attributes.Count > 0)
                    {
                        FrameDLRObject obj = FrameDLRObject.CreateInstance(flags);
                        foreach (XmlAttribute attr in node.Attributes)
                        {
                            obj.SetValue(attr.Name, ConvertObjectFromXml(attr.Value));
                        }
                        dparent.SetValue(node.Name, obj);
                    }
                    else
                    {
                        dparent.SetValue(node.Name, ConvertObjectFromXml(node.InnerText));
                    }
                }
            }

        }

        /// <summary>
        /// 递归建立动态json对象
        /// </summary>
        /// <param name="d"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        private static dynamic BuildLoopDics(IDictionary<string, object> d, FrameDLRFlags flags, Dictionary<string, object> context = null)
        {
            FrameDLRObject rtn = new FrameDLRObject();
            if ((flags & FrameDLRFlags.SensitiveCase) != 0)
            {
                rtn.ignorecase = false;
            }
            else
            {
                rtn.ignorecase = true;
            }

            foreach (var item in d)
            {
                if (item.Value is string && item.Value.ToString().StartsWith("$context:"))
                {
                    object contextvalue = null;
                    var contextkey = item.Value.ToString().Replace("$context:", "");
                    if (context != null)
                    {
                        contextvalue = context.ContainsKey(contextkey) ? context[contextkey] : null;
                    }
                    rtn.SetValue(item.Key, ConvertObjectFrom(contextvalue, flags));
                }
                else
                {
                    rtn.SetValue(item.Key, ConvertObjectFrom(item.Value, flags));
                }
            }

            return rtn;
        }
        private static dynamic BuildLoopDics(IDictionary<string, FrameDLRObject> d, FrameDLRFlags flags)
        {
            FrameDLRObject rtn = new FrameDLRObject();
            if ((flags & FrameDLRFlags.SensitiveCase) != 0)
            {
                rtn.ignorecase = false;
            }
            else
            {
                rtn.ignorecase = true;
            }

            foreach (var item in d)
            {
                rtn.SetValue(item.Key, item.Value);
            }

            return rtn;
        }
        /// <summary>
        /// 将xml中的值进行转化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static object ConvertObjectFromXml(object obj)
        {
            return obj;
        }
        /// <summary>
        /// 递归解析原始对象，将dictionary解析成FrameDLRObject，其它对象解析成标准数据对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        private static object ConvertObjectFrom(object obj, FrameDLRFlags flags)
        {
            if (obj is JArray)
            {
                var jv = (JArray)obj;
                var list = jv.ToObject<List<object>>();
                var rtn = new List<object>();
                foreach (var item in list)
                {
                    rtn.Add(ConvertObjectFrom(item, flags));
                }
                return rtn;
            }
            else if (obj is JObject)
            {
                var jo = (JObject)obj;
                return ConvertObjectFrom(jo.ToObject<Dictionary<string, object>>(), flags);
            }
            else if (obj is JValue)
            {
                var jv = (JValue)obj;
                return ConvertObjectFrom(jv.Value, flags);
            }
            else if (obj is Dictionary<string, object>)
            {
                return BuildLoopDics((Dictionary<string, object>)obj, flags);
            }
            else if (obj is List<object>)
            {
                List<object> list = new List<object>();
                foreach (var m in (List<object>)obj)
                {
                    list.Add(ConvertObjectFrom(m, flags));
                }
                return list.ToArray();
            }
            else if (obj is object[])
            {
                List<object> list = new List<object>();
                foreach (var m in (object[])obj)
                {
                    list.Add(ConvertObjectFrom(m, flags));
                }
                return list.ToArray();
            }
            else if (obj is DateTime)
            {
                var dt = (DateTime)obj;
                if (dt == DateTime.MinValue.ToLocalTime())
                {
                    return DateTime.MinValue;
                }
                else
                {
                    return dt;
                }
            }
            else
            {
                return obj;
            }
        }
        /// <summary>
        /// 所有参数的key
        /// </summary>
        public List<string> Keys
        {
            get
            {
                return _d.Keys.ToList();
            }
        }
        /// <summary>
        /// 所有元素的集合
        /// </summary>
        public List<KeyValuePair<string, object>> Items
        {
            get
            {
                return _d.ToList();
            }
        }
        /// <summary>
        /// set value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetValue(string key, object value)
        {
            var newvalue = value;
            if (value != null && value.GetType().Name.StartsWith("<>f__AnonymousType"))
            {
                newvalue = FrameDLRObject.CreateInstance(value, FrameDLRFlags.SensitiveCase);
            }

            if (ignorecase)
            {
                if (_d.ContainsKey(key.ToLower()))
                    _d[key.ToLower()] = newvalue;
                else
                    _d.Add(key.ToLower(), newvalue);
            }
            else
            {
                if (_d.ContainsKey(key))
                    _d[key] = newvalue;
                else
                    _d.Add(key, newvalue);
            }

        }
        /// <summary>
        /// get value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetValue(string key)
        {
            object result = null;
            if (ignorecase)
            {
                if (_d.ContainsKey(key.ToLower()))
                {
                    result = _d[key.ToLower()];
                }
            }
            else
            {

                //if (_d.ContainsKey(key))
                //{
                //    result = _d[key];
                //}
                //先采用模糊（忽略大小写）搜索方式找出数据，如果有多个则采用精确查找，如果未找到则自动返回第一个
                var tmp = _d.Where(p => p.Key.ToLower() == key.ToLower());
                if (tmp.Count() == 1)
                {
                    result = tmp.First().Value;
                }
                else if (tmp.Count() > 1)
                {
                    if (_d.ContainsKey(key))
                    {
                        result = _d[key];
                    }
                    else
                    {
                        result = tmp.First().Value;
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// 加载一个对象属性到本实例中
        /// </summary>
        /// <param name="model"></param>
        public void Load(object model)
        {
            FrameDLRObject obj = CreateInstance(model, this.IgnoreCase ? FrameDLRFlags.None : FrameDLRFlags.SensitiveCase);
            foreach (var key in obj.Keys)
            {
                SetValue(key, obj.GetValue(key));
            }
        }
        /// <summary>
        /// 动态对象中的Setvalue
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override object SetMetaValue(string key, object value)
        {
            SetValue(key, value);

            return value;
        }
        /// <summary>
        /// 动态对象中的getvalue
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override object GetMetaValue(string key)
        {
            return GetValue(key);
        }
        #region Invoke Delegate
        /// <summary>
        /// 动态对象执行方法
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected override object InvokeMe(string methodInfo, params object[] args)
        {
            if (methodInfo.ToLower() == "setvalue")
            {
                this.SetValue(ComFunc.nvl(args[0]), args[1]);
                return null;
            }
            else if (methodInfo.ToLower() == "getvalue")
            {
                return this.GetValue(ComFunc.nvl(args[0]));
            }
            else if (methodInfo.ToLower() == "tojsonstring")
            {
                var encoding = Encoding.UTF8;
                var ispack = false;

                if (args.Length == 1)
                {
                    if (args[0] is Encoding)
                        encoding = (Encoding)args[0];
                    if (args[0] is bool)
                        ispack = (bool)args[0];

                }
                else if (args.Length > 1)
                {
                    if (args[0] is Encoding)
                        encoding = (Encoding)args[0];
                    if (args[1] is bool)
                        ispack = (bool)args[0];
                }
                return this.ToJSONString(encoding, ispack);
            }
            else if (methodInfo.ToLower() == "tojsonobject")
            {
                return this.ToJSONObject();
            }
            else if (methodInfo.ToLower() == "toxml")
            {
                return this.ToXml();
            }
            else if (methodInfo.ToLower() == "load")
            {
                this.Load(args[0]);
                return this;
            }
            else
            {
                var m = GetValue(methodInfo);
                if (m is MulticastDelegate)
                {
                    var rtn = ((MulticastDelegate)m).DynamicInvoke(args);

                    return rtn;
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion
        /// <summary>
        /// 判定是否为json对象
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static bool IsJson(string json)
        {
            try
            {
                var v = FrameDLRObject.CreateInstance(json);
                if (v != null)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 判定是否为json array对象
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static bool IsJsonArray(string json)
        {
            try
            {
                var v = FrameDLRObject.CreateArray(json);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 判断是否为xml
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsXml(string str)
        {
            XmlDocument xd = null;
            return TryParseXml(str, out xd);
        }
        /// <summary>
        /// 判断是否为json，如果是则直接返回转换后的对象，否则返回默认值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultvalue">默认值，默认为null</param>
        /// <param name="flag">默认为FrameDLRFlags.None</param>
        /// <returns></returns>
        public static FrameDLRObject IsJsonThen(string str, FrameDLRObject defaultvalue = null, FrameDLRFlags flag = FrameDLRFlags.None)
        {
            try
            {
                var v = FrameDLRObject.CreateInstance(str, flag);
                return v;
            }
            catch (Exception ex)
            {
                return defaultvalue;
            }
        }
        /// <summary>
        /// 判断是否为xml，如果是则直接返回转换后的对象，否则返回默认值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultvalue"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static FrameDLRObject IsXmlThen(string str, FrameDLRObject defaultvalue = null, FrameDLRFlags flag = FrameDLRFlags.None)
        {
            XmlDocument xd = null;
            if(TryParseXml(str,out xd))
            {
                return FrameDLRObject.CreateInstance(xd, flag);
            }
            else
            {
                return defaultvalue;
            }
        }
        /// <summary>
        /// 判断是否为json array，如果是则直接返回转换后的对象，否则返回默认值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultvalue">默认值，默认为null</param>
        /// <param name="flag">默认为FrameDLRFlags.None</param>
        /// <returns></returns>
        public static object[] IsJsonArrayThen(string str, object[] defaultvalue = null, FrameDLRFlags flag = FrameDLRFlags.None)
        {
            try
            {
                var v = FrameDLRObject.CreateArray(str, flag);
                return v;
            }
            catch (Exception ex)
            {
                return defaultvalue;
            }
        }
       
        /// <summary>
        /// 判定是否为Xml
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private static bool TryParseXml(string xml, out XmlDocument xd)
        {
            try
            {
                //防xxe攻击
                var x = ComFunc.GetSafeXmlInstance();
                x.LoadXml(xml);
                xd = x;
                return true;
            }
            catch (Exception ex)
            {
                xd = null;
                return false;
            }
        }
        /// <summary>
        /// 尝试将string转化成json对象
        /// </summary>
        /// <param name="json"></param>
        /// <param name="outobj"></param>
        public static void TryParse(string json, FrameDLRFlags flag, out FrameDLRObject outobj)
        {
            try
            {
                outobj = FrameDLRObject.CreateInstance(json, flag);
            }
            catch (Exception ex)
            {
                outobj = null;
            }
        }
        /// <summary>
        /// 尝试将string转化成json对象
        /// </summary>
        /// <param name="json"></param>
        /// <param name="outobj"></param>
        public static void TryParse(string json, out FrameDLRObject outobj)
        {
            TryParse(json, FrameDLRFlags.None, out outobj);
        }
        /// <summary>
        /// 转化成json对象
        /// </summary>
        /// <returns></returns>
        public dynamic ToJSONObject()
        {
            return this;
        }
        /// <summary>
        /// 转成Json串,string或byte类型默认采用utf-8进行编码
        /// </summary>
        /// <returns></returns>
        public string ToJSONString(bool ispack = false)
        {
            return ToJSON(this, 0, ispack);
        }
        /// <summary>
        /// 转成Json串
        /// </summary>
        /// <param name="encode">对于string或byte类型的编码方式</param>
        /// <param name="ispack">是否做压缩</param>
        /// <returns></returns>
        public string ToJSONString(Encoding encode, bool ispack = false)
        {
            return ToJSON(this, 0, encode, ispack);
        }
        /// <summary>
        /// 转化为dictionary
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> ToDictionary()
        {
            return ToDics(this);
        }
        /// <summary>
        /// 将Key都转为大写，然后返回一个新的对象
        /// </summary>
        /// <returns></returns>
        public FrameDLRObject KeyToUpperCase()
        {
            FrameDLRObject rtn = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            foreach (var item in Items)
            {
                rtn.SetValue(item.Key.ToUpper(), item.Value);
            }
            return rtn;
        }
        /// <summary>
        /// 将Key都转为小写，然后返回一个新的对象
        /// </summary>
        /// <returns></returns>
        public FrameDLRObject KeyToLowerCase()
        {
            FrameDLRObject rtn = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            foreach (var item in Items)
            {
                rtn.SetValue(item.Key.ToLower(), item.Value);
            }
            return rtn;
        }
        /// <summary>
        /// 移除数据项
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            if (_d.ContainsKey(key))
                _d.Remove(key);
        }
        /// <summary>
        /// 清空数据
        /// </summary>
        public void Clear()
        {
            _d.Clear();
        }
        private Dictionary<string, object> ToDics(FrameDLRObject obj)
        {
            Dictionary<string, object> rtn = new Dictionary<string, object>();
            foreach (var s in obj._d)
            {
                if (s.Value is FrameDLRObject)
                {
                    rtn.Add(s.Key, ToDics((FrameDLRObject)s.Value));
                }
                else if (s.Value is IEnumerable<FrameDLRObject>)
                {
                    List<object> list = new List<object>();
                    foreach (var m in (IEnumerable<FrameDLRObject>)s.Value)
                    {
                        if (m is FrameDLRObject)
                        {
                            list.Add(ToDics(m));
                        }
                        else
                        {
                            list.Add(m);
                        }
                    }
                    rtn.Add(s.Key, list.ToArray());
                }
                else if (s.Value is IEnumerable<object>)
                {
                    List<object> list = new List<object>();
                    foreach (var m in (IEnumerable<object>)s.Value)
                    {
                        if (m is FrameDLRObject)
                        {
                            list.Add(ToDics((FrameDLRObject)m));
                        }
                        else
                        {
                            list.Add(m);
                        }
                    }
                    rtn.Add(s.Key, list.ToArray());
                }
                else if (s.Value is object[])
                {
                    List<object> list = new List<object>();
                    foreach (var m in (object[])s.Value)
                    {
                        if (m is FrameDLRObject)
                        {
                            list.Add(ToDics((FrameDLRObject)m));
                        }
                        else
                        {
                            list.Add(m);
                        }
                    }
                    rtn.Add(s.Key, list.ToArray());
                }
                else
                {
                    rtn.Add(s.Key, s.Value);
                }
            }
            return rtn;
        }
        /// <summary>
        /// 构建标准结构化json（表达结构，数据采用标准化变量名称标识）
        /// </summary>
        /// <returns></returns>
        private string ToJSON_Structure()
        {
            return ToJSON_Structure(this, 0);
        }

        private string ToJSON_Structure(FrameDLRObject obj, int level)
        {
            var sortd = _d.OrderBy(p => p.Key).ToDictionary(p => p.Key, o => o.Value);
            return ToJSON_Structure(sortd, level);
        }
        private string ToJSON_Structure(Dictionary<string, object> d, int level)
        {
            StringBuilder rtn = new StringBuilder();
            string template = GetTabs(level + 1) + @"""{0}"":{1},";
            int index = 0;
            rtn.AppendLine("{");
            foreach (var item in d)
            {
                if (item.Value is Dictionary<string, object>)
                {
                    rtn.AppendLine(string.Format(template, item.Key, ToJSON_Structure((Dictionary<string, object>)item.Value, level + 1)));
                }
                else if (item.Value is object[])
                {
                    rtn.AppendLine(string.Format(template, item.Key, ToJSON_Structure((object[])item.Value, level + 1)));
                }
                else if (item.Value is IEnumerable<FrameDLRObject>)
                {
                    rtn.AppendLine(string.Format(template, item.Key, ToJSON_Structure(((IEnumerable<FrameDLRObject>)item.Value).ToArray(), level + 1)));
                }
                else if (item.Value is IEnumerable<Object>)
                {
                    rtn.AppendLine(string.Format(template, item.Key, ToJSON_Structure(((IEnumerable<Object>)item.Value).ToArray(), level + 1)));
                }
                else if (item.Value is FrameDLRObject)
                {
                    rtn.AppendLine(string.Format(template, item.Key, ToJSON_Structure((FrameDLRObject)item.Value, level + 1)));
                }
                else
                {
                    string pkey = "{" + level + index + "}";
                    rtn.AppendLine(string.Format(template, item.Key, pkey));
                    _valuelist.Add(pkey, item.Value);
                    index++;
                }


            }

            var str = rtn.ToString().Trim();
            rtn = new StringBuilder(str.Length > 1 ? str.Substring(0, str.Length - 1) : str);
            rtn.AppendLine("");
            rtn.Append(GetTabs(level) + "}");
            return rtn.ToString();
        }

        private string ToJSON_Structure(Object[] arr, int level)
        {
            StringBuilder rtn = new StringBuilder();
            string template = GetTabs(level + 1) + "{0},";
            rtn.AppendLine("[");
            int index = 0;
            foreach (var obj in arr)
            {
                if (obj is Dictionary<string, object>)
                {
                    rtn.AppendLine(string.Format(template, ToJSON_Structure((Dictionary<string, object>)obj, level + 1)));
                }
                else if (obj is object[])
                {
                    rtn.AppendLine(string.Format(template, ToJSON_Structure((object[])obj, level + 1)));
                }
                else if (obj is FrameDLRObject)
                {
                    rtn.AppendLine(string.Format(template, ToJSON_Structure((FrameDLRObject)obj, level + 1)));
                }
                else
                {
                    string pkey = "{" + level + index + "}";
                    rtn.AppendLine(string.Format(template, pkey));
                    _valuelist.Add(pkey, obj);
                    index++;
                }
            }
            var str = rtn.ToString().Trim();
            rtn = new StringBuilder(str.Length > 1 ? str.Substring(0, str.Length - 1) : str);
            rtn.AppendLine("");
            rtn.Append(GetTabs(level) + "]");
            return rtn.ToString();
        }

        private string ToJSON(FrameDLRObject obj, int level, bool ispack = false)
        {
            return ToJSON(obj._d, level, Encoding.UTF8, ispack);
        }
        private string ToJSON(FrameDLRObject obj, int level, Encoding encode, bool ispack = false)
        {
            return ToJSON(obj._d, level, encode, ispack);
        }
        private string GetTabs(int level, bool ispack = false)
        {
            string rtn = "";
            if (!ispack)
            {
                for (int i = 0; i < level; i++)
                {
                    rtn += tabflag;
                }
            }
            return rtn;
        }
        private string ToJSON(Dictionary<string, object> d, int level, Encoding encode, bool ispack = false)
        {
            StringBuilder rtn = new StringBuilder();
            string template = GetTabs(level + 1, ispack) + @"""{0}"":{1},";
            if (ispack) rtn.Append("{");
            else rtn.AppendLine("{");
            foreach (var item in d)
            {
                if (item.Value is int
                    || item.Value is uint
                    || item.Value is short
                    || item.Value is ushort
                    || item.Value is long
                    || item.Value is ulong
                    || item.Value is Int16
                    || item.Value is UInt16
                    || item.Value is Int32
                    || item.Value is UInt32
                    || item.Value is Int64
                    || item.Value is UInt64
                    || item.Value is byte
                    || item.Value is sbyte)
                {
                    rtn.Append(string.Format(template, item.Key, item.Value));
                    if (!ispack) rtn.AppendLine("");
                }
                else if (item.Value is uint)
                {
                    rtn.Append(string.Format(template, item.Key, item.Value));
                    if (!ispack) rtn.AppendLine("");
                }
                else if (item.Value != null && item.Value.GetType().FullName == typeof(double).FullName)
                {
                    var val = (double)item.Value;
                    if (double.IsInfinity(val))
                    {
                        rtn.Append(string.Format(template, item.Key, "\"" + item.Value + "\""));
                    }
                    else
                    {
                        rtn.Append(string.Format(template, item.Key, item.Value));
                    }
                    if (!ispack) rtn.AppendLine("");
                }
                else if (item.Value != null && item.Value.GetType().FullName == typeof(decimal).FullName)
                {
                    var val = (decimal)item.Value;
                    rtn.Append(string.Format(template, item.Key, item.Value));
                    if (!ispack) rtn.AppendLine("");
                }
                else if (item.Value is float)
                {
                    var val = (float)item.Value;
                    if (float.IsInfinity(val))
                    {
                        rtn.Append(string.Format(template, item.Key, "\"" + item.Value + "\""));
                    }
                    else
                    {
                        rtn.Append(string.Format(template, item.Key, item.Value));
                    }
                    if (!ispack) rtn.AppendLine("");
                }
                else if (item.Value is Dictionary<string, object>)
                {
                    rtn.Append(string.Format(template, item.Key, ToJSON((Dictionary<string, object>)item.Value, level + 1, encode, ispack)));
                    if (!ispack) rtn.AppendLine("");
                }
                else if (item.Value is object[])
                {
                    rtn.Append(string.Format(template, item.Key, ToJSON((object[])item.Value, level + 1, encode, ispack)));
                    if (!ispack) rtn.AppendLine("");
                }
                else if (item.Value is IEnumerable<FrameDLRObject>)
                {
                    rtn.Append(string.Format(template, item.Key, ToJSON(((IEnumerable<FrameDLRObject>)item.Value).ToArray(), level + 1, encode, ispack)));
                    if (!ispack) rtn.AppendLine("");
                }
                else if (item.Value is IEnumerable<Object>)
                {
                    rtn.Append(string.Format(template, item.Key, ToJSON(((IEnumerable<Object>)item.Value).ToArray(), level + 1, encode, ispack)));
                    if (!ispack) rtn.AppendLine("");
                }
                else if (item.Value is IJSONable)
                {
                    if (item.Value is FrameDLRObject)
                    {
                        rtn.Append(string.Format(template, item.Key, ToJSON((FrameDLRObject)item.Value, level + 1, ispack)));
                    }
                    else
                    {
                        rtn.Append(string.Format(template, item.Key, ((IJSONable)item.Value).ToJSONString(ispack)));
                    }
                    if (!ispack) rtn.AppendLine("");
                }
                else if (item.Value is DateTime)
                {
                    //rtn.AppendLine(string.Format(template, item.Key, "\"" + ((DateTime)item.Value).ToString("yyyy/MM/dd HH:mm:ss fff") + "\""));
                    var dt = ((DateTime)item.Value);
                    if (dt == DateTime.MinValue)
                    {
                        rtn.Append(string.Format(template, item.Key, string.Format("new Date('0001-01-01')")));
                    }
                    else
                    {
                        rtn.Append(string.Format(template, item.Key, string.Format("new Date({0},{1},{2},{3},{4},{5})", dt.Year, dt.Month - 1, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond)));
                    }
                    if (!ispack) rtn.AppendLine("");
                }
                else if (item.Value is DateTimeStd)
                {
                    //rtn.AppendLine(string.Format(template, item.Key, "\"" + ((DateTime)item.Value).ToString("yyyy/MM/dd HH:mm:ss fff") + "\""));
                    var dt = ((DateTimeStd)item.Value);
                    if (dt == null || dt.Value == DateTime.MinValue)
                    {
                        rtn.Append(string.Format(template, item.Key, string.Format("new Date('0001-01-01')")));
                    }
                    else
                    {
                        rtn.Append(string.Format(template, item.Key, string.Format("new Date({0},{1},{2},{3},{4},{5})", dt.Value.Year, dt.Value.Month - 1, dt.Value.Day, dt.Value.Hour, dt.Value.Minute, dt.Value.Second, dt.Value.Millisecond)));
                    }
                    if (!ispack) rtn.AppendLine("");
                }
                else if (item.Value is bool)
                {
                    rtn.Append(string.Format(template, item.Key, item.Value.ToString().ToLower()));
                    if (!ispack) rtn.AppendLine("");
                }
                else if (item.Value is string)
                {
                    rtn.Append(string.Format(template, item.Key, "\"" + item.Value.ToString().Replace("\r", "").Replace("\n", "#n#").Replace(@"\", @"\\").Replace("#n#", "\\n").Replace("\"", @"\""") + "\""));
                    if (!ispack) rtn.AppendLine("");
                }
                else if (item.Value is byte[])
                {
                    //byte可以直接转化成base64，不需要中间转一次
                    //rtn.AppendLine(string.Format(template, item.Key, "\"" + ComFunc.Base64Code(ComFunc.ByteToString((byte[])item.Value, encode), encode) + "\""));
                    rtn.Append(string.Format(template, item.Key, "\"" + ComFunc.Base64Code((byte[])item.Value) + "\""));
                    if (!ispack) rtn.AppendLine("");
                }
                else if (item.Value == null || item.Value == DBNull.Value)
                {
                    rtn.Append(string.Format(template, item.Key, "\"\""));
                    if (!ispack) rtn.AppendLine("");
                }

            }

            var str = rtn.ToString().Trim();
            rtn = new StringBuilder(str.Length > 1 ? str.Substring(0, str.Length - 1) : str);
            if (!ispack) rtn.AppendLine("");
            rtn.Append(GetTabs(level, ispack) + "}");
            return rtn.ToString();
        }

        private string ToJSON(Object[] arr, int level, Encoding encode, bool ispack = false)
        {
            StringBuilder rtn = new StringBuilder();
            string template = GetTabs(level + 1, ispack) + "{0},";
            if (!ispack) rtn.AppendLine("[");
            else rtn.Append("[");
            foreach (var obj in arr)
            {
                if (obj is int
                    || obj is uint
                    || obj is short
                    || obj is ushort
                    || obj is long
                    || obj is ulong
                    || obj is Int16
                    || obj is UInt16
                    || obj is Int32
                    || obj is UInt32
                    || obj is Int64
                    || obj is UInt64
                    || obj is byte
                    || obj is sbyte)
                {
                    rtn.Append(string.Format(template, obj));
                    if (!ispack) rtn.AppendLine("");
                }
                else if (obj is double)
                {
                    rtn.Append(string.Format(template, obj));
                    if (!ispack) rtn.AppendLine("");
                }
                else if (obj is float)
                {
                    rtn.Append(string.Format(template, obj));
                    if (!ispack) rtn.AppendLine("");
                }
                else if (obj is Dictionary<string, object>)
                {
                    rtn.Append(string.Format(template, ToJSON((Dictionary<string, object>)obj, level + 1, encode, ispack)));
                    if (!ispack) rtn.AppendLine("");
                }
                else if (obj is object[])
                {
                    rtn.AppendLine(string.Format(template, ToJSON((object[])obj, level + 1, encode, ispack)));
                    if (!ispack) rtn.AppendLine("");
                }
                else if (obj is IJSONable)
                {
                    if (obj is FrameDLRObject)
                    {
                        rtn.Append(string.Format(template, ToJSON((FrameDLRObject)obj, level + 1, ispack)));
                    }
                    else
                    {
                        rtn.Append(string.Format(template, ((IJSONable)obj).ToJSONString(), ispack));
                    }
                    if (!ispack) rtn.AppendLine("");
                }
                else if (obj is DateTime)
                {
                    //rtn.AppendLine(string.Format(template, "\"" + ((DateTime)obj).ToString("yyyy/MM/dd HH:mm:ss fff") + "\""));
                    var dt = ((DateTime)obj);
                    if (dt == DateTime.MinValue)
                    {
                        rtn.Append(string.Format(template, string.Format("new Date('0001-01-01')")));
                    }
                    else
                    {
                        rtn.Append(string.Format(template, string.Format("new Date({0},{1},{2},{3},{4},{5},{6})", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond)));
                    }
                    if (!ispack) rtn.AppendLine("");
                }
                else if (obj is bool)
                {
                    rtn.Append(string.Format(template, obj.ToString().ToLower()));
                    if (!ispack) rtn.AppendLine("");
                }
                else if (obj is string)
                {
                    rtn.Append(string.Format(template, "\"" + obj.ToString().Replace("\r", "").Replace("\n", "#n#").Replace(@"\", @"\\").Replace("#n#", "\\n") + "\""));
                    if (!ispack) rtn.AppendLine("");
                }
                else if (obj is byte[])
                {
                    //byte可以直接转化成base64，不需要中间转一次
                    //rtn.AppendLine(string.Format(template, "\"" + ComFunc.Base64Code(ComFunc.ByteToString((byte[])obj, encode), encode) + "\""));
                    rtn.Append(string.Format(template, "\"" + ComFunc.Base64Code((byte[])obj)));
                    if (!ispack) rtn.AppendLine("");
                }
                else if (obj.GetType().Name.StartsWith("<>f__AnonymousType"))
                {
                    rtn.Append(string.Format(template, ToJSON(((FrameDLRObject)CreateInstance(obj, FrameDLRFlags.SensitiveCase)), level + 1, ispack)));
                    if (!ispack) rtn.AppendLine("");
                }
                else if (obj == null || obj == DBNull.Value)
                {
                    rtn.Append(string.Format(template, "\"\""));
                    if (!ispack) rtn.AppendLine("");
                }
            }
            var str = rtn.ToString().Trim();
            rtn = new StringBuilder(str.Length > 1 ? str.Substring(0, str.Length - 1) : str);
            if (!ispack) rtn.AppendLine("");
            rtn.Append(GetTabs(level, ispack) + "]");
            return rtn.ToString();
        }
        /// <summary>
        /// 采用反射方式转化为强类型的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public T ToModel<T>(Encoding encoding)
        {
            return ToModel<T>((name, t, v) =>
            {
                if (t == v.GetType())
                    return v;
                else
                {
                    if (t == typeof(string))
                    {
                        return ComFunc.nvl(v);
                    }
                    else if (t == typeof(byte[]))
                    {
                        if (v is string)
                        {
                            if (ComFunc.nvl(v) == "")
                            {
                                return null;
                            }
                            else
                            {
                                return ComFunc.Base64DeCodeToByte(ComFunc.nvl(v));
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }

                }
            });
        }
        /// <summary>
        /// 采用反射方式转化为强类型的对象,默认UTF-8编码，对效能有影响，不建议使用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ToModel<T>()
        {
            return ToModel<T>(Encoding.UTF8);
        }
        /// <summary>
        /// 采用反射方式转化为强类型的对象,对效能有影响，不建议使用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="convert">转化算法</param>
        /// <returns></returns>
        public T ToModel<T>(Func<string, Type, object, object> convert)
        {
            return (T)ToModel(typeof(T), convert);
        }
        /// <summary>
        /// 采用反射方式转化为强类型的对象,对效能有影响,默认编码UTF-8
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public object ToModel(Type t)
        {
            return ToModel(t, Encoding.UTF8);
        }
        /// <summary>
        /// 采用反射方式转化为强类型的对象,对效能有影响，不建议使用
        /// </summary>
        /// <param name="t"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public object ToModel(Type t, Encoding encoding)
        {
            return ToModel(t, (name, type, v) =>
             {
                 if (type == v.GetType())
                     return v;
                 else
                 {
                     if (type == typeof(string))
                     {
                         return ComFunc.nvl(v);
                     }
                     else if (type == typeof(byte[]))
                     {
                         if (v is string)
                         {
                             if (ComFunc.nvl(v) == "")
                             {
                                 return null;
                             }
                             else
                             {
                                 return ComFunc.Base64DeCodeToByte(ComFunc.nvl(v));
                             }
                         }
                         else
                         {
                             return null;
                         }
                     }
                     else
                     {
                         return null;
                     }

                 }
             });
        }
        /// <summary>
        /// 采用反射方式转化为强类型的对象,对效能有影响，不建议使用
        /// </summary>
        /// <param name="t"></param>
        /// <param name="convert">转化算法</param>
        /// <returns></returns>
        public object ToModel(Type t, Func<string, Type, object, object> convert)
        {
            object rtn = Activator.CreateInstance(t);
            foreach (var p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var v = this.GetValue(p.Name);
                if (v != null)
                {
                    object newv = v;
                    if (convert != null)
                    {
                        newv = convert(p.Name, p.PropertyType, v);
                    }
                    if (newv != null)
                        p.SetValue(t, convert(p.Name, p.PropertyType, v));
                }
            }
            foreach (var p in t.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {

                var v = this.GetValue(p.Name);
                if (v != null)
                {
                    object newv = v;
                    if (convert != null)
                    {
                        newv = convert(p.Name, p.FieldType, v);
                    }
                    if (newv != null)
                        p.SetValue(rtn, convert(p.Name, p.FieldType, v));
                }
            }
            return rtn;
        }
        /// <summary>
        /// 转成xml格式
        /// </summary>
        /// <returns></returns>
        public virtual string ToXml()
        {

            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("xml");
            doc.AppendChild(root);
            var obj = this;
            foreach (var k in obj.Keys)
            {
                ToXmlElem(doc, root, k, obj.GetValue(k));
            }
            return doc.InnerXml;
        }

        private void ToXmlElem(XmlDocument doc, XmlElement parent, string name, object obj)
        {


            if (obj is int
                    || obj is long
                    || obj is double
                    || obj is float
                    || obj is decimal)
            {
                XmlElement elem = doc.CreateElement(name);
                elem.AppendChild(doc.CreateTextNode(obj.ToString()));
                parent.AppendChild(elem);
            }
            else if (obj is DateTime)
            {
                var dt = (DateTime)obj;
                XmlElement elem = doc.CreateElement(name);
                elem.AppendChild(doc.CreateTextNode(dt.ToString("yyyy/MM/dd HH:mm:ss")));
                parent.AppendChild(elem);
            }
            else if (obj is string)
            {
                XmlElement elem = doc.CreateElement(name);
                elem.AppendChild(doc.CreateCDataSection(obj.ToString()));
                parent.AppendChild(elem);
            }
            else if (obj is FrameDLRObject)
            {
                var dobj = (FrameDLRObject)obj;
                XmlElement elem = doc.CreateElement(name);
                foreach (var k in dobj.Keys)
                {
                    ToXmlElem(doc, elem, k, dobj.GetValue(k));
                }
                parent.AppendChild(elem);
            }
            else if (obj is Dictionary<string, object>)
            {
                var dobj = (Dictionary<string, object>)obj;
                XmlElement elem = doc.CreateElement(name);
                foreach (var k in dobj)
                {
                    ToXmlElem(doc, elem, k.Key, k.Value);
                }
                parent.AppendChild(elem);
            }
            else if (obj is IEnumerable<FrameDLRObject>)
            {
                var arr = (IEnumerable<FrameDLRObject>)obj;
                foreach (var item in arr)
                {
                    var elemitem = doc.CreateElement(name);
                    var dobj = item;
                    foreach (var k in dobj.Keys)
                    {
                        ToXmlElem(doc, elemitem, k, dobj.GetValue(k));
                    }

                    parent.AppendChild(elemitem);
                }

            }
            else if (obj is IEnumerable<object>)
            {
                var arr = (IEnumerable<object>)obj;
                foreach (var item in arr)
                {
                    var elemitem = doc.CreateElement(name);
                    if (item is FrameDLRObject)
                    {
                        var dobj = (FrameDLRObject)item;
                        foreach (var k in dobj.Keys)
                        {
                            ToXmlElem(doc, elemitem, k, dobj.GetValue(k));
                        }
                    }
                    else if (item is Dictionary<string, object>)
                    {
                        var dobj = (Dictionary<string, object>)item;
                        foreach (var k in dobj)
                        {
                            ToXmlElem(doc, elemitem, k.Key, k.Value);
                        }
                    }
                    else
                    {
                        elemitem.AppendChild(doc.CreateCDataSection(ComFunc.nvl(item)));
                    }
                    parent.AppendChild(elemitem);
                }

            }
            else if (obj is object[])
            {
                var arr = (object[])obj;
                foreach (var item in arr)
                {
                    var elemitem = doc.CreateElement(name);
                    if (item is FrameDLRObject)
                    {
                        var dobj = (FrameDLRObject)item;
                        foreach (var k in dobj.Keys)
                        {
                            ToXmlElem(doc, elemitem, k, dobj.GetValue(k));
                        }
                    }
                    else if (item is Dictionary<string, object>)
                    {
                        var dobj = (Dictionary<string, object>)item;
                        foreach (var k in dobj)
                        {
                            ToXmlElem(doc, elemitem, k.Key, k.Value);
                        }
                    }
                    else
                    {
                        elemitem.AppendChild(doc.CreateCDataSection(ComFunc.nvl(item)));
                    }
                    parent.AppendChild(elemitem);
                }

            }
            else
            {
                XmlElement elem = doc.CreateElement(name);
                elem.AppendChild(doc.CreateCDataSection(ComFunc.nvl(obj)));
                parent.AppendChild(elem);
            }
        }

        public void Dispose()
        {
            if (this._d != null)
            {
                foreach (var item in _d)
                {
                    if (item.Value is IDisposable)
                    {
                        ((IDisposable)item.Value).Dispose();
                    }
                }

                _valuelist.Clear();
                _valuelist = null;
                _cuid = "";
                _d.Clear();
                _d = null;
                _duid = "";
                tabflag = "";
            }
            else
            {
                string log = "出现多线程重复删除资料";
            }
        }

        public object Clone()
        {
            FrameDLRObject rtn = FrameDLRObject.CreateInstance();
            rtn.ignorecase = this.ignorecase;

            foreach (var k in this.Keys)
            {
                var obj = this.GetValue(k);
                if (obj is ICloneable)
                {
                    rtn.SetValue(k, ((ICloneable)obj).Clone());
                }
                else
                {
                    rtn.SetValue(k, obj);
                }

            }

            return rtn;
        }
    }
    /// <summary>
    /// FrameDLRObject的相关扩展，针对linq表达式只做部分扩展，其他的不要实现
    /// </summary>
    public static class FrameDLRExtentds
    {
        /// <summary>
        /// Linq转化成FrameDLRObject
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="keySelector"></param>
        /// <param name="elementSelector"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static dynamic ToFrameDLRObject(this IEnumerable<KeyValuePair<string, object>> obj, Func<KeyValuePair<string, object>, string> keySelector, Func<KeyValuePair<string, object>, object> elementSelector, FrameDLRFlags flags = FrameDLRFlags.None)
        {
            return FrameDLRObject.CreateInstance(obj.ToDictionary(keySelector, elementSelector), flags);
        }
        /// <summary>
        /// FrameDLRObject执行select操作,筛选现有的FrameDLRObejct中的内容，并构成新的FrameDLRObject对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static dynamic Select(this FrameDLRObject obj, Func<dynamic, object> s)
        {
            FrameDLRObject rtn = FrameDLRObject.CreateInstance(s.Invoke(obj), obj.Flags);
            return rtn;
        }
        /// <summary>
        /// 扩展的linq的select操作
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static IEnumerable<FrameDLRObject> Select(this IEnumerable<FrameDLRObject> obj, Func<dynamic, object> s)
        {
            //转为list可以使列表内容固定住，否则会导致无法直接进行操作
            return obj.Select<FrameDLRObject, FrameDLRObject>(p => (FrameDLRObject)FrameDLRObject.CreateInstance(s.Invoke(p), p.Flags)).ToList();
        }

        /// <summary>
        /// 扩展的linq的where操作
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static IEnumerable<FrameDLRObject> Where(this IEnumerable<FrameDLRObject> obj, Func<dynamic, bool> s)
        {
            //转为list可以使列表内容固定住，否则会导致无法直接进行操作
            return obj.Where<FrameDLRObject>(p => s.Invoke(p)).ToList();
        }
        /// <summary>
        /// 扩展的linq的OrderBy操作
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="obj"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<FrameDLRObject> OrderBy<TKey>(this IEnumerable<FrameDLRObject> obj, Func<dynamic, TKey> s)
        {
            return obj.OrderBy<FrameDLRObject, TKey>(p => s.Invoke(p));
        }
        /// <summary>
        /// 扩展的linq的OrderBy操作
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="obj"></param>
        /// <param name="s"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<FrameDLRObject> OrderBy<TKey>(this IEnumerable<FrameDLRObject> obj, Func<dynamic, TKey> s, IComparer<TKey> comparer)
        {
            return obj.OrderBy<FrameDLRObject, TKey>(p => s.Invoke(p), comparer);
        }
        /// <summary>
        /// 扩展的linq的OrderByDescending操作
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="obj"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<FrameDLRObject> OrderByDescending<TKey>(this IEnumerable<FrameDLRObject> obj, Func<dynamic, TKey> s)
        {
            return obj.OrderByDescending<FrameDLRObject, TKey>(p => s.Invoke(p));
        }
        /// <summary>
        /// 扩展的linq的OrderByDescending操作
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="obj"></param>
        /// <param name="s"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<FrameDLRObject> OrderByDescending<TKey>(this IEnumerable<FrameDLRObject> obj, Func<dynamic, TKey> s, IComparer<TKey> comparer)
        {
            return obj.OrderByDescending<FrameDLRObject, TKey>(p => s.Invoke(p), comparer);
        }
        /// <summary>
        /// 扩展的linq的ThenBy操作
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="obj"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<FrameDLRObject> ThenBy<TKey>(this IOrderedEnumerable<FrameDLRObject> obj, Func<dynamic, TKey> keySelector)
        {
            return obj.ThenBy<FrameDLRObject, TKey>(p => keySelector.Invoke(p));
        }
        /// <summary>
        /// 扩展的linq的ThenByDescending操作
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="obj"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<FrameDLRObject> ThenByDescending<TKey>(this IOrderedEnumerable<FrameDLRObject> obj, Func<dynamic, TKey> keySelector)
        {
            return obj.ThenByDescending<FrameDLRObject, TKey>(p => keySelector.Invoke(p));
        }
        /// <summary>
        /// 扩展的linq的tolist操作
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<FrameDLRObject> ToList(this IEnumerable<FrameDLRObject> obj)
        {
            return obj.ToList<FrameDLRObject>();
        }

        /// <summary>
        /// 扩展的linq的todictionry操作
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="obj"></param>
        /// <param name="keySelector"></param>
        /// <param name="elementSelector"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(this IEnumerable<FrameDLRObject> obj, Func<dynamic, TKey> keySelector, Func<dynamic, TElement> elementSelector)
        {
            var rtn = new Dictionary<TKey, TElement>();
            foreach (var item in obj)
            {
                var key = keySelector.Invoke(item);
                var value = elementSelector.Invoke(item);
                if (!rtn.ContainsKey(key))
                {
                    rtn.Add(key, value);
                }
            }
            //return obj.ToDictionary(k => (TKey)keySelector.Invoke(k), v => (TElement)elementSelector.Invoke(v));
            return rtn;
        }
        /// <summary>
        /// 扩展的linq的Any操作
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static bool Any(this IEnumerable<FrameDLRObject> obj, Func<dynamic, bool> a)
        {
            //转为list可以使列表内容固定住，否则会导致无法直接进行操作
            return obj.Any<FrameDLRObject>((p => a.Invoke(p)));
        }
        /// <summary>
        /// 执行join操作
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="outer"></param>
        /// <param name="inner"></param>
        /// <param name="outerKeySelector"></param>
        /// <param name="innerKeySelector"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> Join<TKey, TResult>(this IEnumerable<FrameDLRObject> outer, IEnumerable<FrameDLRObject> inner, Func<dynamic, TKey> outerKeySelector, Func<dynamic, TKey> innerKeySelector, Func<dynamic, dynamic, TResult> resultSelector)
        {
            var rtn = new List<TResult>();
            foreach (dynamic o in outer)
            {
                TKey okey = outerKeySelector.Invoke(o);
                foreach (dynamic i in inner)
                {
                    TKey ikey = innerKeySelector.Invoke(i);
                    if (okey.Equals(ikey))
                        rtn.Add(resultSelector.Invoke(((FrameDLRObject)o).Clone(), ((FrameDLRObject)i).Clone()));
                }
            }
            return rtn;
        }
        /// <summary>
        /// 执行多重join操作
        /// </summary>
        /// <typeparam name="TOuter"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="outer"></param>
        /// <param name="inner"></param>
        /// <param name="outerKeySelector"></param>
        /// <param name="innerKeySelector"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> Join<TOuter, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<FrameDLRObject> inner, Func<TOuter, TKey> outerKeySelector, Func<dynamic, TKey> innerKeySelector, Func<TOuter, dynamic, TResult> resultSelector)
        {
            var rtn = new List<TResult>();
            foreach (TOuter o in outer)
            {
                TKey okey = outerKeySelector.Invoke(o);
                foreach (dynamic i in inner)
                {
                    TKey ikey = innerKeySelector.Invoke(i);
                    if (okey.Equals(ikey))
                    {
                        rtn.Add(resultSelector.Invoke((TOuter)ComFunc.CloneObject(o), ((FrameDLRObject)i).Clone()));
                    }
                }
            }
            return rtn;
        }
        /// <summary>
        /// 数组转json
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ToJsonString(this IEnumerable<object> list)
        {
            if (list == null) return "";
            var slist = new List<string>();
            foreach(var item in list)
            {
                if(item is IJSONable)
                {
                    slist.Add(((IJSONable)item).ToJSONString());
                }
                else
                {
                    FrameDLRObject dobj = FrameDLRObject.CreateInstance(item, FrameDLRFlags.SensitiveCase);
                    slist.Add(dobj.ToJSONString()+",");
                }
            }
            var tmp = string.Concat(slist);
            return $"[{(tmp.Length > 0?tmp.Substring(0,tmp.Length-1):tmp)}]";
        }
    }
}
