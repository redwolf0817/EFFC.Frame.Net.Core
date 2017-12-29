using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Unit.DB.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wechat.Unit.Busi
{
    public class UserUnit:BaseDBUnit
    {
        void recommend(UnitParameter up, dynamic sql)
        {
            sql.sql = @"delete from Map_Recommend where UserUID=@UserUID and RecommededUID=@RecommededUID
insert into Map_Recommend(UserUID,RecommededUID)values(@UserUID,@RecommededUID)
";
        }

        void userinfo(UnitParameter up, dynamic sql)
        {
            sql.sql = @"select a.*,c.CodeText as StoreName,
d.UserUID as RecommendUID,e.[UserAccountNo] as RecommendAccountNo,e.[UserMobile] as recommendMobile, e.[UserName] as RecommendName
from [UserInfo] a
left join CodeDictionary c on c.CodeValue=a.BelongToStore
left join [Map_Recommend] d on a.UserUID=d.[RecommededUID]
left join [UserInfo] e on e.UserUID=d.UserUID
where (a.UserUID=@id or a.UserMobile=@id or a.UserAccountNo=@id)";
        }

        void addUserInfo(UnitParameter up, dynamic sql)
        {
            sql.sql = @"INSERT INTO [UserInfo]
           ([UserUID]
           ,[UserAccountNo]
           ,[UserMobile]
           ,[UserName]
           ,[UserSex]
           ,[UserRealName]
           ,[UserRealID]
           ,[UserType]
           ,[BelongToStore])
     VALUES
           (@UserUID
           ,@UserAccountNo
           ,@UserMobile
           ,@UserName
           ,@UserSex
           ,@UserRealName
           ,@UserRealID
           ,@UserType
           ,@BelongToStore)";

            if (ComFunc.nvl(up.GetValue("RecommededUID")) != "")
            {
                sql.sql += @"delete from Map_Recommend where UserUID=@UserUID and RecommededUID=@RecommededUID
insert into Map_Recommend(UserUID,RecommededUID)values(@UserUID,@RecommededUID)";
            }
        }

        void QueryUserList(UnitParameter up, dynamic sql)
        {
            sql.sql = @"select a.*,c.CodeText as StoreName,
d.UserUID as RecommendUID,e.[UserAccountNo] as RecommendAccountNo,
e.[UserMobile] as recommendMobile, e.[UserName] as RecommendName,f.CodeText as RecommededStoreName
from [UserInfo] a
left join CodeDictionary c on c.CodeValue=a.BelongToStore
join [Map_Recommend] d on a.UserUID=d.UserUID
join [UserInfo] e on e.UserUID=d.[RecommededUID] 
left join CodeDictionary f on f.CodeValue=e.BelongToStore";

            var where = "";
            if (ComFunc.nvl(up.GetValue("UserID")) != "")
            {
                where += "(a.UserMobile like '%'+@UserID+'%' or a.UserAccountNo like '%'+@UserID+'%')";
            }
            if (ComFunc.nvl(up.GetValue("RecommendedID")) != "")
            {
                where += (where==""?"":" and ")+ "(e.UserMobile like '%'+@RecommendedID+'%' or e.UserAccountNo like '%'+@RecommendedID+'%')";
            }

            where = where == "" ? "" : " where " + where;

            sql.sql += where;
            

        }
    }
}
