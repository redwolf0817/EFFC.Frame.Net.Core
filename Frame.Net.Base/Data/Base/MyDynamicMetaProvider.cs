using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Base.Data.Base
{
    public abstract class MyDynamicMetaProvider : IDynamicMetaObjectProvider
    {
        /// <summary>
        /// 动态对象给属性赋值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected abstract object SetMetaValue(string key, object value);
        /// <summary>
        /// 动态对象根据属性名称获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected abstract object GetMetaValue(string key);
        /// <summary>
        /// 动态对象根据下标赋值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="indexs"></param>
        /// <returns></returns>
        protected virtual object SetMetaIndexValue(object[] indexs, object value)
        {
            return value;
        }
        /// <summary>
        /// 动态对象根据下标获取值
        /// </summary>
        /// <param name="indexs"></param>
        /// <returns></returns>
        protected virtual object GetMetaIndexValue(object[] indexs)
        {
            return null;
        }
        #region Invoke Delegate
        /// <summary>
        /// 动态对象执行方法
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected abstract object InvokeMe(string methodInfo, params object[] args);
        /// <summary>
        /// 动态对象执行方法
        /// </summary>
        /// <param name="methodinfo"></param>
        /// <returns></returns>
        public object InvokeDelegate0(string methodinfo)
        {
            return InvokeMe(methodinfo);
        }
        /// <summary>
        /// 动态对象执行方法
        /// </summary>
        /// <param name="methodinfo"></param>
        /// <param name="arg1"></param>
        /// <returns></returns>
        public object InvokeDelegate1(string methodinfo, object arg1)
        {
            return InvokeMe(methodinfo, arg1);
        }
        /// <summary>
        /// 动态对象执行方法
        /// </summary>
        /// <param name="methodinfo"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        public object InvokeDelegate2(string methodinfo, object arg1, object arg2)
        {
            return InvokeMe(methodinfo, arg1, arg2);
        }
        /// <summary>
        /// 动态对象执行方法
        /// </summary>
        /// <param name="methodinfo"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <returns></returns>
        public object InvokeDelegate3(string methodinfo, object arg1, object arg2, object arg3)
        {
            return InvokeMe(methodinfo, arg1, arg2, arg3);
        }
        /// <summary>
        /// 动态对象执行方法
        /// </summary>
        /// <param name="methodinfo"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <returns></returns>
        public object InvokeDelegate4(string methodinfo, object arg1, object arg2, object arg3, object arg4)
        {
            return InvokeMe(methodinfo, arg1, arg2, arg3, arg4);
        }
        /// <summary>
        /// 动态对象执行方法
        /// </summary>
        /// <param name="methodinfo"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <returns></returns>
        public object InvokeDelegate5(string methodinfo, object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            return InvokeMe(methodinfo, arg1, arg2, arg3, arg4, arg5);
        }
        /// <summary>
        /// 动态对象执行方法
        /// </summary>
        /// <param name="methodinfo"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <param name="arg6"></param>
        /// <returns></returns>
        public object InvokeDelegate6(string methodinfo, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            return InvokeMe(methodinfo, arg1, arg2, arg3, arg4, arg5, arg6);
        }
        /// <summary>
        /// 动态对象执行方法
        /// </summary>
        /// <param name="methodinfo"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <param name="arg6"></param>
        /// <param name="arg7"></param>
        /// <returns></returns>
        public object InvokeDelegate7(string methodinfo, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)
        {
            return InvokeMe(methodinfo, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
        /// <summary>
        /// 动态对象执行方法
        /// </summary>
        /// <param name="methodinfo"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <param name="arg6"></param>
        /// <param name="arg7"></param>
        /// <param name="arg8"></param>
        /// <returns></returns>
        public object InvokeDelegate8(string methodinfo, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)
        {
            return InvokeMe(methodinfo, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }
        #endregion

        public class MyDynamicMetaObject : DynamicMetaObject
        {
            internal MyDynamicMetaObject(Expression parameter, MyDynamicMetaProvider dynamicDictionary)
                : base(parameter, BindingRestrictions.Empty, dynamicDictionary)
            {

            }
            public override DynamicMetaObject BindConvert(ConvertBinder binder)
            {
                return base.BindConvert(binder);
            }
            public override DynamicMetaObject BindSetIndex(SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
            {
                // Method to call in the containing class:
                string methodName = "SetMetaIndexValue";
                // setup the binding restrictions.
                BindingRestrictions restrictions = BindingRestrictions.GetTypeRestriction(Expression, LimitType);
                // setup the parameters:
                Expression[] args = new Expression[2];
                // First parameter is the name of the property to Set
                var indexlist = new List<Object>();
                foreach (var item in indexes)
                {
                    indexlist.Add(item.Value);
                }
                args[0] = Expression.Constant(indexlist.ToArray(),typeof(object[]));
                // Second parameter is the value
                args[1] = Expression.Convert(value.Expression, typeof(object));
                // Setup the 'this' reference
                Expression self = Expression.Convert(Expression, LimitType);
                // Setup the method call expression
                Expression methodCall = Expression.Call(self, typeof(MyDynamicMetaProvider).GetMethod(methodName, BindingFlags.NonPublic
                    | BindingFlags.Public
                    | BindingFlags.Instance), args);
                // Create a meta object to invoke Set later:
                DynamicMetaObject setDictionaryEntry = new DynamicMetaObject(methodCall, restrictions);
                // return that dynamic object
                return setDictionaryEntry;

            }
            public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
            {

                // Method call in the containing class:
                string methodName = "GetMetaIndexValue";
                // One parameter
                var indexlist = new List<Object>();
                foreach (var item in indexes)
                {
                    indexlist.Add(item.Value);
                }
                Expression[] parameters = new Expression[] { Expression.Constant(indexlist.ToArray(),typeof(object[])) };
                DynamicMetaObject getDictionaryEntry = new DynamicMetaObject(
                                    Expression.Call(
                                                Expression.Convert(Expression, LimitType),
                                                typeof(MyDynamicMetaProvider).GetMethod(methodName, BindingFlags.NonPublic
                                                | BindingFlags.Public
                                                | BindingFlags.Instance),
                                                parameters),
                                    BindingRestrictions.GetTypeRestriction(Expression, LimitType));
                return getDictionaryEntry;

            }
            public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
            {

                // Method call in the containing class:
                string methodName = "GetMetaValue";
                // One parameter
                Expression[] parameters = new Expression[] { Expression.Constant(binder.Name) };
                DynamicMetaObject getDictionaryEntry = new DynamicMetaObject(
                                    Expression.Call(
                                                Expression.Convert(Expression, LimitType),
                                                typeof(MyDynamicMetaProvider).GetMethod(methodName, BindingFlags.NonPublic
                                                | BindingFlags.Public
                                                | BindingFlags.Instance),
                                                parameters),
                                    BindingRestrictions.GetTypeRestriction(Expression, LimitType));
                return getDictionaryEntry;
            }

            public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
            {
                // Method to call in the containing class:
                string methodName = "SetMetaValue";
                // setup the binding restrictions.
                BindingRestrictions restrictions = BindingRestrictions.GetTypeRestriction(Expression, LimitType);
                // setup the parameters:
                Expression[] args = new Expression[2];
                // First parameter is the name of the property to Set
                args[0] = Expression.Constant(binder.Name);
                // Second parameter is the value
                args[1] = Expression.Convert(value.Expression, typeof(object));
                // Setup the 'this' reference
                Expression self = Expression.Convert(Expression, LimitType);
                // Setup the method call expression
                Expression methodCall = Expression.Call(self, typeof(MyDynamicMetaProvider).GetMethod(methodName, BindingFlags.NonPublic
                    | BindingFlags.Public
                    | BindingFlags.Instance), args);
                // Create a meta object to invoke Set later:
                DynamicMetaObject setDictionaryEntry = new DynamicMetaObject(methodCall, restrictions);
                // return that dynamic object
                return setDictionaryEntry;
            }
            public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
            {
                // Method call in the containing class:
                string methodName = "InvokeDelegate" + (args.Length > 8 ? 8 : args.Length);
                // One parameter
                var a = from la in args select la.Value;
                List<Expression> lp = new List<System.Linq.Expressions.Expression>();
                lp.Add(Expression.Constant(binder.Name));
                var index = 0;
                foreach (var item in args)
                {
                    if (index >= 8)
                    {
                        break;
                    }
                    //.net中的struct为valuetype，与object是无法直接等效的，需要将对应的struct进行convert才行，否则会报ArgumentException
                    //lp.Add(item.Expression);
                    lp.Add(Expression.Convert(item.Expression, typeof(object)));
                    index++;
                }

                DynamicMetaObject getDictionaryEntry = new DynamicMetaObject(
                                    Expression.Call(
                                                Expression.Convert(Expression, LimitType),
                                                typeof(MyDynamicMetaProvider).GetMethod(methodName),
                                                lp.ToArray()),
                                    BindingRestrictions.GetTypeRestriction(Expression, LimitType));
                return getDictionaryEntry;
            }
        }

        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new MyDynamicMetaObject(parameter, this);
        }

    }
}
