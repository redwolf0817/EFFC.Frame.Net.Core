using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Global;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace EFFC.Extends.JWT
{
    /// <summary>
    /// JWT的呼叫API
    /// </summary>
    public sealed class JWTHelper
    {
        string privateKey = "";
        string publicKey = "";
        RsaSecurityKey privateSK = null;
        RsaSecurityKey publicSK = null;
        /// <summary>
        /// JWTHelper初始化
        /// </summary>
        /// <param name="server_root_path">key所在的根目录</param>
        /// <param name="public_key_save_path">公钥的路径，根目录用~表示</param>
        /// <param name="private_key_save_path">私钥的路径，根目录用~表示</param>
        /// <param name="expire">超时设定，默认20分钟</param>
        /// <param name="issuer">授权者标记</param>
        /// <param name="audience">接受者标记</param>
        public JWTHelper(string server_root_path,string public_key_save_path,string private_key_save_path, TimeSpan expire, string issuer= "Issuer_effc", string audience= "Audience_98771")
        {
            ServerRootPath = server_root_path;
            PublicKeySavePath = public_key_save_path;
            PrivateKeySavePath = private_key_save_path;
            Expire = expire;
            Issuer = issuer;
            Audience = audience;
        }
        /// <summary>
        /// RSA公共秘钥存放目录，如果没有则写入内存中
        /// </summary>
        public string PublicKeySavePath
        {
            get; private set;
        }
        /// <summary>
        /// RSA私有秘钥存放目录，如果没有则写入内存中
        /// </summary>
        public string PrivateKeySavePath
        {
            get; private set;
        }
        /// <summary>
        /// 授权者
        /// </summary>
        public string Issuer
        {
            get; private set;
        }
        /// <summary>
        /// 接受者
        /// </summary>
        public string Audience
        {
            get; private set;
        }
        /// <summary>
        /// 超时的时长
        /// </summary>
        public TimeSpan Expire
        {
            get;set;
        }
        /// <summary>
        /// 应用服务所在的物理路径
        /// </summary>
        public string ServerRootPath
        {
            get;protected set;
        }
        /// <summary>
        /// 根据参数创建JWT Token
        /// </summary>
        /// <param name="id"></param>
        /// <param name="claimparameters"></param>
        /// <returns></returns>
        public string CreateToken(string id, object claimparameters = null)
        {
            if (GetRSAPrivateKey() == null)
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
            if (claimparameters != null)
            {
                FrameDLRObject obj = FrameDLRObject.CreateInstance(claimparameters, FrameDLRFlags.SensitiveCase);
                foreach (var item in obj.Items)
                {
                    claims.Add(new Claim(item.Key, ComFunc.nvl(item.Value), ClaimValueTypes.String));
                }
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
        /// 校验用的方法
        /// </summary>
        /// <param name="token"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool IsValid(string token, ref string msg)
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
                GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $"授权访问验证失败,原因：{stve.Message}");
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
                GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $"授权访问验证失败，原因：{ae.Message}");
                msg = "Invalid:授权无效";
                return false;
            }
        }
        /// <summary>
        /// 从token中获取加载的参数信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public FrameDLRObject GetPayLoadInfoFromToken(string token)
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

                        privateKey = FrameDLRObject.CreateInstance(privateKeys, FrameDLRFlags.SensitiveCase).tojsonstring(Encoding.Unicode);
                        publicKey = FrameDLRObject.CreateInstance(publicKeys, FrameDLRFlags.SensitiveCase).tojsonstring(Encoding.Unicode);

                        if (!string.IsNullOrEmpty(PrivateKeySavePath))
                        {
                            GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, $"RestAPI Token私有秘钥存放位置为{PrivateKeySavePath}");
                            var physicalPath = PrivateKeySavePath.Replace("~", ServerRootPath);
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
                            var physicalPath = PublicKeySavePath.Replace("~", ServerRootPath);
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
            var physicalPath = PrivateKeySavePath.Replace("~", ServerRootPath);
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
                var physicalPath = PublicKeySavePath.Replace("~", ServerRootPath);
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
    }
}
