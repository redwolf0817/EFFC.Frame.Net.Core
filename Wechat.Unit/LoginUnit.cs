using EFFC.Frame.Net.Unit.DB.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wechat.Unit
{
    public class LoginUnit:BaseDBUnit
    {
        void login(UnitParameter up,dynamic sql)
        {
            sql.sql = @"select * from LoginInfo where LoginID=$LoginID";
        }

        void updateLoginTime(UnitParameter up, dynamic sql)
        {
            sql.sql = @"update LoginInfo set LastLoginTime=$LastLoginTime where LoginID=$LoginID";
        }

        void loginInfo(UnitParameter up,dynamic sql)
        {
            sql.sql = @"SELECT c.* from LoginInfo a
join Map_LoginRole b on a.UID=b.LoginUID
join RoleInfo c on b.RoleUID=c.RoleUID
where a.LoginID=$LoginID;

SELECT distinct d.* from LoginInfo a
join Map_LoginRole b on a.UID=b.LoginUID
join Map_RoleFunction c on b.RoleUID=c.RoleUID
join FunctionInfo d on c.FunctionNo=d.FunctionNo
where a.LoginID=$LoginID and d.IsMenu=1;";
        }

        void deleteinfo(UnitParameter up,dynamic sql)
        {
            sql.sql = @"delete from Map_LoginRole where LoginUID=$uid;
delete from LoginInfo where uid=$uid;";
        }

        void roleMapByLoginUID(UnitParameter up, dynamic sql)
        {
            sql.sql = @"select case when ifnull(b.LoginUID,'')<>'' then 1 else 0 end as isselected, a.* from RoleInfo a
left join Map_LoginRole b on a.RoleUID=b.RoleUID and b.LoginUID=$uid
where a.IsActive=1;";
        }
    }
}
