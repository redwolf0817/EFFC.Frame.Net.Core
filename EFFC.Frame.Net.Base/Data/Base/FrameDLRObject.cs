/*
 * Used for EFFC.Frame 1.0 & EFFC.Frame 2.5
 * Added by chuan.yin in 2014/11/13
 */
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Interfaces.DataConvert;
using Frame.Net.Base.Interfaces.DataConvert;
using Frame.Net.Base.ResouceManage.JsEngine;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Collections;

namespace EFFC.Frame.Net.Base.Data.Base
{
    /// <summary>
    /// 动态数据对象，对象属性的为忽略大小的方式
    /// </summary>
    [Serializable]
    public class FrameDLRObject : MyDynamicMetaProvider, IJSONable, IDisposable, ICloneable
    {
        static HostJs jse = null;
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
        public string OriJsonString
        {
            get
            {
                if (ori_jsongstring == "")
                {
                    ori_jsongstring = ToJSON_Structure();
                }
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
        public string DUID
        {
            get
            {
                if (_duid == "")
                {
                    _duid = ComFunc.getMD5_String(OriJsonString, Encoding.UTF8);
                }
                return _duid;
            }
        }
        /// <summary>
        /// 获取结构唯一标示码，该码源自结构化定义json，用于标示本对象的结构定义唯一性
        /// </summary>
        public string CUID
        {
            get
            {
                if (_cuid == "")
                {
                    var s = ToJSON_Structure();
                    _cuid = ComFunc.getMD5_String(s, Encoding.UTF8);
                }
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
        public static object[] CreateArray(string arrjsonstring, params KeyValuePair<string, object>[] context)
        {
            if (!string.IsNullOrEmpty(arrjsonstring))
            {
                lock (lockobj)
                {
                    CreateJSE();
                    try
                    {

                        string js = "var out=" + arrjsonstring;
                        if (context != null)
                        {
                            jse.Evaluate(js, context);
                        }
                        else
                        {
                            jse.Evaluate(js);
                        }
                        var d = jse.GetOutObject("out");
                        if (d is object[])
                        {
                            return (object[])d;
                        }
                        else
                        {
                            return null;
                        }

                    }
                    finally
                    {
                        ReleaseJSE();
                    }
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
        public static object[] CreateArray(string arrjsonstring)
        {
            return CreateArray(arrjsonstring, null);
        }
        /// <summary>
        /// 根据格式化的json串创建数组对象
        /// </summary>
        /// <param name="formatjson"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static object[] CreateArrayFormat(string formatjson, params object[] values)
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

            return CreateArray(json, context.ToArray());
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
                else if (model is Dictionary<string, object>)
                {
                    rtn = BuildLoopDics((Dictionary<string, object>)model, flags);
                }
                else if (model is Dictionary<string, FrameDLRObject>)
                {
                    rtn = BuildLoopDics((Dictionary<string, FrameDLRObject>)model, flags);
                }
                else if (model is XmlDocument)
                {
                    rtn = BuildLoopXml(((XmlDocument)model).FirstChild.ChildNodes, flags);
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
                        if (str.Trim().StartsWith("<") && TryParseXml(ComFunc.nvl(str), out xd))
                        {
                            if (xd.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
                            {
                                if (xd.ChildNodes.Count == 2)
                                {
                                    rtn = BuildLoopXml(xd.ChildNodes[1].ChildNodes, flags);
                                }
                                else
                                {
                                    rtn = BuildLoopXml(xd.ChildNodes, flags);
                                }
                            }
                            else
                            {
                                if (xd.ChildNodes.Count == 1)
                                {
                                    rtn = BuildLoopXml(xd.FirstChild.ChildNodes, flags);
                                }
                                else
                                {
                                    rtn = BuildLoopXml(xd.ChildNodes, flags);
                                }
                            }
                        }
                        else
                        {
                            lock (lockobj)
                            {
                                try
                                {
                                    CreateJSE();
                                    string js = "var out=" + str;
                                    if (context != null)
                                    {
                                        jse.Evaluate(js, context);
                                    }
                                    else
                                    {
                                        jse.Evaluate(js);
                                    }
                                    var d = (Dictionary<string, object>)jse.GetOutObject("out");
                                    rtn = BuildLoopDics(d, flags);
                                }
                                finally
                                {
                                    ReleaseJSE();
                                }
                            }
                        }
                    }
                }
                else
                {
                    var t = model.GetType();
                    rtn.ignorecase = flags == FrameDLRFlags.SensitiveCase ? false : true;
                    foreach (var p in t.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    {
                        rtn.SetValue(p.Name, p.GetValue(model));
                    }
                    foreach (var p in t.GetFields(BindingFlags.Instance | BindingFlags.Public))
                    {
                        rtn.SetValue(p.Name, p.GetValue(model));
                    }
                }
            }
            return rtn;
        }
        /// <summary>
        /// 通过xml建立动态对象，最简单的xml结构，如下：
        /// <xxx>
        ///     <n1></n1>
        ///     <n2></n2>
        ///     <nodes>
        ///         <nn1></nn1>
        ///         <nnn></nnn>
        ///     </nodes>
        ///     <nn></nn>
        /// </xxx>
        /// </summary>
        /// <param name="node"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        private static dynamic BuildLoopXml(XmlNodeList nodelist, FrameDLRFlags flags)
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

            foreach (XmlNode item in nodelist)
            {
                if (item.HasChildNodes && item.FirstChild.NodeType == XmlNodeType.Element)
                {
                    FrameDLRObject rtnloop = BuildLoopXml(item.ChildNodes, flags);
                    rtn.SetValue(item.Name, rtnloop);
                }
                else
                {
                    if (DateTimeStd.IsDateTime(item.InnerText))
                    {
                        rtn.SetValue(item.Name, DateTimeStd.ParseStd(item.InnerText).Value);
                    }
                    else
                    {
                        rtn.SetValue(item.Name, item.InnerText);
                    }

                }
            }


            return rtn;
        }

        /// <summary>
        /// 递归建立动态json对象
        /// </summary>
        /// <param name="d"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        private static dynamic BuildLoopDics(Dictionary<string, object> d, FrameDLRFlags flags)
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
                if (item.Value is Dictionary<string, object>)
                {
                    FrameDLRObject rtnloop = BuildLoopDics((Dictionary<string, object>)item.Value, flags);
                    rtn.SetValue(item.Key, (object)rtnloop);
                }
                else if (item.Value is object[])
                {
                    List<object> list = new List<object>();
                    foreach (var m in (object[])item.Value)
                    {
                        if (m is Dictionary<string, object>)
                        {
                            list.Add(BuildLoopDics((Dictionary<string, object>)m, flags));
                        }
                        else
                        {
                            list.Add(m);
                        }
                    }
                    rtn.SetValue(item.Key, list.ToArray());
                }
                else if (item.Value is DateTime)
                {
                    var dt = (DateTime)item.Value;
                    if (dt == DateTime.MinValue.ToLocalTime())
                    {
                        rtn.SetValue(item.Key, DateTime.MinValue);
                    }
                    else
                    {
                        rtn.SetValue(item.Key, item.Value);
                    }
                }
                else
                {
                    rtn.SetValue(item.Key, item.Value);
                }
            }

            return rtn;
        }
        private static dynamic BuildLoopDics(Dictionary<string, FrameDLRObject> d, FrameDLRFlags flags)
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
                if (_d.ContainsKey(key))
                {
                    result = _d[key];
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
                if (args.Length <= 0)
                    return this.ToJSONString();
                else
                    return this.ToJSONString((Encoding)args[0]);
            }
            else if (methodInfo.ToLower() == "tojsonobject")
            {
                return this.ToJSONObject();
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
                return true;
            }
            catch (Exception ex)
            {
                return false;
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
                var x = new XmlDocument();
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
        public string ToJSONString()
        {
            return ToJSON(this, 0);
        }
        /// <summary>
        /// 转成Json串
        /// </summary>
        /// <param name="encode">对于string或byte类型的编码方式</param>
        /// <returns></returns>
        public string ToJSONString(Encoding encode)
        {
            return ToJSON(this, 0, encode);
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

        private string ToJSON(FrameDLRObject obj, int level)
        {
            return ToJSON(obj._d, level, Encoding.UTF8);
        }
        private string ToJSON(FrameDLRObject obj, int level, Encoding encode)
        {
            return ToJSON(obj._d, level, encode);
        }
        private string GetTabs(int level)
        {
            string rtn = "";
            for (int i = 0; i < level; i++)
            {
                rtn += tabflag;
            }
            return rtn;
        }
        private string ToJSON(Dictionary<string, object> d, int level, Encoding encode)
        {
            StringBuilder rtn = new StringBuilder();
            string template = GetTabs(level + 1) + @"""{0}"":{1},";
            rtn.AppendLine("{");
            foreach (var item in d)
            {
                if (item.Value is int)
                {
                    rtn.AppendLine(string.Format(template, item.Key, item.Value));
                }
                else if (item.Value != null && item.Value.GetType().FullName == typeof(double).FullName)
                {
                    var val = (double)item.Value;
                    if (double.IsInfinity(val))
                    {
                        rtn.AppendLine(string.Format(template, item.Key, "\"" + item.Value + "\""));
                    }
                    else
                    {
                        rtn.AppendLine(string.Format(template, item.Key, item.Value));
                    }
                }
                else if (item.Value != null && item.Value.GetType().FullName == typeof(decimal).FullName)
                {
                    var val = (decimal)item.Value;
                    rtn.AppendLine(string.Format(template, item.Key, item.Value));
                }
                else if (item.Value is float)
                {
                    var val = (float)item.Value;
                    if (float.IsInfinity(val))
                    {
                        rtn.AppendLine(string.Format(template, item.Key, "\"" + item.Value + "\""));
                    }
                    else
                    {
                        rtn.AppendLine(string.Format(template, item.Key, item.Value));
                    }

                }
                else if (item.Value is long)
                {
                    rtn.AppendLine(string.Format(template, item.Key, item.Value));
                }
                else if (item.Value is Dictionary<string, object>)
                {
                    rtn.AppendLine(string.Format(template, item.Key, ToJSON((Dictionary<string, object>)item.Value, level + 1, encode)));
                }
                else if (item.Value is object[])
                {
                    rtn.AppendLine(string.Format(template, item.Key, ToJSON((object[])item.Value, level + 1, encode)));
                }
                else if (item.Value is IEnumerable<FrameDLRObject>)
                {
                    rtn.AppendLine(string.Format(template, item.Key, ToJSON(((IEnumerable<FrameDLRObject>)item.Value).ToArray(), level + 1, encode)));
                }
                else if (item.Value is IEnumerable<Object>)
                {
                    rtn.AppendLine(string.Format(template, item.Key, ToJSON(((IEnumerable<Object>)item.Value).ToArray(), level + 1, encode)));
                }
                else if (item.Value is IJSONable)
                {
                    if (item.Value is FrameDLRObject)
                    {
                        rtn.AppendLine(string.Format(template, item.Key, ToJSON((FrameDLRObject)item.Value, level + 1)));
                    }
                    else
                    {
                        rtn.AppendLine(string.Format(template, item.Key, ((IJSONable)item.Value).ToJSONString()));
                    }
                }
                else if (item.Value is DateTime)
                {
                    //rtn.AppendLine(string.Format(template, item.Key, "\"" + ((DateTime)item.Value).ToString("yyyy/MM/dd HH:mm:ss fff") + "\""));
                    var dt = ((DateTime)item.Value);
                    if (dt == DateTime.MinValue)
                    {
                        rtn.AppendLine(string.Format(template, item.Key, string.Format("new Date('0001-01-01')")));
                    }
                    else
                    {
                        rtn.AppendLine(string.Format(template, item.Key, string.Format("new Date({0},{1},{2},{3},{4},{5})", dt.Year, dt.Month - 1, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond)));
                    }

                }
                else if (item.Value is bool)
                {
                    rtn.AppendLine(string.Format(template, item.Key, item.Value.ToString().ToLower()));
                }
                else if (item.Value is string)
                {
                    rtn.AppendLine(string.Format(template, item.Key, "\"" + item.Value.ToString().Replace("\r", "").Replace("\n", "#n#").Replace(@"\", @"\\").Replace("#n#", "\\n").Replace("\"", @"\""") + "\""));
                }
                else if (item.Value is byte[])
                {
                    //byte可以直接转化成base64，不需要中间转一次
                    //rtn.AppendLine(string.Format(template, item.Key, "\"" + ComFunc.Base64Code(ComFunc.ByteToString((byte[])item.Value, encode), encode) + "\""));
                    rtn.AppendLine(string.Format(template, item.Key, "\"" + ComFunc.Base64Code((byte[])item.Value) + "\""));
                }
                else if (item.Value == null || item.Value == DBNull.Value)
                {
                    rtn.AppendLine(string.Format(template, item.Key, "\"\""));
                }

            }

            var str = rtn.ToString().Trim();
            rtn = new StringBuilder(str.Length > 1 ? str.Substring(0, str.Length - 1) : str);
            rtn.AppendLine("");
            rtn.Append(GetTabs(level) + "}");
            return rtn.ToString();
        }

        private string ToJSON(Object[] arr, int level, Encoding encode)
        {
            StringBuilder rtn = new StringBuilder();
            string template = GetTabs(level + 1) + "{0},";
            rtn.AppendLine("[");
            foreach (var obj in arr)
            {
                if (obj is int)
                {
                    rtn.AppendLine(string.Format(template, obj));
                }
                else if (obj is double)
                {
                    rtn.AppendLine(string.Format(template, obj));
                }
                else if (obj is float)
                {
                    rtn.AppendLine(string.Format(template, obj));
                }
                else if (obj is long)
                {
                    rtn.AppendLine(string.Format(template, obj));
                }
                else if (obj is Dictionary<string, object>)
                {
                    rtn.AppendLine(string.Format(template, ToJSON((Dictionary<string, object>)obj, level + 1, encode)));
                }
                else if (obj is object[])
                {
                    rtn.AppendLine(string.Format(template, ToJSON((object[])obj, level + 1, encode)));
                }
                else if (obj is IJSONable)
                {
                    if (obj is FrameDLRObject)
                    {
                        rtn.AppendLine(string.Format(template, ToJSON((FrameDLRObject)obj, level + 1)));
                    }
                    else
                    {
                        rtn.AppendLine(string.Format(template, ((IJSONable)obj).ToJSONString()));
                    }
                }
                else if (obj is DateTime)
                {
                    //rtn.AppendLine(string.Format(template, "\"" + ((DateTime)obj).ToString("yyyy/MM/dd HH:mm:ss fff") + "\""));
                    var dt = ((DateTime)obj);
                    if (dt == DateTime.MinValue)
                    {
                        rtn.AppendLine(string.Format(template, string.Format("new Date('0001-01-01')")));
                    }
                    else
                    {
                        rtn.AppendLine(string.Format(template, string.Format("new Date({0},{1},{2},{3},{4},{5},{6})", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond)));
                    }

                }
                else if (obj is bool)
                {
                    rtn.AppendLine(string.Format(template, obj.ToString().ToLower()));
                }
                else if (obj is string)
                {
                    rtn.AppendLine(string.Format(template, "\"" + obj.ToString().Replace("\r", "").Replace("\n", "#n#").Replace(@"\", @"\\").Replace("#n#", "\\n") + "\""));
                }
                else if (obj is byte[])
                {
                    //byte可以直接转化成base64，不需要中间转一次
                    //rtn.AppendLine(string.Format(template, "\"" + ComFunc.Base64Code(ComFunc.ByteToString((byte[])obj, encode), encode) + "\""));
                    rtn.AppendLine(string.Format(template, "\"" + ComFunc.Base64Code((byte[])obj)));
                }
                else if (obj == null || obj == DBNull.Value)
                {
                    rtn.AppendLine(string.Format(template, "\"\""));
                }
            }
            var str = rtn.ToString().Trim();
            rtn = new StringBuilder(str.Length > 1 ? str.Substring(0, str.Length - 1) : str);
            rtn.AppendLine("");
            rtn.Append(GetTabs(level) + "]");
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
                            if(ComFunc.nvl(v) == "")
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
            if(this._d != null)
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
        /// <summary>
        /// 通过公共创建js引擎以加快DLR对象创建时的速度
        /// </summary>
        private static void CreateJSE()
        {
            lock (lockobj)
            {
                if (jse == null) jse = HostJs.NewInstance();
                createTime = DateTime.Now;
            }
        }
        /// <summary>
        /// 采用定时释放的机制
        /// </summary>
        private static async void ReleaseJSE()
        {
            lock (lockobj)
            {
                if ((DateTime.Now - createTime).TotalMinutes > jseexpire_min)
                {
                    jse.Dispose();
                }
            }
        }
    }
    /// <summary>
    /// FrameDLRObject的相关扩展
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
            return obj.Select<FrameDLRObject, FrameDLRObject>(p => (FrameDLRObject)FrameDLRObject.CreateInstance(s.Invoke(p), p.Flags));
        }

        /// <summary>
        /// 扩展的linq的where操作
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static IEnumerable<FrameDLRObject> Where(this IEnumerable<FrameDLRObject> obj, Func<dynamic, bool> s)
        {
            return obj.Where<FrameDLRObject>(p => s.Invoke(p));
        }
        /// <summary>
        /// 扩展的linq的OrderBy操作
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="obj"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static IEnumerable<FrameDLRObject> OrderBy<TKey>(this IEnumerable<FrameDLRObject> obj, Func<dynamic, TKey> s)
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
        public static IEnumerable<FrameDLRObject> OrderBy<TKey>(this IEnumerable<FrameDLRObject> obj, Func<dynamic, TKey> s, IComparer<TKey> comparer)
        {
            return obj.OrderBy<FrameDLRObject, TKey>(p => s.Invoke(p));
        }
        /// <summary>
        /// 扩展的linq的OrderByDescending操作
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="obj"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static IEnumerable<FrameDLRObject> OrderByDescending<TKey>(this IEnumerable<FrameDLRObject> obj, Func<dynamic, TKey> s)
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
        public static IEnumerable<FrameDLRObject> OrderByDescending<TKey>(this IEnumerable<FrameDLRObject> obj, Func<dynamic, TKey> s, IComparer<TKey> comparer)
        {
            return obj.OrderByDescending<FrameDLRObject, TKey>(p => s.Invoke(p));
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
        /// 扩展的linq的Any操作
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static bool Any(this IEnumerable<FrameDLRObject> obj, Func<dynamic, bool> a)
        {
            return obj.Any<FrameDLRObject>((p => a.Invoke(p)));
        }
    }
}
