using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Module.Web.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EWRA
{
    public class EWRAMiddleWare : EFFCWebMiddleWare
    {
        public EWRAMiddleWare(RequestDelegate next, IHostingEnvironment hostingEnv, FrameDLRObject options) : base(next, hostingEnv, options)
        {
        }
        protected override void DoInvoke(Microsoft.AspNetCore.Http.HttpContext context, string requestExtType)
        {
            object result = new object();

            GlobalCommon.Proxys["ewra"].CallModule(ref result, context);
        }

        protected override void LoadProxys(Net.Base.Module.Proxy.ProxyManager pm, dynamic options)
        {
            pm.UseProxy<EWRAGoProxy>("ewra", options);
        }
    }

    public static class EWRAMiddleWareExtensions
    {
        public static IApplicationBuilder UseEWRAMiddleWare(this IApplicationBuilder builder, FrameDLRObject options = null)
        {
            return builder.UseMiddleware<EWRAMiddleWare>(options);
        }
    }
}
