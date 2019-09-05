using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Module.Extend.WebGo.Datas;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wechat.Business.Busi
{
    class Notice:MyBaseLogic
    {
        object SendMsg(LogicData arg)
        {
            var touser = ComFunc.nvl(arg.GetValue("to"));
            var content = ComFunc.IsBase64Then(ComFunc.nvl(arg.GetValue("content")).Replace(" ","+"));
            return Weixin.SendTextCardMsg(touser, "", "","本月薪资",$"{DateTime.Now.ToString("yyyy年MM月")}工作单","http://www.baidu.com");
        }
    }
}
