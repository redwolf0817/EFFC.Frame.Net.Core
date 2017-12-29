using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.Parameter;
using System.IO;

namespace EFFC.Frame.Net.Base.ResouceManage.FTP
{
    [Serializable]
    public class FtpParameter : ParameterStd
    {
        public FtpParameter()
        {
            UseBinary = false;
            KeepAlive = false;
            UsePassive = false;
            TimeOut = 30000;
            EncodingString = "UTF-8";
            BufferSize = 1024;
        }
        /// <summary>
        /// FTP URL含完整的ftp访问串 ftp://xxxxx:xxx/xxxx
        /// </summary>
        public string FTP_URL
        {
            get { return GetValue<string>("FTP_URL"); }
            set { SetValue("FTP_URL", value); }
        }
        /// <summary>
        /// ftp登陸的用戶ID
        /// </summary>
        public string Login_UserId
        {
            get { return GetValue<string>("Login_UserId"); }
            set { SetValue("Login_UserId", value); }
        }
        /// <summary>
        /// FTP登陸的密码
        /// </summary>
        public string Login_Password
        {
            get { return GetValue<string>("Login_Password"); }
            set { SetValue("Login_Password", value); }
        }
        /// <summary>
        /// 下载目標位置路徑
        /// </summary>
        public string DownLoad_Path
        {
            get { return GetValue<string>("DownLoad_Path"); }
            set { SetValue("DownLoad_Path", value); }
        }
        /// <summary>
        /// 下載后文件名稱
        /// </summary>
        public string DownLoad_FileName
        {
            get { return GetValue<string>("DownLoad_FileName"); }
            set { SetValue("DownLoad_FileName", value); }
        }
        /// <summary>
        /// 上傳的問價路徑（含名称）
        /// </summary>
        public string Upload_FilePath
        {
            get { return GetValue<string>("Upload_FilePath"); }
            set { SetValue("Upload_FilePath", value); }
        }
        /// <summary>
        /// true 向伺服器表示，要傳輸的資料是二進位資料，而 false 則表示資料是文字。預設值為 false。
        /// </summary>
        public bool UseBinary
        {
            get { return GetValue<bool>("UseBinary"); }
            set { SetValue("UseBinary", value); }
        }
        /// <summary>
        /// 取得或設定 System.Boolean 值，指定在要求完成之後，與 FTP 伺服器的控制連接是否關閉。預設值為 false。
        /// </summary>
        public bool KeepAlive
        {
            get { return GetValue<bool>("KeepAlive"); }
            set { SetValue("KeepAlive", value); }
        }
        /// <summary>
        /// 如果用戶端應用程式的資料傳輸處理序會接聽資料通訊埠上的連接，則為 false，如果用戶端應該在資料通訊埠上啟始連接，則為 true。預設值為 false。
        /// </summary>
        public bool UsePassive
        {
            get { return GetValue<bool>("UsePassive"); }
            set { SetValue("UsePassive", value); }
        }
        /// <summary>
        /// FTP连接超时设定,預設值為 30。
        /// </summary>
        public int TimeOut
        {
            get { return GetValue<int>("TimeOut"); }
            set { SetValue("TimeOut", value); }
        }
        /// <summary>
        /// 文字編碼，如UTF-8，big5等,預設值為 UTF-8。
        /// </summary>
        public string EncodingString
        {
            get { return GetValue<string>("EncodingString"); }
            set { SetValue("EncodingString", value); }
        }
        /// <summary>
        /// 传输过程中的缓存大小，单位为byte，默认为1024
        /// </summary>
        public int BufferSize
        {
            get { return GetValue<int>("BufferSize"); }
            set { SetValue("BufferSize", value); }
        }
    }
}
