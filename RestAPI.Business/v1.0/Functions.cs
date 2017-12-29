using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
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
    class Functions:User
    {
        [EWRARouteDesc("获取指定用户的功能菜单")]
        public object get(dynamic parent_info)
        {
            object rtn = null;
            //该功能不需要缓存
            SetCacheEnable(false);
            if (IsValidBy("缺少参数", () => parent_info != null)
                && IsValidBy("缺少参数", () => !(parent_info is FrameDLRObject)))
            {
                var loginuid = parent_info.uid;
                var up = DB.NewDefaultDBUnitParameter<SqliteAccess>();
                UnitDataCollection re = DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
	$acttype: 'Query',
	'a.FunctionNo':true,
	'a.FunctionName':true,
	'a.FunctionLevel':true,
	'a.IsMenu':true,
	'a.ParentNo':true,
	'a.Url':true,
	$table: {
		FunctionInfo: 'a',
		Map_RoleFunction: {
			$as: 'b',
			$join: {
				$by: 'a',
				$on: {
					'b.FunctionNo': '#sql:a.FunctionNo'
				}
			}
		},
		Map_LoginRole:{
			$as: 'c',
			$join: {
				$by: 'b',
				$on: {
					'c.RoleUID': '#sql:b.RoleUID'
				}
			}
		}
	},
	$where: {
		'c.LoginUID': {0},
        'a.IsMenu':{1}
	}
}", loginuid,"1"));

                var list = re.QueryData<FrameDLRObject>();
                /*var level0 = from t in list
                             where t.FunctionLevel == 0
                             select new { No = t.FunctionNo, Name = t.FunctionName, Level = t.FunctionLevel, Url = t.Url, IsMenu = t.IsMenu, Sub = BuildFunctionTree(t, list) };
*/

                rtn = list.ToList();
            }

            return rtn;
        }

        private List<FrameDLRObject> BuildFunctionTree(dynamic root,List<FrameDLRObject> list)
        {
            var tmp = from t in list
                      where t.ParentNo == root.FunctionNo
                      select new { No = t.FunctionNo, Name = t.FunctionName, Level = t.FunctionLevel, Url = t.Url, IsMenu = t.IsMenu ,Sub=BuildFunctionTree(t,list)};
            return tmp.ToList();
        }
        [EWRARouteDesc("获取功能菜单列表")]
        public new List<FrameDLRObject> get()
        {
            SetCacheEnable(false);
            var up = DB.NewDefaultDBUnitParameter<SqliteAccess>();
            UnitDataCollection re = DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
	$acttype: 'Query',
	$table: 'FunctionInfo',
}"));

            var list = re.QueryData<FrameDLRObject>();
            var level0 = from t in list
                         where t.FunctionLevel == 0
                         select new { No = t.FunctionNo, Name = t.FunctionName, Level = t.FunctionLevel, Url = t.Url, IsMenu = t.IsMenu,Remark=t.Remark, Sub = BuildFunctionTree4GetList(t, list) };


            return level0.ToList();
        }
        private List<FrameDLRObject> BuildFunctionTree4GetList(dynamic root, List<FrameDLRObject> list)
        {
            var tmp = from t in list
                      where t.ParentNo == root.FunctionNo
                      select new { No = t.FunctionNo, Name = t.FunctionName, Level = t.FunctionLevel, Url = t.Url, IsMenu = t.IsMenu, Remark = t.Remark, Sub = BuildFunctionTree4GetList(t, list) };
            return tmp.ToList();
        }
        [EWRARouteDesc("获取指定功能信息")]
        public override object get(string no)
        {
            var up = DB.NewDefaultDBUnitParameter<SqliteAccess>();
            UnitDataCollection re = DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
	$acttype: 'Query',
	$table: 'FunctionInfo',
    $where:{
        FunctionNo:{0}
    }
}",no));

            var list = re.QueryData<FrameDLRObject>();
            if (list.Count() > 0)
            {
                dynamic t = list[0];
                return new
                {
                    No = t.FunctionNo,
                    Name = t.FunctionName,
                    Level = t.FunctionLevel,
                    Url = t.Url,
                    IsMenu = t.IsMenu,
                    Remark = t.Remark
                };
            }
            else
            {
                return null;
            }
        }
        [EWRARouteDesc("新增功能")]
        [EWRAEmptyValid("no,name,level,url,ismenu,parentno")]
        [EWRAIntValid("level")]
        public override object post()
        {
            object rtn = null;
            var up = DB.NewDefaultDBUnitParameter<SqliteAccess>();
            string no = ComFunc.nvl(PostDataD.no);
            string name = ComFunc.nvl(PostDataD.name);
            int level = PostDataD.level;
            string url = ComFunc.nvl(PostDataD.url);
            string parentno = ComFunc.nvl(PostDataD.parentno);
            string ismenu = ComFunc.nvl(PostDataD.ismenu);
            string remark = ComFunc.nvl(PostDataD.remark);

            BeginTrans();
            UnitDataCollection re = DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
	$acttype: 'Query',
	$table: 'FunctionInfo',
	$where: {
        $or:[
            FunctionNo: {0}，
            FunctionName:{1}
        ]
		
	}
}", no,name));
            if (IsValidBy("功能已存在", () => re.QueryTable.RowLength <= 0))
            {
                DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Insert',
$table : 'FunctionInfo',
FunctionNo:{0},
FunctionName:{1},
ParentNo:{2},
Url:{3},
IsMenu:{4},
Remark:{5},
CreateTime:{6},
FunctionLevel:{7}
}", no, name, parentno, url, (ismenu == "true" || ismenu == "1") ? 1 : 0, remark, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),level));

                rtn = new
                {
                    no = no
                };
            }

            return rtn;
        }
        [EWRARouteDesc("修改指定的功能资料")]
        [EWRAEmptyValid("name,url,ismenu")]
        public new object patch(string no)
        {
            object rtn = null;
            var up = DB.NewDefaultDBUnitParameter<SqliteAccess>();
            var name = ComFunc.nvl(PostDataD.name);
            var url = ComFunc.nvl(PostDataD.url);
            var ismenu = ComFunc.nvl(PostDataD.ismenu);
            var remark = ComFunc.nvl(PostDataD.remark);

            BeginTrans();
            UnitDataCollection re = DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
	$acttype: 'Query',
	$table: 'FunctionInfo',
	$where: {
        $or:[
            FunctionNo: {0}，
            FunctionName:{1}
        ]
		
	}
}", no, name));
            if (IsValidBy("功能已存在", () => re.QueryTable.RowLength <= 0))
            {
                DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Update',
$table : 'FunctionInfo',
FunctionName:{1},
Url:{2},
IsMenu:{3},
Remark:{4},
$where:{
        FunctionNo:{0}
    }
}", no, name, url, (ismenu == "true" || ismenu == "1") ? 1 : 0, remark));

                rtn = new
                {
                    no = no
                };
            }
            CommitTrans();
            SetRefreshCacheRoute($"/functions/{no}");
            return rtn;
        }

        [EWRARouteDesc("删除指定的功能资料")]
        public override bool delete(string no)
        {
            var up = DB.NewDefaultDBUnitParameter<SqliteAccess>();

            BeginTrans();

            DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Delete',
$table : 'Map_RoleFunction',
$where:{
        FunctionNo:{0}
    }
}", no));

            DB.Excute(up, FrameDLRObject.CreateInstanceFromat(@"{
$acttype : 'Delete',
$table : 'FunctionInfo',
$where:{
        FunctionNo:{0}
    }
}", no));

            CommitTrans();
            SetRefreshCacheRoute($"/functions/{no}");
            return true;
        }
    }
}
