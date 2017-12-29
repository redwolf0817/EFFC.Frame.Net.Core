using EFFC.Frame.Net.Base.Module.Proxy;
using EFFC.Frame.Net.Module.Web.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using EFFC.Frame.Net.Global;
using Microsoft.AspNetCore.Builder;
using EFFC.Frame.Net.Base.Data.Base;

namespace EFFC.Frame.Net.Module.Extend.WebGo
{
    /// <summary>
    /// Go请求的中间件
    /// </summary>
    public class GoMiddleWare : EFFCWebMiddleWare
    {
        public GoMiddleWare(RequestDelegate next, IHostingEnvironment hostingEnv,FrameDLRObject options) : base(next, hostingEnv, options)
        {
        }

        protected override void DoInvoke(HttpContext context, string requestExtType)
        {
            object result = new object();
            
            GlobalCommon.Proxys["go"].CallModule(ref result, context);
        }

        protected override void LoadProxys(ProxyManager pm,dynamic options)
        {
            pm.UseProxy<WebGoProxy>("go", options);
        }
    }
    public static class GoMiddleWareExtensions
    {
        public static IApplicationBuilder UseWebGoMiddleWare(this IApplicationBuilder builder,FrameDLRObject options=null)
        {
            return builder.UseMiddleware<GoMiddleWare>(options);
        }
    }
}
