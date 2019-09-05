using EFFC.Extends.JWT;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Module.Business.Logic;
using EFFC.Frame.Net.Module.Extend.EWRA.DataCollections;
using EFFC.Frame.Net.Module.Extend.EWRA.Parameters;
using EFFC.Frame.Net.Module.Web.Logic;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EWRA.Logic
{
    /// <summary>
    /// 验证用的logic
    /// </summary>
    public partial class AuthorizationLogic : WebBaseLogic<EWRAParameter, EWRAData>
    {
        JWTHelper jwt = null;
        Dictionary<string, string> claim_save_parameters = new Dictionary<string, string>();
        /// <summary>
        /// RSA公共秘钥存放目录，如果没有则写入内存中
        /// </summary>
        public virtual string PublicKeySavePath
        {
            get { return ""; }
        }
        /// <summary>
        /// RSA私有秘钥存放目录，如果没有则写入内存中
        /// </summary>
        public virtual string PrivateKeySavePath
        {
            get { return ""; }
        }
        /// <summary>
        /// 超时设置
        /// </summary>
        public virtual TimeSpan Expire
        {
            get { return TimeSpan.FromMinutes(20); }
        }
        /// <summary>
        /// 授权者
        /// </summary>
        public virtual string Issuer
        {
            get { return "Issuer_EWRA"; }
        }
        /// <summary>
        /// 接受者
        /// </summary>
        public virtual string Audience
        {
            get; private set;
        }
        /// <summary>
        /// 用于往token中存入参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        protected void SetClaimSaveParameter(string key, string value)
        {
            if (claim_save_parameters.ContainsKey(key))
            {
                claim_save_parameters[key] = value;
            }
            else
            {
                claim_save_parameters.Add(key, value);
            }
        }
        protected override void DoProcess(EWRAParameter p, EWRAData d)
        {
            if (jwt == null)
                jwt = new JWTHelper(CallContext_Parameter.ServerRootPath, PublicKeySavePath, PrivateKeySavePath, Expire, Issuer, Audience);

            if (p.__AuthMethod.ToLower() == "validauth")
            {
                var msg = "";
                p.IsAuth = IsValid(p.AuthorizedToken, ref msg);
                p.__Auth_ValidMsg = msg;

                if (p.IsAuth)
                {
                    if (p.AuthorizedToken != "")
                    {
                        //获取payload信息
                        p.AuthorizedTokenPayLoad = GetPayLoadInfoFromToken(p.AuthorizedToken);
                        //执行二级扩展验证
                        p.IsAuth = IsValid_Level2(p.AuthorizedToken, p.AuthorizedTokenPayLoad, ref msg);
                    }
                    p.__Auth_ValidMsg = string.IsNullOrEmpty(msg) ? p.__Auth_ValidMsg : msg;
                }

            }
            else
            {
                if (p.MethodName == "post")
                {
                    var requestarr = p.RequestRoute.Split('/').Where(sp => sp != "").ToArray();
                    var id = requestarr.Length > 1 ? requestarr[1] : "";
                    d.Result = CreateToken(id);
                    if (d.Result == null || ComFunc.nvl(d.Result) == "")
                    {
                        d.StatusCode = Constants.RestStatusCode.UNAUTHORIZED;
                    }
                    else
                    {
                        d.StatusCode = Constants.RestStatusCode.OK;
                    }
                }
                else
                {
                    d.StatusCode = Constants.RestStatusCode.FORBIDDEN;
                    d.Error = "该请求不支持";
                }
            }
        }
        protected virtual object CreateToken(string id)
        {
            if (!DoLogin(id))
            {
                return "";
            }
            return jwt.CreateToken(id, claim_save_parameters);
        }
        /// <summary>
        /// 执行登录验证，请子类实现该方法，该方法被CreateToken调用
        /// </summary>
        /// <param name="id"></param>
        /// <param name="saveparameter">待传出的要保存的参数</param>
        /// <returns></returns>
        protected virtual bool DoLogin(string id)
        {
            return true;
        }


        /// <summary>
        /// 校验用的方法
        /// </summary>
        /// <param name="token"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected virtual bool IsValid(string token, ref string msg)
        {
            return jwt.IsValid(token, ref msg);
        }
        /// <summary>
        /// 从token中获取加载的参数信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected virtual FrameDLRObject GetPayLoadInfoFromToken(string token)
        {
            return jwt.GetPayLoadInfoFromToken(token);

        }
        /// <summary>
        /// token的第二级验证，
        /// 通过第一次验证（基本算法验证）完成后，如果需要进行其他扩展性的验证可以在此处理
        /// 二级验证只有在一级验证返回true的时候才执行
        /// </summary>
        /// <param name="token"></param>
        /// <param name="payloadinfo">一级验证完成后解析出来的参数信息</param>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected virtual bool IsValid_Level2(string token, FrameDLRObject payloadinfo, ref string msg)
        {
            var expiretime = (DateTime)payloadinfo.GetValue("expire_time");
            var id = ComFunc.nvl(payloadinfo.GetValue("id"));
            var key = $"EWRA_auth_token_{id}";
            var cachetoken = ComFunc.nvl(GlobalCommon.ApplicationCache.Get(key));
            if (cachetoken != "")
            {
                if (cachetoken != token)
                {
                    var handler = new JwtSecurityTokenHandler();
                    var result = handler.ReadJwtToken(cachetoken);
                    //如果本次超时时间比缓存中的大，则刷新缓存，并返回true，否则表明本次的token是过期的，返回false
                    if (expiretime > result.ValidTo)
                    {
                        GlobalCommon.ApplicationCache.Set(key, token, expiretime);
                        return true;
                    }
                    else
                    {
                        msg = "TimeOut:Token已过期";
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                GlobalCommon.ApplicationCache.Set(key, token, expiretime);
                return true;
            }
        }
    }
}
