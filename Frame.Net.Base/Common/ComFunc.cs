using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using System.Management;
using System.Reflection;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Constants;
using System.Linq;
using System.Collections.Specialized;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;
using System.Drawing;

namespace EFFC.Frame.Net.Base.Common
{
    /// <summary>
    /// 公共方法
    /// </summary>
    public class ComFunc
    {
        /// <summary>
        /// SetProcessWorkingSetSize
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="minimumWorkingSetSize"></param>
        /// <param name="maximumWorkingSetSize"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern bool SetProcessWorkingSetSize(IntPtr handle,
                     int minimumWorkingSetSize, int maximumWorkingSetSize);

        /// <summary>
        /// 字符串的Null處理，并作Trim處理
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string nvl(object obj)
        {
            if (obj == null)
            {
                return "";
            }
            else
            {
                return obj.ToString().ToString().Trim();
            }
        }
        /// <summary>
        /// 字符串的Null處理，不作Trim處理
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string nvlNotrim(object obj)
        {
            if (obj == null)
            {
                return "";
            }
            else
            {
                return obj.ToString().ToString();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string FindFileName(string s)
        {
            return Path.GetFileName(s);
            //string filereg = @"[a-zA-Z _0-9\-]+(.[a-zA-Z]+)?$";
            //Regex reg = new Regex(filereg);
            //return reg.Match(s).Value; ;
        }


        /// <summary>
        /// string數組轉化ArrayList
        /// </summary>
        /// <param name="strs"></param>
        /// <returns></returns>
        public static ArrayList StringArrayToArrayList(string[] strs)
        {
            ArrayList rtn = new ArrayList();
            for (int i = 0; strs != null && i < strs.Length; i++)
            {
                rtn.Add(strs[i]);
            }

            return rtn;
        }
        /// <summary>
        /// 返回指定長度的隨機數
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static int Random(int length)
        {
            string l = "1";
            for (int i = 0; i < length; i++)
            {
                l += "0";
            }
            Random r = new Random();
            return r.Next(int.Parse(l));
        }

        /// <summary>
        /// 調用Exe程序
        /// </summary>
        /// <param name="exepath"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static bool CallExe(string exepath, List<string> parameters)
        {
            try
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = exepath;

                string paramstr = "";
                for (int i = 0; parameters != null && i < parameters.Count; i++)
                {
                    paramstr += " " + parameters[i];
                }

                proc.StartInfo.Arguments = " " + paramstr;
                string skd = "";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = false;

                if (proc.Start())
                {

                    proc.WaitForExit();
                    skd = proc.StandardOutput.ReadToEnd();
                    proc.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 將字符串轉換成對應的Enum類型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public static T EnumParse<T>(string s)
        {
            return (T)Enum.Parse(typeof(T), s);
        }
        /// <summary>
        /// 将enum的值名称转成string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string Enum2String<T>(T e)
        {
            return Enum.GetName(typeof(T), e);
        }
        /// <summary>
        /// 合并两段脚本
        /// </summary>
        /// <param name="firstScript"></param>
        /// <param name="secondScript"></param>
        /// <returns></returns>
        public static string MergeScript(string firstScript, string secondScript)
        {
            if (!string.IsNullOrEmpty(firstScript))
            {
                return (firstScript + secondScript);
            }
            if (secondScript.TrimStart(new char[0]).StartsWith("javascript:", StringComparison.Ordinal))
            {
                return secondScript;
            }
            return ("javascript:" + secondScript);
        }
        /// <summary>
        /// 字符串结尾加上分号
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EnsureEndWithSemiColon(string value)
        {
            if (value != null)
            {
                int length = value.Length;
                if ((length > 0) && (value[length - 1] != ';'))
                {
                    return (value + ";");
                }
            }
            return value;
        }
        /// <summary>
        ///強制清理內存，慎用，適用於win32NT平臺
        /// </summary>
        public static void MemoryCollect()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }

        /// <summary>
        ///判斷是否為中文
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsChineseString(string str)
        {
            return Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(str)) != str;
        }
        /// <summary>
        /// 將字符串從一種編碼轉變成另一種編碼
        /// </summary>
        /// <param name="str">待轉換的字符串</param>
        /// <param name="from">原編碼</param>
        /// <param name="to">待轉出的編碼</param>
        /// <returns></returns>
        public static string ConvertEncode(string str, Encoding from, Encoding to)
        {
            return to.GetString(from.GetBytes(str));
        }

        /// <summary>
        ///  將文件進行md5加密 , md5 值保存在filename.md5 文件中
        /// </summary>
        /// <param name="path">需要加密的文件路徑</param>
        /// <returns>加密的MD5碼</returns>
        public static string getMD5(string path)
        {
            MD5CryptoServiceProvider get_md5 = new MD5CryptoServiceProvider();
            FileStream get_file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] hash_byte = get_md5.ComputeHash(get_file);
            string result = BitConverter.ToString(hash_byte);
            result = result.Replace("-", "");
            get_file.Close();
            return result;
        }
        /// <summary>
        /// 將字串進行md5加密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string getMD5_String(string data, Encoding encoding)
        {
            byte[] btdata = encoding.GetBytes(data);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] btout = md5.ComputeHash(btdata, 0, btdata.Length);
            string strout = BitConverter.ToString(btout).Replace("-", "");
            return strout;
        }
        /// <summary>
        /// 用Base64进行加密,默认编码UTF8
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Base64Code(string value)
        {
            return Base64Code(value, Encoding.UTF8);
        }
        /// <summary>
        /// 用Base64进行加密
        /// </summary>
        /// <param name="value"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string Base64Code(string value, Encoding encode)
        {
            byte[] bytes = encode.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }
        /// <summary>
        /// 用Base64进行解码
        /// </summary>
        /// <param name="value"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string Base64DeCode(string value, Encoding encode)
        {
            byte[] bytes = Convert.FromBase64String(value);
            return encode.GetString(bytes);
        }
        /// <summary>
        /// 用Base64进行解码，默认编码UTF8
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Base64DeCode(string value)
        {
            return Base64DeCode(value, Encoding.UTF8);
        }
        /// <summary>
        /// 3DES加密算法
        /// </summary>
        /// <param name="strString">待加密串</param>
        /// <param name="strKey">加密的密钥key</param>
        /// <returns></returns>
        public static string DES3Encrypt(string strString, string strKey)
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5 = new MD5CryptoServiceProvider();

