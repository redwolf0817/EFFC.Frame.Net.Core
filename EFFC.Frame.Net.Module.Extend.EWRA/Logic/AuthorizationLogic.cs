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
        static string privateKey = "";
        static string publicKey = "";
        static RsaSecurityKey privateSK = null;
        static RsaSecurityKey publicSK = null;
        Dictionary<string, string> claim_save_parameters = new Dictionary<string, string>();
        /// <summary>
        /// RSA公共秘钥存放目录，如果没有则写入内存中
        /// </summary>
        public virtual string PublicKeySavePath
        {
            get
            {
                return "";
            }
        }
        /// <summary>
        /// RSA私有秘钥存放目录，如果没有则写入内存中
        /// </summary>
        public virtual string PrivateKeySavePath
        {
            get
            {
                return "";
            }
        }
        /// <summary>
        /// 授权者
        /// </summary>
        public virtual string Issuer
        {
            get
            {
                return "Issuer_EWRA";
            }
        }
        /// <summary>
        /// 接受者
        /// </summary>
        public virtual string Audience
        {
            get
            {
                return "Audience_98771";
            }
        }
        /// <summary>
        /// 超时的时长
        /// </summary>
        public virtual TimeSpan Expire
        {
            get
            {
                return TimeSpan.FromMinutes(20);
            }
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
        #region 独立方法

        #endregion
        protected override void DoProcess(EWRAParameter p, EWRAData d)
        {

            if (p.__AuthMethod.ToLower() == "validauth")
            {
                var msg = "";
                p.IsAuth = IsValid(p.AuthorizedToken, ref msg);
                p.__Auth_ValidMsg = msg;

                if (p.IsAuth)
                {
                    //获取payload信息
                    p.AuthorizedTokenPayLoad = GetPayLoadInfoFromToken(p.AuthorizedToken);
                    //执行二级扩展验证
                    p.IsAuth = IsValid_Level2(p.AuthorizedToken, p.AuthorizedTokenPayLoad, ref msg);
                    p.__Auth_ValidMsg = string.IsNullOrEmpty(msg) ? p.__Auth_ValidMsg : msg;
                }

            }
            else
            {
                if (p.MethodName == "post")
                {
                    var requestarr = p.RequestRoute.ToLower().Split('/').Where(sp => sp != "").ToArray();
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
            if(GetRSAPrivateKey() == null)
            {
                GenerateAndSaveKey();
            }
            var dtExpire = DateTime.Now.Add(Expire);
            var handler = new JwtSecurityTokenHandler();
            string jti = Audience + id + ComFunc.ToTimestamp(dtExpire);
            jti = ComFunc.getMD5_String(jti); // Jwt 的一个参数，用来标识 Token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, id), // 用户 Id ClaimValueTypes.Integer32),
                new Claim("jti",jti,ClaimValueTypes.String) // jti，用来标识 token
            };
            foreach (var item in claim_save_parameters)
            {
                claims.Add(new Claim(item.Key, ComFunc.nvl(item.Value), ClaimValueTypes.String));
            }
            ClaimsIdentity identity = new ClaimsIdentity(new GenericIdentity(id, "TokenAuth"), claims);
            var token = handler.CreateEncodedJwt(new SecurityTokenDescriptor
            {
                Issuer = Issuer, // 指定 Token 签发者，也就是这个签发服务器的名称
                Audience = Audience, // 指定 Token 接收者
                SigningCredentials = new SigningCredentials(privateSK, SecurityAlgorithms.RsaSha256Signature),
                Subject = identity,
                Expires = dtExpire
            });
            return token;
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
        /// 生成并保存 RSA 公钥与私钥
        /// </summary>
        /// <returns></returns>
        private void GenerateAndSaveKey()
        {
            if (privateKey == "")
            {
                RSA.Create();
                using (var rsa = new RSACryptoServiceProvider(2048))
                {
                    try
                    {
                        var privateKeys = rsa.ExportParameters(true);
                        var publicKeys = rsa.ExportParameters(false);

                        privateSK = new RsaSecurityKey(privateKeys);
                        publicSK = new RsaSecurityKey(publicKeys);

                        privateKey = FrameDLRObject.CreateInstance(privateKeys, Base.Constants.FrameDLRFlags.SensitiveCase).tojsonstring(Encoding.Unicode);
                        publicKey = FrameDLRObject.CreateInstance(publicKeys, Base.Constants.FrameDLRFlags.SensitiveCase).tojsonstring(Encoding.Unicode);

                        if (!string.IsNullOrEmpty(PrivateKeySavePath))
                        {
                            GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $"RestAPI Token私有秘钥存放位置为{PrivateKeySavePath}");
                            var physicalPath = PrivateKeySavePath.Replace("~", CallContext_Parameter.ServerRootPath);
                            var dirPath = Path.GetDirectoryName(physicalPath);
                            if (!Directory.Exists(dirPath))
                            {
                                Directory.CreateDirectory(dirPath);
                            }
                            File.WriteAllText(physicalPath, privateKey);

                        }
                        if (!string.IsNullOrEmpty(PublicKeySavePath))
                        {
                            GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $"RestAPI Token公有秘钥存放位置为{PublicKeySavePath}");
                            var physicalPath = PublicKeySavePath.Replace("~", CallContext_Parameter.ServerRootPath);
                            var dirPath = Path.GetDirectoryName(physicalPath);
                            if (!Directory.Exists(dirPath))
                            {
                                Directory.CreateDirectory(dirPath);
                            }
                            File.WriteAllText(physicalPath, publicKey);
                        }
                    }
                    finally
                    {
                        rsa.PersistKeyInCsp = false;
                    }
                }
            }
        }
        private RsaSecurityKey GetRSAPrivateKey()
        {
            var physicalPath = PrivateKeySavePath.Replace("~", CallContext_Parameter.ServerRootPath);
            if (File.Exists(physicalPath))
            {
                var content = File.ReadAllText(physicalPath);
                privateKey = content;
                RSAParameters rp = ((FrameDLRObject)FrameDLRObject.CreateInstance(content, FrameDLRFlags.SensitiveCase)).ToModel<RSAParameters>(Encoding.Unicode);
                privateSK = new RsaSecurityKey(rp);
            }
            else
            {
                GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $"Rest验证读取PrivateKey文件失败，原因是目录文件{PrivateKeySavePath}不存在，请给出正确的秘钥钥文件路径（请在验证的Logic中重载PrivateKeySavePath的get方法），没有密钥会导致token生成失败甚至出现异常");
            }

            if (string.IsNullOrEmpty(privateKey))
            {
                GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $"Rest创建token密钥不存在，没有密钥会导致创建失败甚至出现异常，如果本API服务提供生成Token的功能则请不要重载PublicKeySavePath和PrivateKeySavePath两个属性的get方法或在get时返回空串，如果不是则请提供正确的PrivateKeySavePath路径值");
            }

            return privateSK;
        }
        private RsaSecurityKey GetRSAPublicKey()
        {
            if (string.IsNullOrEmpty(publicKey) && !string.IsNullOrEmpty(PublicKeySavePath))
            {
                var physicalPath = PublicKeySavePath.Replace("~", CallContext_Parameter.ServerRootPath);
                if (File.Exists(physicalPath))
                {
                    var content = File.ReadAllText(physicalPath);
                    publicKey = content;
                    RSAParameters rp = ((FrameDLRObject)FrameDLRObject.CreateInstance(content, FrameDLRFlags.SensitiveCase)).ToModel<RSAParameters>(Encoding.Unicode);
                    publicSK = new RsaSecurityKey(rp);
                }
                else
                {
                    GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $"Rest验证读取PublicKey文件失败，原因是目录文件{PublicKeySavePath}不存在，请给出正确的公钥文件路径（请在验证的Logic中重载PublicKeySavePath的get方法），没有公钥会导致验证失败甚至出现异常");
                }
            }

            if (string.IsNullOrEmpty(publicKey))
            {
                GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $"Rest验证公钥不存在，没有公钥会导致验证失败甚至出现异常，如果本API服务提供生成Token的功能则请不要重载PublicKeySavePath和PrivateKeySavePath两个属性的get方法或在get时返回空串，如果不是则请提供正确的PublicKeySavePath路径值");
            }

            return publicSK;
        }
        /// <summary>
        /// 校验用的方法
        /// </summary>
        /// <param name="token"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected virtual bool IsValid(string token, ref string msg)
        {
            if (string.IsNullOrEmpty(token))
            {
                msg = "Invalid:缺少授权Token";
                return false;
            }

            var handler = new JwtSecurityTokenHandler();
            SecurityToken re = null;
            try
            {
                var result = handler.ValidateToken(token, new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidAudience = Audience,

                    ValidateIssuer = true,
                    ValidIssuer = Issuer,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SigningCredentials(GetRSAPublicKey(), SecurityAlgorithms.RsaSha256Signature).Key,

                    ValidateLifetime = true,

                    ClockSkew = TimeSpan.Zero
                }, out re);

                if (result == null)
                {
                    msg = "Invalid:授权无效";
                    return false;
                }
                else
                {
                    if (!result.Identity.IsAuthenticated)
                    {
                        msg = "Invalid:授权无效";
                    }

                    return result.Identity.IsAuthenticated;
                }
            }
            catch (SecurityTokenValidationException stve)
            {
                GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.INFO, $"授权访问验证失败，请求链接为{CallContext_Parameter.MethodName}:{CallContext_Parameter.RequestRoute},请求的IP为{this.ClientInfo.IP}，原因：{stve.Message}");
                if (stve.Message.StartsWith("IDX10223:") || stve.Message.IndexOf(" The token is expired") > 0)
                {
                    msg = "TimeOut:Token已过期";
                }
                else
                {
                    msg = "Invalid:授权无效";
                }
                return false;
            }
            catch (ArgumentException ae)
            {
                GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.INFO, $"授权访问验证失败，请求链接为{CallContext_Parameter.MethodName}:{CallContext_Parameter.RequestRoute},请求的IP为{this.ClientInfo.IP}，原因：{ae.Message}");
                msg = "Invalid:授权无效";
                return false;
            }
        }
        /// <summary>
        /// 从token中获取加载的参数信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected FrameDLRObject GetPayLoadInfoFromToken(string token)
        {
            var payloadobj = FrameDLRObject.CreateInstance();
            var handler = new JwtSecurityTokenHandler();
            var result = handler.ReadJwtToken(token);
            payloadobj.id = result.Claims.First(t => t.Type == "unique_name").Value;
            payloadobj.jti = result.Payload.Jti;
            payloadobj.validFrom = result.ValidFrom;
            payloadobj.expire_time = result.ValidTo;
            foreach (var item in result.Claims)
            {
                if ("unique_name,jti,nbf,exp,iat,iss,aud".Contains(item.Type)) continue;
                ((FrameDLRObject)payloadobj).SetValue(item.Type, item.Value);
            }

            return payloadobj;

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
