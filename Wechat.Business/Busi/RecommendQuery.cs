using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Module.Extend.WebGo.Datas;
using System;
using System.Collections.Generic;
using System.Text;
using Wechat.Unit.Busi;

namespace Wechat.Business.Busi
{
    public class RecommendQuery:MyBaseLogic
    {
        object load(LogicData arg)
        {
            SetContentType(EFFC.Frame.Net.Base.Constants.GoResponseDataType.RazorView);
            Razor.SetViewPath("~/Views/Busi/recommend_query.cshtml");
            return null;
        }

        object query(LogicData arg)
        {
            var rtn = FrameDLRObject.CreateInstance(@"{
issuccess:true,
msg:''
}");
            var user_mobile = ComFunc.nvl(arg["usermobile"]);
            var recommeded_mobile = ComFunc.nvl(arg["recommended_mobile"]);

            var up = DB.NewDBUnitParameter4Business();
            up.SetValue("UserID", user_mobile);
            up.SetValue("RecommendedID", recommeded_mobile);
            var re = DB.QueryByPage<UserUnit>(up, "QueryUserList");
            var list = new List<FrameDLRObject>();
            var lre = re.QueryData<FrameDLRObject>();
            foreach(dynamic item in lre)
            {
                var dobj = FrameDLRObject.CreateInstance();
                dobj.storename = item.storename;
                dobj.usermobile = item.usermobile;
                dobj.username = item.username;
                dobj.recommendmobile = item.recommendmobile;
                dobj.recommededstorename = item.recommededstorename;

                list.Add(dobj);
            }

            rtn.data = list;
            rtn.currentpage = re.CurrentPage;
            rtn.count_per_page = re.Count_Of_OnePage;
            rtn.total_page = re.TotalPage;
            rtn.total_rows = re.TotalRow;

            return rtn;
        }
    }
}
