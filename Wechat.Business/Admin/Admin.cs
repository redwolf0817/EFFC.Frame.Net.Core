using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Module.Extend.WebGo.Datas;
using EFFC.Frame.Net.Module.Extend.WeixinWeb.Logic;
using System;
using System.Collections.Generic;
using System.Text;
using Wechat.Unit;

namespace Wechat.Business.Admin
{
    public class Admin:MyBaseLogic
    {
        object Load(LogicData arg)
        {
            SetContentType(EFFC.Frame.Net.Base.Constants.GoResponseDataType.RazorView);
            Razor.SetViewPath("~/Views/Admin/login.cshtml");
            return null;
        }

        object Index(LogicData arg)
        {
            SetContentType(EFFC.Frame.Net.Base.Constants.GoResponseDataType.RazorView);
            Razor.SetViewPath("~/Views/Admin/main.cshtml");
            return null;
        }

        object Logout(LogicData arg)
        {
            SetContentType(EFFC.Frame.Net.Base.Constants.GoResponseDataType.RazorView);
            Razor.SetViewPath("~/Views/Admin/login.cshtml");
            UpdateLoginInfo(null);
            return null;
        }

        object Login(LogicData arg)
        {
            var rtn = FrameDLRObject.CreateInstance(@"{
issuccess:true,
msg:''
}");
            BeginTrans();
            //登录账号判断
            var up = DB.NewDBUnitParameter();
            up.SetValue("LoginID", arg["loginid"]);
            var re = DB.Query<LoginUnit>(up, "login");
            
            if(re.QueryTable.RowLength <= 0)
            {
                rtn.issuccess = false;
                rtn.msg = "用户不存在";
                return rtn;
            }
            if(ComFunc.nvl(re.QueryTable[0,"LoginPass"]) != ComFunc.nvl(arg["loginpass"]))
            {
                rtn.issuccess = false;
                rtn.msg = "密码不正确";
                return rtn;
            }

            up.SetValue("LastLoginTime", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            DB.NonQuery<LoginUnit>(up, "updatelogintime");
            //获取登录用户信息
            re = DB.Query<LoginUnit>(up, "logininfo");
            var roleinfo = re.QueryData<FrameDLRObject>();
            var functioninfo = re.QueryData<FrameDLRObject>(1);
            var tree = new List<object>();
            foreach (dynamic item in functioninfo)
            {
                if (item.functionlevel == 0)
                {
                    var dobj = FrameDLRObject.CreateInstance();
                    dobj.text = item.functionname;
                    dobj.url = item.url;
                    dobj.no = item.functionno;
                    dobj.nodes = LoadChildNodes(functioninfo, item.functionno, 1);
                    dobj.level = item.functionlevel;

                    tree.Add(dobj);
                }
            }
            rtn.info = FrameDLRObject.CreateInstance();
            rtn.info.role = roleinfo;
            rtn.info.function = tree;

            UpdateLoginInfo(rtn.info);
           

            CommitTrans();
            return rtn;
        }

        List<object> LoadChildNodes(List<FrameDLRObject> data, string parentno, int level)
        {
            var rtn = new List<object>();
            foreach (dynamic item in data)
            {
                if (item.functionlevel == level && item.parentno == parentno)
                {
                    var dobj = FrameDLRObject.CreateInstance();
                    dobj.text = item.functionname;
                    dobj.url = item.url;
                    dobj.no = item.functionno;
                    dobj.nodes = LoadChildNodes(data, item.functionno, level + 1);
                    dobj.level = item.functionlevel;

                    rtn.Add(dobj);
                }
            }
            return rtn;
        }
    }
}
