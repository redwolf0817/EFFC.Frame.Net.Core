using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Module.Extend.WebGo.Datas;
using EFFC.Frame.Net.Unit.DB.Datas;
using Wechat.Unit;
using System;

namespace Wechat.Business.Admin
{
    public class Account : MyBaseLogic
    {
        object load(LogicData arg)
        {
            SetContentType(EFFC.Frame.Net.Base.Constants.GoResponseDataType.RazorView);
            Razor.SetViewPath("~/Views/Admin/account.cshtml");
            return null;
        }

        object list(LogicData arg)
        {
            var id = ComFunc.nvl(arg["filter"]);
            var up = DB.NewDBUnitParameter();
            up.SetValue("id", id);
            FrameDLRObject express = FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'QueryByPage',
$table : 'LoginInfo'
}");
            if (id != "")
            {
                express.SetValue("$where", FrameDLRObject.CreateInstanceFromat(@"{
	$or:[
        {
            LoginID:{
                    $like:{0}
                }
        },
        {
            LoginName:{
                    $like:{0}
                }
        }
    ]
}", id));
            }
            
            UnitDataCollection re = DB.Excute(up, express);
            var list = re.QueryData<FrameDLRObject>();
            var rtn = FrameDLRObject.CreateInstance(true, "");
            rtn.data = list;
            rtn.current_page = re.CurrentPage;
            rtn.count_per_page = re.Count_Of_OnePage;
            rtn.to_page = re.CurrentPage;
            rtn.total_page = re.TotalPage;
            rtn.total_row = re.TotalRow;

            return rtn;
        }

        object delete(LogicData arg)
        {
            var uid = ComFunc.nvl(arg["uid"]);
            var up = DB.NewDBUnitParameter();
            up.SetValue("uid", uid);
            DB.NonQuery<LoginUnit>(up, "deleteinfo");
            return FrameDLRObject.CreateInstance(true, "操作成功");
        }

        object add(LogicData arg)
        {
            var uid = Guid.NewGuid().ToString();
            var loginid = ComFunc.nvl(arg["loginid"]);
            var loginname = ComFunc.nvl(arg["loginname"]);
            var pass = ComFunc.nvl(arg["pass"]);
            var isactive = ComFunc.nvl(arg["isactive"]);
            var remark = ComFunc.nvl(arg["remark"]);
            var up = DB.NewDBUnitParameter();


            BeginTrans();
            FrameDLRObject express = FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Query',
$table : 'LoginInfo',
$where:{
    LoginID:{0}
}
}", loginid);
            var re = DB.Excute(up, express);
            if (re.QueryTable.RowLength > 0)
                return FrameDLRObject.CreateInstance(false, "登录账号已经存在");

            express = FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Insert',
$table : 'LoginInfo',
UID:{0},
LoginID:{1},
LoginPass:{2},
LoginName:{3},
Remark:{4},
IsActive:{5}
}", uid, loginid, pass, loginname, remark, isactive);

            DB.Excute(up, express);

            CommitTrans();

            return FrameDLRObject.CreateInstance(true, "操作成功");
        }

        object update(LogicData arg)
        {
            var uid = ComFunc.nvl(arg["uid"]);
            var loginid = ComFunc.nvl(arg["loginid"]);
            var loginname = ComFunc.nvl(arg["loginname"]);
            var pass = ComFunc.nvl(arg["pass"]);
            var isactive = ComFunc.nvl(arg["isactive"]);
            var remark = ComFunc.nvl(arg["remark"]);

            if(uid == "")
                return FrameDLRObject.CreateInstance(false, "缺少用户参数");
            if(loginid == "")
                return FrameDLRObject.CreateInstance(false, "缺少用户登录账号");
            if(pass == "")
                return FrameDLRObject.CreateInstance(false, "密码不能为空");

            var up = DB.NewDBUnitParameter();
            FrameDLRObject express = FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Query',
$table : 'LoginInfo',
$where:{
    UID:{0}
}
}", uid);
            var re = DB.Excute(up, express);
            if(re.QueryTable.RowLength <= 0)
                return FrameDLRObject.CreateInstance(false, "用户资料不存在");

            express = FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Query',
$table : 'LoginInfo',
$where:{
    UID:{
        $neq:{0}
    },
    LoginName:{1}
}
}", uid,loginname);
            re = DB.Excute(up, express);
            if (re.QueryTable.RowLength > 0)
                return FrameDLRObject.CreateInstance(false, "用户姓名已经存在");

            express = FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Query',
$table : 'LoginInfo',
$where:{
    UID:{
        $neq:{0}
    },
    LoginID:{1}
}
}", uid, loginid);
            re = DB.Excute(up, express);
            if (re.QueryTable.RowLength > 0)
                return FrameDLRObject.CreateInstance(false, "登录账号已经存在");


            BeginTrans();
            express = FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Update',
$table : 'LoginInfo',
LoginID:{1},
LoginPass:{2},
LoginName:{3},
Remark:{4},
IsActive:{5},
$where:{
    UID:{0}
}
}", uid, loginid, pass, loginname, remark, isactive);
            DB.Excute(up, express);

            CommitTrans();

            return FrameDLRObject.CreateInstance(true, "操作成功"); 
        }

        object rolemap(LogicData arg)
        {
            var uid = ComFunc.nvl(arg["uid"]);
            var up = DB.NewDBUnitParameter();
            up.SetValue("uid", uid);
            var re = DB.Query<LoginUnit>(up, "roleMapByLoginUID");
            var rtn = FrameDLRObject.CreateInstance(true, "操作成功");
            rtn.data = re.QueryData<FrameDLRObject>();
            return rtn;
        }

        object saveRoleMap(LogicData arg)
        {
            var rtn = FrameDLRObject.CreateInstance(@"{
issuccess:true,
msg:'操作成功'
}");
            var uid = ComFunc.nvl(arg["uid"]);
            var roles = ComFunc.nvl(arg["roles"]);
            if (uid == "")
                return FrameDLRObject.CreateInstance(false, "缺少角色参数");

            BeginTrans();
            var up = DB.NewDBUnitParameter();
            FrameDLRObject express = FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Query',
$table : 'LoginInfo',
$where:{
    UID:{0}
}
}", uid);
            var re = DB.Excute(up, express);
            if (re.QueryTable.RowLength <= 0)
                return FrameDLRObject.CreateInstance(false, "用户资料不存在");

            express = FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Delete',
$table : 'Map_LoginRole',
$where:{
    LoginUID:{0}
}
}", uid);
            DB.Excute(up, express);

            express = FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Insert',
$table : 'Map_LoginRole',
LoginUID:{0}
}", uid);
            foreach (var s in roles.Split(','))
            {
                express.SetValue("RoleUID", s);
                express.SetValue("$acttype", "Insert");
                DB.Excute(up, express);
            }
            CommitTrans();
            return rtn;
        }
    }
}

