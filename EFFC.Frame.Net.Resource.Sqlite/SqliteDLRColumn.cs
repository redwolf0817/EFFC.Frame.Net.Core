using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Resource.Sqlite
{
    /// <summary>
    /// Sqlite专用的DLRColumn
    /// </summary>
    public class SqliteDLRColumn:LinqDLRColumn
    {
       

        public override LinqDLR2SqlWhereOperator WhereIn(object[] args)
        {
            if (args == null || args.Length <= 0) return null;

            var splitchar = ",";
            if (args.Length > 1) splitchar = ComFunc.nvl(args[1]);
            var pname = "" + Convert.ToInt64(ComFunc.RandomCode(4));
            ConditionValues.Add(pname, splitchar + args[0] + splitchar);
            var re = "";
            re = $"charindex('{splitchar}'{SqlFlags.LinkFlag}{ColumnExpress}{SqlFlags.LinkFlag}'{splitchar}',{SqlFlags.ParamFlag}{pname})>0";


            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }
        public override LinqDLR2SqlWhereOperator WhereNotIn(object[] args)
        {
            if (args == null || args.Length <= 0) return null;

            var splitchar = ",";
            if (args.Length > 1) splitchar = ComFunc.nvl(args[1]);
            var pname = "" + Convert.ToInt64(ComFunc.RandomCode(4));
            ConditionValues.Add(pname, splitchar + args[0] + splitchar);
            var re = "";
            re = $"charindex('{splitchar}'{SqlFlags.LinkFlag}{ColumnExpress}{SqlFlags.LinkFlag}'{splitchar}',{SqlFlags.ParamFlag}{pname})<=0";


            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }
        public override LinqDLRColumn IsNullInSql(object[] args)
        {
            if (args == null || args.Length <= 0) return null;
            var rtn = New(this.GetType(), "", this.BelongToObject,SqlFlags);
            var re = "";
            re = $"IFNULL({ColumnExpress},{Convert2Express(args[0])})";
            rtn.ColumnExpress = re;
            foreach (var item in ConditionValues)
            {
                if (!rtn.ConditionValues.ContainsKey(item.Key))
                {
                    rtn.ConditionValues.Add(item.Key, item.Value);
                }
            }
            return rtn;
        }

        protected override object SubString(object[] args)
        {
            if (args == null || args.Length != 2) return this;


            var rtn = New(this.GetType(), "", this.BelongToObject, SqlFlags);

            var express = $"substr({this.ColumnExpress},#replace#)";
            foreach (var item in args)
            {
                express = express.Replace("#replace#", $"{rtn.Convert2Express(item)},#replace#");
            }
            express = express.Replace(",#replace#", "");
            rtn.ColumnExpress = express;
            foreach (var item in ConditionValues)
            {
                if (!rtn.ConditionValues.ContainsKey(item.Key))
                {
                    rtn.ConditionValues.Add(item.Key, item.Value);
                }
            }
            return rtn;
        }
        protected override object Concat(object[] args)
        {
            if (args == null || args.Length <= 0) return this;

            var rtn = (SqliteDLRColumn)New(this.GetType(), "", this.BelongToObject, SqlFlags);
            var express = $"{this.ColumnExpress}";
            foreach (var item in args)
            {
                express += $" || {rtn.Convert2Express(item)}";
            }
            rtn.ColumnExpress = express;
            foreach (var item in ConditionValues)
            {
                if (!rtn.ConditionValues.ContainsKey(item.Key))
                {
                    rtn.ConditionValues.Add(item.Key, item.Value);
                }
            }
            return rtn;
        }
        protected override object ConvertToString(object[] args)
        {
            if (args == null || args.Length <= 0) return this;

            var rtn = (SqliteDLRColumn)New(this.GetType(), "", this.BelongToObject, SqlFlags);
            var express = $"cast({this.ColumnExpress} as text(#length#))";
            express = express.Replace("#length#", $"{IntStd.IsNotIntThen(args[0], 50)}");
            rtn.ColumnExpress = express;
            foreach (var item in ConditionValues)
            {
                if (!rtn.ConditionValues.ContainsKey(item.Key))
                {
                    rtn.ConditionValues.Add(item.Key, item.Value);
                }
            }
            return rtn;
        }
        protected override object ConvertToDateTime(object[] args)
        {
            var rtn = (SqliteDLRColumn)New(this.GetType(), "", this.BelongToObject, SqlFlags);
            var express = $"datetime({this.ColumnExpress})";
            rtn.ColumnExpress = express;
            foreach (var item in ConditionValues)
            {
                if (!rtn.ConditionValues.ContainsKey(item.Key))
                {
                    rtn.ConditionValues.Add(item.Key, item.Value);
                }
            }
            return rtn;
        }
    }
    /// <summary>
    /// Sqlite数据相关的操作符
    /// </summary>
    public class SqliteOperatorFlags:SqlOperatorFlags
    {
        public override string EqualFlag => "=";
        public override string GreaterEqualFlag => ">=";
        public override string GreaterFlag => ">";
        public override string LessEqualFlag => "<=";
        public override string LessFlag => "<";
        public override string LikeMatchFlag => "%";
        public override string LinkFlag => "||";
        public override string NotEqualFlag => "<>";
        public override string ParamFlag => "$";
        public override string Column_Quatation => "`{0}`";
    }
}
