using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Module.Extend.EWRA.Logic;
using EFFC.Frame.Net.Resource.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestAPI.Business
{
    class MyAuth: AuthorizationLogic
    {
        protected override bool DoLogin(string id)
        {
            var rtn = true;
            var pass = ComFunc.nvl(PostDataD.pw);
            BeginTrans();
            //登录账号判断
            var up = DB.NewDefaultDBUnitParameter<SqliteAccess>();
            var re = DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Query',
$table : 'LoginInfo',
$where : {
		LoginID:{0},
        IsActive:'true'
}
}", id));

            if (re.QueryTable.RowLength <= 0)
            {
                return false;
            }
            if (ComFunc.nvl(re.QueryTable[0, "LoginPass"]) != pass)
            {
                return false;
            }

            up.SetValue("LastLoginTime", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Update',
$table : 'LoginInfo',
$where : {
		LoginID:{0}
},
LastLoginTime:{1}
}", id, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));


            CommitTrans();
            return rtn;
        }

        protected override bool IsValid(string token, ref string msg)
        {
            var result = base.IsValid(token, ref msg);
            if (result)
            {
                //此处可以做功能权限验证
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string PublicKeySavePath => "~/token/public.json";
        public override string PrivateKeySavePath => "~/token/private.json";
    }
}
