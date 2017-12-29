using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Module.Web.Datas;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Web.Parameters
{
    /// <summary>
    /// Web处理用到的参数集
    /// </summary>
    public class WebParameter : ParameterStd
    {
        List<string> _forbiddenname = null;
        /// <summary>
        /// Web模块的参数结构集
        /// </summary>
        public WebParameter()
        {

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
        //public enum ParameterDomain
        //{
        //    RequestQueryString,
        //    Session,
        //    Config
        //}
        /// <summary>
        /// 禁用的Key名
        /// </summary>
        public List<string> ForbiddenName
        {
            get
            {
                if (_forbiddenname == null)
                {
                    _forbiddenname = new List<string>();
                    _forbiddenname.Add("LoginInfo");
                }

                return _forbiddenname;
            }
        }
        /// <summary>
        /// 登陆的信息数据
        /// </summary>
        public FrameDLRObject LoginInfo
        {
            get
            {
                return (FrameDLRObject)this[DomainKey.SESSION, ParameterKey.LOGIN_INFO];
            }
            set
            {
                this[DomainKey.SESSION, ParameterKey.LOGIN_INFO] = value;
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

        /// <summary>
        /// client端请求的资源路径
        /// </summary>
        public string RequestResourcePath
        {
            get
            {
                return ComFunc.nvl(this["RequestSourcePath"]);
            }
            set
            {
                SetValue("RequestSourcePath", value);
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
        /// client端请求的资源名称
        /// </summary>
        public string RequestResourceName
        {
            get
            {
                return ComFunc.nvl(this["RequestResourceName"]);
            }
            set
            {
                SetValue("RequestResourceName", value);
            }
        }
        /// <summary>
        /// 资源请求数组
        /// </summary>
        public string[] RequestResources
        {
            get;
            set;
        }
        /// <summary>
        /// 页面请求动作
        /// </summary>
        public string Action
        {
            get
            {
                return ComFunc.nvl(this[ParameterKey.ACTION]);
            }
            set
            {
                SetValue(ParameterKey.ACTION, value);
            }
        }

        /// <summary>
        /// 主流程是否可以继续往下走
        /// </summary>
        public bool CanContinue
        {
            get
            {
                return GetValue<bool>("CanContinue");
            }
            set
            {
                SetValue("CanContinue", value);
            }
        }
        /// <summary>
        /// 是否需要清理Session
        /// </summary>
        public bool IsNeedSessionAbandon
        {
            get
            {
                return GetValue<bool>("IsNeedSessionAbandon");
            }
            set
            {
                SetValue("IsNeedSessionAbandon", value);
            }
        }
        /// <summary>
        /// 登陆请求的路径
        /// </summary>
        public string LoginPath
        {
            get { return ComFunc.nvl(this[DomainKey.CONFIG, "LoginPath"]); }
        }
        /// <summary>
        /// 主页面请求的路径
        /// </summary>
        public string IndexPath
        {
            get { return ComFunc.nvl(this[DomainKey.CONFIG, "IndexPath"]); }
        }
        /// <summary>
        /// 出错页面请求路径
        /// </summary>
        public string ErrorPath
        {
            get { return ComFunc.nvl(this[DomainKey.CONFIG, "ErrorPath"]); }
        }

        public string SessionID
        {
            get { return ComFunc.nvl(this[DomainKey.SESSION, "SessionID"]); }
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
        /// 是否为websocket调用
        /// </summary>
        public bool IsWebSocket
        {
            get
            {
                if (this[DomainKey.APPLICATION_ENVIRONMENT, "iswebsocket"] == null)
                {
                    return false;
                }
                else
                {
                    return (bool)this[DomainKey.APPLICATION_ENVIRONMENT, "iswebsocket"];
                }
            }
            set
            {
                this[DomainKey.APPLICATION_ENVIRONMENT, "iswebsocket"] = value;
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
        /// Http请求的Method
        /// </summary>
        public string RequestMethod
        {
            get
            {
                return ComFunc.nvl(this[ParameterKey.REQUEST_METHOD]);
            }
            set
            {
                SetValue(ParameterKey.REQUEST_METHOD, value);
            }
        }
        /// <summary>
        /// 复制一个WebParameter对象
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return this.DeepCopy<WebParameter>();
        }
    }
}
