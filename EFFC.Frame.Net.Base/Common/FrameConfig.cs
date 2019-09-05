using EFFC.Frame.Net.Base.Data.Base;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EFFC.Frame.Net.Base.Common
{
    /// <summary>
    /// 配置档相关操作
    /// </summary>
    public class FrameConfig
    {
        IConfigurationRoot _config;
        /// <summary>
        /// 初始化配置档
        /// </summary>
        /// <param name="filepath">配置档路径，~表示应用根路径</param>
        public FrameConfig(string filepath)
        {
            ConfigFilePath = filepath.Replace("~", Directory.GetCurrentDirectory());
            if (!File.Exists(ConfigFilePath))
            {
                File.Create(ConfigFilePath);
            }
            Console.WriteLine($"当前配置档加载路径：{ConfigFilePath}");
            var builder = new ConfigurationBuilder()
                        .SetBasePath(Path.GetDirectoryName(ConfigFilePath))
                        .AddJsonFile(Path.GetFileName(ConfigFilePath), optional: true, reloadOnChange: true);

            _config = builder.Build();
        }
        /// <summary>
        /// 配置档文件路径
        /// </summary>
        public string ConfigFilePath
        {
            get; protected set;
        }
        /// <summary>
        /// 获取指定section下的指定参数
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetConfiguration(string section, string key)
        {
            return _config.GetSection(section)[key];
        }
        /// <summary>
        /// 取得appSettings裏的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetConfiguration(string key)
        {
            return _config[key];
        }
        /// <summary>
        /// 取得指定section裏的值列表
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetConfigurationList(string section)
        {
            Dictionary<string, object> k = new Dictionary<string, object>();                   //返回的鍵值對類型

            //取得AppSettings節
            var list = _config.GetSection(section).GetChildren();
            foreach (var item in list)
            {
                k.Add(item.Key, item.Value);
            }

            return k;
        }

        /// <summary>
        /// 設置appSetting的值
        /// </summary>
        /// <param name="section">对应section</param>
        /// <param name="key">鍵</param>
        /// <param name="value">值</param>
        public void SetConfiguration(string section, string key, object value)
        {
            FrameDLRObject config = FrameDLRObject.CreateInstance(File.ReadAllText(ConfigFilePath), Constants.FrameDLRFlags.SensitiveCase);
            if (config.GetValue(section) == null)
            {
                config.SetValue(section, FrameDLRObject.CreateInstance(Constants.FrameDLRFlags.SensitiveCase));
            }
                ((FrameDLRObject)config.GetValue(section)).SetValue(key, value);
            File.WriteAllText(ConfigFilePath, config.ToJSONString());
            _config.Reload();
        }
        /// <summary>
        /// 設置appSetting的值
        /// </summary>
        /// <param name="key">鍵</param>
        /// <param name="value">值</param>
        public void SetConfiguration(string key, object value)
        {
            FrameDLRObject config = FrameDLRObject.CreateInstance(File.ReadAllText(ConfigFilePath), Constants.FrameDLRFlags.SensitiveCase);
            config.SetValue(key, value);
            File.WriteAllText(ConfigFilePath, config.ToJSONString());
            _config.Reload();
        }
    }
}
