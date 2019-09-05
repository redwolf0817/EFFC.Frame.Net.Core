using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using System.Reflection;
using System.Linq;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Constants;
using Frame.Net.Base.Interfaces.DataConvert;
using System.DrawingCore;
using QRCoder;
using System.DrawingCore.Imaging;
using System.Xml;
using System.Runtime.CompilerServices;

namespace EFFC.Frame.Net.Base.Common
{
    /// <summary>
    /// 公共方法
    /// </summary>
    public class ComFunc
    {
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
        public static List<string> StringArrayToArrayList(string[] strs)
        {
            List<string> rtn = new List<string>();
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
            Random r = new Random(Guid.NewGuid().GetHashCode());
            var rtn = r.Next(int.Parse(l));
            while (rtn.ToString().Length < length)
            {
                rtn = r.Next(int.Parse(l));
            }
            return rtn;
        }
        /// <summary>
        /// 生成不重复的随机码
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomCode(int length)
        {
            var result = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                var r = new Random(Guid.NewGuid().GetHashCode());
                result.Append(r.Next(0, 10));
            }
            var rtn = result.ToString();
            return rtn;
        }
        ///<summary>
        ///生成随机字符串 
        ///</summary>
        ///<param name="length">目标字符串的长度</param>
        ///<param name="useNum">是否包含数字，1=包含，默认为包含</param>
        ///<param name="useLow">是否包含小写字母，1=包含，默认为包含</param>
        ///<param name="useUpp">是否包含大写字母，1=包含，默认为包含</param>
        ///<param name="useSpe">是否包含特殊字符，1=包含，默认为不包含</param>
        ///<param name="custom">要包含的自定义字符，直接输入要包含的字符列表</param>
        ///<returns>指定长度的随机字符串</returns>
        public static string RandomString(int length, bool useNum = true, bool useLow = true, bool useUpp = true, bool useSpe = true, string custom = "")
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, str = custom;
            if (useNum == true) { str += "0123456789"; }
            if (useLow == true) { str += "abcdefghijklmnopqrstuvwxyz"; }
            if (useUpp == true) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
            if (useSpe == true) { str += "!#$%&*+"; }
            for (int i = 0; i < length; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
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
                    proc.Dispose();
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
        /// 將字串進行md5加密,默认使用utf-8编码
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string getMD5_String(string data)
        {
            return getMD5_String(data, Encoding.UTF8);
        }
        /// <summary>
        /// 將字串進行md5加密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string getMD5_String(string data, Encoding encoding)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(encoding.GetBytes(data));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "");
            }
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
        /// 用Base64进行加密
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Base64Code(byte[] value)
        {
            return Convert.ToBase64String(value);
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
        /// 用Base64进行解码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] Base64DeCodeToByte(string value)
        {
            return Convert.FromBase64String(value);
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
            TripleDES DES = TripleDES.Create();
            MD5 hashMD5 = MD5.Create();

            DES.Key = hashMD5.ComputeHash(Encoding.UTF8.GetBytes(strKey));
            DES.Mode = CipherMode.ECB;

            ICryptoTransform DESEncrypt = DES.CreateEncryptor();
            byte[] Buffer = Encoding.UTF8.GetBytes(strString);
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
            TripleDES DES = TripleDES.Create();
            MD5 hashMD5 = MD5.Create();

            DES.Key = hashMD5.ComputeHash(Encoding.UTF8.GetBytes(strKey));
            DES.Mode = CipherMode.ECB;
            ICryptoTransform DESDecrypt = DES.CreateDecryptor();

            string result = "";
            try
            {
                byte[] Buffer = Convert.FromBase64String(strString);
                result = Encoding.UTF8.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
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

            Aes rDel = Aes.Create();
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

            Aes rDel = Aes.Create();
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
            return WebUtility.HtmlEncode(nvlNotrim(str));
        }
        /// <summary>
        /// 对字串进行html解码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HTMLDecode(object str)
        {
            return WebUtility.HtmlDecode(nvlNotrim(str));
        }
        /// <summary>
        /// 对字串进行url转码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UrlEncode(object str)
        {
            return WebUtility.UrlEncode(nvl(str));
        }
        /// <summary>
        /// 对字串进行url解码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UrlDecode(object str)
        {
            return WebUtility.UrlDecode(nvl(str));
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
                s.Dispose();
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
                s.Dispose();
            }
            return rtn;
        }
        /// <summary>
        /// 将二进制数据存为文件
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="savepath"></param>
        public static void SaveToFile(Stream stream, string savepath)
        {
            // 把 Stream 转换成 byte[] 
            byte[] bytes = new byte[stream.Length];
            // 设置当前流的位置为流的开始 
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(bytes, 0, bytes.Length);
            var filename = Path.GetFileName(savepath);
            var directorypath = savepath.Replace(filename, "");
            if (!Directory.Exists(directorypath))
            {
                Directory.CreateDirectory(directorypath);
            }
            // 把 byte[] 写入文件,using等效try finally
            using (FileStream fs = new FileStream(savepath, FileMode.Create))
            {
                fs.Write(bytes, 0, bytes.Length);
            }
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
                fs.Dispose();
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
        //public static FrameDLRObject ParseQueryString(string qr)
        //{
        //    FrameDLRObject rtn = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
        //    if (qr != "")
        //    {
        //        var nvc = HttpUtility.ParseQueryString(qr);
        //        foreach (var key in nvc.AllKeys)
        //        {
        //            rtn.SetValue(key, nvc[key]);
        //        }
        //    }

        //    return rtn;
        //}
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
        /// 时间转为时间戳
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TimeSpan ToTimeStampTS(DateTime value)
        {
            TimeSpan span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());
            return span;
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
            // 设置当前流的位置为流的开始
            if (stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

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
        public static string StreamToString(Stream s, Encoding encode)
        {
            var bytes = StreamToBytes(s);
            return encode.GetString(bytes);
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
        /// 哈希加密算法
        /// </summary>
        /// <param name="hashAlgorithm"> 所有加密哈希算法实现均必须从中派生的基类 </param>
        /// <param name="input"> 待加密的字符串 </param>
        /// <param name="encoding"> 字符编码 </param>
        /// <returns></returns>
        private static string HashEncrypt(HashAlgorithm hashAlgorithm, string input, Encoding encoding)
        {
            var data = hashAlgorithm.ComputeHash(encoding.GetBytes(input));

            return BitConverter.ToString(data).Replace("-", "");
        }
        /// <summary>
        /// HMAC-sha1加密,默认UTF-8编码
        /// </summary>
        /// <param name="text"></param>
        /// <param name="key"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string HmacSha1Sign(string text, string key, Encoding encode=null)
        {
            if (encode == null) encode = Encoding.UTF8;
            return HashEncrypt(new HMACSHA1(encode.GetBytes(key)), text, encode);
        }
        /// <summary>
        /// HMAC-sha256哈希加密,默认UTF-8编码
        /// </summary>
        /// <param name="text"></param>
        /// <param name="key"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string HmacSha256Sign(string text, string key, Encoding encode = null)
        {
            if (encode == null) encode = Encoding.UTF8;
            return HashEncrypt(new HMACSHA256(encode.GetBytes(key)), text, encode);
        }
        /// <summary>
        /// HMAC-sha384哈希加密,默认UTF-8编码
        /// </summary>
        /// <param name="text"></param>
        /// <param name="key"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string HmacSha384Sign(string text, string key, Encoding encode = null)
        {
            if (encode == null) encode = Encoding.UTF8;
            return HashEncrypt(new HMACSHA384(encode.GetBytes(key)), text, encode);
        }
        /// <summary>
        /// HMAC-sha512哈希加密,默认UTF-8编码
        /// </summary>
        /// <param name="text"></param>
        /// <param name="key"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string HmacSha512Sign(string text, string key, Encoding encode = null)
        {
            if (encode == null) encode = Encoding.UTF8;
            return HashEncrypt(new HMACSHA512(encode.GetBytes(key)), text, encode);
        }
        /// <summary>
        /// SHA1 16进制哈希算法加密,默认UTF-8编码
        /// </summary>
        /// <param name="input"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string SHA1hash(string input, Encoding encode = null)
        {
            if (encode == null) encode = Encoding.UTF8;
            return HashEncrypt(SHA1.Create(), input, encode);
        }
        /// <summary>
        /// SHA256 16进制哈希算法加密,默认UTF-8编码
        /// </summary>
        /// <param name="input"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string SHA256hash(string input,Encoding encode=null)
        {
            if (encode == null) encode = Encoding.UTF8;
            return HashEncrypt(SHA256.Create(), input, encode);
        }
        /// <summary>
        /// SHA384 16进制哈希算法加密,默认UTF-8编码
        /// </summary>
        /// <param name="input"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string SHA384hash(string input, Encoding encode=null)
        {
            if (encode == null) encode = Encoding.UTF8;
            return HashEncrypt(SHA384.Create(), input, encode);
        }
        /// <summary>
        /// SHA512 16进制哈希算法加密,默认UTF-8编码
        /// </summary>
        /// <param name="input"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string SHA512hash(string input, Encoding encode=null)
        {
            if (encode == null) encode = Encoding.UTF8;
            return HashEncrypt(SHA512.Create(), input, encode);
        }
        /// <summary>
        /// Convert Image to Byte[]
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] ImageToBytes(Image image)
        {
            ImageFormat format = image.RawFormat;
            using (MemoryStream ms = new MemoryStream())
            {
                if (format.Equals(ImageFormat.Jpeg))
                {
                    image.Save(ms, ImageFormat.Jpeg);
                }
                else if (format.Equals(ImageFormat.Png))
                {
                    image.Save(ms, ImageFormat.Png);
                }
                else if (format.Equals(ImageFormat.Bmp))
                {
                    image.Save(ms, ImageFormat.Bmp);
                }
                else if (format.Equals(ImageFormat.Gif))
                {
                    image.Save(ms, ImageFormat.Gif);
                }
                else if (format.Equals(ImageFormat.Icon))
                {
                    image.Save(ms, ImageFormat.Icon);
                }
                byte[] buffer = new byte[ms.Length];
                //Image.Save()会改变MemoryStream的Position，需要重新Seek到Begin
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }
        /// <summary>
        /// 文字转二维码
        /// </summary>
        /// <param name="nr"></param>
        public static byte[] QRCode(string nr)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode("The text which should be encoded.", QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            return ImageToBytes(qrCodeImage);
        }


        //public static string QRDecode(Stream file)
        //{
        //    // create a barcode reader instance
        //    IBarcodeReader reader = new BarcodeReader();
        //    // load a bitmap
        //    var barcodeBitmap = (Bitmap)Image.FromStream(file);
        //    // detect and decode the barcode inside the bitmap

        //    var result = reader.Decode(barcodeBitmap)(ImageToBytes(barcodeBitmap), barcodeBitmap.Width, barcodeBitmap.Height, RGBLuminanceSource.BitmapFormat.Unknown);
        //    string decodedString = result.BarcodeFormat.ToString();
        //    return decodedString;
        //}
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
        public static int[] BytesSplit(byte[] searchWithin, string split, Encoding encode)
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
        /// 复制一个对象，如果对象为ICloneable则返回一个对象的深度副本
        /// 如果为List和dictionary会做深度复制，否则返回原对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object CloneObject(object obj)
        {
            if (obj is ICloneable)
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
            }else if (IsAnonymousType(obj.GetType()))
            {
                return CloneAnonymousObject(obj);
            }
            else
            {
                return obj;
            }
        }

        /// <summary>
        /// 根据User-agent判定浏览器类型
        /// </summary>
        /// <param name="useragent"></param>
        /// <returns></returns>
        public static string GetBrowserType(string useragent)
        {
            if (string.IsNullOrEmpty(useragent)) return "";

            if (useragent.Contains("MSIE") || useragent.Contains("Trident"))
            {
                return "IE";
            }
            if (useragent.Contains("Edge"))
            {
                return "Edge";
            }
            if (useragent.Contains("Chrome"))
            {
                return "Chrome";
            }
            if (useragent.Contains("Firefox"))
            {
                return "Firefox";
            }
            if (useragent.Contains("Safari") && !useragent.Contains("Chrome"))
            {
                return "Safari";
            }
            if (useragent.Contains("Opera"))
            {
                return "Opera";
            }
            return "Unknown";
        }
        /// <summary>
        /// 根据User-agent获取浏览器的版本号
        /// </summary>
        /// <param name="useragent"></param>
        /// <returns></returns>
        public static string GetBrowserVersion(string useragent)
        {
            if (string.IsNullOrEmpty(useragent)) return "";
            var browser = GetBrowserType(useragent);

            var regStr_ie = new Regex(@"(?<=MSIE )[\d.]+", RegexOptions.IgnoreCase);
            var regStr_ie2 = new Regex(@"(?<=rv:)[\d.]+", RegexOptions.IgnoreCase);
            var regStr_edge = new Regex(@"(?<=Edge/)[\d.]+", RegexOptions.IgnoreCase);
            var regStr_ff = new Regex(@"(?<=Firefox/)[\d.]+", RegexOptions.IgnoreCase);
            var regStr_chrome = new Regex(@"(?<=Chrome/)[\d.]+", RegexOptions.IgnoreCase);
            var regStr_saf = new Regex(@"(?<=Version/)[\d.]+", RegexOptions.IgnoreCase);
            var regStr_opr = new Regex(@"(?<=Version/)[\d.]+", RegexOptions.IgnoreCase);

            if (browser == "IE")
            {
                if (useragent.Contains("MSIE"))
                {
                    return regStr_ie.IsMatch(useragent) ? regStr_ie.Match(useragent).Value : "";
                }
                else
                {
                    return regStr_ie2.IsMatch(useragent) ? regStr_ie2.Match(useragent).Value : "";
                }

            }
            if (browser == "Edge")
            {
                return regStr_edge.IsMatch(useragent) ? regStr_edge.Match(useragent).Value : "";
            }
            if (browser == "Chrome")
            {
                return regStr_chrome.IsMatch(useragent) ? regStr_chrome.Match(useragent).Value : "";
            }
            if (browser == "Firefox")
            {
                return regStr_ff.IsMatch(useragent) ? regStr_ff.Match(useragent).Value : "";
            }
            if (browser == "Safari")
            {
                return regStr_saf.IsMatch(useragent) ? regStr_saf.Match(useragent).Value : "";
            }
            if (browser == "Opera")
            {
                return regStr_opr.IsMatch(useragent) ? regStr_opr.Match(useragent).Value : "";
            }
            return "";
        }
        /// <summary>
        /// 获取application的目录
        /// </summary>
        /// <returns></returns>
        public static string GetApplicationRoot()
        {
            var exePath = Path.GetDirectoryName(System.Reflection
                              .Assembly.GetExecutingAssembly().Location);
            return exePath.Replace("file:\\", "").Replace("file:", "");
        }
        /// <summary>
        /// 获取工程的根目录，非bin目录
        /// </summary>
        /// <returns></returns>
        public static string GetProjectRoot()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory.Split("bin");
            if(path!=null && path.Length > 0)
            {
                return path[0];
            }
            else
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }
        /// <summary>
        /// 判断是否为base64编码
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IsBase64(string o)
        {
            try
            {
                var a = Base64DeCode(o);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 判断是否为base64，如果是则自动解码，否则返回默认值
        /// </summary>
        /// <param name="o">待解码对象</param>
        /// <param name="defaultvalue">默认值,默认为空串</param>
        /// <returns></returns>
        public static string IsBase64Then(string o,string defaultvalue = "")
        {
            try
            {
                return Base64DeCode(o);
            }
            catch
            {
                return defaultvalue;
            }
        }
        /// <summary>
        /// 将IEnumerable转为json
        /// </summary>
        /// <param name="list"></param>
        /// <param name="isIgnorecase"></param>
        /// <returns></returns>
        public static string ToJson(IEnumerable<object> list,bool isIgnorecase=true)
        {
            if (list == null) return "";
            var rtn = "[";
            var body = "";
            foreach(var item in list)
            {
                FrameDLRObject jsonobj = FrameDLRObject.CreateInstance(item, isIgnorecase ? FrameDLRFlags.None : FrameDLRFlags.SensitiveCase);
                body += ","+jsonobj.ToJSONString();
            }
            body = body == "" ? "" : body.Substring(1);
            rtn += body + "]";
            return rtn;
        }

        /// <summary>
        /// 获取安全的XML的实例对象，防范xxe攻击
        /// </summary>
        /// <returns></returns>
        public static XmlDocument GetSafeXmlInstance()
        {
            var rtn = new XmlDocument();
            rtn.XmlResolver = null;
            return rtn;
        }
        /// <summary>
        /// 判断是否为合法格式的URI
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool IsValidURI(string url)
        {
            return Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute);
        }
        /// <summary>
        /// 判定一个类型是否为匿名对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsAnonymousType(Type type)
        {
            if (!type.IsGenericType)
                return false;

            if ((type.Attributes & TypeAttributes.NotPublic) != TypeAttributes.NotPublic)
                return false;

            if (!Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false))
                return false;

            return type.Name.Contains("AnonymousType");
        }
        /// <summary>
        /// 克隆一个匿名对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object CloneAnonymousObject(object obj)
        {
            if (!IsAnonymousType(obj.GetType())) return null;

            var type = obj.GetType();
            var parameters = type.GetConstructors()[0].GetParameters();
            var args = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var propertyInfo = type.GetProperty(parameters[i].Name);
                var value = propertyInfo.GetValue(obj, null);
                args[i] = CloneObject(value);
            }

            var instance = Activator.CreateInstance(type, args);
            return instance;
        }

        /// <summary> 
        /// 计算某日起始日期（礼拜一的日期） 
        /// </summary> 
        /// <param name="someDate">该周中任意一天</param> 
        /// <returns>返回礼拜一日期，后面的具体时、分、秒和传入值相等</returns> 
        public static DateTime GetMondayDate(DateTime someDate)
        {
            int i = someDate.DayOfWeek - DayOfWeek.Monday;
            if (i == -1) i = 6;// i值 > = 0 ，因为枚举原因，Sunday排在最前，此时Sunday-Monday=-1，必须+7=6。 
            TimeSpan ts = new TimeSpan(i, 0, 0, 0);
            return someDate.Subtract(ts);
        }
        /// <summary> 
        /// 计算某日结束日期（礼拜日的日期） 
        /// </summary> 
        /// <param name="someDate">该周中任意一天</param> 
        /// <returns>返回礼拜日日期，后面的具体时、分、秒和传入值相等</returns> 
        public static DateTime GetSundayDate(DateTime someDate)
        {
            int i = someDate.DayOfWeek - DayOfWeek.Sunday;
            if (i != 0) i = 7 - i;// 因为枚举原因，Sunday排在最前，相减间隔要被7减。 
            TimeSpan ts = new TimeSpan(i, 0, 0, 0);
            return someDate.Add(ts);
        }
        /// <summary>
        /// 判断指定的类型 <paramref name="type"/> 是否是指定泛型类型的子类型，或实现了指定泛型接口。
        /// </summary>
        /// <param name="type">需要测试的类型。</param>
        /// <param name="generic">泛型接口类型，传入 typeof(IXxx&lt;&gt;)</param>
        /// <returns>如果是泛型接口的子类型，则返回 true，否则返回 false。</returns>
        public static bool IsImplementedRawGeneric(Type type,Type generic)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (generic == null) throw new ArgumentNullException(nameof(generic));

            // 测试接口。
            var isTheRawGenericType = type.GetInterfaces().Any(IsTheRawGenericType);
            if (isTheRawGenericType) return true;

            // 测试类型。
            while (type != null && type != typeof(object))
            {
                isTheRawGenericType = IsTheRawGenericType(type);
                if (isTheRawGenericType) return true;
                type = type.BaseType;
            }

            // 没有找到任何匹配的接口或类型。
            return false;

            // 测试某个类型是否是指定的原始接口。
            bool IsTheRawGenericType(Type test)
                => generic == (test.IsGenericType ? test.GetGenericTypeDefinition() : test);
        }
        ///// <summary>
        ///// 获取本机的IPv4地址
        ///// </summary>
        ///// <returns></returns>
        //public static string GetHostIPv4()
        //{
        //    string hostName = Dns.GetHostName();   //获取本机名
        //    var localhost = Dns.GetHostAddresses(hostName);
        //    if (localhost.Length > 1)
        //    {
        //        return localhost[1].ToString();
        //    }
        //    else
        //    {
        //        return "";
        //    }
        //}
        ///// <summary>
        ///// 获取本机的IPv6地址
        ///// </summary>
        ///// <returns></returns>
        //public static string GetHostIPv6()
        //{
        //    string hostName = Dns.GetHostName();   //获取本机名
        //    var localhost = Dns.GetHostAddresses(hostName);
        //    if (localhost.Length > 0)
        //    {
        //        return localhost[0].ToString();
        //    }
        //    else
        //    {
        //        return "";
        //    }
        //}
    }
}
