using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Module.Web.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EWRA.Parameters
{
    /// <summary>
    /// EWRA专用参数集合
    /// </summary>
    public class EWRAParameter : RestParameter
    {
        public EWRAParameter()
        {
            __AuthMethod = "";
            __Auth_ValidMsg = "";
        }
        /// <summary>
        /// 当前API的主版本号，格式：v{0.0}
        /// </summary>
        public string CurrentAPIVersion
        {
            get
            {
                return ComFunc.nvl(this["__CURRENT_API_VERSION__"]);
            }
            set
            {
                this["__CURRENT_API_VERSION__"] = value;
            }
        }
        /// <summary>
        /// 判断当前请求是否已经通过验证
        /// </summary>
        public bool IsAuth
        {
            get
            {
                return (bool)this["__IS_AUTH__"];
            }
            set
            {
                this["__IS_AUTH__"] = value;
            }
        }
        /// <summary>
        /// 校验用的Token
        /// </summary>
        public string AuthorizedToken
        {
            get
            {
                return ComFunc.nvl(this["__AUTHORIZED_TOKEN__"]);
            }
            set
            {
                this["__AUTHORIZED_TOKEN__"] = value;
            }
        }
        /// <summary>
        /// Token中附加的业务参数信息
        /// </summary>
        public FrameDLRObject AuthorizedTokenPayLoad
        {
            get
            {
                return (FrameDLRObject)this["__AUTHORIZED_TOKEN_PAYLOAD__"];
            }
            set
            {
                this["__AUTHORIZED_TOKEN_PAYLOAD__"] = value;
            }
        }
        /// <summary>
        /// 执行校验的方法名称
        /// </summary>
        internal string __AuthMethod
        {
            get;
            set;
        }
        /// <summary>
        /// 校验后的结果信息
        /// </summary>
        internal string __Auth_ValidMsg
        {
            get;
            set;
        }
    }
}
