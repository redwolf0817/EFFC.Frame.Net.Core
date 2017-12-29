/*
 * Used for EFFC.Frame 1.0 & EFFC.Frame 2.5
 * Added by chuan.yin in 2014/11/13
 */
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Interfaces.DataConvert;
using EFFC.Frame.Net.Base.ResouceManage.JsEngine;
using Noesis.Javascript;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Json;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EFFC.Frame.Net.Base.Data.Base
{
    /// <summary>
    /// 动态数据对象，对象属性的为忽略大小的方式
    /// </summary>
    [Serializable]
    public class FrameDLRObject : MyDynamicMetaProvider, IJSONable,IDisposable,ICloneable
    {
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
        /// 获取创建时的json定义串
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
                HostJs jse = HostJs.NewInstance();
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
                    jse.Release();
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
        public static dynamic CreateInstanceFromat(string fromatjsonstring,FrameDLRFlags flag, params object[] values)
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

            rtn.ori_jsongstring = fromatjsonstring;
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
        /// 创建一个DLR的新实例对象,默认忽略大小写
        /// </summary>
        /// <param name="jsonstring"></param>
        /// <returns></returns>
        public static dynamic CreateInstance(string jsonstring)
        {
            return CreateInstance(jsonstring, null);
        }
        /// <summary>
        /// 根据json串创建动态对象
        /// </summary>
        /// <param name="jsonstring"></param>
        /// <param name="flags"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static dynamic CreateInstance(string jsonstring, FrameDLRFlags flags, params KeyValuePair<string, object>[] context)
        {
            FrameDLRObject rtn = null;
            if (!string.IsNullOrEmpty(jsonstring))
            {
                HostJs jse = HostJs.NewInstance();
                try
                {
                     XmlDocument xd = null;
                     if (jsonstring.Trim().StartsWith("<") && TryParseXml(ComFunc.nvl(jsonstring), out xd))
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
                         string js = "var out=" + jsonstring;
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
                }
                finally
                {
                    jse.Release();
                }

            }
            else
            {
                rtn = CreateInstance();
            }
            rtn.ori_jsongstring = jsonstring;
            return rtn;
        }
        /// <summary>
        /// 根据json串创建动态对象,默认忽略大小写
        /// </summary>
        /// <param name="jsonstring"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static dynamic CreateInstance(string jsonstring, params KeyValuePair<string, object>[] context)
        {
            return CreateInstance(jsonstring, FrameDLRFlags.None, context);
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
        /// 创建动态对象，通过反射将model的property转成动态对象,添加 成功失败参数 和 消息参数
        /// </summary>
        /// <param name="issuccess">是否成功失败</param>
        /// <param name="msg">消息</param>
        /// <param name="model">其他需要扩展的model</param>
        /// <returns></returns>
        public static dynamic CreateInstance(bool issuccess,string msg, object model = null)
        {
            var rtn = CreateInstance(model, FrameDLRFlags.None);
            rtn.issuccess = issuccess;
            rtn.msg = msg;
            return rtn;
        }

        /// <summary>
        /// 创建动态对象，通过反射将model的property转成动态对象。
        /// </summary>
        /// <param name="model"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static dynamic CreateInstance(object model, FrameDLRFlags flags)
        {
            FrameDLRObject rtn = CreateInstance();
            if (model != null)
            {
                if (model is Dictionary<string, object>)
                {
                    rtn = BuildLoopDics((Dictionary<string, object>)model, flags);
                }
                else if (model is Dictionary<string, FrameDLRObject>)
                {
                    rtn = BuildLoopDics((Dictionary<string, FrameDLRObject>)model, flags);
                }
                else if (model is string)
                {
                    XmlDocument xd = null;
                    if (TryParseXml(ComFunc.nvl(model), out xd))
                    {
                        rtn = BuildLoopXml(xd.FirstChild.ChildNodes, flags);
                    }
                    else
                    {
                        rtn = CreateInstance(ComFunc.nvl(model), flags);
                    }
                    return rtn;
                }
                else if (model is XmlDocument)
                {
                    rtn = BuildLoopXml(((XmlDocument)model).FirstChild.ChildNodes, flags);
                }
                else
                {
                    var t = model.GetType();
                    rtn.ignorecase = flags == FrameDLRFlags.SensitiveCase ? false : true;
                    foreach (var p in t.GetProperties(BindingFlags.Instance | BindingFlags.Public))
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
        /// set value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetValue(string key, object value)
        {
            if (ignorecase)
            {
                if (_d.ContainsKey(key.ToLower()))
                    _d[key.ToLower()] = value;
                else
                    _d.Add(key.ToLower(), value);
            }
            else
            {
                if (_d.ContainsKey(key))
                    _d[key] = value;
                else
                    _d.Add(key, value);
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
        private static bool TryParseXml(string xml,out XmlDocument xd)
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
            catch (Exception)
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
        /// 转成Json串
        /// </summary>
        /// <returns></returns>
        public string ToJSONString()
        {
            return ToJSON(this, 0);
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
                else if (s.Value is List<FrameDLRObject>)
                {
                    List<object> list = new List<object>();
                    foreach (var m in (List<FrameDLRObject>)s.Value)
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
                else if (s.Value is List<object>)
                {
                    List<object> list = new List<object>();
                    foreach (var m in (List<object>)s.Value)
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
            return ToJSON_Structure(obj._d, level);
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
                else if (item.Value is List<FrameDLRObject>)
                {
                    rtn.AppendLine(string.Format(template, item.Key, ToJSON_Structure(((List<FrameDLRObject>)item.Value).ToArray(), level + 1)));
                }
                else if (item.Value is List<Object>)
                {
                    rtn.AppendLine(string.Format(template, item.Key, ToJSON_Structure(((List<Object>)item.Value).ToArray(), level + 1)));
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
            return ToJSON(obj._d, level);
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
        private string ToJSON(Dictionary<string, object> d, int level)
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
                    rtn.AppendLine(string.Format(template, item.Key, ToJSON((Dictionary<string, object>)item.Value, level + 1)));
                }
                else if (item.Value is Noesis.Javascript.JavascriptFunction)
                {
                    rtn.AppendLine(string.Format(template, item.Key, "\"" + ((Noesis.Javascript.JavascriptFunction)item.Value).FunctionString + "\""));
                }
                else if (item.Value is object[])
                {
                    rtn.AppendLine(string.Format(template, item.Key, ToJSON((object[])item.Value, level + 1)));
                }
                else if (item.Value is List<FrameDLRObject>)
                {
                    rtn.AppendLine(string.Format(template, item.Key, ToJSON(((List<FrameDLRObject>)item.Value).ToArray(), level + 1)));
                }
                else if (item.Value is List<Object>)
                {
                    rtn.AppendLine(string.Format(template, item.Key, ToJSON(((List<Object>)item.Value).ToArray(), level + 1)));
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
                    rtn.AppendLine(string.Format(template, item.Key, string.Format("new Date({0},{1},{2},{3},{4},{5})", dt.Year, dt.Month - 1, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond)));
                }
                else if (item.Value is bool)
                {
                    rtn.AppendLine(string.Format(template, item.Key, item.Value.ToString().ToLower()));
                }
                else if (item.Value is string)
                {
                    rtn.AppendLine(string.Format(template, item.Key, "\"" + item.Value.ToString().Replace("\r", "").Replace("\n", "#n#").Replace(@"\", @"\\").Replace("#n#", "\\n").Replace("\"",@"\""") + "\""));
                }
                else if (item.Value == null)
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

        private string ToJSON(Object[] arr, int level)
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
                    rtn.AppendLine(string.Format(template, ToJSON((Dictionary<string, object>)obj, level + 1)));
                }
                else if (obj is Noesis.Javascript.JavascriptFunction)
                {
                    rtn.AppendLine(((Noesis.Javascript.JavascriptFunction)obj).FunctionString);
                }
                else if (obj is object[])
                {
                    rtn.AppendLine(string.Format(template, ToJSON((object[])obj, level + 1)));
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
                    rtn.AppendLine(string.Format(template, string.Format("new Date({0},{1},{2},{3},{4},{5},{6})", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond)));
                }
                else if (obj is bool)
                {
                    rtn.AppendLine(string.Format(template, obj.ToString().ToLower()));
                }
                else if (obj is string)
                {
                    rtn.AppendLine(string.Format(template, "\"" + obj.ToString().Replace("\r", "").Replace("\n", "#n#").Replace(@"\", @"\\").Replace("#n#", "\\n") + "\""));
                }
            }
            var str = rtn.ToString().Trim();
            rtn = new StringBuilder(str.Length > 1 ? str.Substring(0, str.Length - 1) : str);
            rtn.AppendLine("");
            rtn.Append(GetTabs(level) + "]");
            return rtn.ToString();
        }
        /// <summary>
        /// 采用反射方式转化为强类型的对象,对效能有影响，不建议使用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ToModel<T>()
        {
            T t = (T)Activator.CreateInstance(typeof(T), true);
            foreach (var p in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                p.SetValue(t, this.GetValue(p.Name));
            }
            return t;
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
            if (_d != null)
            {
                foreach (var item in _d)
                {
                    if (item.Value is IDisposable)
                    {
                        ((IDisposable)item.Value).Dispose();
                    }
                }
                _d.Clear();
                _d = null;
            }
            if (_valuelist != null)
            {
                _valuelist.Clear();
                _valuelist = null;
            }
            _cuid = "";
            _duid = "";
            tabflag = "";
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
}
