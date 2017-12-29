using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Interfaces.Unit;
using EFFC.Frame.Net.Data.Parameters;
using System;

namespace EFFC.Frame.Net.Business.Unit.HostUnit
{
    public class HostQueryByPageUnit : IDBUnit<UnitParameter>
    {


        public Func<UnitParameter, dynamic> GetSqlFunc(string flag)
        {
            return GetSql;
        }

        private dynamic GetSql(UnitParameter arg)
        {
            var rtn = FrameDLRObject.CreateInstance();
            rtn.sql = ComFunc.nvl(arg["___host_sql___"]);
            rtn.orderby = ComFunc.nvl(arg["___host_orderby___"]);
            rtn.presql = ComFunc.nvl(arg["___host_pre___"]);
            rtn.aftersql = ComFunc.nvl(arg["___host_after___"]);
            return rtn;
        }
    }
}
