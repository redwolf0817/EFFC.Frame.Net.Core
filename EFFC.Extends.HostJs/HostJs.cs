using EFFC.Frame.Net.Base.Interfaces.Core;
using Frame.Net.Base.Interfaces.Extentions;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace EFFC.Extends.HostJs
{
    /// <summary>
    /// HostJs的基类
    /// </summary>
    public abstract class HostJs : IHostJsEngine, IResourceEntity, IDisposable
    {
        string _id = "hostjs_" + DateTime.Now.ToString("yyyyMMddHhmmssfff");
        static string _default_engine = "Chakra";
        /// <summary>
        /// 默认的引擎名称为:VRoomJs,Chakra，默认为Chakra
        /// </summary>
        public static string DefaultEngineName
        {
            get
            {
                return _default_engine;
            }
            set
            {
                _default_engine = value;
            }
        }

        /// <summary>
        /// 通过反射获取指定的Js引擎
        /// </summary>
        /// <param name="typeName">参数为:VRoomJs,Chakra，默认为Chakra</param>
        /// <returns></returns>
        public static HostJs NewInstance(string typeName= "")
        {
            if(typeName == "")
            {
                typeName = _default_engine;
            }
            return new JsSwitcher(typeName);
        }
        /// <summary>
        /// 通过反射获取指定的Js引擎
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T NewInstance<T>() where T : HostJs
        {
            return Activator.CreateInstance<T>();
        }

        public virtual string ID
        {
            get
            {
                return _id;
            }
        }
        /// <summary>
        /// 资源释放，垃圾回收
        /// </summary>
        public virtual void Dispose()
        {
            if (!IsDisposed())
            {
                Release();
            }
        }
        /// <summary>
        /// 执行js，并返回结果
        /// </summary>
        /// <param name="script"></param>
        /// <param name="kvp"></param>
        /// <returns></returns>
        public abstract object Evaluate(string script, params KeyValuePair<string, object>[] kvp);
        /// <summary>
        /// 执行js，不返回结果
        /// </summary>
        /// <param name="script"></param>
        /// <param name="kvp"></param>
        public abstract void Excute(string script, params KeyValuePair<string, object>[] kvp);
        /// <summary>
        /// 获取当前js环境中的变量对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract object GetOutObject(string key);
        /// <summary>
        /// 是否已经释放
        /// </summary>
        /// <returns></returns>
        public abstract bool IsDisposed();
        /// <summary>
        /// 释放资源
        /// </summary>
        public abstract void Release();

        static List<string> list = null;
        /// <summary>
        /// Js的保留关键字
        /// </summary>
        public static string[] ReserveKeys
        {
            get
            {
                if (list == null)
                {
                    list = new List<string>();
                    list.Add("break");
                    list.Add("case");
                    list.Add("catch");
                    list.Add("continue");
                    list.Add("default");
                    list.Add("delete");
                    list.Add("do");
                    list.Add("else");
                    list.Add("finally");
                    list.Add("for");
                    list.Add("function");
                    list.Add("if");
                    list.Add("in");
                    list.Add("instanceof");
                    list.Add("new");
                    list.Add("return");
                    list.Add("switch");
                    list.Add("this");
                    list.Add("throw");
                    list.Add("try");
                    list.Add("typeof");
                    list.Add("var");
                    list.Add("void");
                    list.Add("while");
                    list.Add("with");

                    list.Add("abstract");
                    list.Add("boolean");
                    list.Add("byte");
                    list.Add("char");
                    list.Add("class");
                    list.Add("const");
                    list.Add("debugger");
                    list.Add("double");
                    list.Add("enum");
                    list.Add("export");
                    list.Add("extends");
                    list.Add("final");
                    list.Add("float");
                    list.Add("goto");
                    list.Add("implements");
                    list.Add("import");
                    list.Add("int");
                    list.Add("interface");
                    list.Add("long");
                    list.Add("native");
                    list.Add("package");
                    list.Add("private");
                    list.Add("protected");
                    list.Add("public");
                    list.Add("short");
                    list.Add("static");
                    list.Add("super");
                    list.Add("synchronized");
                    list.Add("throws");
                    list.Add("transient");
                    list.Add("volatile");
                }
                return list.ToArray();
            }
        }
    }
}
