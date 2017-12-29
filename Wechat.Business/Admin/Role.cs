using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Module.Extend.WebGo.Datas;
using EFFC.Frame.Net.Unit.DB.Datas;
using System;
using System.Collections.Generic;
using System.Text;
using Wechat.Unit;

namespace Wechat.Business.Admin
{
    public class Role:MyBaseLogic
    {
        object load(LogicData arg)
        {
            SetContentType(EFFC.Frame.Net.Base.Constants.GoResponseDataType.RazorView);
            Razor.SetViewPath("~/Views/Admin/role.cshtml");
            Razor.SetViewData("isDebug", (bool)Configs["DebugMode"]);
            return null;
        }

        object list(LogicData arg)
        {
            var rtn = FrameDLRObject.CreateInstance(@"{
issuccess:true,
msg:''
}");
            var id = ComFunc.nvl(arg["filter"]);
            var up = DB.NewDBUnitParameter();
            up.SetValue("id", id);
            FrameDLRObject express = FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'QueryByPage',
$table : 'RoleInfo'
}", id);
            if (id != "")
            {
                express.SetValue("$where", FrameDLRObject.CreateInstanceFromat(@"{
        RoleName:{
                $like:{0}
        }
}", id));
            }

            UnitDataCollection re = DB.Excute(up, express);
            var list = re.QueryData<FrameDLRObject>();
            rtn.data = list;
            rtn.current_page = re.CurrentPage;
            rtn.count_per_page = re.Count_Of_OnePage;
            rtn.to_page = re.CurrentPage;
            rtn.total_page = re.TotalPage;
            rtn.total_row = re.TotalRow;

            return rtn;
        }

        object add(LogicData arg)
        {
            var rtn = FrameDLRObject.CreateInstance(@"{
issuccess:true,
msg:'操作成功'
}");
            var uid = Guid.NewGuid().ToString();
            var rolename = ComFunc.nvl(arg["rolename"]);
            var isactive = ComFunc.nvl(arg["isactive"]);
            var remark = ComFunc.nvl(arg["remark"]);
            var up = DB.NewDBUnitParameter();


            BeginTrans();
            FrameDLRObject express = FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Query',
$table : 'RoleInfo',
$where:{
    RoleName:{0}
}
}", rolename);
            var re = DB.Excute(up, express);
            if (re.QueryTable.RowLength > 0)
            {
                rtn.issuccess = false;
                rtn.msg = "角色已经存在";
                return rtn;
            }

            express = FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Insert',
$table : 'RoleInfo',
RoleUID:{0},
RoleName:{1},
Remark:{2},
CreateTime:{3},
IsActive:{4}
}", uid, rolename, remark, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), isactive == "true" ? 1 : 0);

            DB.Excute(up, express);

            CommitTrans();

            return rtn;
        }

        object update(LogicData arg)
        {
            var rtn = FrameDLRObject.CreateInstance(@"{
issuccess:true,
msg:'操作成功'
}");
            var uid = ComFunc.nvl(arg["roleuid"]);
            var rolename = ComFunc.nvl(arg["rolename"]);
            var isactive = ComFunc.nvl(arg["isactive"]);
            var remark = ComFunc.nvl(arg["remark"]);

            if (uid == "")
            {
                rtn.issuccess = false;
                rtn.msg = "缺少角色参数";
                return rtn;
            }
            if (rolename == "")
            {
                rtn.issuccess = false;
                rtn.msg = "角色名称不能为空";
                return rtn;
            }

            BeginTrans();
            var up = DB.NewDBUnitParameter();
            FrameDLRObject express = FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Query',
$table : 'RoleInfo',
$where:{
    RoleUID:{0}
}
}", uid);
            var re = DB.Excute(up, express);
            if (re.QueryTable.RowLength <= 0)
            {
                rtn.issuccess = false;
                rtn.msg = "角色资料不存在";
                return rtn;
            }
            express = FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Query',
$table : 'RoleInfo',
$where:{
    RoleUID:{
        $neq:{0}
    },
    RoleName:{1}
}
}", uid, rolename);
            re = DB.Excute(up, express);
            if (re.QueryTable.RowLength > 0)
            {
                rtn.issuccess = false;
                rtn.msg = "角色名称已经存在";
                return rtn;
            }

            express = FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Update',
