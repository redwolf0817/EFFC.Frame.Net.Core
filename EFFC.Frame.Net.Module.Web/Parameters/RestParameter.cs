using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Parameter;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Web.Parameters
{
    /// <summary>
    /// Rest请求的专用参数集
    /// </summary>
    public class RestParameter:ParameterStd
    {
        /// <summary>
        /// API请求的入口节点
        /// </summary>
        public string RequestRoute
        {
            get
            {
                return ComFunc.nvl(this["__REQUEST_ROUTE__"]);
            }
            set
            {
                this["__REQUEST_ROUTE__"] = value;
            }
        }

        /// <summary>
        /// 请求的Method的名称
        /// </summary>
        public string MethodName
        {
            get
            {
                return ComFunc.nvl(this["__METHOD_NAME__"]);
            }
            set
            {
                this["__METHOD_NAME__"] = value;
            }
        }
        /// <summary>
        /// Rest URL请求的资源解析数组
        /// </summary>
        public string[] RestResourcesArray
        {
            get
            {
                if(this["__REQUEST_RESOURCES_ARRAY__"] == null)
                {
                    return new string[0];
                }
                else
                {
                    return (string[])this["__REQUEST_RESOURCES_ARRAY__"];
                }
            }
            set
            {
                this["__REQUEST_LOGIC__"] = value;
            }
        }

        /// <summary>
        /// 获取当前HttpContext
        /// </summary>
        public HttpContext CurrentHttpContext
        {
            get
            {
                if (GetValue("__HTTP_CONTEXT__") != null)
                    return (HttpContext)GetValue("__HTTP_CONTEXT__");
                else
                    return null;
            }
            set
            {
                SetValue("__HTTP_CONTEXT__", value);
            }
        }
        /// <summary>
        /// 获取上传的文件对象，如果没有则返回null
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public FrameUploadFile UploadFile(string name)
        {
            object obj = this[DomainKey.UPDATE_FILE, name];

            if (obj != null && obj is FrameUploadFile)
            {
                return (FrameUploadFile)obj;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 请求的内容
        /// </summary>
        public string RequestContent
        {
            get
            {
                return ComFunc.nvl(this[ParameterKey.REQUEST_CONTENT]);
            }
            set
            {
                SetValue(ParameterKey.REQUEST_CONTENT, value);
            }
        }

        /// <summary>
        /// DBConnectionString
        /// </summary>
        public string DBConnectionString
        {
            get
            {
                return ComFunc.nvl(this[DomainKey.CONFIG, ParameterKey.DBCONNECT_STRING]);
            }
            set
            {
                this[DomainKey.CONFIG, ParameterKey.DBCONNECT_STRING] = value;
            }
        }

        /// <summary>
        /// Server的RootPath,物理路径
        /// </summary>
        public string ServerRootPath
        {
            get
            {
                return ComFunc.nvl(this[DomainKey.APPLICATION_ENVIRONMENT, "ServerRootPath"]);
            }
        }
        /// <summary>
        /// Server的RootPath,站点路径
        /// </summary>
        public string ServerRootPath_URL
        {
            get
            {
                return ComFunc.nvl(this[DomainKey.APPLICATION_ENVIRONMENT, "ServerRootPath_URL"]);
            }
        }
        /// <summary>
        /// Server的RootPath
        /// </summary>
        public string ServerPhysicPath
        {
            get
            {
                return ComFunc.nvl(this[DomainKey.APPLICATION_ENVIRONMENT, "ServerPhysicPath"]);
            }
        }
        /// <summary>
        /// 站点的请求url
        /// </summary>
        public string SiteUrl
        {
            get
            {
                return ComFunc.nvl(this[DomainKey.APPLICATION_ENVIRONMENT, "WebPath"]);
            }
        }
        /// <summary>
        /// 站点域名
        /// </summary>
        public string Domain
        {
            get
            {
                return ComFunc.nvl(this[DomainKey.APPLICATION_ENVIRONMENT, "Domain"]);
            }
        }

        /// <summary>
        /// 获取当前请求的Uri
        /// </summary>
        public Uri RequestUri
        {
            get
            {
                return (Uri)(this["RequestUri"]);
            }
            set
            {
                SetValue("RequestUri", value);
            }
        }

        public override object Clone()
        {
            return base.Clone();
        }
    }
}
