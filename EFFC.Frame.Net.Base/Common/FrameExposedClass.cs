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
    public class FrameExposedClass: DynamicObject
    {
        private Type m_type;
        protected Dictionary<string, Dictionary<int, List<MethodInfo>>> m_staticMethods;
        protected Dictionary<string, Dictionary<int, List<MethodInfo>>> m_genStaticMethods;
        protected Dictionary<string, Dictionary<int, List<MethodInfo>>> m_instanceMethods;
        protected Dictionary<string, Dictionary<int, List<MethodInfo>>> m_genInstanceMethods;
        protected Dictionary<string, PropertyInfo> m_staticProperties;
        protected Dictionary<string, FieldInfo> m_staticFields;
        private bool _is_throw_exception_when_method_not_exits = false;
        private bool _is_ignore_case = false;

        protected FrameExposedClass(Type type,bool is_throw_exception,bool is_ignore_case)
        {
            m_type = type;
            _is_throw_exception_when_method_not_exits = is_throw_exception;
            _is_ignore_case = is_ignore_case;

            var methods = m_type
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            //for循环的效率比linq要高一个数量级
            m_staticMethods = new Dictionary<string, Dictionary<int, List<MethodInfo>>>();
            m_genStaticMethods = new Dictionary<string, Dictionary<int, List<MethodInfo>>>();
            foreach (var item in methods)
            {
                var key = _is_ignore_case ? item.Name.ToLower() : item.Name;
                if (item.IsGenericMethod)
                {
                    if (!m_genStaticMethods.ContainsKey(key))
                    {
                        m_genStaticMethods.Add(key, new Dictionary<int, List<MethodInfo>>());
                    }
                    var ikey = item.GetParameters().Length;
                    if (!m_genStaticMethods[key].ContainsKey(ikey))
                    {
                        m_genStaticMethods[key].Add(ikey, new List<MethodInfo>());
                    }
                    m_genStaticMethods[key][ikey].Add(item);
                }
                else
                {
                    if (!m_staticMethods.ContainsKey(key))
                    {
                        m_staticMethods.Add(key, new Dictionary<int, List<MethodInfo>>());
                    }
                    var ikey = item.GetParameters().Length;
                    if (!m_staticMethods[key].ContainsKey(ikey))
                    {
                        m_staticMethods[key].Add(ikey, new List<MethodInfo>());
                    }
                    m_staticMethods[key][ikey].Add(item);
                }
            }

            var instance_methods = m_type
                   .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            //for循环的效率比linq要高一个数量级
            m_instanceMethods = new Dictionary<string, Dictionary<int, List<MethodInfo>>>();
            m_genInstanceMethods = new Dictionary<string, Dictionary<int, List<MethodInfo>>>();
            foreach (var item in instance_methods)
            {
                var key = _is_ignore_case ? item.Name.ToLower() : item.Name;
                if (item.IsGenericMethod)
                {
                    if (!m_genInstanceMethods.ContainsKey(item.Name))
                    {
                        m_genInstanceMethods.Add(key, new Dictionary<int, List<MethodInfo>>());
                    }
                    var ikey = item.GetParameters().Length;
                    if (!m_genInstanceMethods[key].ContainsKey(ikey))
                    {
                        m_genInstanceMethods[key].Add(ikey, new List<MethodInfo>());
                    }
                    m_genInstanceMethods[key][ikey].Add(item);
                }
                else
                {
                    if (!m_instanceMethods.ContainsKey(item.Name))
                    {
                        m_instanceMethods.Add(key, new Dictionary<int, List<MethodInfo>>());
                    }
                    var ikey = item.GetParameters().Length;
                    if (!m_instanceMethods[key].ContainsKey(ikey))
                    {
                        m_instanceMethods[key].Add(ikey, new List<MethodInfo>());
                    }
                    m_instanceMethods[key][ikey].Add(item);
                }
            }

            m_staticProperties = new Dictionary<string, PropertyInfo>();
            m_staticFields = new Dictionary<string, FieldInfo>();
            var properties = m_type.GetProperties(
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            foreach(var item in properties)
            {
                var key = _is_ignore_case ? item.Name.ToLower() : item.Name;
                m_staticProperties.Add(key, item);
            }
            var fields = m_type.GetFields(
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            foreach (var item in fields)
            {
                var key = _is_ignore_case ? item.Name.ToLower() : item.Name;
                m_staticFields.Add(key, item);
            }
            //m_staticMethods =
            //    methods
            //        .Where(m => !m.IsGenericMethod)
            //        .GroupBy(m => m.Name)
            //        .ToDictionary(
            //            p => p.Key,
            //            p => p.GroupBy(r => r.GetParameters().Length).ToDictionary(r => r.Key, r => r.ToList()));

            //m_genStaticMethods =
            //    m_type
            //        .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
            //        .Where(m => m.IsGenericMethod)
            //        .GroupBy(m => m.Name)
            //        .ToDictionary(
            //            p => p.Key,
            //            p => p.GroupBy(r => r.GetParameters().Length).ToDictionary(r => r.Key, r => r.ToList()));
        }
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            // Get type args of the call
            Type[] typeArgs = ExposedObjectHelper.GetTypeArgs(binder);
            if (typeArgs != null && typeArgs.Length == 0) typeArgs = null;

            var key = _is_ignore_case ? binder.Name.ToLower() : binder.Name;
            //
            // Try to call a non-generic instance method
            //
            if (typeArgs == null
                    && m_staticMethods.ContainsKey(key)
                    && m_staticMethods[key].ContainsKey(args.Length)
                    && ExposedObjectHelper.InvokeBestMethod(args, null, m_staticMethods[key][args.Length], out result))
            {
                return true;
            }

            //
            // Try to call a generic instance method
            //
            if (m_staticMethods.ContainsKey(key)
                    && m_staticMethods[key].ContainsKey(args.Length))
            {
                List<MethodInfo> methods = new List<MethodInfo>();

                foreach (var method in m_genStaticMethods[key][args.Length])
                {
                    if (method.GetGenericArguments().Length == typeArgs.Length)
                    {
                        methods.Add(method.MakeGenericMethod(typeArgs));
                    }
                }

                if (ExposedObjectHelper.InvokeBestMethod(args, null, methods, out result))
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

            if (m_staticProperties.ContainsKey(key))
            {
                m_staticProperties[key].SetValue(null, value, null);
                return true;
            }

            if (m_staticFields.ContainsKey(key))
            {
                m_staticFields[key].SetValue(null, value);
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

            if (m_staticProperties.ContainsKey(key))
            {
                result = m_staticProperties[key].GetValue(null, null);
                return true;
            }

            if (m_staticFields.ContainsKey(key))
            {
                result = m_staticFields[key].GetValue(null);
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
            return m_staticMethods.ContainsKey(name) | m_genStaticMethods.ContainsKey(name);
        }
        /// <summary>
        /// 获取指定对象的静态方法信息
        /// </summary>
        /// <param name="type">对象的类型</param>
        /// <param name="is_throw_exception">如果执行是，对象方法或属性不存在，是否要抛出异常，默认为false</param>
        /// <returns></returns>
        public static dynamic From(Type type,bool is_throw_exception=false,bool is_ignore_case=false)
        {
            return new FrameExposedClass(type, is_throw_exception, is_ignore_case);
        }

        /// <summary>
        /// 尝试执行非泛型方法
        /// </summary>
        /// <param name="methodname"></param>
        /// <param name="target"></param>
        /// <param name="result"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool InvokeMethod(string methodname,object target, out object result, params object[] args)
        {
            var key = _is_ignore_case ? methodname.ToLower() : methodname;
            if (m_instanceMethods.ContainsKey(key)
                    && m_instanceMethods[key].ContainsKey(args.Length)
                    && ExposedObjectHelper.InvokeBestMethod(args, target, m_instanceMethods[key][args.Length], out result))
            {
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
    }
}
