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
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Module.Web.Options;
using System.Reflection;

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
        private WebMiddleWareProcessOptions _middleware_options = null;
        /// <summary>
        /// EFFC框架中间件基类
        /// </summary>
        /// <param name="next"></param>
        public EFFCWebMiddleWare(RequestDelegate next, IHostingEnvironment hostingEnv,FrameDLRObject options)
        {
            GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, "Web服务启动设定...");
            // This is an HTTP Handler, so no need to store next

            _next = next;
            _hostenv = hostingEnv;
            Type middlewaretype = typeof(WebMiddleWareProcessOptions);
            if (options != null)
            {
                dynamic dobj = options;
                
                if(dobj.MiddleWareOptionsType != null)
                {
                    if (dobj.MiddleWareOptionsType is string)
                    {
                        middlewaretype = Type.GetType(ComFunc.nvl(dobj.MiddleWareOptionsType));
                    }
                    else if (dobj.MiddleWareOptionsType is Type && ((Type)dobj.MiddleWareOptionsType).GetTypeInfo().IsSubclassOf(typeof(WebMiddleWareProcessOptions)))
                    {
                        middlewaretype = (Type)dobj.MiddleWareOptionsType;
                    }
                    else
                    {
                        middlewaretype = typeof(WebMiddleWareProcessOptions);
                    }
                }
            }
            _middleware_options = (WebMiddleWareProcessOptions)Activator.CreateInstance(middlewaretype, true);

            GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.INFO, $"{this.GetType().Name}：当前启动中间件加载的Options设定为{middlewaretype.Name},如需要修改，请在调用ProxyManager.UseProxy中的options中设定该参数（MiddleWareOptionsType类型为Type类型,必须为WebMiddleWareProcessOptions类型或其子类）");
            //将以下启动参数下沉到各个module中
            options.SetValue("PagePath4Forbidden", _middleware_options.PagePath4Forbidden);
            options.SetValue("PagePath4NotFound", _middleware_options.PagePath4NotFound);
            options.SetValue("PagePath4Error", _middleware_options.PagePath4Error);
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

            ext = _middleware_options.ConvertExtTo(context, ext);

            if (_middleware_options.IsStaticType(context, ext))
            {
                ProcessStaticFile(context, ext);
                return;
            }
            else
            {
                DoInvoke(context, ext);
            }
        }
        /// <summary>
        /// 静态文件资源请求处理，可扩展
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requestExtType"></param>
        protected virtual void ProcessStaticFile(HttpContext context,string requestExtType)
        {
            IFileInfo fileInfo = _middleware_options.GetStaticFileInfo(_hostenv.ContentRootPath, context, requestExtType);

            if (fileInfo.Exists)
            {
                if (_middleware_options.IsOpenStaticFileType(context, requestExtType))
                {
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = ResponseHeader_ContentType.Map(requestExtType);
                    context.Response.Headers.Add("Content-Length", fileInfo.Length + "");
                    //var bytes = ComFunc.StreamToBytes(fileInfo.CreateReadStream());
                    //context.Response.Body.Write(bytes, 0, bytes.Length);
                    context.Response.SendFileAsync(fileInfo).Wait();
                }
                else
                {
                    context.Response.StatusCode = 403;
                    if(string.IsNullOrEmpty(_middleware_options.PagePath4Forbidden))
                    {
                        context.Response.Headers.Add("Content-Length", "0");
                    }
                    else
                    {
                        var path = _middleware_options.PagePath4Forbidden.Replace("~", _hostenv.ContentRootPath);
                        if (File.Exists(path))
                        {
                            var resultmsg = File.ReadAllText(path);
                            var msgbytelength = Encoding.UTF8.GetByteCount(resultmsg);
                            context.Response.ContentType = ResponseHeader_ContentType.html + ";charset=utf-8";
                            context.Response.Headers.Add("Content-Length", msgbytelength + "");
                            context.Response.WriteAsync(resultmsg).Wait();
                        }
                        else
                        {
                            context.Response.Headers.Add("Content-Length", "0");
                        }
                    }
                    
                }
            }
            else
            {
                context.Response.StatusCode = 404;
                if (string.IsNullOrEmpty(_middleware_options.PagePath4NotFound))
                {
                    context.Response.Headers.Add("Content-Length", "0");
                }
                else
                {
                    var path = _middleware_options.PagePath4NotFound.Replace("~", _hostenv.ContentRootPath);
                    if (File.Exists(path))
                    {
                        var resultmsg = File.ReadAllText(path);
                        var msgbytelength = Encoding.UTF8.GetByteCount(resultmsg);
                        context.Response.ContentType = ResponseHeader_ContentType.html + ";charset=utf-8";
                        context.Response.Headers.Add("Content-Length", msgbytelength + "");
                        context.Response.WriteAsync(resultmsg).Wait();
                    }
                    else
                    {
                        context.Response.Headers.Add("Content-Length", "0");
                    }
                }
            }
            
            
            context.Response.Body.Flush();
        }
        protected abstract void DoInvoke(HttpContext context, string requestExtType);
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
