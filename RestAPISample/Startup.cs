using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Module.Extend.EWRA;
using EFFC.Frame.Net.Module.Extend.EWRA.Logic;
using RestAPI.Business;

namespace RestAPISample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var options = FrameDLRObject.CreateInstance();
            //Logic参数设定
            options.RestAPILogicAssemblyName = "RestAPI.Business";
            //RestAPI模块启用定义
            options.RestAPILogicBaseType = typeof(MyRestLogic);

            app.UseEWRAMiddleWare((FrameDLRObject)options);
        }
    }
}
