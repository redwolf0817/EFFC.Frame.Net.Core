using EFFC.Frame.Net.Module.Extend.EWRA.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Unit.DB;

namespace RestAPISample.Business.v1._0
{
    public class LoginUser:MyRestLogic
    {
        [EWRARouteDesc("获取所有登录者的列表")]
        [EWRAOutputDesc("返回结果", @"{
result:[数据集]
}")]
        public override List<object> get()
        {
            var up = DB.NewDBUnitParameter();
            var s = from t in DB.LamdaTable(up, "EXTEND_LOGIN", "a")
                    join t2 in DB.LamdaTable(up, "EXTEND_ROLE_LOGIN", "b").LeftJoin() on t.uid equals t2.loginuid
                    select new
                    {
                        t.uid,
                        t.loginid,
                        t.userno,
                        t.usertype,
                        t.loginpass,
                        t2.roleno
                    };
            var list = s.GetQueryList(up);
            var newlist = from t in list
                          group t by t.GetValue("uid") into g
                          select new
                          {
                              uid = g.First().GetValue("uid"),
                              loginid = g.First().GetValue("loginid"),
                              userno = g.First().GetValue("userno"),
                              usertype = g.First().GetValue("usertype"),
                              loginpass = g.First().GetValue("loginpass"),
                              roles = (from tt in g
                                       select ComFunc.nvl(tt.GetValue("roleno"))).ToList()
                          };
            return newlist.ToList<object>();
        }
    }
}
