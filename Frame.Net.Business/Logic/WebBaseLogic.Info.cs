using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Interfaces;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.ResouceManage;
using EFFC.Frame.Net.Base.ResouceManage.DB;
using EFFC.Frame.Net.Base.Token;
using EFFC.Frame.Net.Base.Unit;
using EFFC.Frame.Net.Data;
using EFFC.Frame.Net.Data.LogicData;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.UnitData;
using EFFC.Frame.Net.Data.WebData;
using EFFC.Frame.Net.Base.Constants;

namespace EFFC.Frame.Net.Business.Logic
{
    public partial class WebBaseLogic<P, D>
    {
        private ClientInfoProperty _ci;
        private ServerInfoProperty _si;

        /// <summary>
        /// 客户端信息集
        /// </summary>
        public ClientInfoProperty ClientInfo
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
        public ServerInfoProperty ServerInfo
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
        public class ServerInfoProperty
        {
            WebBaseLogic<P, D> _logic;

            string _ip;
            string _servername;
            public ServerInfoProperty(WebBaseLogic<P, D> logic)
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
                get { return _logic.CallContext_Parameter.ServerRootPath; }
            }
            /// <summary>
            /// Web服务器的站点路径
            /// </summary>
            public string ServerRootPath_URL
            {
                get { return _logic.CallContext_Parameter.ServerRootPath_URL; }
            }
            /// <summary>
            /// 站点Host的url
            /// </summary>
            public string SiteHostUrl
            {
                get { return _logic.CallContext_Parameter.SiteUrl; }
            }

            /// <summary>
            /// 获取站点的域名
            /// </summary>
            public string Domain
            {
                get
                {
                    return _logic.CallContext_Parameter.Domain;
                }
            }
        }

        public class ClientInfoProperty
        {
            string _ip;
            string _userhostname;
            string _browserversion;
            string _platform;
            WebBaseLogic<P, D> _logic;
            public ClientInfoProperty(WebBaseLogic<P, D> logic)
            {
                _logic = logic;
            }
            /// <summary>
            /// IP
            /// </summary>
            public string IP { get { return ComFunc.nvl(_logic.CallContext_Parameter[DomainKey.APPLICATION_ENVIRONMENT, "clientinfo_ip"]); } }
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
    }
}
