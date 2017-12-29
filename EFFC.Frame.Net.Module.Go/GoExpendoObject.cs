using EFFC.Frame.Net.Base.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Dynamic;
using System.Reflection;
using EFFC.Frame.Net.Base.Constants;
using System.Linq;
using EFFC.Frame.Net.Module.Extend.WebGo.Attributes;

namespace EFFC.Frame.Net.Module.Extend.WebGo
{
    public class GoExpendoObject:FrameExposedObject
    {
        private Dictionary<string, AttributeMethodCollection> rest_method = new Dictionary<string, AttributeMethodCollection>();
        string currentAttribute = "";
        protected GoExpendoObject(object obj, bool is_throw_exception) : base(obj, is_throw_exception,true)
        {
            var list = m_type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (var m in list)
            {
                var attrs = m.GetCustomAttributes();
                foreach(var a in attrs)
                {
                    var aname = a.GetType().Name.ToLower().Replace("attribute", "");
                    if (!rest_method.ContainsKey(aname))
                    {
                        rest_method.Add(aname, new AttributeMethodCollection(a));
                    }
                    rest_method[aname].AddMethod(m);
                }
            }
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            //如果为rest请求方法
            if (RestFul.RestFulMethod.Contains(binder.Name.ToUpper()))
            {
                currentAttribute = binder.Name.ToLower();
                result = this;
                return true;
            }
            //如果为attribute的方法
            if (rest_method.ContainsKey(binder.Name.ToLower()))
            {
                currentAttribute = binder.Name.ToLower();
                result = this;
                return true;
            }

            var propertyInfo = m_object.GetType().GetProperty(
                 binder.Name,
                 BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (propertyInfo != null)
            {
                result = propertyInfo.GetValue(m_object, null);
                return true;
            }

            var fieldInfo = m_object.GetType().GetField(
                binder.Name,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (fieldInfo != null)
            {
                result = fieldInfo.GetValue(m_object);
                return true;
            }

            result = null;
            if (_is_throw_exception_when_method_not_exits)
                return false;
            else
                return true;
        }
        public object InvokeRestMethod(string method,object target,params object[] args)
        {
            object rtn = null;
            if (currentAttribute != "" && rest_method.ContainsKey(currentAttribute))
            {
                ExposedObjectHelper.InvokeBestMethod(args, target, rest_method[currentAttribute].GetMethod(method, args.Length), out rtn);
            }
            else
            {

            }
            return rtn;
        }
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if(currentAttribute != "" && rest_method.ContainsKey(currentAttribute))
            {
                ExposedObjectHelper.InvokeBestMethod(args, m_object,rest_method[currentAttribute].GetMethod(binder.Name, args.Length),out result);
                return true;
            }

            return base.TryInvokeMember(binder, args, out result);
        }
        private class AttributeMethodCollection
        {
            private Dictionary<string, Dictionary<int, List<MethodInfo>>> rest_method = new Dictionary<string, Dictionary<int, List<MethodInfo>>>();
            public AttributeMethodCollection(Attribute a)
            {
                AttributeName = a.GetType().Name.ToLower().Replace("attribute", "");
            }
            public string AttributeName
            {
                get;set;
            }
            public List<MethodInfo> GetMethod(string name, int paramlength)
            {
                if (rest_method.ContainsKey(name.ToLower()) && rest_method[name.ToLower()].ContainsKey(paramlength))
                    return rest_method[name][paramlength];
                return new List<MethodInfo>();
            }
            public void AddMethod(MethodInfo m)
            {
                if (!rest_method.ContainsKey(m.Name.ToLower()))
                    rest_method.Add(m.Name.ToLower(), new Dictionary<int, List<MethodInfo>>());
                if (!rest_method[m.Name.ToLower()].ContainsKey(m.GetParameters().Length))
                    rest_method[m.Name.ToLower()].Add(m.GetParameters().Length, new List<MethodInfo>());

                if (!rest_method[m.Name.ToLower()][m.GetParameters().Length].Contains(m))
                    rest_method[m.Name.ToLower()][m.GetParameters().Length].Add(m);
            }
        }
    }
}
