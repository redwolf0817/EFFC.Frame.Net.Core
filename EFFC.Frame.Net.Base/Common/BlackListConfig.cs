using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EFFC.Frame.Net.Base.Common
{
    /// <summary>
    /// 黑名单配置
    /// </summary>
    public class BlackListConfig
    {
        private static FrameConfig _config;
        private static string _filepath = "";
        private static string _jsonfilename = "black_list.json";
        /// <summary>
        /// 系统配置档对象
        /// </summary>
        public static FrameConfig Configuration
        {
            get
            {
                if (_config == null)
                {
                    _config = new FrameConfig(Path.Combine(ConfigFilePath, _jsonfilename));
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
                    _filepath = ComFunc.GetApplicationRoot();
                    var filepath = Path.Combine(_filepath, _jsonfilename);
                    if (!File.Exists(filepath))
                    {
                        _filepath = Directory.GetCurrentDirectory();
                    }
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
            return Configuration.GetConfiguration(section, key);
        }
        /// <summary>
        /// 取得appSettings裏的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfiguration(string key)
        {
            return Configuration.GetConfiguration(key);
        }

        #endregion

        #region GetConfigurationList
        /// <summary>
        /// 取得指定section裏的值列表
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetConfigurationList(string section)
        {
            return Configuration.GetConfigurationList(section);

        }
        #endregion

        #region SetConfiguration
        /// <summary>
        /// 設置appSetting的值
        /// </summary>
        /// <param name="section">对应section</param>
        /// <param name="key">鍵</param>
        /// <param name="value">值</param>
        public static void SetConfiguration(string section, string key, object value)
        {
            Configuration.SetConfiguration(section, key, value);
        }
        #endregion

        #region SetConfiguration
        /// <summary>
        /// 設置appSetting的值
        /// </summary>
        /// <param name="key">鍵</param>
        /// <param name="value">值</param>
        public static void SetConfiguration(string key, object value)
        {
            Configuration.SetConfiguration(key, value);
        }
        #endregion
    }
}
