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
        /// <param name="arg"></param>
        /// <returns></returns>
        public object InvokeDelegate(string methodinfo, object[] arg)
        {
            return InvokeMe(methodinfo, arg);
        }
        /// <summary>
        /// 动态对象执行方法
        /// </summary>
        /// <param name="methodinfo"></param>
        /// <returns></returns>
        public object InvokeDelegateNoParam(string methodinfo)
        {
            return InvokeMe(methodinfo, new object[] { });
        }
        #endregion

        #region Operation
        protected object MetaBinaryOperation(string methodname,object v)
        {
            if (methodname == "MetaEqual")
            {
                return MetaEqual(v);
            }
            else if (methodname == "MetaNotEqual")
            {
                return MetaNotEqual(v);
            }
            else if (methodname == "MetaGreaterThan")
            {
                return MetaGreaterThan(v);
            }
            else if (methodname == "MetaGreaterThanEqual")
            {
                return MetaGreaterThanEqual(v);
            }
            else if (methodname == "MetaLessThan")
            {
                return MetaLessThan(v);
            }
            else if (methodname == "MetaLessThanEqual")
            {
                return MetaLessThanEqual(v);
            }
            else if (methodname == "MetaAnd")
            {
                return MetaAnd(v);
            }
            else if (methodname == "MetaAndAlso")
            {
                return MetaAndAlso(v);
            }
            else if (methodname == "MetaOr")
            {
                return MetaOr(v);
            }
            else if (methodname == "MetaOrElse")
            {
                return MetaOrElse(v);
            }
            else if (methodname == "MetaAdd")
            {
                return MetaAdd(v);
            }
            else if (methodname == "MetaAddAssign")
            {
                return MetaAddAssign(v);
            }
            else if (methodname == "MetaSubstract")
            {
                return MetaSubstract(v);
            }
            else if (methodname == "MetaSubstractAssign")
            {
                return MetaSubstractAssign(v);
            }
            else if (methodname == "MetaMultiply")
            {
                return MetaMultiply(v);
            }
            else if (methodname == "MetaMultiplyAssign")
            {
                return MetaMultiplyAssign(v);
            }
            else if (methodname == "MetaDivide")
            {
                return MetaDivide(v);
            }
            else if (methodname == "MetaDivideAssign")
            {
                return MetaDivideAssign(v);
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// a+b操作
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        protected virtual object MetaAdd(object v)
        {
            throw new NotSupportedException("不支持+操作");
        }
        /// <summary>
        /// a+=b操作
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        protected virtual object MetaAddAssign(object v)
        {
            throw new NotSupportedException("不支持+=操作");
        }
        /// <summary>
        /// a-b操作
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        protected virtual object MetaSubstract(object v)
        {
            throw new NotSupportedException("不支持-操作");
        }
        /// <summary>
        /// a-=b操作
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        protected virtual object MetaSubstractAssign(object v)
        {
            throw new NotSupportedException("不支持-=操作");
        }
        /// <summary>
        /// a*b操作
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        protected virtual object MetaMultiply(object v)
        {
            throw new NotSupportedException("不支持*操作");
        }
        /// <summary>
        /// a*=b操作
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        protected virtual object MetaMultiplyAssign(object v)
        {
            throw new NotSupportedException("不支持*=操作");
        }
        /// <summary>
        /// a/b操作
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        protected virtual object MetaDivide(object v)
        {
            throw new NotSupportedException("不支持/操作");
        }
        /// <summary>
        /// a/=b操作
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        protected virtual object MetaDivideAssign(object v)
        {
            throw new NotSupportedException("不支持/=操作");
        }
        /// <summary>
        /// ==操作
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        protected virtual object MetaEqual(object v)
        {
            return this.Equals(v);
        }
        /// <summary>
        /// !=操作
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        protected virtual object MetaNotEqual(object v)
        {
            return !this.Equals(v);
        }
        /// <summary>
        /// >操作
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        protected virtual object MetaGreaterThan(object v)
        {
            throw new NotSupportedException("不支持>操作");
        }
        /// <summary>
        /// >=操作
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        protected virtual object MetaGreaterThanEqual(object v)
        {
            throw new NotSupportedException("不支持>=操作");
        }
        /// <summary>
        /// 小于操作
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        protected virtual object MetaLessThan(object v)
        {
            throw new NotSupportedException("不支持<操作");
        }
        /// <summary>
        /// 小于等于操作
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        protected virtual object MetaLessThanEqual(object v)
        {
            throw new NotSupportedException("不支持<=操作");
        }
        /// <summary>
        /// And操作
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        protected virtual object MetaAnd(object v)
        {
            throw new NotSupportedException("不支持&操作");
        }
        /// <summary>
        /// AndAlso操作
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        protected virtual object MetaAndAlso(object v)
        {
            throw new NotSupportedException("不支持&&操作");
        }
        /// <summary>
        /// Or操作
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        protected virtual object MetaOr(object v)
        {
            throw new NotSupportedException("不支持|操作");
        }
        /// <summary>
        /// OrElse操作
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        protected virtual object MetaOrElse(object v)
        {
            throw new NotSupportedException("不支持||操作");
        }
        /// <summary>
        /// IsFalse操作(一元运算符)
        /// </summary>
        /// <returns></returns>
        protected virtual bool MetaIsFalse()
        {
            return true;
        }
        /// <summary>
        /// IsTrue操作(一元运算符)
        /// </summary>
        /// <returns></returns>
        protected virtual bool MetaIsTrue()
        {
            return true;
        }
        #endregion

        public class MyDynamicMetaObject : DynamicMetaObject
        {
            public MyDynamicMetaObject(Expression parameter, MyDynamicMetaProvider dynamicDictionary)
                : base(parameter, BindingRestrictions.Empty, dynamicDictionary)
            {

            }
            public override DynamicMetaObject BindConvert(ConvertBinder binder)
            {
                // setup the binding restrictions.
                BindingRestrictions restrictions = BindingRestrictions.GetTypeRestriction(Expression, LimitType);
                // setup the parameters:
                Expression[] args = new Expression[1];
                // First parameter is the name of the property to Set
                args[0] = Expression.Parameter(binder.ReturnType);
                // Setup the 'this' reference
                Expression methodCall = Expression.Convert(Expression, LimitType);
                // Create a meta object to invoke Set later:
                DynamicMetaObject setDictionaryEntry = new DynamicMetaObject(methodCall, restrictions);
                // return that dynamic object
                return setDictionaryEntry;

                //return base.BindConvert(binder);
            }

            public override DynamicMetaObject BindBinaryOperation(BinaryOperationBinder binder, DynamicMetaObject arg)
            {
                // Method call in the containing class:
                string methodName = "";
                switch (binder.Operation)
                {
                    case ExpressionType.Equal:
                        methodName = "MetaEqual";
                        break;
                    case ExpressionType.NotEqual:
                        methodName = "MetaNotEqual";
                        break;
                    case ExpressionType.GreaterThan:
                        methodName = "MetaGreaterThan";
                        break;
                    case ExpressionType.GreaterThanOrEqual:
                        methodName = "MetaGreaterThanEqual";
                        break;
                    case ExpressionType.LessThan:
                        methodName = "MetaLessThan";
                        break;
                    case ExpressionType.LessThanOrEqual:
                        methodName = "MetaLessThanEqual";
                        break;
                    case ExpressionType.And:
                        methodName = "MetaAnd";
                        break;
                    case ExpressionType.AndAlso:
                        methodName = "MetaAndAlso";
                        break;
                    case ExpressionType.Or:
                        methodName = "MetaOr";
                        break;
                    case ExpressionType.OrElse:
                        methodName = "MetaOrElse";
                        break;
                    case ExpressionType.Add:
                        methodName = "MetaAdd";
                        break;
                    case ExpressionType.AddAssign:
                        methodName = "MetaAddAssign";
                        break;
                    case ExpressionType.Subtract:
                        methodName = "MetaSubstract";
                        break;
                    case ExpressionType.SubtractAssign:
                        methodName = "MetaSubstractAssign";
                        break;
                    case ExpressionType.Multiply:
                        methodName = "MetaMultiply";
                        break;
                    case ExpressionType.MultiplyAssign:
                        methodName = "MetaMultiplyAssign";
                        break;
                    case ExpressionType.Divide:
                        methodName = "MetaDivide";
                        break;
                    case ExpressionType.DivideAssign:
                        methodName = "MetaDivideAssign";
                        break;
                    default:
                        return base.BindBinaryOperation(binder, arg);
                }
                // One parameter
                Expression[] parameters = new Expression[] {Expression.Constant(methodName), Expression.Convert(arg.Expression,typeof(object)) };
                DynamicMetaObject entry = new DynamicMetaObject(
                                    Expression.Call(
                                                Expression.Convert(Expression, LimitType),
                                                typeof(MyDynamicMetaProvider).GetMethod("MetaBinaryOperation", BindingFlags.NonPublic
                                                | BindingFlags.Public
                                                | BindingFlags.Instance),
                                                parameters),
                                    BindingRestrictions.GetTypeRestriction(Expression, LimitType));
                
                return entry;
            }
            public override DynamicMetaObject BindCreateInstance(CreateInstanceBinder binder, DynamicMetaObject[] args)
            {
                return base.BindCreateInstance(binder, args);
            }
            public override DynamicMetaObject BindInvoke(InvokeBinder binder, DynamicMetaObject[] args)
            {
                return base.BindInvoke(binder, args);
            }
            public override DynamicMetaObject BindUnaryOperation(UnaryOperationBinder binder)
            {
                // Method call in the containing class:
                string methodName = "";
                switch (binder.Operation)
                {
                    case ExpressionType.IsFalse:
                        methodName = "MetaIsFalse";
                        break;
                    case ExpressionType.IsTrue:
                        methodName = "MetaIsTrue";
                        break;
                    default:
                        return base.BindUnaryOperation(binder);
                }
                // One parameter
                Expression[] parameters = new Expression[] {};
                DynamicMetaObject entry = new DynamicMetaObject(
                                    Expression.Call(
                                                Expression.Convert(Expression, LimitType),
                                                typeof(MyDynamicMetaProvider).GetMethod(methodName, BindingFlags.NonPublic
                                                | BindingFlags.Public
                                                | BindingFlags.Instance),
                                                null),
                                    BindingRestrictions.GetTypeRestriction(Expression, LimitType));
                return entry;
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
                var indexlist = new List<Expression>();
                foreach (var item in indexes)
                {
                    indexlist.Add(item.Expression);
                }
                args[0] = Expression.NewArrayInit(typeof(object), indexlist);
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
                var indexlist = new List<Expression>();
                foreach (var item in indexes)
                {
                    indexlist.Add(item.Expression);
                }
                Expression[] parameters = new Expression[] { Expression.NewArrayInit(typeof(object),indexlist) };
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
                string methodName = args.Length>0? "InvokeDelegate": "InvokeDelegateNoParam";//"InvokeDelegate" + (args.Length > 8 ? 8 : args.Length);
                // One parameter
                var a = from la in args select la.Value;
                
                List<Expression> lp = new List<System.Linq.Expressions.Expression>();
                
                lp.Add(Expression.Constant(binder.Name));
                if (args.Length > 0)
                {
                    var lp2 = new List<System.Linq.Expressions.Expression>();
                    var index = 0;
                    foreach (var item in args)
                    {
                        if (index >= 8)
                        {
                            break;
                        }
                        //.net中的struct为valuetype，与object是无法直接等效的，需要将对应的struct进行convert才行，否则会报ArgumentException
                        //lp.Add(item.Expression);
                        lp2.Add(Expression.Convert(item.Expression, typeof(object)));
                        index++;
                    }
                    var are = Expression.NewArrayInit(typeof(object), lp2);
                    lp.Add(are);
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
