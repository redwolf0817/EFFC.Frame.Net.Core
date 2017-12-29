using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Module.Extend.EWRA.Attributes;
using EFFC.Frame.Net.Module.Extend.EWRA.Attributes.Validation;
using EFFC.Frame.Net.Module.Extend.EWRA.Logic;
using EFFC.Frame.Net.Resource.Sqlite;
using EFFC.Frame.Net.Unit.DB.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RestAPI.Business.v1._0
{
    class User:MyRestLogic
    {
        [EWRARouteDesc("获取所有用户资料")]
        public new List<FrameDLRObject> get()
        {
            var up = DB.NewDefaultDBUnitParameter<SqliteAccess>();
            UnitDataCollection re = DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Query',
$table : 'LoginInfo',
}"));

            if (re.QueryTable.RowLength <= 0)
            {
                return null;
            }
            else
            {
                var l = from t in re.QueryData<FrameDLRObject>()
                        select new
                        {
                            uid = t.UID,
                            id = t.LoginID,
                            name = t.LoginName,
                            remark = t.remark
                        };
                return l.ToList();
            }
        }
        [EWRARouteDesc("根据id获取用户资料")]
        public override object get(string id)
        {
            var up = DB.NewDefaultDBUnitParameter<SqliteAccess>();
            UnitDataCollection re = DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Query',
$table : 'LoginInfo',
$where : {
		LoginID:{0}
}
}", id));

            if (re.QueryTable.RowLength <= 0)
            {
                return null;
            }
            else
            {
                dynamic obj = re.QueryData<FrameDLRObject>()[0];
                return new
                {
                    uid=obj.UID,
                    id = obj.LoginID,
                    name = obj.LoginName,
                    remark = obj.remark
                };
            }
        }
        [EWRAEmptyValid("id,pw,name")]
        [EWRARouteDesc("登录用户资料新增")]
        public override object post()
        {
            object rtn = null;

            var id = ComFunc.nvl(PostDataD.id);
            var pass = ComFunc.nvl(PostDataD.pw);
            var name = ComFunc.nvl(PostDataD.name);
            var remark = ComFunc.nvl(PostDataD.remark);
            BeginTrans();
            var up = DB.NewDefaultDBUnitParameter<SqliteAccess>();
            UnitDataCollection re = DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Query',
$table : 'LoginInfo',
$where : {
		LoginID:{0}
}
}", id));
            if(IsValidBy("账号已经存在", ()=> !(re.QueryTable.RowLength > 0)))
            {
                var uid = Guid.NewGuid().ToString();
                DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Insert',
$table : 'LoginInfo',
UID:{0},
LoginID:{1},
LoginPass:{2},
LoginName:{3},
Remark:{4},
IsActive:'true'
}", uid, id, pass, name, remark));

                rtn = new
                {
                    uid = uid,
                    id = id
                };

            }
            CommitTrans();
            return rtn;
        }
        [EWRARouteDesc("登录用户资料更新")]
        public object patch(string id)
        {
            object rtn = null;

            id = ComFunc.nvl(id);
            var name = ComFunc.nvl(PostDataD.name);
            var remark = ComFunc.nvl(PostDataD.remark);

            BeginTrans();
            var up = DB.NewDefaultDBUnitParameter<SqliteAccess>();
            UnitDataCollection re = DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Query',
$table : 'LoginInfo',
$where : {
		LoginID:{0}
}
}", id));
            if (IsValidBy("账号不存在", () => re.QueryTable.RowLength > 0))
            {
                DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Update',
$table : 'LoginInfo',
LoginName:{1},
Remark:{2},
$where:{
    LoginID:{0}
}
}", id, name, remark));

                rtn = new
                {
                    id = id
                };

            }
            CommitTrans();
            SetRefreshCacheRoute($"/user/{id}");
            return rtn;
        }
        [EWRARoute("patch","/user/{id}/{isactive}")]
        [EWRARouteDesc("登录用户资料激活或失效")]
        public object Active(string id,string isactive)
        {
            object rtn = null;

            id = ComFunc.nvl(id);
            isactive = ComFunc.nvl(isactive);
            BeginTrans();
            var up = DB.NewDefaultDBUnitParameter<SqliteAccess>();
            UnitDataCollection re = DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Query',
$table : 'LoginInfo',
$where : {
		LoginID:{0}
}
}", id));
            if (IsValidBy("账号不存在", () => re.QueryTable.RowLength > 0))
            {
                DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Update',
$table : 'LoginInfo',
IsActive:{1},
$where:{
    LoginID:{0}
}
}", id, isactive.ToLower()=="true"?"true":"false"));

                rtn = new
                {
                    id = id
                };

            }
            CommitTrans();
            SetRefreshCacheRoute($"/user/{id}");
            return rtn;
        }
        [EWRARouteDesc("删除指定用户资料")]
        public override bool delete(string id)
        {
            var up = DB.NewDefaultDBUnitParameter<SqliteAccess>();

            BeginTrans();
            UnitDataCollection re = DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Query',
$table : 'LoginInfo',
$where : {
		LoginID:{0}
}
}", id));
            var uid = re.QueryTable.RowLength > 0 ? ComFunc.nvl(re.QueryTable[0, "UID"]) : "";

            DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Delete',
$table : 'Map_LoginRole',
$where:{
        LoginUID:{0}
    }
}", uid));

            DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Delete',
$table : 'LoginInfo',
$where:{
        LoginID:{0}
    }
}", id));
            CommitTrans();
            SetRefreshCacheRoute($"/user/{id}");
            return true;
        }
    }
}
