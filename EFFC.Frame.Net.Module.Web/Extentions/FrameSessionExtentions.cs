using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Interfaces.DataConvert;
using Frame.Net.Base.Interfaces.DataConvert;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EFFC.Frame.Net.Module.Web.Extentions
{
    /// <summary>
    /// Frame下的session的方法扩展
    /// </summary>
    public static class FrameSessionExtentions
    {
        /// <summary>
        /// 从session中获取对象
        /// </summary>
        /// <param name="session"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object GetObject(this ISession session, string key)
        {
            var str = session.GetString(key);
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            var arr = str.Split(';');
            if (arr.Length != 2)
            {
                return null;
            }
            var assemblefullname = arr[0];
            var base64 = arr[1];

            var value = ComFunc.Base64DeCode(base64.Replace(" ", "+"));
            var typefullname = assemblefullname.Substring(0, assemblefullname.IndexOf(","));
            var t = Type.GetType(assemblefullname);
            if (typefullname == typeof(string).FullName)
            {
                return value;
            }
            if (typefullname == typeof(FrameDLRObject).FullName)
            {
                return (FrameDLRObject)FrameDLRObject.CreateInstance(value, FrameDLRFlags.SensitiveCase);
            }
            if (t.GetTypeInfo().GetInterface(typeof(IJSONParsable).Name, true) != null)
            {
                var obj = (IJSONParsable)Activator.CreateInstance(t);
                obj.TryParseJSON(value);
                return obj;
            }
            if (typefullname == typeof(int).FullName)
            {
                return int.Parse(value);
            }
            if (typefullname == typeof(long).FullName)
            {
                return long.Parse(value);
            }
            if (typefullname == typeof(float).FullName)
            {
                return float.Parse(value);
            }
            if (typefullname == typeof(double).FullName)
            {
                return double.Parse(value);
            }
            if (typefullname == typeof(decimal).FullName)
            {
                return decimal.Parse(value);
            }
            if (typefullname == typeof(DateTime).FullName)
            {
                return DateTimeStd.ParseStd(value, "yyyy/MM/dd HH:mm:ss fff").Value;
            }
            if (typefullname == typeof(bool).FullName)
            {
                return bool.Parse(value);
            }


            FrameDLRObject dobj = FrameDLRObject.CreateInstance(value, FrameDLRFlags.SensitiveCase);

            return dobj.ToModel(t);
        }
        /// <summary>
        /// 将对象写入session
        /// </summary>
        /// <param name="session"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetObject(this ISession session, string key, object value)
        {
            if (value == null)
            {
                session.Remove(key);
                return;
            }
            var t = value.GetType();
            string assemblyQualifiedName = t.AssemblyQualifiedName;
            var controlType = t.FullName;
            string assemblyInformation = assemblyQualifiedName.Substring(assemblyQualifiedName.IndexOf(","));
            var assemblefullname = controlType + assemblyInformation;
            if (value is FrameDLRObject)
            {

                session.SetString(key, assemblefullname + ";" + ComFunc.Base64Code(((FrameDLRObject)value).ToJSONString()));
            }
            else if (value is IJSONable)
            {
                session.SetString(key, assemblefullname + ";" + ComFunc.Base64Code(((IJSONable)value).ToJSONString()));
            }
            else if (value is string)
            {
                session.SetString(key, assemblefullname + ";" + ComFunc.Base64Code(ComFunc.nvl(value)));
            }
            else if (value is int)
            {
                session.SetString(key, assemblefullname + ";" + ComFunc.Base64Code(ComFunc.nvl(value)));
            }
            else if (value is long)
            {
                session.SetString(key, assemblefullname + ";" + ComFunc.Base64Code(ComFunc.nvl(value)));
            }
            else if (value is double)
            {
                session.SetString(key, assemblefullname + ";" + ComFunc.Base64Code(ComFunc.nvl(value)));
            }
            else if (value is float)
            {
                session.SetString(key, assemblefullname + ";" + ComFunc.Base64Code(ComFunc.nvl(value)));
            }
            else if (value is decimal)
            {
                session.SetString(key, assemblefullname + ";" + ComFunc.Base64Code(ComFunc.nvl(value)));
            }
            else if (value is DateTime)
            {
                session.SetString(key, assemblefullname + ";" + ComFunc.Base64Code(((DateTime)value).ToString("yyyy/MM/dd HH:mm:ss fff")));
            }
            else if (value is bool)
            {
                session.SetString(key, assemblefullname + ";" + ComFunc.Base64Code(ComFunc.nvl(value)));
            }
            else
            {
                FrameDLRObject dobj = FrameDLRObject.CreateInstance(value);
                session.SetString(key, assemblefullname + ";" + ComFunc.Base64Code(dobj.ToJSONString()));
            }
        }
    }
}
