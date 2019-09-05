using EFFC.Frame.Net.Base.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Extends.LinqDLR2SQL.DLRColumns
{
    public class PostgreSqlDLRColumn : LinqDLRColumn
    {
        public override string EqualFlag => "=";
        public override string GreaterEqualFlag => ">=";
        public override string GreaterFlag => ">";
        public override string LessEqualFlag => "<=";
        public override string LessFlag => "<";
        public override string LikeMatchFlag => "%";
        public override string LinkFlag => "+";
        public override string NotEqualFlag => "<>";
        public override string ParamFlag => "@";
        public override string Column_Quatation => "\"{0}\"";

        public override LinqDLR2SqlWhereOperator WhereIn(object[] args)
        {
            if (args == null || args.Length <= 0) return null;

            var splitchar = ",";
            if (args.Length > 1) splitchar = ComFunc.nvl(args[1]);
            var pname = "" + Convert.ToInt64(ComFunc.RandomCode(4));
           
            var re = "";
            ConditionValues.Add(pname, splitchar + args[0] + splitchar);
            re = $"POSITION('{splitchar}'{LinkFlag}{ColumnExpress}{LinkFlag}'{splitchar}' in {ParamFlag}{pname})>0";


            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }
        public override LinqDLR2SqlWhereOperator WhereNotIn(object[] args)
        {
            if (args == null || args.Length <= 0) return null;

            var splitchar = ",";
            if (args.Length > 1) splitchar = ComFunc.nvl(args[1]);
            var pname = "" + Convert.ToInt64(ComFunc.RandomCode(4));

            var re = "";
            ConditionValues.Add(pname, splitchar + args[0] + splitchar);
            re = $"POSITION('{splitchar}'{LinkFlag}{ColumnExpress}{LinkFlag}'{splitchar}' in {ParamFlag}{pname})<=0";


            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }
        public override LinqDLRColumn IsNullInSql(object[] args)
        {
            if (args == null || args.Length <= 0) return null;
            var rtn = New(this.GetType(), "", this.BelongToObject);
            var re = "";
            re = $"COALESCE({ColumnExpress},{Convert2Express(args[0])})";
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
            if (args == null || args.Length <= 0) return this;


            var rtn = New(this.GetType(), "", this.BelongToObject);

            var express = $"substring({this.ColumnExpress} #from# #for#)";

            if (args[0] is string)
            {
                express = express.Replace("#from#", $"from '{rtn.Convert2Express(args[0])}'");
            }
            else
            {
                express = express.Replace("#from#", $"from {rtn.Convert2Express(args[0])}");
            }
            if(args.Length > 1)
            {
                if (args[0] is string)
                {
                    express = express.Replace("#for#", $"for '{rtn.Convert2Express(args[1])}'");
                }
                else
                {
                    express = express.Replace("#for#", $"for {rtn.Convert2Express(args[1])}");
                }
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
    }
}
