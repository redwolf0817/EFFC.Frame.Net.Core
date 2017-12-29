using EFFC.Frame.Net.Base.AttributeDefine;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Base.ResouceManage.JsEngine
{
    /// <summary>
    /// Host Js的扩展对象
    /// 可用于获取扩展对象的反射结构
    /// </summary>
    public abstract class BaseHostJsObject:IDisposable
    {
        FrameDLRObject _properties = null;
        FrameDLRObject _functions = null;
        /// <summary>
        /// 对象作用描述
        /// </summary>
        public abstract string Description
        {
            get;
        }
        /// <summary>
        /// 对象名称,作为hostjs中对象的访问名称
        /// </summary>
        public abstract string Name
        {
            get;
        }
        /// <summary>
        /// 获取属性
        /// </summary>
        public FrameDLRObject Properties
        {
            get
            {
                if (_properties == null)
                {
                    _properties = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                    var plist = new List<string>();
                    _properties.SetValue("list", plist);
                    var t = this.GetType();
                    var ps = t.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    foreach (var item in ps)
                    {
                        var iscanuse = true;
                        var attrs = item.GetCustomAttributes(false);
                        foreach (var attr in attrs)
                        {
                            if (attr is CanUseAttribute)
                            {
                                iscanuse = ((CanUseAttribute)attr).IsCanUse;
                                break;
                            }
                        }

                        if (iscanuse && item.Name != "Functions" && item.Name != "Properties" && item.Name != "ConstructorDesc"
                            && item.Name != "Description" && item.Name != "Name")
                        {
                            var o = FrameDLRObject.CreateInstance();
                            o.name = item.Name;
                            o.type = item.PropertyType.Name;
                            o.desc = "";
                            foreach (var attr in attrs)
                            {
                                if (attr is DescAttribute)
                                {
                                    o.desc = ((DescAttribute)attr).Desc;
                                    break;
                                }
                            }
                            plist.Add(o.name);
                            _properties.SetValue(o.name, o);
                        }
                    }
                }

                return _properties;
            }
        }
        /// <summary>
        /// 获取所有方法的定义
        /// </summary>
        public FrameDLRObject Functions
        {
            get
            {
                if (_functions == null)
                {
                    _functions = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                    var plist = new List<string>();
                    _functions.SetValue("list", plist);
                    var t = this.GetType();
                    var fs = t.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    foreach (var item in fs)
                    {
                        var iscanuse = true;
                        var attrs = item.GetCustomAttributes(false);
                        foreach (var attr in attrs)
                        {
                            if (attr is CanUseAttribute)
                            {
                                iscanuse = ((CanUseAttribute)attr).IsCanUse;
                                break;
                            }
                        }

                        if (!iscanuse 
                            ||item.Name.StartsWith("get_")
                            || item.Name.StartsWith("set_")
                            || item.Name == "Equals"
                            || item.Name == "GetHashCode"
                            || item.Name == "GetType"
                            || item.Name == "ToString")
                        {
                            continue;
                        }
                        var o = FrameDLRObject.CreateInstance();
                        o.name = item.Name;
                        var pps = item.GetParameters();
                        var lp = new List<FrameDLRObject>();
                        foreach (var p in pps)
                        {
                            var fp = FrameDLRObject.CreateInstance();
                            fp.name = p.Name;
                            fp.type = p.ParameterType.Name;
                            fp.defaultvalue = p.DefaultValue;
                            lp.Add(fp);
                        }
                        o.arguements = lp;
                        o.returntype = item.ReturnType.Name;
                        foreach (var attr in attrs)
                        {
                            if (attr is DescAttribute)
                            {
                                o.desc = ((DescAttribute)attr).Desc;
                                break;
                            }
                        }
                        plist.Add(o.name);
                        _functions.SetValue(o.name, o);
                    }
                }

                return _functions;
            }
        }
        /// <summary>
        /// 获取整个对象结构的描述
        /// </summary>
        public FrameDLRObject ConstructorDesc
        {
            get
            {
                var contructordesc = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                contructordesc.properties = this.Properties;
                contructordesc.functions = this.Functions;
                contructordesc.description = this.Description;
                contructordesc.name = this.Name;
                return contructordesc;
            }
        }

        public void Dispose()
        {
            _functions.Dispose();
            _properties.Dispose();
        }
    }
}
