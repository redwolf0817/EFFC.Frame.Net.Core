using EFFC.Frame.Net.Unit.DB.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wechat.Unit
{
    public class RoleUnit:BaseDBUnit
    {
        void deleteinfo(UnitParameter up, dynamic sql)
        {
            sql.sql = @"delete from Map_LoginRole where RoleUID=$uid;
delete from Map_RoleFunction where RoleUID=$uid;
delete from RoleInfo where RoleUID=$uid;";
        }

        void functiontree(UnitParameter up,dynamic sql)
        {
            sql.sql = @"SELECT 
case when ifnull(c.RoleUID,'')<> '' then 1 else 0 end as ischecked, 
d.* from FunctionInfo d
left join Map_RoleFunction c on d.FunctionNo=c.FunctionNo and c.RoleUID=$roleuid
order by FunctionNo;
";
        }
    }
}