$table : 'RoleInfo',
RoleName:{1},
Remark:{2},
IsActive:{3},
$where:{
    RoleUID:{0}
}
}", uid, rolename, remark, isactive == "true" ? 1 : 0);

            DB.Excute(up, express);

            CommitTrans();

            return rtn;
        }

        object delete(LogicData arg)
        {
            var rtn = FrameDLRObject.CreateInstance(@"{
issuccess:true,
msg:'操作成功'
}");
            var uid = ComFunc.nvl(arg["roleuid"]);
            var up = DB.NewDBUnitParameter();
            up.SetValue("uid", uid);
            DB.NonQuery<RoleUnit>(up, "deleteinfo");
            return rtn;
        }

        object map(LogicData arg)
        {
            var rtn = FrameDLRObject.CreateInstance(@"{
issuccess:true,
msg:'操作成功'
}");
            var uid = ComFunc.nvl(arg["roleuid"]);
            var functions = ComFunc.nvl(arg["functions"]);
            if (uid == "")
            {
                rtn.issuccess = false;
                rtn.msg = "缺少角色参数";
                return rtn;
            }
            BeginTrans();
            var up = DB.NewDBUnitParameter();
            FrameDLRObject express = FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Query',
$table : 'RoleInfo',
$where:{
    RoleUID:{0}
}
}", uid);
            var re = DB.Excute(up, express);
            if (re.QueryTable.RowLength <= 0)
            {
                rtn.issuccess = false;
                rtn.msg = "角色资料不存在";
                return rtn;
            }

            express = FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Delete',
$table : 'Map_RoleFunction',
$where:{
    RoleUID:{0}
}
}", uid);
            DB.Excute(up, express);

            express = FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Insert',
$table : 'Map_RoleFunction',
RoleUID:{0}
}", uid);
            foreach(var s in functions.Split(','))
            {
                express.SetValue("FunctionNo", s);
                express.SetValue("$acttype", "Insert");
                DB.Excute(up, express);
            }
            CommitTrans();
            return rtn;
        }

        object tree(LogicData arg)
        {
            var rtn = FrameDLRObject.CreateInstance(@"{
issuccess:true,
msg:'操作成功'
}");
            var uid = ComFunc.nvl(arg["roleuid"]);
            var up = DB.NewDBUnitParameter();
            up.SetValue("roleuid", uid);
            var udc = DB.Query<RoleUnit>(up, "functiontree");
            var functioninfo = udc.QueryData<FrameDLRObject>();
            var tree = new List<object>();
            for (int i=0;i<functioninfo.Count;i++)
            {
                dynamic item = functioninfo[i];
                if (item.functionlevel == 0)
                {
                    var dobj = FrameDLRObject.CreateInstance();
                    dobj.text = item.functionname;
                    dobj.url = item.url;
                    dobj.no = item.functionno;
                    dobj.level = item.functionlevel;
                    dobj.ischecked = item.ischecked == 1 ? true : false;
                    dobj.parentno = "";
                    dobj.parentindex = 0;
                    dobj.index = i + 1;

                    tree.Add(dobj);

                    LoadChildNodes(functioninfo, item.functionno, dobj.index, 1, ref tree);
                }
            }
            rtn.tree = tree;
            return rtn;
        }

        void LoadChildNodes(List<FrameDLRObject> data, string parentno,int parentIndex, int level,ref List<object> tree)
        {
            for (int i = 0; i < data.Count; i++)
            {
                dynamic item = data[i];
                if (item.functionlevel == level && item.parentno == parentno)
                {
                    var dobj = FrameDLRObject.CreateInstance();
                    dobj.text = item.functionname;
                    dobj.url = item.url;
                    dobj.no = item.functionno;
                    dobj.level = item.functionlevel;
                    dobj.ischecked = item.ischecked == 1 ? true : false;
                    dobj.parentno = parentno;
                    dobj.parentindex = parentIndex;
                    dobj.index = i + 1;
                    tree.Add(dobj);

                    LoadChildNodes(data, item.functionno, dobj.index, level + 1, ref tree);
                }
            }
        }


    }
}
