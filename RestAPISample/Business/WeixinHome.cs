using EFFC.Frame.Net.Module.Extend.WebGo.Datas;
using EFFC.Frame.Net.Module.Extend.WeixinWeb.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPISample.Business
{
    public class WeixinHome:WeixinGoLogic
    {
        protected override object msg_text(LogicData arg)
        {
            var userinfo = Weixin.GetUserInfo(Weixin.FromUserName);

            return Weixin.GenResponseText($@"本公众号目前仅针对公司内部员工开放，目前开发功能如下:
1.进行各种后台消息通知");
        }
    }
}
