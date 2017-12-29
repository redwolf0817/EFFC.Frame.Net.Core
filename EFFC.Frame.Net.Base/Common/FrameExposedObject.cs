using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EFFC.Frame.Net.Base.Common
{
    /// <summary>
    /// 框架中通过动态对象扩展对class的访问
    /// </summary>
    public class FrameExposedObject:DynamicObject
    {
        protected object m_object;
        protected Type m_type;
        protected Dictionary<string, Dictionary<int, List<MethodInfo>>> m_instanceMethods;
        protected Dictionary<string, Dictionary<int, List<MethodInfo>>> m_genInstanceMethods;
        protected Dictionary<string, PropertyInfo> m_instanceProperties;
        protected Dictionary<string, FieldInfo> m_instanceFields;
        protected bool _is_throw_exception_when_method_not_exits = false;
        private bool _is_ignore_case = false;
        protected FrameExposedObject(object obj,bool is_throw_exception, bool is_ignore_case)
        {
            m_object = obj;
            m_type = obj.GetType();
            _is_throw_exception_when_method_not_exits = is_throw_exception;
            _is_ignore_case = is_ignore_case;

            var methods = m_type
                   .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            //for循环的效率比linq要高一个数量级
            m_instanceMethods = new Dictionary<string, Dictionary<int, List<MethodInfo>>>();
            m_genInstanceMethods = new Dictionary<string, Dictionary<int, List<MethodInfo>>>();
            foreach (var item in methods)
            {
                if (item.IsGenericMethod)
                {
                    if (!m_genInstanceMethods.ContainsKey(item.Name))
                    {
                        m_genInstanceMethods.Add(item.Name, new Dictionary<int, List<MethodInfo>>());
                    }
                    var ikey = item.GetParameters().Length;
                    if (!m_genInstanceMethods[item.Name].ContainsKey(ikey))
                    {
                        m_genInstanceMethods[item.Name].Add(ikey, new List<MethodInfo>());
                    }
                    m_genInstanceMethods[item.Name][ikey].Add(item);
                }
                else
                {
                    if (!m_instanceMethods.ContainsKey(item.Name))
                    {
                        m_instanceMethods.Add(item.Name, new Dictionary<int, List<MethodInfo>>());
                    }
                    var ikey = item.GetParameters().Length;
                    if (!m_instanceMethods[item.Name].ContainsKey(ikey))
                    {
                        m_instanceMethods[item.Name].Add(ikey, new List<MethodInfo>());
                    }
                    m_instanceMethods[item.Name][ikey].Add(item);
                }
            }
            m_instanceProperties = new Dictionary<string, PropertyInfo>();
            m_instanceFields = new Dictionary<string, FieldInfo>();
            var properties = m_type.GetProperties(
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (var item in properties)
            {
                var key = _is_ignore_case ? item.Name.ToLower() : item.Name;
                m_instanceProperties.Add(key, item);
            }
            var fields = m_type.GetFields(
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (var item in fields)
            {
                var key = _is_ignore_case ? item.Name.ToLower() : item.Name;
                m_instanceFields.Add(key, item);
            }
            //m_instanceMethods =
            //    m_type
            //        .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            //        .Where(m => !m.IsGenericMethod)
            //        .GroupBy(m => m.Name)
            //        .ToDictionary(
            //            p => p.Key,
            //            p => p.GroupBy(r => r.GetParameters().Length).ToDictionary(r => r.Key, r => r.ToList()));

            //m_genInstanceMethods =
            //    m_type
            //        .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            //        .Where(m => m.IsGenericMethod)
            //        .GroupBy(m => m.Name)
            //        .ToDictionary(
            //            p => p.Key,
            //            p => p.GroupBy(r => r.GetParameters().Length).ToDictionary(r => r.Key, r => r.ToList()));
        }

        public object Object { get { return m_object; } }
        /// <summary>
        /// 将指定的对象转换成动态对象，可以执行隐藏方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static dynamic New<T>()
        {
            return New(typeof(T));
        }
        /// <summary>
        /// 将指定的对象转换成动态对象，可以执行隐藏方法
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="is_throw_exception">如果执行是，对象方法或属性不存在，是否要抛出异常，默认为false</param>
        /// <returns></returns>
        public static dynamic New(Type type, bool is_throw_exception = false, bool is_ignore_case = false)
        {
            return new FrameExposedObject(Create(type), is_throw_exception, is_ignore_case);
        }
        /// <summary>
        /// 工具方法：根据type和参数进行实例创建
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <returns>该type的实例</returns>
        public static object Create(Type type,params object[] args)
        {
            List<Type> lt_args = new List<Type>();
            if(args != null)
            {
                foreach(var arg in args)
                {
                    lt_args.Add(arg.GetType());
                }
            }
            ConstructorInfo constructorInfo = GetConstructorInfo(type,lt_args.ToArray());
            if (args != null)
                return constructorInfo.Invoke(args);
            else
                return constructorInfo.Invoke(new object[0]);
        }
        /// <summary>
        /// 工具方法：根据TObject和参数进行实例创建
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        public static TObject Create<TObject>(params object[] args)
        {
            return (TObject)Create(typeof(TObject), args);
        }
        /// <summary>
        /// 工具方法：根据type创建实例
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Create(Type type)
        {
            return Create(type, new object[0]);
        }

        private static ConstructorInfo GetConstructorInfo(Type type, params Type[] args)
        {
            ConstructorInfo constructorInfo = type.GetConstructor(args);
            if (constructorInfo != null)
            {
                return constructorInfo;
            }

            throw new MissingMemberException(type.FullName, new Exception(string.Format(".ctor({0})", string.Join(", ", args.Select(t => t.FullName).ToArray()))));
        }
        /// <summary>
        /// 将对象转为动态对象，可以执行隐藏方法
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="is_throw_exception">如果执行是，对象方法或属性不存在，是否要抛出异常，默认为false</param>
        /// <returns></returns>
        public static dynamic From(object obj, bool is_throw_exception = false, bool is_ignore_case = false)
        {
            return new FrameExposedObject(obj, is_throw_exception, is_ignore_case);
        }
        /// <summary>
        /// 将FrameExposedObject转化为原型对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static T Cast<T>(FrameExposedObject t)
        {
            return (T)t.m_object;
        }
        /// <summary>
        /// 尝试执行非泛型方法
        /// </summary>
        /// <param name="methodname"></param>
        /// <param name="result"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool InvokeMethod(string methodname, out object result, params object[] args)
        {
            if(m_instanceMethods.ContainsKey(methodname)
                    && m_instanceMethods[methodname].ContainsKey(args.Length)
                    && ExposedObjectHelper.InvokeBestMethod(args, m_object, m_instanceMethods[methodname][args.Length], out result))
            {
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var key = _is_ignore_case ? binder.Name.ToLower() : binder.Name;
            // Get type args of the call
            Type[] typeArgs = ExposedObjectHelper.GetTypeArgs(binder);
            if (typeArgs != null && typeArgs.Length == 0) typeArgs = null;

            //
            // Try to call a non-generic instance method
            //
            if (typeArgs == null
                    && m_instanceMethods.ContainsKey(key)
                    && m_instanceMethods[key].ContainsKey(args.Length)
                    && ExposedObjectHelper.InvokeBestMethod(args, m_object, m_instanceMethods[key][args.Length], out result))
            {
                return true;
            }

            //
            // Try to call a generic instance method
            //
            if (m_genInstanceMethods.ContainsKey(key)
                    && m_genInstanceMethods[key].ContainsKey(args.Length))
            {
                List<MethodInfo> methods = new List<MethodInfo>();

                foreach (var method in m_genInstanceMethods[key][args.Length])
                {
                    if (method.GetGenericArguments().Length == typeArgs.Length)
                    {
                        methods.Add(method.MakeGenericMethod(typeArgs));
                    }
                }

                if (ExposedObjectHelper.InvokeBestMethod(args, m_object, methods, out result))
                {
                    return true;
                }
            }

            result = null;
            if (_is_throw_exception_when_method_not_exits)
                return false;
            else
                return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var key = _is_ignore_case ? binder.Name.ToLower() : binder.Name;

            if (m_instanceProperties.ContainsKey(key))
            {
                m_instanceProperties[key].SetValue(m_object, value, null);
                return true;
            }

            if (m_instanceFields.ContainsKey(key))
            {
                m_instanceFields[key].SetValue(m_object, value);
                return true;
            }

            if (_is_throw_exception_when_method_not_exits)
                return false;
            else
                return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var key = _is_ignore_case ? binder.Name.ToLower() : binder.Name;

          
            if (m_instanceProperties.ContainsKey(key))
            {
                result = m_instanceProperties[key].GetValue(m_object, null);
                return true;
            }

            if (m_instanceFields.ContainsKey(key))
            {
                result = m_instanceFields[key].GetValue(m_object);
                return true;
            }

            result = null;
            if (_is_throw_exception_when_method_not_exits)
                return false;
            else
                return true;
        }
        /// <summary>
        /// 判断是否有指定名称的方法
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasMethod(string name)
        {
            return m_instanceMethods.ContainsKey(name) | m_genInstanceMethods.ContainsKey(name);
        }
        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            result = m_object;
            return true;
        }
    }
}
