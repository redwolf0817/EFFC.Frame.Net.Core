using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Module.Extend.WebGo.Datas;
using System;
using System.Collections.Generic;
using System.Text;
using Wechat.Unit.Busi;

namespace Wechat.Business.Busi
{
    public class CustBind:MyBaseLogic
    {
        object load(LogicData arg)
        {
            SetContentType(EFFC.Frame.Net.Base.Constants.GoResponseDataType.RazorView);
            Razor.SetViewPath("~/Views/Busi/cust_recommended.cshtml");

            var up = DB.NewDBUnitParameter4Business();
            var re = DB.Excute(up, @"{
$acttype : 'Query',
$table : 'CodeDictionary',
$orderby:{
		SortNum:'asc'
	},
$where : {
		Category : '门店',
    }
}");
            Razor.SetViewData("selectvalues", re.QueryData<FrameDLRObject>());

            return null;
        }

        object bind(LogicData arg)
        {
            var rtn = FrameDLRObject.CreateInstance(@"{
issuccess:true,
msg:''
}");
            var user_mobile = ComFunc.nvl(arg["usermobile"]);
            var user_name = ComFunc.nvl(arg["username"]);
            var recommeded_mobile = ComFunc.nvl(arg["recommended_mobile"]);
            var storecode = ComFunc.nvl(arg["storecode"]);
            if (user_mobile == "" || recommeded_mobile == "" || user_name == "")
            {
                rtn.issuccess = false;
                rtn.msg = "\"推荐人电话号码/推荐人的姓名/被推荐人电话号码\"不可为空";
                return rtn;
            }

            if (user_mobile == recommeded_mobile)
            {
                rtn.issuccess = false;
                rtn.msg = "\"推荐人\"不可推荐自己";
                return rtn;
            }

            BeginTrans();
            var up = DB.NewDBUnitParameter4Business();
            up.SetValue("id", user_mobile);
            var re = DB.Query<UserUnit>(up, "userinfo");
            up.SetValue("id", recommeded_mobile);
            var re2 = DB.Query<UserUnit>(up, "userinfo");

            var useruid = "";
            var recommendeduid = "";

            //检查推荐人关系是否存在
            //门店只用来记录被推荐人所在门店
            //if (ComFunc.nvl(re.QueryTable[0, "BelongToStore"]) != "" && ComFunc.nvl(re.QueryTable[0, "BelongToStore"]) != storecode)
            //{
            //    rtn.issuccess = false;
            //    rtn.msg = "推荐人所属门店不符，操作终止";
            //    return rtn;
            //}
            if (ComFunc.nvl(re2.QueryTable[0, "BelongToStore"]) != "" && ComFunc.nvl(re2.QueryTable[0, "BelongToStore"]) != storecode)
            {
                rtn.issuccess = false;
                rtn.msg = "被推荐人所属门店不符，操作终止";
                return rtn;
            }
            
            if (re2.QueryTable.RowLength > 0)
            {
                if(user_mobile == ComFunc.nvl(re2.QueryTable[0, "recommendMobile"]))
                {
                    rtn.issuccess = false;
                    rtn.msg = "推荐关系已经存在，无须重复设置";
                    return rtn;
                }
                else
                {
                    if(ComFunc.nvl(re2.QueryTable[0, "recommendMobile"])!= "" && user_mobile != ComFunc.nvl(re2.QueryTable[0, "recommendMobile"]))
                    {
                        rtn.issuccess = false;
                        rtn.msg = "被推荐人已有他人推荐，操作终止";
                        return rtn;
                    }
                }
            }
            //新增用户资料，并获取UID
            if (re2.QueryTable.RowLength <= 0)
            {
                recommendeduid = Guid.NewGuid().ToString();
                up.SetValue("UserUID", recommendeduid);
                up.SetValue("UserAccountNo", null);
                up.SetValue("UserMobile", recommeded_mobile);
                up.SetValue("UserName", null);
                up.SetValue("UserSex", null);
                up.SetValue("UserRealName", null);
                up.SetValue("UserRealID", null);
                up.SetValue("UserType", "Customer");
                up.SetValue("BelongToStore", storecode);

                DB.NonQuery<UserUnit>(up, "addUserInfo");
            }
            else
            {
                recommendeduid = ComFunc.nvl(re2.QueryTable[0, "UserUID"]);
            }
            
            if (re.QueryTable.RowLength > 0)
            {
                useruid = ComFunc.nvl(re.QueryTable[0, "UserUID"]);
            }
            else
            {
                useruid = Guid.NewGuid().ToString();
                up.SetValue("UserUID", useruid);
                up.SetValue("UserAccountNo", null);
                up.SetValue("UserMobile", user_mobile);
                up.SetValue("UserName", user_name);
                up.SetValue("UserSex", null);
                up.SetValue("UserRealName", null);
                up.SetValue("UserRealID", null);
                up.SetValue("UserType", "Sale");
                up.SetValue("BelongToStore", storecode);

                DB.NonQuery<UserUnit>(up, "addUserInfo");

            }


            //更新用户推荐关系数据
            up.SetValue("UserUID", useruid);
            up.SetValue("RecommededUID", recommendeduid);
            DB.NonQuery<UserUnit>(up, "recommend");

            CommitTrans();
            return rtn;
        }
    }
}
