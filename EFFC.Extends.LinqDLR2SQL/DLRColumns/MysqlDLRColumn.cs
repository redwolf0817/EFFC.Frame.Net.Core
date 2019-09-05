using EFFC.Frame.Net.Base.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Extends.LinqDLR2SQL.DLRColumns
{
    public class MysqlDLRColumn:LinqDLRColumn
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
        public override string Column_Quatation => "`{0}`";

        public override LinqDLR2SqlWhereOperator WhereIn(object[] args)
        {
            if (args == null || args.Length <= 0) return null;

            var splitchar = ",";
            if (args.Length > 1) splitchar = ComFunc.nvl(args[1]);
            var pname = "" + Convert.ToInt64(ComFunc.RandomCode(4));
            
            var re = "";
            ConditionValues.Add(pname, splitchar + args[0] + splitchar);
            re = $"locate(concat('{splitchar}',{ColumnExpress},'{splitchar}'),{ParamFlag}{pname})>0";



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
            re = $"locate(concat('{splitchar}',{ColumnExpress},'{splitchar}'),{ParamFlag}{pname})<=0";



            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }
        public override LinqDLRColumn IsNullInSql(object[] args)
        {
            if (args == null || args.Length <= 0) return null;
            var rtn = New(this.GetType(), "", this.BelongToObject);
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

        protected override object Contains(object[] args)
        {
            if (args == null || args.Length <= 0) return null;

            var pname = "" + Convert.ToInt64(ComFunc.RandomCode(4));
            ConditionValues.Add(pname, args[0]);
            var re = "";
            re = $"{ColumnExpress} LIKE CONCAT('{LikeMatchFlag}',{ParamFlag}{pname},'{LikeMatchFlag}')";


            return new LinqDLR2SqlWhereOperator(re, ConditionValues);
        }

        
    }
}
