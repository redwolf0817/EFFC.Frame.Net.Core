using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Text;

namespace EFFC.Frame.Net.Base.Data.Base
{
    public abstract class MyDynamicObject:DynamicObject
    {
        #region Get，Set
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
        #endregion

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

        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            return base.TryInvoke(binder, args, out result);
        }
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = InvokeMe(binder.Name, args);
            return true;
        }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            this.SetMetaValue(binder.Name, value);
            return true;
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetMetaValue(binder.Name);
            return true;
        }
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            SetMetaIndexValue(indexes, value);
            return true;
        }
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            result = GetMetaIndexValue(indexes);
            return true;
        }
        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
        {
            switch (binder.Operation)
            {
                case ExpressionType.Equal:
                    result = MetaEqual(arg);
                    return true;
                case ExpressionType.NotEqual:
                    result = MetaNotEqual(arg);
                    return true;
                case ExpressionType.GreaterThan:
                    result = MetaGreaterThan(arg);
                    return true;
                case ExpressionType.GreaterThanOrEqual:
                    result = MetaGreaterThanEqual(arg);
                    return true;
                case ExpressionType.LessThan:
                    result = MetaLessThan(arg);
                    return true;
                case ExpressionType.LessThanOrEqual:
                    result = MetaLessThanEqual(arg);
                    return true;
                case ExpressionType.And:
                    result = MetaAnd(arg);
                    return true;
                case ExpressionType.AndAlso:
                    result = MetaAndAlso(arg);
                    return true;
                case ExpressionType.Or:
                    result = MetaOr(arg);
                    return true;
                case ExpressionType.OrElse:
                    result = MetaOrElse(arg);
                    return true;
                default:
                    return base.TryBinaryOperation(binder, arg,out result);
            }
        }
        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
        {
            switch (binder.Operation)
            {
                case ExpressionType.IsFalse:
                    result = MetaIsFalse();
                    return true;
                case ExpressionType.IsTrue:
                    result = MetaIsTrue();
                    return true;
                default:
                    return base.TryUnaryOperation(binder,out result);
            }
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            return base.TryConvert(binder, out result);
        }
        public override bool TryCreateInstance(CreateInstanceBinder binder, object[] args, out object result)
        {
            return base.TryCreateInstance(binder, args, out result);
        }
    }
}
