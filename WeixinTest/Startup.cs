﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using EFFC.Frame.Net.Module.Extend.WeixinWeb;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Common;

namespace WeixinTest
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.CookieName = ".AdventureWorks.Session";
                options.IdleTimeout = TimeSpan.FromSeconds(10);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseStaticFiles();
            app.UseSession();
            var options = FrameDLRObject.CreateInstance();
            //Logic参数设定
            options.BusinessAssemblyName = "Web.Business";
            //Razor参数设定
            options.ExcuteFilePath = env.ContentRootPath;
            //起始页设定
            options.StartPage = MyConfig.GetConfiguration("EFFC", "FrameStartPage");
            app.UseWeixinGoMiddleWare((FrameDLRObject)options);
        }
    }
}
