using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Interfaces.Unit;
using EFFC.Frame.Net.Data.Parameters;
using System;

namespace EFFC.Frame.Net.Business.Unit.HostUnit
{
    public class HostNonQueryUnit : IDBUnit<UnitParameter>
    {


        public Func<UnitParameter, dynamic> GetSqlFunc(string flag)
        {
            return GetSql;
        }

        private dynamic GetSql(UnitParameter arg)
        {
            var rtn = FrameDLRObject.CreateInstance();
            rtn.sql = ComFunc.nvl(arg["___host_sql___"]);
            return rtn;
        }
    }
}
