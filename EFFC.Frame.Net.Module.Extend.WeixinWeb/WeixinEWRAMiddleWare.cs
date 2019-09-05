using EFFC.Frame.Net.Base.Data.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using EFFC.Frame.Net.Base.Module.Proxy;
using EFFC.Frame.Net.Module.Web.Core;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Module.Extend.EWRA;

namespace EFFC.Frame.Net.Module.Extend.WeixinWeb
{
    public class WeixinEWRAMiddleWare : EFFCWebMiddleWare
    {
        public WeixinEWRAMiddleWare(RequestDelegate next, IHostingEnvironment hostingEnv, FrameDLRObject options) : base(next, hostingEnv, options)
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
                GlobalCommon.Proxys["ewra"].CallModule(ref result, context);
            }
        }

        protected override void LoadProxys(ProxyManager pm, dynamic options)
        {
            pm.UseProxy<WeixinWebGoProxy>("weixin", options);
            pm.UseProxy<EWRAGoProxy>("ewra", options);
        }
    }
    public static class WeixinEWRAMiddleWareExtensions
    {
        public static IApplicationBuilder UseWeixinEWRAMiddleWare(this IApplicationBuilder builder, FrameDLRObject options = null)
        {
            if (options != null)
            {
                dynamic obj = options;
                if (ComFunc.nvl(obj.RestAPIModuleName) == "")
                {
                    obj.RestAPIModuleName = typeof(EWRAGoExtend).FullName;
                }
            }
            return builder.UseMiddleware<WeixinEWRAMiddleWare>(options);
        }
    }
}
