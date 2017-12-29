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
    class Function:MyBaseLogic
    {
        object load(LogicData arg)
        {
            SetContentType(EFFC.Frame.Net.Base.Constants.GoResponseDataType.RazorView);
            Razor.SetViewPath("~/Views/Admin/function.cshtml");
            return null;
        }
        object add(LogicData arg)
        {
            var rtn = FrameDLRObject.CreateInstance(@"{
issuccess:true,
msg:''
}");
            BeginTrans();
            var up = DB.NewDBUnitParameter();
            var parentno = ComFunc.nvl(arg["parentno"]);
            var no = ComFunc.nvl(arg["no"]);
            var name = ComFunc.nvl(arg["name"]);
            var url = ComFunc.nvl(arg["url"]);
            var remark = ComFunc.nvl(arg["remark"]);
            var level = ComFunc.nvl(arg["level"]);
            var ilevel = level == "" ? 0 : int.Parse(level);

            if(no == "")
            {
                rtn.issuccess = false;
                rtn.msg = "请输入功能编号";
                return rtn;
            }
            if (name == "")
            {
                rtn.issuccess = false;
                rtn.msg = "请输入功能名称";
                return rtn;
            }

            UnitDataCollection re = DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Query',
$table : 'FunctionInfo',
$where : {
		FunctionNo:{0}
}
}", no));

            if(re.QueryTable.RowLength > 0)
            {
                rtn.issuccess = false;
                rtn.msg = "功能编号已经存在";
                return rtn;
            }
            re = DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Query',
$table : 'FunctionInfo',
$where : {
		FunctionName:{0}
}
}", name));
            if (re.QueryTable.RowLength > 0)
            {
                rtn.issuccess = false;
                rtn.msg = "功能名称已经存在";
                return rtn;
            }

            DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Insert',
$table : 'FunctionInfo',
FunctionNo:{0},
FunctionName:{1},
ParentNo:{2},
Url:{3},
Remark:{4},
FunctionLevel:{5},
IsMenu:1
}", no,name,parentno==""?null:parentno,url,remark, ilevel));

            CommitTrans();
            return rtn;
        }
        object update(LogicData arg)
        {
            var rtn = FrameDLRObject.CreateInstance(@"{
issuccess:true,
msg:'操作成功'
}");
            BeginTrans();
            var up = DB.NewDBUnitParameter();
            var parentno = ComFunc.nvl(arg["parentno"]);
            var no = ComFunc.nvl(arg["no"]);
            var name = ComFunc.nvl(arg["name"]);
            var url = ComFunc.nvl(arg["url"]);
            var remark = ComFunc.nvl(arg["remark"]);
            var level = ComFunc.nvl(arg["level"]);
            var ilevel = level == "" ? 0 : int.Parse(level);

            if (no == "")
            {
                rtn.issuccess = false;
                rtn.msg = "请输入功能编号";
                return rtn;
            }
            if (name == "")
            {
                rtn.issuccess = false;
                rtn.msg = "请输入功能名称";
                return rtn;
            }

            UnitDataCollection re = DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Query',
$table : 'FunctionInfo',
$where : {
		FunctionName:{0},
        FunctionNo:{
            $neq:{1}
        }
}
}", name, no));
            if (re.QueryTable.RowLength > 0)
            {
                rtn.issuccess = false;
                rtn.msg = "功能名称已经存在";
                return rtn;
            }

            DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Update',
$table : 'FunctionInfo',
$where : {
		FunctionNo:{0}
},
FunctionName:{1},
Url:{2},
Remark:{3},
FunctionLevel:{4}
}", no, name, url, remark, ilevel));

            CommitTrans();

            return rtn;
        }
        object delete(LogicData arg)
        {
            var rtn = FrameDLRObject.CreateInstance(@"{
issuccess:true,
msg:'操作成功'
}");
            var no = ComFunc.nvl(arg["no"]);

            BeginTrans();
            var up = DB.NewDBUnitParameter();
            UnitDataCollection re = DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Query',
$table : 'FunctionInfo',
$where : {
		FunctionNo:{0}
}
}", no));
            if(re.QueryTable.RowLength<= 0)
            {
                rtn.issuccess = false;
                rtn.msg = "待删除的功能不存在";
                return rtn;
            }
            up.SetValue("no", no);
            DB.NonQuery<FunctionUnit>(up, "delete");

            CommitTrans();
            return rtn;
        }

        object tree(LogicData ld)
        {
            var rtn = FrameDLRObject.CreateInstance(@"{
issuccess:true,
msg:''
}");
            var up = DB.NewDBUnitParameter();
            var re = DB.Query<FunctionUnit>(up, "all");
            var maxlevel = int.Parse(ComFunc.nvl( re.QueryTable[0, "maxlevel"]));
            var tree = new List<object>();
            var items = re.QueryData<FrameDLRObject>(1);
            foreach(dynamic item in items)
            {
                if (item.functionlevel == 0)
                {
                    var dobj = FrameDLRObject.CreateInstance();
                    dobj.text = item.functionname;
                    dobj.url = item.url;
                    dobj.no = item.functionno;
                    dobj.nodes = LoadChildNodes(items, item.functionno, 1);
                    dobj.level = item.functionlevel;
                    dobj.remark = item.remark;

                    tree.Add(dobj);
                }
            }
            rtn.data = tree;

            return rtn;
        }

        List<object> LoadChildNodes(List<FrameDLRObject> data,string parentno, int level)
        {
            var rtn = new List<object>();
            foreach(dynamic item in data)
            {
                if(item.functionlevel == level && item.parentno == parentno)
                {
                    var dobj = FrameDLRObject.CreateInstance();
                    dobj.text = item.functionname;
                    dobj.url = item.url;
                    dobj.no = item.functionno;
                    dobj.nodes = LoadChildNodes(data, item.functionno, level + 1);
                    dobj.level = item.functionlevel;
                    dobj.remark = item.remark;

                    rtn.Add(dobj);
                }
            }
            return rtn;
        }
    }
}
