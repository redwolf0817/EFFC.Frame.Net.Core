using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Linq2SQL
{
    public class LinqDLRColumn : MyDynamicMetaProvider
    {
        public virtual string ParamFlag
        {
            get
            {
                return "@";
            }
        }
        /// <summary>
        /// 字符串链接符号
        /// </summary>
        public virtual string LinkFlag
        {
            get { return "&"; }
        }
        /// <summary>
        /// 不等于符号
        /// </summary>
        public virtual string NotEqualFlag
        {
            get { return "<>"; }
        }
        /// <summary>
        /// 等于符号
        /// </summary>
        public virtual string EqualFlag
        {
            get { return "="; }
        }
        /// <summary>
        /// 大于符号
        /// </summary>
        public virtual string GreaterFlag
        {
            get { return ">"; }
        }
        /// <summary>
        /// 大于等于符号
        /// </summary>
        public virtual string GreaterEqualFlag
        {
            get { return ">="; }
        }
        /// <summary>
        /// 小于符号
        /// </summary>
        public virtual string LessFlag
        {
            get { return "<"; }
        }
        /// <summary>
        /// 小于等于符号
        /// </summary>
        public virtual string LessEqualFlag
        {
            get { return "<="; }
        }
        /// <summary>
        /// 用于like的匹配符号
        /// </summary>
        public virtual string LikeMatchFlag
        {
            get { return "%"; }
        }
        /// <summary>
        /// 获取一个LinqDLRColumn的新实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnexpress"></param>
        /// <param name="belongto"></param>
        /// <returns></returns>
        public static T New<T>(string columnexpress, LamdaSQLObject<T> belongto) where T:LinqDLRColumn
        {
            T t = (T)Activator.CreateInstance(typeof(T), true);
            t.ColumnExpress = columnexpress;
            t.BelongToObject = belongto;
            t.ConditionValues = new Dictionary<string, object>();
            return t;
        }
        protected LinqDLRColumn()
        {
        }
        /// <summary>
        /// 所属的LamdaSQLObject的对象
        /// </summary>
        public Object BelongToObject
        {
            get;
            private set;
        }
        /// <summary>
        /// select出来的栏位表达式，可以为名称，别名或表达式
        /// </summary>
        public string ColumnExpress
        {
            get;
            set;
        }
        public Dictionary<string, object> ConditionValues
        {
            get;
            private set;
        }
        public object ConditionValue
        {
            get;
            private set;
        }
        public string LastOperatorString
        {
            get;
            private set;
        }
        /// <summary>
        /// 模糊匹配操作
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual object Contains(object[] args)
        {
            if (args == null || args.Length <= 0) return null;

            var pname = "" + Convert.ToInt64(ComFunc.ToTimeStampTS(DateTime.Now).TotalMilliseconds);
            ConditionValues.Add(pname, args[0]);
            var re = "";
            re = $"{ColumnExpress} LIKE '{LikeMatchFlag}'{LinkFlag}{ParamFlag}{pname}{LinkFlag}'{LikeMatchFlag}'";


            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }
        /// <summary>
        /// 左模糊匹配操作
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual object StartWith(object[] args)
        {
            if (args == null || args.Length <= 0) return null;

            var pname = "" + Convert.ToInt64(ComFunc.ToTimeStampTS(DateTime.Now).TotalMilliseconds);
            ConditionValues.Add(pname, args[0]);
            var re = "";

            re = $"{ColumnExpress} LIKE '{LikeMatchFlag}'{LinkFlag}{ParamFlag}{pname}{LinkFlag}";

            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }
        /// <summary>
        /// 右模糊匹配操作
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual object EndWith(object[] args)
        {
            if (args == null || args.Length <= 0) return null;

            var pname = "" + Convert.ToInt64(ComFunc.ToTimeStampTS(DateTime.Now).TotalMilliseconds);
            ConditionValues.Add(pname, args[0]);
            var re = "";
            re = $"{ColumnExpress} LIKE {LinkFlag}{ParamFlag}{pname}{LinkFlag}'{LikeMatchFlag}'";

            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }

        protected override object GetMetaValue(string key)
        {
            return "";
        }

        protected override object InvokeMe(string methodInfo, params object[] args)
        {
            if(methodInfo.ToLower() == "contains")
            {
                return Contains(args);
            }
            if (methodInfo.ToLower() == "startwith")
            {
                return StartWith(args);
            }
            if (methodInfo.ToLower() == "endwith")
            {
                return EndWith(args);
            }
            return null;
        }

        protected override object SetMetaValue(string key, object value)
        {
            return null;
        }

        protected override object MetaEqual(object v)
        {
            var re = "";

            re = $"{ColumnExpress}{EqualFlag}{Convert2Express(v)}";

            var rtn = new LinqDLR2SqlWhereOperator(re, ConditionValues);

            return rtn;
        }

        protected override object MetaNotEqual(object v)
        {
            var re = "";
            re = $"{ColumnExpress}{NotEqualFlag}{Convert2Express(v)}";

            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }

        protected override object MetaGreaterThan(object v)
        {
            var re = "";
            re = $"{ColumnExpress}{GreaterFlag}{Convert2Express(v)}";

            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }
        protected override object MetaGreaterThanEqual(object v)
        {
            var re = "";

            re = $"{ColumnExpress}{GreaterEqualFlag}{Convert2Express(v)}";

            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }
        protected override object MetaLessThan(object v)
        {
            var re = "";
            re = $"{ColumnExpress}{LessFlag}{Convert2Express(v)}";

            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }
        protected override object MetaLessThanEqual(object v)
        {
            var re = "";
            re = $"{ColumnExpress}{LessEqualFlag}{Convert2Express(v)}";

            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        /// <summary>
        /// 将右侧值转化成对应的表达式，并添加条件值
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private string Convert2Express(object v)
        {
            var pname = "" + Convert.ToInt64(ComFunc.ToTimeStampTS(DateTime.Now).TotalMilliseconds);
            var re = "";
            if (v is LinqDLRColumn)
            {
                re = $"{((LinqDLRColumn)v).ColumnExpress}";
            }
            else
            {
                ConditionValues.Add(pname, v);

                re = $"{ParamFlag}{pname}";
            }

            return re;
        }
    }
}
