using EFFC.Frame.Net.Module.Extend.EWRA.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFFC.Extends.LinqDLR2SQL;
using System.IO;
using EFFC.Frame.Net.Unit.DB;

namespace RestAPISample.Business.v1._0
{
    public class System:MyRestLogic
    {
        const string Create_Function_File = "SystemSetup/Create_Extend_Function.json";
        const string Create_Role_File = "SystemSetup/Create_Extend_Role.json";
        const string Create_Login_File = "SystemSetup/Create_Extend_Login.json";
        const string Create_RoleLogin_File = "SystemSetup/Create_Extend_RoleLogin.json";
        const string Create_RoleFunction_File = "SystemSetup/Create_Extend_RoleFunction.json";
        const string Create_MetaData_File = "SystemSetup/Create_Extend_MetaData.json";
        const string Create_MetaData_Column_File = "SystemSetup/Create_Extend_MetaData_Columns.json";

        [EWRAVisible(false)]
        [EWRAAuth(false)]
        [EWRARoute("post","/system/setup")]
        [EWRARouteDesc("系统初始化安装")]
        [EWRAOutputDesc("返回结果", @"{
code:""success-成功，failed-失败"",
msg:""提示信息""
}")]
        public object Setup()
        {
            BeginTrans();
            var up = DB.NewDBUnitParameter();
            if (!DB.IsTableExists(up, "EXTEND_FUNCTION"))
            {
                if (File.Exists(Create_Function_File))
                {
                    DB.Excute(up, File.ReadAllText(Create_Function_File).Trim());
                }
            }
            if (!DB.IsTableExists(up, "EXTEND_ROLE"))
            {
                if (File.Exists(Create_Role_File))
                {
                    DB.Excute(up, File.ReadAllText(Create_Role_File).Trim());
                }
            }
            if (!DB.IsTableExists(up, "EXTEND_LOGIN"))
            {
                if (File.Exists(Create_Login_File))
                {
                    DB.Excute(up, File.ReadAllText(Create_Login_File).Trim());
                }
            }
            if (!DB.IsTableExists(up, "EXTEND_ROLE_FUNCTION"))
            {
                if (File.Exists(Create_RoleFunction_File))
                {
                    DB.Excute(up, File.ReadAllText(Create_RoleFunction_File).Trim());
                }
            }
            if (!DB.IsTableExists(up, "EXTEND_ROLE_LOGIN"))
            {
                if (File.Exists(Create_RoleLogin_File))
                {
                    DB.Excute(up, File.ReadAllText(Create_RoleLogin_File).Trim());
                }
            }
            if (!DB.IsTableExists(up, "EXTEND_METADATA"))
            {
                if (File.Exists(Create_MetaData_File))
                {
                    DB.Excute(up, File.ReadAllText(Create_MetaData_File).Trim());
                }
            }
            if (!DB.IsTableExists(up, "EXTEND_METADATA_COLUMNS"))
            {
                if (File.Exists(Create_MetaData_Column_File))
                {
                    DB.Excute(up, File.ReadAllText(Create_MetaData_Column_File).Trim());
                }
            }
            //新增admin账号
            var admin_uid = Guid.NewGuid().ToString();
            DB.QuickDelete(up, "EXTEND_LOGIN", new
            {
                LoginID = "admin"
            });
            DB.QuickInsert(up, "EXTEND_LOGIN", new
            {
                UID = admin_uid,
                LoginID = "admin",
                UserNo = "",
                UserType = "System",
                LoginPass = "123456",
                add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                last_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
            //新增角色账号
            DB.QuickDelete(up, "EXTEND_ROLE", new
            {
                RoleNo = "R999"
            });
            DB.QuickInsert(up, "EXTEND_ROLE", new
            {
                RoleNo = "R999",
                RoleName = "超级管理员",
                add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                last_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
            #region 新增功能资料
            (from t in DB.LamdaTable(up, "EXTEND_FUNCTION", "a")
             where t.FunctionNo == "F001" || t.P_FunctionNo == "F001"
             select t).Delete(up);
            DB.QuickInsert(up, "EXTEND_FUNCTION", new
            {
                FunctionNo = "F001",
                FunctionName = "系统管理",
                Is_Menu = 1,
                FunctionLevel=0,
                add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                last_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
            DB.QuickInsert(up, "EXTEND_FUNCTION", new
            {
                FunctionNo = "F001001",
                FunctionName = "功能管理",
                Is_Menu = 1,
                FunctionLevel = 1,
                P_FunctionNo = "F001",
                add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                last_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
            DB.QuickInsert(up, "EXTEND_FUNCTION", new
            {
                FunctionNo = "F001002",
                FunctionName = "角色管理",
                Is_Menu = 1,
                FunctionLevel = 1,
                P_FunctionNo = "F001",
                add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                last_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
            DB.QuickInsert(up, "EXTEND_FUNCTION", new
            {
                FunctionNo = "F001003",
                FunctionName = "登录账号管理",
                Is_Menu = 1,
                FunctionLevel = 1,
                P_FunctionNo = "F001",
                add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                last_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
            DB.QuickInsert(up, "EXTEND_FUNCTION", new
            {
                FunctionNo = "F001004",
                FunctionName = "角色权限管理",
                Is_Menu = 1,
                FunctionLevel = 1,
                P_FunctionNo = "F001",
                add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                last_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
            #endregion
            #region 生成角色权限
            DB.QuickDelete(up, "EXTEND_ROLE_FUNCTION", new
            {
                RoleNo = "R999"
            });
            DB.QuickInsert(up, "EXTEND_ROLE_FUNCTION", new
            {
                RoleNo = "R999",
                FunctionNo = "F001",
                add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                last_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
            DB.QuickInsert(up, "EXTEND_ROLE_FUNCTION", new
            {
                RoleNo = "R999",
                FunctionNo = "F001001",
                add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                last_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
            DB.QuickInsert(up, "EXTEND_ROLE_FUNCTION", new
            {
                RoleNo = "R999",
                FunctionNo = "F001002",
                add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                last_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
            DB.QuickInsert(up, "EXTEND_ROLE_FUNCTION", new
            {
                RoleNo = "R999",
                FunctionNo = "F001003",
                add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                last_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
            DB.QuickInsert(up, "EXTEND_ROLE_FUNCTION", new
            {
                RoleNo = "R999",
                FunctionNo = "F001004",
                add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                last_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
            #endregion
            //生成登录账号授权
            DB.QuickDelete(up, "EXTEND_ROLE_LOGIN", new
            {
                RoleNo = "R999"
            });
            DB.QuickInsert(up, "EXTEND_ROLE_LOGIN", new
            {
                RoleNo = "R999",
                LoginUID = admin_uid,
                add_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                last_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
            CommitTrans();
            return new
            {
                code = "success",
                msg = "创建成功"
            };
        }

        [EWRARoute("patch", "/apimanage/{url}/on")]
        [EWRARouteDesc("开启API服务")]
        [EWRAAddInput("id", "string", @"API编号", "无默认值", EFFC.Frame.Net.Base.Constants.RestInputPosition.Path, false)]
        [EWRAOutputDesc("返回结果", @"{
code:'success-成功，failed-失败',
msg:'提示信息'
}")]
        object APIOn(string id)
        {
            var up = DB.NewDBUnitParameter();
            var s = from t in DB.LamdaTable(up, "EXTEND_API_INFO", "a")
                    where t.apino == id
                    select t;
            if (!s.IsExists(up))
            {
                return new
                {
                    code = "failed",
                    msg = "资料不存在"
                };
            }
            dynamic info = s.GetQueryList(up).First();
            return null;
        }
    }
}
