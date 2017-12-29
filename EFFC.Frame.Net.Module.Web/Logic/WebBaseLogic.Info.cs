using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Module.Business.Logic;
using EFFC.Frame.Net.Module.Web.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Web.Logic
{
    /// <summary>
    /// WebBaseLogic.Info
    /// </summary>
    /// <typeparam name="PType"></typeparam>
    /// <typeparam name="DType"></typeparam>
    public partial class WebBaseLogic<PType,DType>
    {
        private ClientInfoProperty _ci;
        private ServerInfoProperty _si;
        private EFFCFrameInfoProperty _fi;

        /// <summary>
        /// 客户端信息集
        /// </summary>
        public virtual ClientInfoProperty ClientInfo
        {
            get
            {
                if (_ci == null)
                {
                    _ci = new ClientInfoProperty(this);
                }
                return _ci;
            }
        }
        /// <summary>
        /// 服务器端信息集
        /// </summary>
        public virtual ServerInfoProperty ServerInfo
        {
            get
            {
                if (_si == null)
                {
                    _si = new ServerInfoProperty(this);
                }
                return _si;
            }
        }
        /// <summary>
        /// EFFC框架信息
        /// </summary>
        public virtual EFFCFrameInfoProperty EFFCFrameInfo
        {
            get
            {
                if (_fi == null)
                {
                    _fi = new EFFCFrameInfoProperty(this);
                }
                return _fi;
            }
        }
        public class ServerInfoProperty
        {
            WebBaseLogic<PType, DType> _logic;

            string _ip;
            string _servername;
            public ServerInfoProperty(WebBaseLogic<PType, DType> logic)
            {
                _logic = logic;
                _ip = ComFunc.nvl(_logic.CallContext_Parameter[DomainKey.APPLICATION_ENVIRONMENT, "serverinfo_ip"]);
                _servername = ComFunc.nvl(_logic.CallContext_Parameter[DomainKey.APPLICATION_ENVIRONMENT, "server_servername"]);
            }
            /// <summary>
            /// IP
            /// </summary>
            public string IP { get { return _ip; } }
            /// <summary>
            /// server的机器名称
            /// </summary>
            public string ServerName { get { return _servername; } }

            /// <summary>
            /// Web服务器的物理路径
            /// </summary>
            public string ServerRootPath
            {
                get
                {
                    return ComFunc.nvl(_logic.CallContext_Parameter[DomainKey.APPLICATION_ENVIRONMENT, "ServerRootPath"]);
                }
            }
            /// <summary>
            /// Web服务器的站点路径
            /// </summary>
            public string ServerRootPath_URL
            {
                get
                {
                    return ComFunc.nvl(_logic.CallContext_Parameter[DomainKey.APPLICATION_ENVIRONMENT, "ServerRootPath_URL"]);
                }
            }
            /// <summary>
            /// 站点Host的url
            /// </summary>
            public string SiteHostUrl
            {
                get
                {
                    return ComFunc.nvl(_logic.CallContext_Parameter[DomainKey.APPLICATION_ENVIRONMENT, "WebPath"]);
                }
            }

            /// <summary>
            /// 获取站点的域名
            /// </summary>
            public string Domain
            {
                get
                {
                    return ComFunc.nvl(_logic.CallContext_Parameter[DomainKey.APPLICATION_ENVIRONMENT, "Domain"]);
                }
            }
        }

        public class ClientInfoProperty
        {
            string _ip;
            string _userhostname;
            string _browserversion;
            string _platform;
            WebBaseLogic<PType, DType> _logic;
            public ClientInfoProperty(WebBaseLogic<PType, DType> logic)
            {
                _logic = logic;
            }
            /// <summary>
            /// IP
            /// </summary>
            public string IP
            {
                get
                {
                    return ComFunc.nvl(_logic.CallContext_Parameter[DomainKey.APPLICATION_ENVIRONMENT, "clientinfo_ip"]);
                }
            }
            /// <summary>
            /// Client端机器名称
            /// </summary>
            public string UserHostName { get { return ComFunc.nvl(_logic.CallContext_Parameter[DomainKey.APPLICATION_ENVIRONMENT, "clientinfo_userhostname"]); } }
            /// <summary>
            /// Client端浏览器版本号
            /// </summary>
            public string BrowserVersion { get { return ComFunc.nvl(_logic.CallContext_Parameter[DomainKey.APPLICATION_ENVIRONMENT, "clientinfo_browserversion"]); } }
            /// <summary>
            /// Client端操作平台名称
            /// </summary>
            public string Platform { get { return ComFunc.nvl(_logic.CallContext_Parameter[DomainKey.APPLICATION_ENVIRONMENT, "clientinfo_platform"]); } }
        }

        public class EFFCFrameInfoProperty
        {
            WebBaseLogic<PType, DType> _logic;
            public EFFCFrameInfoProperty(WebBaseLogic<PType, DType> logic)
            {
                _logic = logic;
            }
            /// <summary>
            /// EFFC.Base程式版本号
            /// </summary>
            public string Base_Version { get { return ComFunc.nvl(_logic.CallContext_Parameter[DomainKey.APPLICATION_ENVIRONMENT, "effcinfo_base_version"]); } }
            /// <summary>
            /// EFFC.Base产品版本号
            /// </summary>
            public string Base_Product_Version { get { return ComFunc.nvl(_logic.CallContext_Parameter[DomainKey.APPLICATION_ENVIRONMENT, "effcinfo_base_product_version"]); } }
        }
    }
}
