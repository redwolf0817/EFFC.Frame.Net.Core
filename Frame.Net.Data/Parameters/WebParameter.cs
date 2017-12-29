using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Interfaces;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Base.ResouceManage;
using EFFC.Frame.Net.Base.Token;
using EFFC.Frame.Net.Base.Constants;
using System.Web;

namespace EFFC.Frame.Net.Data.Parameters
{
    public class WebParameter:ParameterStd
    {
        List<string> _forbiddenname = null;
        public WebParameter()
        {  

        }
        /// <summary>
        /// 获取上传的文件对象，如果没有则返回null
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public HttpPostedFile UploadFile(string name)
        {
            object obj = this[DomainKey.UPDATE_FILE, name];

            if (obj != null && obj is HttpPostedFile)
            {
                return (HttpPostedFile)obj;
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
        public LoginUserData LoginInfo
        {
            get
            {
                return (LoginUserData)this[DomainKey.SESSION, "LoginInfo"];
            }
            set
            {
                this[DomainKey.SESSION, "LoginInfo"] = value;
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
            get { return ComFunc.nvl(this[DomainKey.CONFIG,"LoginPath"]); }
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
        /// 复制一个WebParameter对象
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return this.Clone<WebParameter>();
        }
    }
}
