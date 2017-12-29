using EFFC.Frame.Net.Module.Extend.WebGo;
using EFFC.Frame.Net.Base.Data.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using EFFC.Frame.Net.Base.Module.Proxy;
using EFFC.Frame.Net.Module.Web.Core;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Base.Common;

namespace EFFC.Frame.Net.Module.Extend.WeixinWeb
{
    public class WeixinGoMiddleWare : EFFCWebMiddleWare
    {
        public WeixinGoMiddleWare(RequestDelegate next, IHostingEnvironment hostingEnv, FrameDLRObject options) : base(next, hostingEnv, options)
        {
        }
        protected override void DoInvoke(HttpContext context, string requestExtType)
        {
            object result = new object();

            if (requestExtType == "wx")
            {
                GlobalCommon.Proxys["weixin"].CallModule(ref result, context);
            }
            else
            {
                GlobalCommon.Proxys["go"].CallModule(ref result, context);
            }
        }

        protected override void LoadProxys(ProxyManager pm, dynamic options)
        {
            pm.UseProxy<WeixinWebGoProxy>("weixin", options);
            pm.UseProxy<WebGoProxy>("go", options);
        }
    }
    public static class WeixinGoMiddleWareExtensions
    {
        public static IApplicationBuilder UseWeixinGoMiddleWare(this IApplicationBuilder builder, FrameDLRObject options = null)
        {
            
            if(options != null)
            {
                dynamic obj = options;
                if (ComFunc.nvl( obj.WebModuleName) == "")
                {
                    obj.WebModuleName = typeof(WebGoExtend).FullName;
                }
            }
            return builder.UseMiddleware<WeixinGoMiddleWare>(options);
        }
    }
}
