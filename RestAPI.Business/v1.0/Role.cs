using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Module.Extend.EWRA.Attributes;
using EFFC.Frame.Net.Module.Extend.EWRA.Attributes.Validation;
using EFFC.Frame.Net.Resource.Sqlite;
using EFFC.Frame.Net.Unit.DB.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RestAPI.Business.v1._0
{
    class Role:User
    {
        [EWRARouteDesc("获取指定用户的所有角色信息")]
        public object get(dynamic parent_info)
        {
            SetCacheEnable(false);
            if (parent_info == null) return null;

            var up = DB.NewDefaultDBUnitParameter<SqliteAccess>();
            UnitDataCollection re = DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"
{
	$acttype: 'Query',
	'a.RoleUID':true,
	'a.RoleName':true,
	'a.Remark':true,
	$table: {
		RoleInfo: 'a',
		Map_LoginRole: {
			$as: 'b',
			$join: {
				$by: 'a',
				$on: {
					'b.RoleUID': '#sql:a.RoleUID'
				}
			}
		}
	},
	$where: {
		'b.LoginUID': {0},
        'a.IsActive':{1}
	}
}", parent_info.uid, "1"));

            var l = from t in re.QueryData<FrameDLRObject>()
                    select new { UID = t.RoleUID, Name = t.RoleName, Remark = t.Remark };

            return l.ToList();
        }

        [EWRARouteDesc("获取角色列表")]
        public new List<FrameDLRObject> get()
        {
            SetCacheEnable(false);
            var up = DB.NewDefaultDBUnitParameter<SqliteAccess>();
            UnitDataCollection re = DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
	$acttype: 'Query',
	$table: 'RoleInfo',
}"));

            var list = re.QueryData<FrameDLRObject>();
            var level0 = from t in list
                         select new { Id = t.RoleUID, Name = t.RoleName, Remark = t.Remark };


            return level0.ToList();
        }

        [EWRARouteDesc("获取指定角色信息")]
        public override object get(string id)
        {
            var up = DB.NewDefaultDBUnitParameter<SqliteAccess>();
            UnitDataCollection re = DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
	$acttype: 'Query',
	$table: 'RoleInfo',
    $where:{
        RoleUID:{0}
    }
}", id));

            var list = re.QueryData<FrameDLRObject>();
            if (list.Any())
            {
                dynamic t = list[0];
                return new
                {
                    Id = t.RoleUID,
                    Name = t.RoleName,
                    Remark = t.Remark
                };
            }
            else
            {
                return null;
            }
        }
        [EWRARouteDesc("新增角色")]
        [EWRAEmptyValid("name")]
        public override object post()
        {
            object rtn = null;
            var up = DB.NewDefaultDBUnitParameter<SqliteAccess>();
            string name = ComFunc.nvl(PostDataD.name);
            string remark = ComFunc.nvl(PostDataD.remark);

            BeginTrans();
            UnitDataCollection re = DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
	$acttype: 'Query',
	$table: 'RoleInfo',
	$where: {
        RoleName:{0}
	}
}", name));
            if (IsValidBy("角色已存在", () => re.QueryTable.RowLength <= 0))
            {
                var uid = Guid.NewGuid().ToString();
                DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Insert',
$table : 'RoleInfo',
RoleUID:{0},
RoleName:{1},
Remark:{2},
CreateTime:{3},
IsActive:{4}
}", uid, name, remark,DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),"1"));

                rtn = new
                {
                    id = uid
                };
            }
            CommitTrans();
            
            return rtn;
        }
        [EWRARouteDesc("修改指定的功能资料")]
        [EWRAEmptyValid("name")]
        public new object patch(string id)
        {
            object rtn = null;
            var up = DB.NewDefaultDBUnitParameter<SqliteAccess>();
            var name = ComFunc.nvl(PostDataD.name);
            var remark = ComFunc.nvl(PostDataD.remark);

            BeginTrans();
            UnitDataCollection re = DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
	$acttype: 'Query',
	$table: 'RoleInfo',
	$where: {
        RoleUID:{0}
	}
}", id));
            if (IsValidBy("角色不存在", () => re.QueryTable.RowLength > 0))
            {
                DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Update',
$table : 'RoleInfo',
RoleName:{1},
Remark:{2},
$where:{
        RoleUID:{0}
    }
}", id, name, remark));

                rtn = new
                {
                    Id = id
                };
            }
            CommitTrans();
            SetRefreshCacheRoute($"/role/{id}", "/role");
            return rtn;
        }

        [EWRARouteDesc("删除指定的功能资料")]
        public new bool delete(string id)
        {
            var up = DB.NewDefaultDBUnitParameter<SqliteAccess>();

            BeginTrans();

            DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Delete',
$table : 'Map_RoleFunction',
$where:{
        RoleUID:{0}
    }
}", id));
            DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Delete',
$table : 'Map_LoginRole',
$where:{
        RoleUID:{0}
    }
}", id));

            DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Delete',
$table : 'RoleInfo',
$where:{
        RoleUID:{0}
    }
}", id));
            CommitTrans();
            SetRefreshCacheRoute($"/role/{id}", "/role");
            return true;
        }
    }
}
