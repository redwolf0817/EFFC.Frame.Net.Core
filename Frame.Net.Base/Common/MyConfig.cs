using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using System.Reflection;
using System.Web;

namespace EFFC.Frame.Net.Base.Common
{
    /// <summary>
    /// Frame config
    /// </summary>
    public class MyConfig
    {
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
        /// 獲取connectionStrings中的配置數據
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConnections(string key)
        {
            string sgf = ConfigurationManager.ConnectionStrings[key].ToString();
            return sgf;
        }

        #region GetConfiguration
        /// <summary>
        /// 取得appSettings裏的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfiguration(string key)
        {
            string sgf = ConfigurationManager.AppSettings[key];
            return sgf;
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
            string sgf = ConfigurationManager.AppSettings[key];
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
            return ConfigurationManager.ConnectionStrings[key].ConnectionString;
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
            string sgf = ConfigurationManager.AppSettings[key];

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
        /// 取得appSettings裏的值列表
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetConfigurationList(string filePath)
        {
            Configuration configuration = null;                         //Configuration對象     
            Dictionary<string, object> k = null;                   //返回的鍵值對類型

            configuration = ConfigurationManager.OpenExeConfiguration(filePath);

            //取得AppSettings節

            foreach (KeyValueConfigurationElement key in configuration.AppSettings.Settings)
            {
                k.Add(key.Key, key.Value);
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

            foreach (var key in ConfigurationManager.AppSettings.Keys)
            {
                k.Add(key.ToString(), ConfigurationManager.AppSettings.Get(key.ToString()));
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
        /// <param name="filePath">App.config檔路徑</param>
        public static void SetConfiguration(string key, string value, string filePath)
        {
            Configuration configuration = null;                 //Configuration對象
            AppSettingsSection appSection = null;               //AppSection對象 

            configuration = ConfigurationManager.OpenExeConfiguration(filePath);

            //取得AppSetting節
            appSection = configuration.AppSettings;

            //賦值並保存
            appSection.Settings[key].Value = value;
            configuration.Save();


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
            string assemblyConfigFile = Assembly.GetEntryAssembly().Location;
            SetConfiguration(key, value, assemblyConfigFile);

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
            var config = WebConfigurationManager.OpenWebConfiguration(path);
            AppSettingsSection appSetting = (AppSettingsSection)config.GetSection("appSettings");
            if (appSetting.Settings[key] == null)//如果不存在此节点,则添加  
            {
                appSetting.Settings.Add(key, value);
            }
            else//如果存在此节点,则修改  
            {
                appSetting.Settings[key].Value = value;
            }
            config.Save();
            config = null;  
        }
        /// <summary>
        /// 设置web.config中appsetting的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetWebConfiguration(string key, string value)
        {
            SetWebConfiguration(key, value, HttpContext.Current.Request.ApplicationPath);
        }
    }
}
