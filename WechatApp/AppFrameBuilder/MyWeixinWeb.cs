using EFFC.Frame.Net.Module.Extend.WeixinWeb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFFC.Frame.Net.Module.Extend.WebGo.Datas;
using EFFC.Frame.Net.Module.Web.Parameters;
using System.IO;

namespace WechatApp.AppFrameBuilder
{
    public class MyWeixinWeb:QiyeWxWebGo
    {
        protected override bool IsValid4Invoke(WebParameter p, GoData d)
        {
            var rtn = base.IsValid4Invoke(p, d);
            if (!rtn) return rtn;

            var ext = Path.GetExtension(CurrentContext.Request.Path).Replace(".", "").ToLower();
            if(ext == "go")
            {

            }

            return rtn;
        }
    }
}
