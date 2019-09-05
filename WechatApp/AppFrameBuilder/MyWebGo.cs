using EFFC.Frame.Net.Module.Extend.WeixinWeb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFFC.Frame.Net.Module.Extend.WebGo.Datas;
using EFFC.Frame.Net.Module.Web.Parameters;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;

namespace WechatApp.AppFrameBuilder
{
    public class MyWebGo:WebGoExtend
    {
        protected override bool IsValid4Invoke(WebParameter p, GoData d)
        {
            var rtn = true;
//            if (!GlobalPrepare.IsIgnoreLoginAuth(p))
//            {
//                if (p.LoginInfo == null)
//                {
//                    rtn = false;
//                    if(ComFunc.nvl(p.CurrentHttpContext.Request.Headers["x-requested-with"].FirstOrDefault()) == "XMLHttpRequest"
//                        && ComFunc.nvl(p.CurrentHttpContext.Request.Headers["x-request-async"].FirstOrDefault()) == "true")
//                    {
//                        d.ResponseData = FrameDLRObject.CreateInstance(@"{
//__isneedlogin__:true,
//__loginurl__:'/admin'
//}");
//                    }
//                    else
//                    {
//                        d.ResponseData = FrameDLRObject.CreateInstance();
//                        d.RedirectUri = "/admin";
//                    }
                    
//                }
//            }
            return rtn;
        }
    }
}
