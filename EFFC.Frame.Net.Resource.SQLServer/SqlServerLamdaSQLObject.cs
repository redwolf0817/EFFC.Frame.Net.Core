using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Resource.SQLServer
{
    /// <summary>
    /// 用于LinqDLR2Sql构建动态栏位使用
    /// </summary>
    public class SqlServerLamdaSQLObject : LamdaSQLObject<SqlServerDLRColumn>
    {
        public SqlServerLamdaSQLObject(string table, SqlOperatorFlags sqlflags) : base(table, sqlflags)
        {
        }
        protected override object Convert2DateTime(object[] args)
        {
            if (args == null || args.Length <= 0) return this;

            var rtn = LinqDLRColumn.New<SqlServerDLRColumn>("", this, SqlFlags);
            object value = null;
            if(args.Length == 1)
            {
                if(args[0] is LinqDLRColumn)
                {
                    value = args[0];
                }
                else
                {
                    value = DateTimeStd.IsDateTimeThen(args[0], "yyyy-MM-dd HH:mm:ss");
                }
            }
            else if(args.Length == 3)
            {
                var year = ComFunc.nvl(args[0]);
                var month = ComFunc.nvl(args[1]);
                var day = ComFunc.nvl(args[2]);

                value = $"{year}-{month.PadLeft(2, '0')}-{day.PadLeft(2, '0')}";
            }
            else if (args.Length > 3)
            {
                var year = ComFunc.nvl(args[0]);
                var month = ComFunc.nvl(args[1]);
                var day = ComFunc.nvl(args[2]);
                var hour = args.Length > 3 ? ComFunc.nvl(args[3]) : "00";
                var minute = args.Length > 4 ? ComFunc.nvl(args[4]) : "00";
                var second = args.Length > 5 ? ComFunc.nvl(args[5]) : "00";

                value = $"{year}-{month.PadLeft(2, '0')}-{day.PadLeft(2, '0')} {hour.PadLeft(2,'0')}:{minute.PadLeft(2,'0')}:{second.PadLeft(2,'0')}";
            }

            var express = $"convert(datetime,{rtn.Convert2Express(value)})";
            rtn.ColumnExpress = express;
            return rtn;
        }
    }
}
