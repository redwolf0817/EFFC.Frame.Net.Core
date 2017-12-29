using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.WebData;
using Frame.Net.Web.Proxy;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Frame.Net.Web.Core
{
    public abstract class EFFCWebMiddleWare
    {
        /// <summary>
        /// EFFC框架中间件基类
        /// </summary>
        /// <param name="next"></param>
        public EFFCWebMiddleWare(RequestDelegate next)
        {
            // This is an HTTP Handler, so no need to store next
        }

        public async Task Invoke(HttpContext context)
        {
            var location = new Uri($"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}");
            var requestpath = Path.GetFileName(context.Request.Path);
            var ext = Path.GetExtension(context.Request.Path).Replace(".","").ToLower();
            var arr = requestpath.Split('.');

            if(string.IsNullOrEmpty(ext))
            {
                ext = "go";
            }
            else
            {
                //非静态资源请求
                if(!ReservedStaticFileExt.Contains(ext))
                {
                    ext = "go";
                }
            }

            DoInvoke(context, ext);
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
                if(_static_file_ext.Count <= 0)
                {
                    _static_file_ext.AddRange( new string[]
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
                }
                return _static_file_ext;
            }
        }
    }
}
