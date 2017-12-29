using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Compilation;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.WebData;
using EFFC.Frame.Net.Base.Constants;

namespace EFFC.Frame.Net.Business.Engine
{
    public class WMvcView : IView
    {
        public string ViewPath { get; set; }
        public string StartPageFile { get; set; }
        public WMvcView(string viewPath)
        {
            this.ViewPath = viewPath;
            StartPageFile = "_ViewStart";
        }
        public WMvcView(string viewPath, string startpagefile)
        {
            this.ViewPath = viewPath;
            StartPageFile = startpagefile;
        }
        public void Render(ViewContext viewContext, TextWriter writer)
        {
            Type viewType = BuildManager.GetCompiledType(this.ViewPath);
            object instance = Activator.CreateInstance(viewType);
            WebViewPage page = (WebViewPage)instance as WebViewPage;
            page.VirtualPath = this.ViewPath;
            page.ViewContext = viewContext;
            page.ViewData = viewContext.ViewData;
            page.InitHelpers();
            WebPageContext pageContext = new WebPageContext(viewContext.HttpContext, null, null);
            WebPageRenderingBase startPage = StartPage.GetStartPage(page, StartPageFile, new string[] { "cshtml", "vbhtml" });
            page.ExecutePageHierarchy(pageContext, writer, startPage);
        }

        public void Dispose()
        {
            ViewPath = null;
            StartPageFile = null;
        }

        public static void RenderView(WebParameter wp, WMvcData wd, HttpContext context, TextWriter writer)
        {
            RouteData rd = new RouteData();
            ControllerContext cc = new ControllerContext();//new ControllerContext(new HttpContextWrapper(context), new RouteData(), new MyController());
            cc.HttpContext = new HttpContextWrapper(context);
            cc.RouteData = new RouteData();
            cc.RouteData.Values.Add("controller", wp.RequestResourceName);
            cc.RouteData.Values.Add("action", "Process");
            //添加Mvc Module数据
            ViewDataDictionary vdd = null;
            if (wd.MvcModuleData != null)
                vdd = new ViewDataDictionary(wd.MvcModuleData);
            else
                vdd = new ViewDataDictionary();
            //添加ViewData
            foreach (var val in wd.Domain(DomainKey.VIEW_LIST))
            {
                if (vdd.ContainsKey(val.Key))
                {
                    vdd[val.Key] = val.Value;
                }
                else
                {
                    vdd.Add(val.Key, val.Value);
                }
            }

            TempDataDictionary tdd = new TempDataDictionary();
            if (string.IsNullOrEmpty(wd.ViewPath))
            {
                throw new Exception("没有获得ViewPath，无法展现页面");
            }
            WMvcView rv = new WMvcView(wd.ViewPath);
            TextWriter _tw = new StringWriter();
            try
            {
                ViewContext vc = new ViewContext(cc, rv, vdd, tdd, _tw);
                rv.Render(vc, writer);
            }
            finally
            {
                _tw.Close();
                _tw.Dispose();
                _tw = null;
                vdd.Clear();
                rv.Dispose();
            }
        }

        public static void RenderView(WebParameter wp, GoData gd, HttpContext context, TextWriter writer)
        {
            RouteData rd = new RouteData();
            ControllerContext cc = new ControllerContext();//new ControllerContext(new HttpContextWrapper(context), new RouteData(), new MyController());
            cc.HttpContext = new HttpContextWrapper(context);
            cc.RouteData = new RouteData();
            cc.RouteData.Values.Add("controller", wp.RequestResourceName);
            cc.RouteData.Values.Add("action", "Process");
            //添加Mvc Module数据
            ViewDataDictionary vdd = null;
            if (gd.MvcModuleData != null)
                vdd = new ViewDataDictionary(gd.MvcModuleData);
            else
                vdd = new ViewDataDictionary();
            //添加ViewData
            foreach (var val in gd.Domain(DomainKey.VIEW_LIST))
            {
                if (vdd.ContainsKey(val.Key))
                {
                    vdd[val.Key] = val.Value;
                }
                else
                {
                    vdd.Add(val.Key, val.Value);
                }
            }

            TempDataDictionary tdd = new TempDataDictionary();
            if (string.IsNullOrEmpty(gd.ViewPath))
            {
                throw new Exception("没有获得ViewPath，无法展现页面");
            }
            WMvcView rv = new WMvcView(gd.ViewPath);
            TextWriter _tw = new StringWriter();
            try
            {
                ViewContext vc = new ViewContext(cc, rv, vdd, tdd, _tw);
                rv.Render(vc, writer);
            }
            finally
            {
                _tw.Close();
                _tw.Dispose();
                _tw = null;
                rv.Dispose();
            }
        }

        
    }
}
