using System;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Module.Extend.WeChat;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using EFFC.Frame.Net.Global;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Collections.Generic;
using System.Security.Principal;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using EFFC.Extends.JWT;

namespace EFFC.Frame.Net.Module.Extend.WeixinWeb.Logic
{
    public abstract partial class WeixinGoLogic
    {
        JWTHelper _jwt = null;
        /// <summary>
        /// JWT校验API
        /// </summary>
        protected virtual JWTHelper JWT
        {
            get
            {
                if (_jwt == null) _jwt = new JWTHelper(ServerInfo.ServerRootPath,"~/token/public.json", "~/token/private.json",TimeSpan.FromMinutes(20));
                return _jwt;
            }
        }
    }
}
