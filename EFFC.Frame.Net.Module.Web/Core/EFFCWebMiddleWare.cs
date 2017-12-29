using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using EFFC.Frame.Net.Module.Web.Modules;
using Microsoft.AspNetCore.Builder;
using EFFC.Frame.Net.Global;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Module.Proxy;
using EFFC.Frame.Net.Base.Data.Base;

namespace EFFC.Frame.Net.Module.Web.Core
{
    /// <summary>
    /// EFFC的Web中间件
    /// </summary>
    public abstract class EFFCWebMiddleWare
    {
        #region Init On MiddleWare used
        private IHostingEnvironment _hostenv;
        private RequestDelegate _next;
        private ProxyManager pm;
        /// <summary>
        /// EFFC框架中间件基类
        /// </summary>
        /// <param name="next"></param>
        public EFFCWebMiddleWare(RequestDelegate next, IHostingEnvironment hostingEnv,FrameDLRObject options)
        {
            GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, "Web服务启动设定...");
            // This is an HTTP Handler, so no need to store next
            _static_file_ext.AddRange(new string[]
                    {
                        "html",
                        "htm",
                        "jpg",
                        "jpeg",
                        "jpe",
                        "png",
                        "ico",
                        "bmp",
                        "gif",
                        "txt",
                        "css",
                        "js"
                    });

            _next = next;
            _hostenv = hostingEnv;

            //各个中间件在此处加载一次代理器
            LoadProxys(GlobalCommon.Proxys, options);
            GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, "Web服务启动设定完成\n");
        }
        /// <summary>
        /// 加载代理器
        /// </summary>
        /// <param name="pm"></param>
        protected abstract void LoadProxys(ProxyManager pm,dynamic options);
        #endregion
        public async Task Invoke(HttpContext context)
        {
            var ext = Path.GetExtension(context.Request.Path).Replace(".", "").ToLower();
            
            if (string.IsNullOrEmpty(ext))
            {
                ext = "go";
            }
            if (ReservedStaticFileExt.Contains(ext))
            {
                ProcessStaticFile(context, ext);
                return;
            }

            DoInvoke(context, ext);
        }
        /// <summary>
        /// 静态文件资源请求处理，可扩展
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requestExtType"></param>
        protected virtual void ProcessStaticFile(HttpContext context,string requestExtType)
        {
            IFileProvider provider = new PhysicalFileProvider(_hostenv.ContentRootPath);
            IFileInfo fileInfo = provider.GetFileInfo(context.Request.Path); // a file under applicationRoot

            if (fileInfo.Exists)
            {
                if (IsAllowed(context, requestExtType))
                {
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = ResponseHeader_ContentType.Map(requestExtType);
                    context.Response.Headers.Add("Content-Length", fileInfo.Length + "");
                }
                else
                {
                    context.Response.StatusCode = 403;
                    context.Response.Headers.Add("Content-Length", "0");
                }
            }
            else
            {
                context.Response.StatusCode = 404;
                context.Response.Headers.Add("Content-Length", "0");
            }
            
            context.Response.SendFileAsync(fileInfo);
        }
        /// <summary>
        /// 根据请求的类型判断是否为允许浏览的资源
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requestExtType"></param>
        /// <returns></returns>
        protected virtual bool IsAllowed(HttpContext context, string requestExtType)
        {
            return true;
        }
        protected abstract void DoInvoke(HttpContext context, string requestExtType);

        static List<string> _static_file_ext = new List<string>();
        /// <summary>
        /// 静态文件扩展名
        /// </summary>
        protected List<string> ReservedStaticFileExt
        {
            get
            {
                return _static_file_ext;
            }
        }
        /// <summary>
        /// 当前应用系统环境参数
        /// </summary>
        protected IHostingEnvironment HostEnvironment
        {
            get
            {
                return _hostenv;
            }
        }
    }

}
