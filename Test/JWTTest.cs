using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace Test
{
    public class JWTTest
    {
        static SigningCredentials Credentials = null;
        static SigningCredentials PublicCredentials = null;
        public static void Test()
        {
            Console.OutputEncoding = Encoding.UTF8;
            var key = new RsaSecurityKey(GenerateAndSaveKey($"D:\\jwt\\"));
            Credentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256Signature);
            var token = CreateToken("ych", "123456", "admin", DateTime.Now.AddMinutes(0), "u0013512");
            ValidToken(token);
            Console.Read();
        }
        public static void GenerateHMAC256Key()
        {
            HMACSHA256 h = new HMACSHA256();
           
            
        }
        /// <summary>
        /// 生成并保存 RSA 公钥与私钥
        /// </summary>
        /// <param name="filePath">存放密钥的文件夹路径</param>
        /// <returns></returns>
        public static RSAParameters GenerateAndSaveKey(string path)
        {
            RSAParameters publicKeys, privateKeys;
            RSA.Create();
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                try
                {
                    privateKeys = rsa.ExportParameters(true);
                    publicKeys = rsa.ExportParameters(false);


                    //dynamic dlr = FrameDLRObject.CreateInstance(privateKeys, EFFC.Frame.Net.Base.Constants.FrameDLRFlags.SensitiveCase);
                    //dlr.D_str = ComFunc.Base64Code(ComFunc.ByteToString(dlr.D, Encoding.Unicode), Encoding.Unicode);
                    Console.WriteLine(JsonConvert.SerializeObject(privateKeys));
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }

                var key = new RsaSecurityKey(privateKeys);
                Credentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256Signature);
                key = new RsaSecurityKey(publicKeys);
                PublicCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256Signature);
                //if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                //File.WriteAllText(Path.Combine(path, "key.json"), FrameDLRObject.CreateInstance(privateKeys, EFFC.Frame.Net.Base.Constants.FrameDLRFlags.SensitiveCase).tojsonstring(Encoding.Unicode));
                //File.WriteAllText(Path.Combine(path, "key.public.json"), FrameDLRObject.CreateInstance(publicKeys, EFFC.Frame.Net.Base.Constants.FrameDLRFlags.SensitiveCase).tojsonstring(Encoding.Unicode));
            }
            return privateKeys;
        }

        private static string CreateToken(string userid,string userpass,string roleid, DateTime expire, string audience)
        {
            var handler = new JwtSecurityTokenHandler();
            string jti = audience + userid + ComFunc.ToTimestamp(expire);
            jti = ComFunc.getMD5_String(jti); // Jwt 的一个参数，用来标识 Token
            var claims = new[]
            {
                new Claim(ClaimTypes.Role, roleid), // 添加角色信息
                new Claim(ClaimTypes.NameIdentifier, userid), // 用户 Id ClaimValueTypes.Integer32),
                new Claim("jti",jti,ClaimValueTypes.String) // jti，用来标识 token
            };
            ClaimsIdentity identity = new ClaimsIdentity(new GenericIdentity(userid, "TokenAuth"), claims);
            var token = handler.CreateEncodedJwt(new SecurityTokenDescriptor
            {
                Issuer = "effc", // 指定 Token 签发者，也就是这个签发服务器的名称
                Audience = audience, // 指定 Token 接收者
                SigningCredentials = Credentials,
                Subject = identity,
                Expires = expire
            });
            return token;
        }

        private static bool ValidToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            SecurityToken re = null;
            try
            {
                var result = handler.ValidateToken(token, new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidAudience = "u0013511",

                    ValidateIssuer = true,
                    ValidIssuer = "effc",

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = PublicCredentials.Key,

                    ValidateLifetime = true,

                    ClockSkew = TimeSpan.Zero
                }, out re);

                if(result == null)
                {
                    return false;
                }
                else
                {
                    return result.Identity.IsAuthenticated;
                }
            }
            catch (SecurityTokenValidationException stve)
            {
                Console.WriteLine(stve.Message);
                return false;
            }
            catch (ArgumentException ae)
            {
                Console.WriteLine(ae.Message);
                return false;
            }

        }

        


    }
}
