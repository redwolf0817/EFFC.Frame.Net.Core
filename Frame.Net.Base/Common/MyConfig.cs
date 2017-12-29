using System;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace EFFC.Frame.Net.Base.Common
{
    /// <summary>
    /// Frame config
    /// </summary>
    public class MyConfig
    {

        private static IConfigurationRoot _config;
        private static string _filepath = "";
        /// <summary>
        /// 加密类型
        /// </summary>
        public enum EncryptionType
        {
            /// <summary>
            /// Base64
            /// </summary>
            Base64,
            /// <summary>
            /// AES256
            /// </summary>
            AES256
        }
        /// <summary>
        /// 系统配置档对象
        /// </summary>
        public static IConfigurationRoot Configuration
        {
            get
            {
                if (_config == null)
                {
                    var builder = new ConfigurationBuilder()
                                .SetBasePath(ConfigFilePath)
                                .AddJsonFile("appsettings.json", optional: true);

                    _config = builder.Build();
                }

                return _config;
            }
        }
        /// <summary>
        /// 配置档文件目录
        /// </summary>
        public static string ConfigFilePath
        {
            get
            {
                if (string.IsNullOrEmpty(_filepath))
                {
                    _filepath = Path.Combine(Directory.GetCurrentDirectory());
                }

                return _filepath;
            }
            set
            {
                _filepath = value;
            }
        }


        #region GetConfiguration
        /// <summary>
        /// 获取指定section下的指定参数
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfiguration(string section, string key)
        {
            return Configuration.GetSection(section)[key];
        }
        /// <summary>
        /// 獲取connectionStrings中的配置數據
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConnections(string key)
        {
            return Configuration.GetSection("connections")[key];
        }
        /// <summary>
        /// 取得appSettings裏的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfiguration(string key)
        {
            return Configuration.GetSection("appsettings")[key];
        }
        /// <summary>
        /// 取得appSettings裏的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfigurationByEncrypt(string key)
        {
            return GetConfigurationByEncrypt(key, EncryptionType.Base64);
        }
        /// <summary>
        /// 取得appSettings裏的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetConfigurationByEncrypt(string key, EncryptionType type)
        {
            string sgf = Configuration.GetSection("appsettings")[key];
            string oldpass = DeCode(sgf, type);

            return oldpass;
        }
        /// <summary>
        /// 取得WebConnectionString裏的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetWebConnectionString(string key)
        {
            return Configuration.GetSection("connections")[key];
        }
        /// <summary>
        /// 取得Webconfig裏DBConnstring的值，Password和userid采用base64加密
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetWebDBConnStringByEncrypt(string key)
        {
            return GetWebDBConnStringByEncrypt(key, EncryptionType.Base64);
        }
        /// <summary>
        /// 取得Webconfig裏DBConnstring的值，Password和userid采用指定類型的方式加密
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetWebDBConnStringByEncrypt(string key, EncryptionType type)
        {
            string sgf = GetWebConnectionString(key);

            string spassword = "";
            string userid = "";

            string p = "(?<=password=)[^;]*(?=;)";
            Regex r = new Regex(p, RegexOptions.IgnoreCase);
            if (r.IsMatch(sgf))
            {
                spassword = r.Match(sgf).Value;
                var de = DeCode(spassword, type);
                sgf = sgf.Replace(spassword, de);
            }

            p = @"(?<=user\s+id=)[^;]*(?=;)";
            r = new Regex(p, RegexOptions.IgnoreCase);
            if (r.IsMatch(sgf))
            {
                userid = r.Match(sgf).Value;
                var de = DeCode(userid, type);
                sgf = sgf.Replace(userid, de);
            }
            return sgf;
        }

        private static string DeCode(string s, EncryptionType type)
        {
            switch (type)
            {
                case EncryptionType.AES256:
                    return ComFunc.AESDecrypt(s);
                default:
                    return ComFunc.Base64DeCode(s);
            }
        }
        /// <summary>
        /// 取得appSettings裏DBConnstring的值，Password和userid采用Base64加密
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetDBConnStringByEncrypt(string key)
        {
            return GetDBConnStringByEncrypt(key, EncryptionType.Base64);
        }
        /// <summary>
        /// 取得appSettings裏DBConnstring的值，Password和userid采用指定類型的方式加密
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetDBConnStringByEncrypt(string key, EncryptionType type)
        {
            string sgf = Configuration.GetSection("connections")[key];

            string spassword = "";
            string userid = "";

            string p = "(?<=password=)[^;]*(?=;)";
            Regex r = new Regex(p, RegexOptions.IgnoreCase);
            if (r.IsMatch(sgf))
            {
                spassword = r.Match(sgf).Value;
                var de = DeCode(spassword, type);
                sgf = sgf.Replace(spassword, de);
            }

            p = @"(?<=user\s+id=)[^;]*(?=;)";
            r = new Regex(p, RegexOptions.IgnoreCase);
            if (r.IsMatch(sgf))
            {
                userid = r.Match(sgf).Value;
                var de = DeCode(userid, type);
                sgf = sgf.Replace(userid, de);
            }
            return sgf;
        }
        #endregion

        #region GetConfigurationList
        /// <summary>
        /// 取得指定section裏的值列表
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetConfigurationList(string section)
        {
            Dictionary<string, object> k = new Dictionary<string, object>();                   //返回的鍵值對類型

            //取得AppSettings節
            var list = Configuration.GetSection(section).GetChildren();
            foreach (var item in list)
            {
                k.Add(item.Key, item.Value);
            }

            return k;

        }
        #endregion
        /// <summary>
        /// 取得appSettings裏的值列表
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string,object> GetConfigurationList()
        {
            Dictionary<string, object> k = new Dictionary<string,object>();                   //返回的鍵值對類型

            //取得AppSettings節
            var list = Configuration.GetSection("appsettings").GetChildren();
            foreach(var item in list)
            {
                k.Add(item.Key, item.Value);
            }

            return k;
        }

        #region SetConfiguration
        /**/
        /// <summary>
        /// 設置appSetting的值
        /// </summary>
        /// <param name="key">鍵</param>
        /// <param name="value">值</param>
        /// <param name="section">对应section</param>
        public static void SetConfiguration(string key, string value, string section)
        {
            

        }
        #endregion

        #region SetConfiguration
        /**/
        /// <summary>
        /// 設置appSetting的值
        /// </summary>
        /// <param name="key">鍵</param>
        /// <param name="value">值</param>
        public static void SetConfiguration(string key, string value)
        {
          

        }
        #endregion
        /// <summary>
        /// 设置web.config中appsetting的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="path"></param>
        public static void SetWebConfiguration(string key, string value, string path)
        {
           
        }
        /// <summary>
        /// 设置web.config中appsetting的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetWebConfiguration(string key, string value)
        {
            
        }
    }
}
