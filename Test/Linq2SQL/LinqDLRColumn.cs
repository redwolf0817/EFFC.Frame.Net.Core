using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Linq2SQL
{
    public class LinqDLRColumn : MyDynamicMetaProvider
    {
        public string ParamFlag
        {
            get
            {
                return "@";
            }
        }
        public string LinkFlag
        {
            get { return "&"; }
        }
        public string NotEqualFlag
        {
            get { return "<>"; }
        }
        public string EqualFlag
        {
            get { return "="; }
        }
        public LinqDLRColumn(string columnexpress, LamdaSQLObject belongto)
        {
            ColumnExpress = columnexpress;
            BelongToObject = belongto;
            ConditionValues = new Dictionary<string, object>();
        }
        public LamdaSQLObject BelongToObject
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
        protected object Contains(object[] args)
        {
            if (args == null || args.Length <= 0) return null;

            var pname = "" + Convert.ToInt64(ComFunc.ToTimeStampTS(DateTime.Now).TotalMilliseconds);
            ConditionValues.Add(pname, args[0]);
            var re = "";
            re = $"{ColumnExpress} LIKE '%'{LinkFlag}{ParamFlag}{pname}{LinkFlag}'%'";


            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }
        protected object StartWith(object[] args)
        {
            if (args == null || args.Length <= 0) return null;

            var pname = "" + Convert.ToInt64(ComFunc.ToTimeStampTS(DateTime.Now).TotalMilliseconds);
            ConditionValues.Add(pname, args[0]);
            var re = "";

            re = $"{ColumnExpress} LIKE '%'{LinkFlag}{ParamFlag}{pname}{LinkFlag}";

            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }
        protected object EndWith(object[] args)
        {
            if (args == null || args.Length <= 0) return null;

            var pname = "" + Convert.ToInt64(ComFunc.ToTimeStampTS(DateTime.Now).TotalMilliseconds);
            ConditionValues.Add(pname, args[0]);
            var re = "";
            re = $"{ColumnExpress} LIKE {LinkFlag}{ParamFlag}{pname}{LinkFlag}'%'";

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
            var pname = "" + Convert.ToInt64(ComFunc.ToTimeStampTS(DateTime.Now).TotalMilliseconds);
            ConditionValues.Add(pname, v);
            var re = "";
            re = $"{ColumnExpress}{EqualFlag}{ParamFlag}{pname}";

            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }

        protected override object MetaNotEqual(object v)
        {
            var pname = "" + Convert.ToInt64(ComFunc.ToTimeStampTS(DateTime.Now).TotalMilliseconds);
            ConditionValues.Add(pname, v);
            var re = "";

            re = $"{ColumnExpress}{NotEqualFlag}{ParamFlag}{pname}";

            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }

        protected override object MetaGreaterThan(object v)
        {
            var pname = "" + Convert.ToInt64(ComFunc.ToTimeStampTS(DateTime.Now).TotalMilliseconds);
            ConditionValues.Add(pname, v);
            var re = "";
            re = $"{ColumnExpress}>{ParamFlag}{pname}";

            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }
        protected override object MetaGreaterThanEqual(object v)
        {
            var pname = "" + Convert.ToInt64(ComFunc.ToTimeStampTS(DateTime.Now).TotalMilliseconds);
            ConditionValues.Add(pname, v);
            var re = "";

            re = $"{ColumnExpress}>={ParamFlag}{pname}";

            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }
        protected override object MetaLessThan(object v)
        {
            var pname = "" + Convert.ToInt64(ComFunc.ToTimeStampTS(DateTime.Now).TotalMilliseconds);
            ConditionValues.Add(pname, v);
            var re = "";
            re = $"{ColumnExpress}<{ParamFlag}{pname}";

            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }
        protected override object MetaLessThanEqual(object v)
        {
            var pname = "" + Convert.ToInt64(ComFunc.ToTimeStampTS(DateTime.Now).TotalMilliseconds);
            ConditionValues.Add(pname, v);
            var re = "";
            re = $"{ColumnExpress}<={ParamFlag}{pname}";

            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }
    }
}
