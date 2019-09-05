using EFFC.Frame.Net.Module.Extend.WebGo.Datas;
using EFFC.Frame.Net.Module.Extend.WeixinWeb.Logic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wechat.Business
{
    public class WeixinHome:MyBaseLogic
    {
        protected override object msg_text(LogicData arg)
        {
            return Weixin.GenResponseText($@"本公众号目前仅针对公司内部员工开放，目前开发功能如下:
1.进行各种后台消息通知");
        }
    }
}