            DES.Key = hashMD5.ComputeHash(Encoding.Default.GetBytes(strKey));
            DES.Mode = CipherMode.ECB;

            ICryptoTransform DESEncrypt = DES.CreateEncryptor();
            byte[] Buffer = Encoding.Default.GetBytes(strString);
            return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
        }


        /// <summary>
        /// 3DES解密算法
        /// </summary>
        /// <param name="strString">待加密串</param>
        /// <param name="strKey">加密的密钥key</param>
        /// <returns></returns>
        public static string DES3Decrypt(string strString, string strKey)
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5 = new MD5CryptoServiceProvider();

            DES.Key = hashMD5.ComputeHash(Encoding.Default.GetBytes(strKey));
            DES.Mode = CipherMode.ECB;
            ICryptoTransform DESDecrypt = DES.CreateDecryptor();

            string result = "";
            try
            {
                byte[] Buffer = Convert.FromBase64String(strString);
                result = Encoding.Default.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
            }
            catch (System.Exception e)
            {
                throw (new System.Exception("null", e));
            }

            return result;

        }
        /// <summary>
        /// 256位AES加密
        /// </summary>
        /// <param name="toEncrypt"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string AESEncrypt(string toEncrypt, string key)
        {
            // 256-AES key    
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        /// <summary>
        /// 256位AES加密，采用默认的Key
        /// </summary>
        /// <param name="toEncrypt"></param>
        /// <returns></returns>
        public static string AESEncrypt(string toEncrypt)
        {
            return AESEncrypt(toEncrypt, "12345678901234567890123456789012");
        }
        /// <summary>
        /// 256位AES解密
        /// </summary>
        /// <param name="toDecrypt"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string AESDecrypt(string toDecrypt, string key)
        {
            // 256-AES key    
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        /// <summary>
        /// 256位AES解密，采用默认的key
        /// </summary>
        /// <param name="toDecrypt"></param>
        /// <returns></returns>
        public static string AESDecrypt(string toDecrypt)
        {
            return AESDecrypt(toDecrypt, "12345678901234567890123456789012");
        }

        /// <summary>
        /// 对字串进行html转码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HTMLEncode(object str)
        {
            return HttpUtility.HtmlEncode(nvlNotrim(str));
        }
        /// <summary>
        /// 对字串进行html解码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HTMLDecode(object str)
        {
            return HttpUtility.HtmlDecode(nvlNotrim(str));
        }
        /// <summary>
        /// 对字串进行url转码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UrlEncode(object str)
        {
            return HttpUtility.UrlEncode(nvl(str), Encoding.UTF8);
        }
        /// <summary>
        /// 对字串进行url解码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UrlDecode(object str)
        {
            return HttpUtility.UrlDecode(nvl(str), Encoding.UTF8);
        }
        /// <summary>
        /// 将流转化为二进制数据
        /// </summary>
        /// <param name="s">文件流</param>
        /// <returns></returns>
        public static byte[] ConvertToBinary(Stream s)
        {
            byte[] rtn = new byte[s.Length];
            try
            {
                s.Read(rtn, 0, Convert.ToInt32(s.Length));
            }
            catch (Exception ex)
            {
                s.Close();
                throw ex;
            }
            return rtn;
        }
        /// <summary>
        /// 将流转化为二进制数据
        /// </summary>
        /// <param name="filepath">文件流</param>
        /// <returns></returns>
        public static byte[] ConvertToBinary(string filepath)
        {
            FileStream s = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            byte[] rtn = new byte[s.Length];
            try
            {
                s.Read(rtn, 0, Convert.ToInt32(s.Length));
            }
            finally
            {
                s.Close();
            }
            return rtn;
        }
        /// <summary>
        /// 将二进制数据存为文件
        /// </summary>
        /// <param name="s"></param>
        /// <param name="savepath"></param>
        public static void SaveToFile(Stream stream, string savepath)
        {
            // 把 Stream 转换成 byte[] 
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始 
            stream.Seek(0, SeekOrigin.Begin);
            var filename = Path.GetFileName(savepath);
            var directorypath = savepath.Replace(filename, "");
            if (!Directory.Exists(directorypath))
            {
                Directory.CreateDirectory(directorypath);
            }
            // 把 byte[] 写入文件 
            FileStream fs = new FileStream(savepath, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(bytes);
            bw.Close();
            fs.Close(); 
        }
        /// <summary>
        /// 将二进制数据存为文件
        /// </summary>
        /// <param name="bs"></param>
        /// <param name="savepath"></param>
        public static void SaveToFile(byte[] bs, string savepath)
        {
            int ArraySize = bs.GetUpperBound(0);
            string path = Path.GetDirectoryName(savepath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            FileStream fs = new FileStream(savepath, FileMode.OpenOrCreate, FileAccess.Write);
            try
            {
                fs.Write(bs, 0, ArraySize);
            }
            finally
            {
                fs.Close();
            }
        }
        /// <summary>
        /// 将文本存为文件
        /// </summary>
        /// <param name="text"></param>
        /// <param name="savepath"></param>
        public static void SaveToFile(string text, string savepath)
        {
            SaveToFile(text, savepath, Encoding.UTF8);
        }
        /// <summary>
        /// 将文本存为文件
        /// </summary>
        /// <param name="text"></param>
        /// <param name="savepath"></param>
        /// <param name="encoding"></param>
        public static void SaveToFile(string text, string savepath, Encoding encoding)
        {
            string path = Path.GetDirectoryName(savepath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            try
            {
                File.WriteAllText(savepath, text, encoding);
            }
            finally
            {

            }
        }
        /// <summary>
        /// 获取呼叫者的type
        /// </summary>
        /// <param name="offset">偏移量</param>
        /// <returns></returns>
        public static Type GetCaller(int offset)
        {
            StackTrace trace = new StackTrace();
            StackFrame frame = trace.GetFrame(offset);
            return frame.GetMethod().DeclaringType;
        }
        /// <summary>
        /// 判断呼叫者的类型是否为指定类型
        /// </summary>
        /// <param name="t">待判定类型</param>
        /// <param name="offset">偏移量</param>
        /// <returns></returns>
        public static bool IsCaller(Type t, int offset)
        {
            Type mt = GetCaller(offset);
            return mt.FullName == t.FullName || mt.IsSubclassOf(t);
        }
        /// <summary>
        /// 判断当前呼叫者是否为指定类型
        /// </summary>
        /// <param name="t">待判定类型</param>
        /// <returns></returns>
        public static bool IsCaller(Type t)
        {
            return IsCaller(t, 2);
        }
        /// <summary>
        ///格式化json格式
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static FrameDLRObject FormatJSON(FrameDLRObject json)
        {
            var rtn = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            rtn.ErrorCode = "";
            rtn.ErrorMessage = "";
            if (json != null)
            {
                rtn.Content = json;
            }

            return rtn;
        }
        /// <summary>
        /// 格式化json格式
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static FrameDLRObject FormatJSON(string json)
        {
            var rtn = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            rtn.ErrorCode = "";
            rtn.ErrorMessage = "";
            if (json != null)
            {
                rtn.Content = json;
            }

            return rtn;
        }
        /// <summary>
        /// 格式化json格式
        /// </summary>
        /// <param name="errorcode"></param>
        /// <param name="errormessage"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static FrameDLRObject FormatJSON(string errorcode, string errormessage, FrameDLRObject content)
        {
            var rtn = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            rtn.ErrorCode = errorcode;
            rtn.ErrorMessage = errormessage;
            if (content != null)
            {
                rtn.Content = content;
            }

            return rtn;
        }
        /// <summary>
        /// 格式化json格式
        /// </summary>
        /// <param name="errorcode"></param>
        /// <param name="errormessage"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static FrameDLRObject FormatJSON(string errorcode, string errormessage, string content)
        {
            var rtn = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            rtn.ErrorCode = errorcode;
            rtn.ErrorMessage = errormessage;
            if (content != null)
            {
                rtn.Content = content;
            }

            return rtn;
        }

        /// <summary>
        /// 拷贝整个目录
        /// </summary>
        /// <param name="srcDir"></param>
        /// <param name="tgtDir"></param>
        /// <param name="includeext">如果指定则只拷贝指定的文件类型，否则拷贝全部文件</param>
        public static void CopyDirectory(string srcDir, string tgtDir, params string[] includeext)
        {
            DirectoryInfo source = new DirectoryInfo(srcDir);
            DirectoryInfo target = new DirectoryInfo(tgtDir);

            if (target.FullName.StartsWith(source.FullName, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new Exception("父目录不能拷贝到子目录！");
            }

            if (!source.Exists)
            {
                return;
            }

            if (!target.Exists)
            {
                target.Create();
            }

            FileInfo[] files = source.GetFiles();

            for (int i = 0; i < files.Length; i++)
            {
                if (includeext == null || includeext.Contains(files[i].Extension))
                {
                    File.Copy(files[i].FullName, target.FullName + @"\" + files[i].Name, true);
                }
            }

            DirectoryInfo[] dirs = source.GetDirectories();

            for (int j = 0; j < dirs.Length; j++)
            {
                CopyDirectory(dirs[j].FullName, target.FullName + @"\" + dirs[j].Name, includeext);
            }
        }
        /// <summary>
        /// 解析queryString
        /// </summary>
        /// <param name="qr"></param>
        /// <returns></returns>
        public static FrameDLRObject ParseQueryString(string qr)
        {
            FrameDLRObject rtn = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            if (qr != "")
            {
                var nvc = HttpUtility.ParseQueryString(qr);
                foreach (var key in nvc.AllKeys)
                {
                    rtn.SetValue(key, nvc[key]);
                }
            }

            return rtn;
        }
        /// <summary>
        /// 时间转为时间戳
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double ToTimestamp(DateTime value)
        {
            TimeSpan span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());
            return (double)span.TotalSeconds;
        }
        /// <summary>
        /// 时间戳转为时间
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime ConvertTimestamp(double timestamp)
        {
            if (timestamp.ToString().Length > 10)
            {
                timestamp = timestamp / 1000;
            }
            DateTime converted = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            DateTime newDateTime = converted.AddSeconds(timestamp);
            return newDateTime.ToLocalTime();
        }
        /// <summary>
        /// stream转化为byte
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] StreamToBytes(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }
        /// <summary>
        /// Base64串转化成stream
        /// </summary>
        /// <param name="base64str"></param>
        /// <returns></returns>
        public static Stream Base64StringToStream(string base64str)
        {
            byte[] arr = Convert.FromBase64String(base64str);
            MemoryStream ms = new MemoryStream(arr);
            return ms;
        }
        /// <summary>
        /// stream转化为base64
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string StreamToBase64String(Stream s)
        {
            byte[] bytes = StreamToBytes(s);
            string base64 = Convert.ToBase64String(bytes);
            return base64;
        }
        /// <summary>
        /// Stream转string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string StreamToString(Stream s)
        {
            return StreamToString(s, Encoding.UTF8);
        }
        /// <summary>
        /// Stream转string
        /// </summary>
        /// <param name="s"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string StreamToString(Stream s,Encoding encode)
        {
            s.Position = 0;
            StreamReader reader = new StreamReader(s, encode);
            string text = reader.ReadToEnd();
            return text;
        }
        /// <summary>
        /// byte[]转string
        /// </summary>
        /// <param name="b"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string ByteToString(byte[] b, Encoding encode)
        {
            return encode.GetString(b);
        }
        /// <summary>
        /// 拷贝Stream
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public static void CopyStream(Stream input, Stream output)
        {
            input.Seek(0, SeekOrigin.Begin);
            int bufferSize = 4096;
            byte[] buffer = new byte[bufferSize];
            while (true)
            {
                int read = input.Read(buffer, 0, buffer.Length);
                if (read <= 0)
                {
                    return;
                }
                output.Write(buffer, 0, read);
            }
        }
        /// <summary>
        /// 拷贝Stream,适用于文本文件流
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="encode"></param>
        public static void CopyStream(Stream input, Stream output, Encoding inputcode, Encoding outputencode)
        {
            input.Seek(0, SeekOrigin.Begin);
            int bufferSize = 4096;
            char[] buffer = new char[bufferSize];
            StreamReader sr = new StreamReader(input, inputcode);
            StreamWriter sw = new StreamWriter(output, outputencode);
            while (!sr.EndOfStream)
            {
                sr.Read(buffer, 0, buffer.Length);
                sw.Write(buffer);
            }
        }
        /// <summary>
        /// HMAC-sha1加密
        /// </summary>
        /// <param name="text"></param>
        /// <param name="key"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string HmacSha1Sign(string text, string key, Encoding encode)
        {
            HMACSHA1 hmacsha1 = new HMACSHA1();
            hmacsha1.Key = encode.GetBytes(key);
            byte[] dataBuffer = encode.GetBytes(text);
            byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);
            return Convert.ToBase64String(hashBytes);
        }
        /// <summary>
        /// HMAC-sha1加密，采用UTF8编码
        /// </summary>
        /// <param name="text"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string HmacSha1Sign(string text, string key)
        {
            return HmacSha1Sign(text, key,Encoding.UTF8);
        }

        /// <summary>
        /// 文字转二维码
        /// </summary>
        /// <param name="nr"></param>
        public static byte[] QRCode(string nr)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = 4;
            qrCodeEncoder.QRCodeVersion = 8;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            var by = qrCodeEncoder.EncodeToByte(nr, Encoding.UTF8);
            return by;
        }

        /// <summary>
        /// 二维码解码
        /// </summary>
        /// <param name="filePath">图片路径</param>
        /// <returns></returns>
        public static string QRDecode(Stream file)
        {
            if (file == null)
                return null;
            QRCodeDecoder decoder = new QRCodeDecoder();
            Bitmap myBitmap = new Bitmap(Image.FromStream(file));
            string decodedString = decoder.decode(new QRCodeBitmapImage(myBitmap));
            return decodedString;
        }
        /// <summary>
        /// 从byte[]中搜索相符的字节流，并返回起始位置
        /// </summary>
        /// <param name="searchWithin"></param>
        /// <param name="serachFor"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static int BytesIndexOf(byte[] searchWithin, byte[] serachFor, int startIndex)
        {
            int index = 0;
            int startPos = Array.IndexOf(searchWithin, serachFor[0], startIndex);

            if (startPos != -1)
            {
                while ((startPos + index) < searchWithin.Length)
                {
                    if (searchWithin[startPos + index] == serachFor[index])
                    {
                        index++;
                        if (index == serachFor.Length)
                        {
                            return startPos;
                        }
                    }
                    else
                    {
                        startPos = Array.IndexOf<byte>(searchWithin, serachFor[0], startPos + index);
                        if (startPos == -1)
                        {
                            return -1;
                        }
                        index = 0;
                    }
                }
            }

            return -1;
        }
        /// <summary>
        /// 将byte[]根据指定的split进行分组，将每个searchWithin的起始index合成数组并返回
        /// </summary>
        /// <param name="searchWithin"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public static int[] BytesSplit(byte[] searchWithin, byte[] split)
        {
            List<int> l = new List<int>();

            var index = BytesIndexOf(searchWithin, split, 0);
            var preindex = index;
            
            while (index >= 0)
            {
                l.Add(preindex);
                var tmp = new byte[searchWithin.Length - preindex - split.Length];
                Array.Copy(searchWithin, preindex + split.Length, tmp, 0, tmp.Length);
                index = BytesIndexOf(tmp, split, 0);
                preindex += index + split.Length;
            }
            return l.ToArray();
        }
        /// <summary>
        /// 将byte[]根据指定的split进行分组，将每个searchWithin的起始index合成数组并返回
        /// </summary>
        /// <param name="searchWithin"></param>
        /// <param name="split"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static int[] BytesSplit(byte[] searchWithin, string split,Encoding encode)
        {
            return BytesSplit(searchWithin, encode.GetBytes(split));
        }

        /// <summary>
        /// 判断目标是文件夹还是目录(目录包括磁盘)
        /// </summary>
        /// <param name="filepath">文件名</param>
        /// <returns></returns>
        public static bool IsDir(string filepath)
        {
            FileInfo fi = new FileInfo(filepath);
            if ((fi.Attributes & FileAttributes.Directory) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 获取当前进程所占用的内存数
        /// </summary>
        /// <returns></returns>
        public static double GetProcessUsedMemory()
        {
            double usedMemory = 0;
            usedMemory = Process.GetCurrentProcess().WorkingSet64 / 1024.0 / 1024.0;
            return usedMemory;
        }
        /// <summary>
        /// 复制一个对象，如果对象为ICloneable则返回一个对象的深度副本
        /// 如果为List和dictionary会做深度复制，否则返回原对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object CloneObject(object obj)
        {
            if(obj is ICloneable)
            {
                return ((ICloneable)obj).Clone();
            }
            else if (obj is List<object>)
            {
                var to = new List<object>();
                var from = (List<Object>)obj;
                foreach (var item in from)
                {
                    if (item is ICloneable)
                    {
                        to.Add(((ICloneable)item).Clone());
                    }
                }

                return to;
            }
            else if (obj is List<string>)
            {
                var to = new List<string>();
                var from = (List<string>)obj;
                foreach (var item in from)
                {
                    to.Add(item);
                }

                return to;
            }
            else if (obj is List<FrameDLRObject>)
            {
                var to = new List<FrameDLRObject>();
                var from = (List<FrameDLRObject>)obj;
                foreach (var item in from)
                {
                    to.Add((FrameDLRObject)item.Clone());
                }

                return to;
            }
            else if (obj is Dictionary<string, object>)
            {
                var to = new Dictionary<string, object>();
                var from = (Dictionary<string, object>)obj;
                foreach (var item in from)
                {
                    if (item.Value is ICloneable)
                    {
                        to.Add(item.Key, ((ICloneable)item.Value).Clone());
                    }
                }

                return to;
            }
            else if (obj is Dictionary<string, string>)
            {
                var to = new Dictionary<string, string>();
                var from = (Dictionary<string, string>)obj;
                foreach (var item in from)
                {
                    to.Add(item.Key, item.Value);
                }

                return to;
            }
            else if (obj is Dictionary<string, FrameDLRObject>)
            {
                var to = new Dictionary<string, FrameDLRObject>();
                var from = (Dictionary<string, FrameDLRObject>)obj;
                foreach (var item in from)
                {
                    to.Add(item.Key, (FrameDLRObject)item.Value.Clone());
                }

                return to;
            }
            else
            {
                return obj;
            }
        }
    }
}
