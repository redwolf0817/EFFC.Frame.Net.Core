using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Module.Extend.WebGo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Test
{
    internal class Startup
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
            options.BusinessAssemblyName = "Web.Business";
            app.UseWebGoMiddleWare((FrameDLRObject)options);
        }
    }
}