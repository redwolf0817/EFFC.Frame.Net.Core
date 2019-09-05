using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Web.Options
{
    /// <summary>
    /// MiddleWare的处理可选项
    /// </summary>
    public class WebMiddleWareProcessOptions
    {
        List<string> _static_file_ext = new List<string>();
        public WebMiddleWareProcessOptions()
        {
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
                        "js",
                        "map"
                    });
        }
        /// <summary>
        /// 根据request的类型，进行转化
        /// </summary>
        /// <param name="context">请求的httpcontext</param>
        /// <param name="requestExtType">请求的类型</param>
        /// <returns>返回值为go类型则表示执行动态请求动作</returns>
        public virtual string ConvertExtTo(HttpContext context, string requestExtType)
        {
            if (string.IsNullOrEmpty(requestExtType))
            {
                return "go";
            }
            else
            {
                return requestExtType;
            }
        }
        /// <summary>
        /// 判定requestExtType是否为静态资源请求类型
        /// </summary>
        /// <param name="context">请求的httpcontext</param>
        /// <param name="requestExtType">请求的类型</param>
        /// <returns></returns>
        public virtual bool IsStaticType(HttpContext context, string requestExtType)
        {
            if(_static_file_ext.Contains(requestExtType))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 判定requestExtType是否为可以对外访问的静态资源请求类型
        /// </summary>
        /// <param name="context">请求的httpcontext</param>
        /// <param name="requestExtType">请求的类型</param>
        /// <returns></returns>
        public virtual bool IsOpenStaticFileType(HttpContext context, string requestExtType)
        {
            if (_static_file_ext.Contains(requestExtType))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 将虚拟请求路径转化为实际文件
        /// </summary>
        /// <param name="server_root_path">路径为相对路径</param>
        /// <param name="context"></param>
        /// <param name="requestExtType"></param>
        /// <returns>返回的路径也为相对路径,根路径为服务端路径+StaticFileRootPath</returns>
        public virtual IFileInfo GetStaticFileInfo(string server_root_path, HttpContext context, string requestExtType)
        {
            var provider = new PhysicalFileProvider(server_root_path + "/" + StaticFileRootPath);
            return provider.GetFileInfo(context.Request.Path); // a file under applicationRoot
        }
        /// <summary>
        /// 静态资源的根路径
        /// </summary>
        public virtual string StaticFileRootPath
        {
            get
            {
                return "/wwwroot/";
            }
        }
        /// <summary>
        /// 403个性化页面的路径，~为站点根路径
        /// </summary>
        public virtual string PagePath4Forbidden
        {
            get
            {
                return "";
            }
        }
        /// <summary>
        /// 404个性化页面的路径，~为站点根路径
        /// </summary>
        public virtual string PagePath4NotFound
        {
            get
            {
                return "";
            }
        }
        /// <summary>
        /// 500个性化页面的路径，~为站点根路径
        /// </summary>
        public virtual string PagePath4Error
        {
            get
            {
                return "";
            }
        }
    }
}
