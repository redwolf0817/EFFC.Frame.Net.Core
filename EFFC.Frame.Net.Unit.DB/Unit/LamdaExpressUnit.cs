using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Unit.DB.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Unit.DB.Unit
{
    public class LamdaExpressUnit : IDBUnit<UnitParameter>
    {
        public Func<UnitParameter, dynamic> GetSqlFunc(string flag)
        {
            switch (flag.ToLower())
            {
                case "nonquery":
                    return NonQuery;
                default:
                    return Load;
            }
        }

        private dynamic NonQuery(UnitParameter arg)
        {
            var rtn = FrameDLRObject.CreateInstance();

            var sql = ComFunc.nvl(arg.GetValue("sql"));
            rtn.sql = sql;

            return rtn;
        }

        private dynamic Load(UnitParameter arg)
        {
            var rtn = FrameDLRObject.CreateInstance();

            var sql = ComFunc.nvl(arg.GetValue("sql"));
            var orderexpress = ComFunc.nvl(arg.GetValue("orderby"));
            rtn.sql = sql;
            rtn.orderby = orderexpress;

            return rtn;
        }
    }
}
