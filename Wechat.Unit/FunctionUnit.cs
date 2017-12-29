using EFFC.Frame.Net.Unit.DB.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wechat.Unit
{
    public class FunctionUnit:BaseDBUnit
    {
        void all(UnitParameter up,dynamic sql)
        {
            sql.sql = @"select ifnull(max(FunctionLevel),0) as maxlevel from FunctionInfo;
select * from FunctionInfo order by FunctionNo";
        }

        void delete(UnitParameter up, dynamic sql)
        {
            sql.sql = @"delete from FunctionInfo where FunctionNo=$no or ParentNo=$no;
delete from Map_RoleFunction where FunctionNo in (select FunctionNo from FunctionInfo where FunctionNo=$no or ParentNo=$no);";
        }
    }
}
