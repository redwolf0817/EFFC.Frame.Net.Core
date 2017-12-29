using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Interfaces.Unit;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Base.ResouceManage.DB;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.UnitData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Business.Unit.HostUnit
{
    public class HostQueryUnit : IDBUnit<UnitParameter>
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
