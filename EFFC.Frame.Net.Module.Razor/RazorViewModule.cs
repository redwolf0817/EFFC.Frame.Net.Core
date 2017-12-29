using EFFC.Frame.Net.Base.Module;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Parameter;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.FileProviders;
using System.Reflection;
using System.Linq;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using Microsoft.Extensions.ObjectPool;
using EFFC.Frame.Net.Base.Module.Proxy;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Base.Common;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;

namespace EFFC.Frame.Net.Module.Razor
{
    /// <summary>
    /// 呼叫RazorView引擎进行页面解析的模块
    /// </summary>
    public class RazorViewModule : BaseModule
    {
        static RazorViewToStringRenderer renderer = null;
        static string excuteFilePath = "";
        protected override void OnUsed(ProxyManager ma, dynamic options)
        {
            if (options.ExcuteFilePath == null)
            {
                Assembly assembly = this.GetType().GetTypeInfo().Assembly;
                AssemblyName assemblyName = assembly.GetName();
                ApplicationEnvironment env = PlatformServices.Default.Application;
                excuteFilePath = env.ApplicationBasePath;
            }
            else
            {
                excuteFilePath = ComFunc.nvl(options.ExcuteFilePath);
            }
            GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.INFO, string.Format("Razor引擎ExcuteFilePath={0}", excuteFilePath));

            var services = new ServiceCollection();
            ConfigureDefaultServices(services);
            var provider = services.BuildServiceProvider();
            var mp = provider.GetRequiredService<IViewBufferScope>();
            renderer = provider.GetRequiredService<RazorViewToStringRenderer>();
        }
        public override string Name => "razor";

        public override string Description => "EFFC框架下的Razor View引擎模块";

        public override bool CheckParametersAndConfig(ParameterStd p, DataCollection d)
        {
            if (!(p is RazorParam)) return false;
            if (!(d is RazorData)) return false;

            var rp = (RazorParam)p;
            if (string.IsNullOrEmpty(rp.ViewPath))
            {
                GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.ERROR, "Razor引擎缺少ViewPath参数");
                return false;
            }
            if (rp.CurrentHttpContext == null)
            {
                GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.ERROR, "Razor引擎缺少HttpContext参数");
                return false;
            }

            return true;
        }

        public override void Dispose()
        {
            
        }

        protected override void OnError(Exception ex, ParameterStd p, DataCollection d)
        {
            throw new RazorException("Razor parse failed!"+ex.Message, ex);
        }

        protected override void Run(ParameterStd p, DataCollection d)
        {
            var rp = (RazorParam)p;
            var rd = (RazorData)d;

            rd.RenderText = renderer.RenderViewToString(excuteFilePath, rp.ViewPath, rp.CurrentHttpContext, rp.Model, rp.ViewList).GetAwaiter().GetResult(); ;
        }

        private void ConfigureDefaultServices(IServiceCollection services)
        {
            var applicationEnvironment = PlatformServices.Default.Application;
            services.AddSingleton(applicationEnvironment);

            var appDirectory = Directory.GetCurrentDirectory();
            services.AddSingleton<IHostingEnvironment>(new HostingEnvironment
            {
                //WebRootFileProvider = new PhysicalFileProvider(applicationEnvironment.ApplicationBasePath)
                WebRootFileProvider = new PhysicalFileProvider(appDirectory),
                ApplicationName = applicationEnvironment.ApplicationName
            });
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.FileProviders.Clear();
                options.FileProviders.Add(new PhysicalFileProvider(appDirectory));
                var previous = options.CompilationCallback;
                options.CompilationCallback = (context) =>
                {
                    previous?.Invoke(context);
                    var assembly = this.GetType().GetTypeInfo().Assembly;
                    //.net编译器会自动将没用到的assembly移除
                    var assemblies = Assembly.GetEntryAssembly().GetReferencedAssemblies()
                                             .Select(x => MetadataReference.CreateFromFile(Assembly.Load(x).Location))
                                             .ToList();
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("mscorlib")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.CSharp")).Location));
                    //不需要System.Private.Corelib，里面的Task会与System.Runtime中的Task重复
                    //assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Private.Corelib")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.ApplicationInsights.AspNetCore")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Html.Abstractions")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Razor")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Razor.Runtime")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Mvc")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Dynamic.Runtime")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Text.Encodings.Web")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(assembly.Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("EFFC.Frame.Net.Base")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("EFFC.Frame.Net.Global")).Location));



                    context.Compilation = context.Compilation.AddReferences(assemblies);
                };
            });
            var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
            services.AddSingleton<DiagnosticSource>(diagnosticSource);
            services.AddLogging();
            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            services.AddDistributedMemoryCache();
            services.AddMvc();
            services.AddSingleton<RazorViewToStringRenderer>();
            
            
        }

        protected class RazorViewToStringRenderer
        {
            private readonly IRazorViewEngine _viewEngine;
            private readonly ITempDataProvider _tempDataProvider;
            private readonly IServiceProvider _serviceProvider;

            public RazorViewToStringRenderer(
                IRazorViewEngine viewEngine,
                ITempDataProvider tempDataProvider,
                IServiceProvider serviceProvider)
            {
                _viewEngine = viewEngine;
                _tempDataProvider = tempDataProvider;
                _serviceProvider = serviceProvider;
            }

            public async Task<string> RenderViewToString(string excuteFilePath, string viewpath, HttpContext context, object model, Dictionary<string, object> viewList)
            {
                var actionContext = GetActionContext(context);
                //var viewEngineResult = _viewEngine.FindView(actionContext, "index", false);
                var viewEngineResult = _viewEngine.GetView(excuteFilePath, viewpath, false);

                if (!viewEngineResult.Success)
                {
                    throw new InvalidOperationException(string.Format("Couldn't find view '{0}'", viewpath));
                }

                var view = viewEngineResult.View;

                using (var output = new StringWriter())
                {
                    var vdd = new ViewDataDictionary(
                        metadataProvider: new EmptyModelMetadataProvider(),
                        modelState: new ModelStateDictionary())
                    {
                        Model = model
                    };
                    var tdd = new TempDataDictionary(
                            actionContext.HttpContext,
                            _tempDataProvider);
                    if (viewList != null)
                    {
                        foreach (var item in viewList)
                        {
                            vdd.Add(item.Key, item.Value);
                        }
                    }

                    var viewContext = new ViewContext(
                        actionContext,
                        view,
                        vdd,
                        tdd,
                        output,
                        new HtmlHelperOptions());
                    await view.RenderAsync(viewContext).ConfigureAwait(false);

                    return output.ToString();
                }
            }

            private ActionContext GetActionContext(HttpContext context)
            {
                //如果使用传入的httpcontext则在外面的middleware中需要AddMvc，不然会报找不到IViewBufferScope的错误
                var httpContext = new DefaultHttpContext();
                httpContext.RequestServices = _serviceProvider;
                return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            }
        }
    }
}
