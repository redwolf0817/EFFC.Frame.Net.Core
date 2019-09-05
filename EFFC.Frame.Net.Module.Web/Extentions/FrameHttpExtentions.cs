using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Module.Web.Parameters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace EFFC.Frame.Net.Module.Web.Extentions
{
    /// <summary>
    /// 框架中对HttpRequest的扩展
    /// </summary>
    public static class FrameHttpExtentions
    {
        /// <summary>
        /// 读取request中的参数写入webparameter中
        /// </summary>
        /// <typeparam name="WP"></typeparam>
        /// <param name="request"></param>
        /// <param name="wp"></param>
        public static void LoadEFFCParameters<WP>(this HttpRequest request, ref WP wp) where WP : WebParameter
        {
            if (wp == null) wp = Activator.CreateInstance<WP>();
            WebParameter p = wp;
            request.LoadEFFCParameters(ref p);
        }
        /// <summary>
        /// 读取request中的参数写入restparameter中
        /// </summary>
        /// <typeparam name="WP"></typeparam>
        /// <param name="request"></param>
        /// <param name="wp"></param>
        public static void LoadEFFCRestParameters<WP>(this HttpRequest request, ref WP wp) where WP : RestParameter
        {
            if (wp == null) wp = Activator.CreateInstance<WP>();
            RestParameter p = wp;
            request.LoadEFFCParameters(ref p);
        }
        /// <summary>
        /// 读取request中的参数写入RestParameter中
        /// </summary>
        /// <param name="request"></param>
        /// <param name="rp"></param>
        public static void LoadEFFCParameters(this HttpRequest request, ref RestParameter rp)
        {
            if (rp == null) rp = new RestParameter();
            rp.MethodName = request.Method.ToLower();
            var contenttype = ComFunc.nvl(request.ContentType).ToLower();
            contenttype = contenttype == "" ? "text/plain" : contenttype;
            foreach (var item in request.Query)
            {
                rp[DomainKey.QUERY_STRING, item.Key] = item.Value.ToString();
            }
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $"request contenttype:{contenttype}");
            if (request.Method.ToLower() == "post"
                || request.Method.ToLower() == "put"
                || request.Method.ToLower() == "patch"
                || request.Method.ToLower() == "delete")
            {
                using (var stream = new MemoryStream())
                {
                    request.Body.CopyTo(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    if (contenttype.IndexOf("/json") > 0)
                    {
                        var sr = new StreamReader(stream);
                        var str = sr.ReadToEnd();
                        GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $"request content:{str}");
                        if (!string.IsNullOrEmpty(str))
                        {
                            FrameDLRObject o = null;
                            FrameDLRObject.TryParse(str, FrameDLRFlags.SensitiveCase, out o);
                            if (o != null)
                            {
                                foreach (var k in o.Keys)
                                {
                                    rp[DomainKey.POST_DATA, k] = o.GetValue(k);
                                }
                            }
                        }
                        rp.RequestContent = str;
                    }
                    //xml
                    if (contenttype.IndexOf("/xml") > 0)
                    {
                        var sr = new StreamReader(stream);
                        var str = sr.ReadToEnd();
                        GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $"request content:{str}");
                        if (!string.IsNullOrEmpty(str))
                        {
                            FrameDLRObject o = FrameDLRObject.CreateInstance(str, FrameDLRFlags.SensitiveCase);
                            if (o != null)
                            {
                                foreach (var k in o.Keys)
                                {
                                    rp[DomainKey.POST_DATA, k] = o.GetValue(k);
                                }
                            }
                        }
                        rp.RequestContent = str;
                    }
                    //multipart/form-data
                    if (contenttype.IndexOf("multipart/form-data") >= 0)
                    {
                        var mp = ParseMultipartFormData(stream, Encoding.UTF8);
                        stream.Seek(0, SeekOrigin.Begin);
                        GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $"request content:{new StreamReader(stream).ReadToEnd()}");
                        foreach (var k in mp.Keys)
                        {
                            if (mp.GetValue(k) is FrameUploadFile)
                            {
                                rp[DomainKey.UPDATE_FILE, k] = mp.GetValue(k);
                            }
                            else
                            {
                                rp[DomainKey.POST_DATA, k] = mp.GetValue(k);
                            }
                        }
                    }

                    if (contenttype.StartsWith("text/"))
                    {
                        var sr = new StreamReader(stream);
                        var str = sr.ReadToEnd();
                        GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $"request content:{str}");
                        if (!string.IsNullOrEmpty(str))
                        {
                            var sarr = QueryHelpers.ParseQuery(str);
                            if (sarr != null)
                            {
                                foreach (var k in sarr)
                                {
                                    rp[DomainKey.POST_DATA, k.Key] = k.Value.ToString();
                                }
                            }
                        }
                    }

                    if (contenttype.IndexOf("application/x-www-form-urlencoded") >= 0)
                    {
                        var sr = new StreamReader(stream);
                        var str = sr.ReadToEnd();
                        GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $"request content:{str}");
                        if (!string.IsNullOrEmpty(str))
                        {
                            var arrstr = str.Split('&');
                            if (arrstr != null)
                            {
                                foreach (var s in arrstr)
                                {
                                    var items = s.Split('=');
                                    if (items != null && items.Length > 0)
                                    {
                                        rp[DomainKey.POST_DATA, items[0]] = items.Length > 1 ? ComFunc.UrlDecode(items[1]) : "";
                                    }

                                }
                            }
                        }
                        rp.RequestContent = str;
                    }
                }
            }
        }
        /// <summary>
        /// 读取request中的参数写入webparameter中
        /// </summary>
        /// <param name="request"></param>
        /// <param name="wp"></param>
        public static void LoadEFFCParameters(this HttpRequest request, ref WebParameter wp)
        {
            if (wp == null) wp = new WebParameter();
            wp.RequestMethod = request.Method;
            var contenttype = ComFunc.nvl(request.ContentType).ToLower();
            contenttype = contenttype == "" ? "text/plain" : contenttype;
            foreach (var item in request.Query)
            {
                wp[DomainKey.QUERY_STRING, item.Key] = item.Value.ToString();
            }
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $"request contenttype:{contenttype}");
            if (request.Method.ToLower() == "post"
                || request.Method.ToLower() == "put"
                || request.Method.ToLower() == "patch"
                || request.Method.ToLower() == "delete")
            {
                using (var stream = new MemoryStream())
                {
                    request.Body.CopyTo(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    if (contenttype.IndexOf("/json") > 0)
                    {
                        var sr = new StreamReader(stream);
                        var str = sr.ReadToEnd();
                        GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $"request content:{str}");
                        if (!string.IsNullOrEmpty(str))
                        {
                            FrameDLRObject o = null;
                            FrameDLRObject.TryParse(str, FrameDLRFlags.SensitiveCase, out o);
                            if (o != null)
                            {
                                foreach (var k in o.Keys)
                                {
                                    wp[DomainKey.POST_DATA, k] = o.GetValue(k);
                                }
                            }
                        }
                        wp.RequestContent = str;
                    }
                    //xml
                    if (contenttype.IndexOf("/xml") > 0)
                    {
                        var sr = new StreamReader(stream);
                        var str = sr.ReadToEnd();
                        GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $"request content:{str}");
                        if (!string.IsNullOrEmpty(str))
                        {
                            FrameDLRObject o = FrameDLRObject.CreateInstance(str, FrameDLRFlags.SensitiveCase);
                            if (o != null)
                            {
                                foreach (var k in o.Keys)
                                {
                                    wp[DomainKey.POST_DATA, k] = o.GetValue(k);
                                }
                            }
                        }
                        wp.RequestContent = str;
                    }
                    //multipart/form-data
                    if (contenttype.IndexOf("multipart/form-data") >= 0)
                    {
                        var mp = ParseMultipartFormData(stream, Encoding.UTF8);
                        stream.Seek(0, SeekOrigin.Begin);
                        GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $"request content:{new StreamReader(stream).ReadToEnd()}");
                        foreach (var k in mp.Keys)
                        {
                            if (mp.GetValue(k) is FrameUploadFile)
                            {
                                wp[DomainKey.UPDATE_FILE, k] = mp.GetValue(k);
                            }
                            else
                            {
                                wp[DomainKey.POST_DATA, k] = mp.GetValue(k);
                            }
                        }
                    }

                    if (contenttype.StartsWith("text/"))
                    {
                        var sr = new StreamReader(stream);
                        var str = sr.ReadToEnd();
                        GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $"request content:{str}");
                        if (!string.IsNullOrEmpty(str))
                        {
                            var sarr = QueryHelpers.ParseQuery(str);
                            if (sarr != null)
                            {
                                foreach (var k in sarr)
                                {
                                    wp[DomainKey.POST_DATA, k.Key] = k.Value.ToString();
                                }
                            }
                        }
                    }

                    if (contenttype.IndexOf("application/x-www-form-urlencoded") >= 0)
                    {
                        var sr = new StreamReader(stream);
                        var str = sr.ReadToEnd();
                        GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $"request content:{str}");
                        if (!string.IsNullOrEmpty(str))
                        {
                            foreach (var s in str.Split('&'))
                            {
                                var items = s.Split('=');
                                wp[DomainKey.POST_DATA, items[0]] = ComFunc.UrlDecode(items[1]);
                            }

                        }
                        wp.RequestContent = str;
                    }
                }
            }
        }
        /// <summary>
        /// 读取request中的user-agent识别出浏览器类型
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetBrowserType(this HttpRequest request)
        {
            return ComFunc.GetBrowserType(ComFunc.nvl(request.Headers["user-agent"]));
        }
        /// <summary>
        /// 读取request中的user-agent识别出浏览器版本号
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetBrowserVersion(this HttpRequest request)
        {
            return ComFunc.GetBrowserVersion(ComFunc.nvl(request.Headers["user-agent"]));
        }
        /// <summary>
        /// 执行FormData的内容解析
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static FrameDLRObject ParseMultipartFormData(Stream stream, Encoding encoding)
        {
            FrameDLRObject rtn = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);

            stream.Seek(0, SeekOrigin.Begin);
            // Read the stream into a byte array
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);
            string str = encoding.GetString(data);
            // Copy to a string for header parsing
            string content = encoding.GetString(data);

            // The first line should contain the delimiter
            int delimiterEndIndex = content.IndexOf("\r\n");

            if (delimiterEndIndex > -1)
            {
                string delimiter = content.Substring(0, content.IndexOf("\r\n"));
                int delimiterByteLength = encoding.GetByteCount(delimiter);

                string[] sections = content.Split(new string[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);

                var splitindexs = ComFunc.BytesSplit(data, delimiter, Encoding.UTF8);
                var i = 0;

                foreach (string s in sections)
                {
                    //搜索分隔喘在byte[]中所在的位置

                    if (s.Contains("Content-Disposition"))
                    {
                        // If we find "Content-Disposition", this is a valid multi-part section
                        // Now, look for the "name" parameter
                        Match nameMatch = new Regex(@"(?<=name\=\"")(.*?)(?=\"")").Match(s);
                        string name = nameMatch.Value.Trim().ToLower();

                        // Look for Content-Type
                        Regex re = new Regex(@"(?<=Content\-Type:)(.*?)(?=\r\n\r\n)");
                        Match contentTypeMatch = re.Match(s);

                        // Look for filename
                        re = new Regex(@"(?<=filename\=\"")(.*?)(?=\"")");
                        Match filenameMatch = re.Match(s);

                        if (contentTypeMatch.Success || filenameMatch.Success)
                        {

                            // Set properties
                            var contentype = contentTypeMatch.Value.Trim();
                            var filename = filenameMatch.Value.Trim();

                            // Get the start & end indexes of the file contents
                            int startIndex = contentTypeMatch.Index + contentTypeMatch.Length + "\r\n\r\n".Length;
                            var prestr = s.Substring(0, startIndex);
                            startIndex = encoding.GetByteCount(prestr) + splitindexs[i] + delimiterByteLength;
                            var endIndex = (i + 1) < splitindexs.Length ? splitindexs[i + 1] : data.Length - 1;
                            //清除首位的换行符
                            for (int j = 0; j < 2; j++)
                            {
                                if (data[startIndex] == 10 || data[startIndex] == 13)
                                {
                                    startIndex += 1;
                                }
                                if (data[endIndex - 1] == 10 || data[endIndex - 1] == 13)
                                {
                                    endIndex -= 1;
                                }
                            }

                            var contentLength = endIndex - startIndex;
                            var fileData = new byte[contentLength];
                            Array.Copy(data, startIndex, fileData, 0, fileData.Length);



                            var ms = new MemoryStream();
                            ms.Write(fileData, 0, fileData.Length);

                            var fuf = new FrameUploadFile(filename, contentype, ms, encoding);
                            rtn.SetValue(name, fuf);
                        }
                        else if (!string.IsNullOrWhiteSpace(name))
                        {
                            // Get the start & end indexes of the file contents
                            int startIndex = nameMatch.Index + nameMatch.Length + "\r\n\r\n".Length;
                            rtn.SetValue(name, s.Substring(startIndex).TrimEnd(new char[] { '\r', '\n' }).Trim());
                        }
                    }
                    i++;
                }
            }

            return rtn;
        }

        
    }
}
