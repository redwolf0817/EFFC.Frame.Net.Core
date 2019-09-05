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
using RestAPISample.Business;
using EFFC.Frame.Net.Module.Extend.WeixinWeb;

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
            options.BusinessAssemblyName = "RestAPISample";
            options.WeixinHome = "weixinhome";
            //EWRA自定义模块加载
            options.RestAPIModuleName = typeof(MyWebModule).FullName;
            //Logic参数设定
            options.RestAPILogicAssemblyName = "RestAPISample";
            //RestAPI模块启用定义
            options.RestAPILogicBaseType = typeof(MyRestLogic);
            //Tag模块启动定义
            options.TagAssembly = "RestAPISample";
            options.RestAPIMainVersion = "v1.0";
            //默认起始路由
            options.DefaultStartRoute = MyConfig.GetConfiguration("Server", "DefaultStartRoute");
            //设置apidoc的路由
            options.APIDocRoute = MyConfig.GetConfiguration("Server", "APIDocRoute");
            //设置是否显示api doc
            options.IsShowRestAPIDoc = BoolStd.IsNotBoolThen(MyConfig.GetConfiguration("Server", "IsShowAPIDoc"), false);
            //设置中间件参数
            options.MiddleWareOptionsType = typeof(MyWebOptions);

            app.UseWeixinEWRAMiddleWare((FrameDLRObject)options);
        }
    }
}
