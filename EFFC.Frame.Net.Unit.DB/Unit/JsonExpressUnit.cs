using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Unit.DB.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Unit.DB.Unit
{
    public class JsonExpressUnit : IDBUnit<UnitParameter>
    {

        public Func<UnitParameter, dynamic> GetSqlFunc(string flag)
        {
            return Load;
        }

        private dynamic Load(UnitParameter arg)
        {
            var rtn = FrameDLRObject.CreateInstance();
            var json = arg.GetValue("__json__");
            if (json != null && json is DBExpress)
            {
                var express = (DBExpress)json;
                var re = express.ToExpress();
                var sql = ComFunc.nvl(re.GetValue("sql"));
                FrameDLRObject data = re.GetValue("data") != null ? (FrameDLRObject)re.GetValue("data") : FrameDLRObject.CreateInstance();
                var orderby = ComFunc.nvl(re.GetValue("orderby"));
                foreach (var k in data.Keys)
                {
                    arg.SetValue(k, data.GetValue(k));
                }
                if (express.CurrentAct == DBExpress.ActType.QueryByPage)
                {
                    rtn.sql = sql;
                    rtn.orderby = orderby;
                }
                else
                {
                    rtn.sql = sql;
                }

            }
            return rtn;
        }
    }
}
