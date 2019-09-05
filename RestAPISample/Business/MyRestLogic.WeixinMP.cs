using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Module.Extend.EWRA.Logic;
using EFFC.Frame.Net.Module.Extend.WeChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPISample.Business
{
    public partial class MyRestLogic : ValidRestLogic
    {
        WeixinMPHelper _weixinmp = null;
        public WeixinMPHelper WeixinMP
        {
            get
            {
                if (_weixinmp == null) _weixinmp = new WeixinMPHelper(this);
                return _weixinmp;
            }


        }
        public class WeixinMPHelper : WechatMPHelper
        {
            MyRestLogic _logic;

            public WeixinMPHelper(MyRestLogic logic) : base("", "")
            {
                _logic = logic;

                AppID = ComFunc.nvl(_logic.Configs["weixinmp_Appid"]);
                AppSecret = ComFunc.nvl(_logic.Configs["weixinmp_Appsecret"]);
                Weixin_Mch_Ssl_Path = ComFunc.nvl(_logic.Configs["weixinmp_Mch_SSL_Path"]);
                Weixin_Mch_Ssl_Pass = ComFunc.nvl(_logic.Configs["weixinmp_Mch_SSL_Pass"]);
            }

        }
    }
}
