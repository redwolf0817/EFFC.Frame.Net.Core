using System;
using System.IO;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.WebData;
using EFFC.Frame.Net.Base.Constants;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using System.Reflection;
using System.Linq;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.PlatformAbstractions;
using System.Collections.Generic;

namespace EFFC.Frame.Net.Business.Engine
{
    public class WMvcView
    {
        RazorViewToStringRenderer renderer = null;

        public string ViewPath { get; set; }
        public string StartPageFile { get; set; }

        public WMvcView():this(Directory.GetCurrentDirectory())
        {   
        }
        public WMvcView(string viewPath):this(viewPath, "_ViewStart")
        {
        }
        public WMvcView(string viewPath, string startpagefile)
        {
            this.ViewPath = viewPath;
            StartPageFile = startpagefile;

            var services = new ServiceCollection();
            ConfigureDefaultServices(services);
            var provider = services.BuildServiceProvider();
            renderer = provider.GetRequiredService<RazorViewToStringRenderer>();
        }
        /// <summary>
        /// 将cshtml渲染成string
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="viewContext"></param>
        /// <param name="writer"></param>
        /// <returns></returns>
        public string Render(string viewName,HttpContext context, object model,Dictionary<string,object> viewList)
        {
            return renderer.RenderViewToString(viewName, context,model,viewList).GetAwaiter().GetResult();
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
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Private.Corelib")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.ApplicationInsights.AspNetCore")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Html.Abstractions")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Razor")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Razor.Runtime")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.AspNetCore.Mvc")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Dynamic.Runtime")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Text.Encodings.Web")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(assembly.Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Frame.Net.Base")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Frame.Net.Global")).Location));
                    assemblies.Add(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Frame.Net.Data")).Location));
                    


                    context.Compilation = context.Compilation.AddReferences(assemblies);
                };
            });
            var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
            services.AddSingleton<DiagnosticSource>(diagnosticSource);
            services.AddLogging();
            services.AddMvc();
            services.AddSingleton<RazorViewToStringRenderer>();
            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        }

        public void Dispose()
        {
            ViewPath = null;
            StartPageFile = null;
        }
        /// <summary>
        /// 渲染cshtml成string
        /// </summary>
        /// <param name="wp"></param>
        /// <param name="wd"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string RenderView(WebParameter wp, WMvcData wd, HttpContext context)
        {

            if (string.IsNullOrEmpty(wd.ViewPath))
            {
                throw new Exception("没有获得ViewPath，无法展现页面");
            }
            WMvcView rv = new WMvcView(wd.ViewPath);
            return rv.Render(Path.GetFileNameWithoutExtension(wd.ViewPath),context, wd.MvcModuleData, wd.Domain(DomainKey.VIEW_LIST));
        }
        /// <summary>
        /// 渲染cshtml成string
        /// </summary>
        /// <param name="wp"></param>
        /// <param name="gd"></param>
        /// <param name="context"></param>
        /// <param name="writer"></param>
        /// <returns></returns>
        public static string RenderView(WebParameter wp, GoData gd, HttpContext context)
        {
            if (string.IsNullOrEmpty(gd.ViewPath))
            {
                throw new Exception("没有获得ViewPath，无法展现页面");
            }
            WMvcView rv = new WMvcView(gd.ViewPath);
            return rv.Render(Path.GetFileNameWithoutExtension(gd.ViewPath), context, gd.MvcModuleData, gd.Domain(DomainKey.VIEW_LIST));

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

            public async Task<string> RenderViewToString(string name,HttpContext context, object model,Dictionary<string,object> viewList)
            {
                var actionContext = GetActionContext(context);
                var viewEngineResult = _viewEngine.FindView(actionContext, name, false);

                if (!viewEngineResult.Success)
                {
                    throw new InvalidOperationException(string.Format("Couldn't find view '{0}'", name));
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
                    await view.RenderAsync(viewContext);

                    return output.ToString();
                }
            }

            private ActionContext GetActionContext(HttpContext context)
            {
                return new ActionContext(context, new RouteData(), new ActionDescriptor());
            }
        }
    }
}
