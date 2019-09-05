using EFFC.Frame.Net.Base.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Extends.LinqDLR2SQL.DLRColumns
{
    /// <summary>
    /// Sqlite专用的DLRColumn
    /// </summary>
    public class OracleDLRColumn:LinqDLRColumn
    {
        public override string EqualFlag => "=";
        public override string GreaterEqualFlag => ">=";
        public override string GreaterFlag => ">";
        public override string LessEqualFlag => "<=";
        public override string LessFlag => "<";
        public override string LikeMatchFlag => "%";
        public override string LinkFlag => "||";
        public override string NotEqualFlag => "<>";
        public override string ParamFlag => ":";
        public override string Column_Quatation => "{0}";

        public override LinqDLR2SqlWhereOperator WhereIn(object[] args)
        {
            if (args == null || args.Length <= 0) return null;

            var splitchar = ",";
            if (args.Length > 1) splitchar = ComFunc.nvl(args[1]);
            var pname = "" + Convert.ToInt64(ComFunc.RandomCode(4));
            ConditionValues.Add(pname, splitchar + args[0] + splitchar);
            var re = "";
            re = $"instr({ParamFlag}{pname},'{splitchar}'{LinkFlag}{ColumnExpress}{LinkFlag}'{splitchar}')>0";


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
            re = $"instr({ParamFlag}{pname},'{splitchar}'{LinkFlag}{ColumnExpress}{LinkFlag}'{splitchar}')<=0";


            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }
        public override LinqDLRColumn IsNullInSql(object[] args)
        {
            if (args == null || args.Length <= 0) return null;
            var rtn = New(this.GetType(), "", this.BelongToObject);
            var re = "";
            re = $"NVL({ColumnExpress},{Convert2Express(args[0])})";
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


            var rtn = New(this.GetType(), "", this.BelongToObject);

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
    }
}
