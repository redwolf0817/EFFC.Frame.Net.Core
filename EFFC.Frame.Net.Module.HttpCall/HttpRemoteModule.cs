using EFFC.Frame.Net.Base.Module;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Module.HttpCall.Parameters;
using EFFC.Frame.Net.Module.HttpCall.Datas;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Constants;
using System.Net.Http;

namespace EFFC.Frame.Net.Module.HttpCall
{
    public class HttpRemoteModule : BaseModule
    {
        public override string Name => "HttpRemoteCall";

        public override string Description => "远程Http呼叫处理集成模块";

        public override bool CheckParametersAndConfig(ParameterStd p, DataCollection d)
        {
            if (!(p is HttpParameter)) return false;
            if (!(d is ResponseObject)) return false;

            var hp = (HttpParameter)p;
            if (hp.ToUrl == "") return false;

            return true;
        }

        public override void Dispose()
        {
            
        }

        protected override void OnError(Exception ex, ParameterStd p, DataCollection d)
        {
            throw ex;
        }

        protected override void Run(ParameterStd p, DataCollection d)
        {
            var hp = (HttpParameter)p;
            var hd = (ResponseObject)d;

            HttpWebRequest hr = GetRequestInstance(hp,hd);

            object postdatastr = GetPostDataData(hp, hd);
            if (hp.ContentType.ToLower().StartsWith("put/"))
            {
                hr.ContentType = hp.ContentType.Substring(4);
            }
            else
            {
                hr.ContentType = hp.ContentType;
            }

            if (postdatastr != null)
            {

                if (hr.Method.ToLower() == "post")
                {
                    if (hp.ContentType.ToLower().IndexOf("/json") > 0)
                    {
                        //提交请求
                        var t = hr.GetRequestStreamAsync();
                        Task.WaitAll(t);
                        var streamWriter = new StreamWriter(t.Result);
                        streamWriter.Write(postdatastr);
                        streamWriter.Flush();
                        streamWriter.Dispose();
                    }
                    else if (hp.ContentType.ToLower().IndexOf("multipart/form-data") >= 0)
                    {
                        byte[] postdata = (byte[])postdatastr;
                        hr.Headers["Content-Length"] = ComFunc.nvl(postdata.Length);
                        //提交请求
                        var t = hr.GetRequestStreamAsync();
                        Task.WaitAll(t);
                        var streamWriter = t.Result;
                        streamWriter.Write(postdata, 0, postdata.Length);
                        streamWriter.Dispose();
                    }
                    else
                    {
                        byte[] postdata = (byte[])postdatastr;
                        hr.Headers["Content-Length"] = ComFunc.nvl(postdata.Length);
                        //提交请求
                        var t = hr.GetRequestStreamAsync();
                        Task.WaitAll(t);
                        Stream stream;
                        stream = t.Result;
                        //提交请求
                        stream.Write(postdata, 0, postdata.Length);
                        stream.Dispose();
                    }
                }
                else if (hr.Method.ToLower() == "put")
                {
                    if (postdatastr != null && postdatastr is byte[])
                    {
                        var postdatabyte = (byte[])postdatastr;
                        Stream stream;
                        var t = hr.GetRequestStreamAsync();
                        Task.WaitAll(t);
                        stream = t.Result;
                        stream.Write(postdatabyte, 0, postdatabyte.Length);
                        stream.Flush();
                        stream.Dispose();
                    }
                }


            }

            HttpWebResponse re = null;
            try
            {
                var rt = hr.GetResponseAsync();
                Task.WaitAll(rt);
                re = rt.Result as HttpWebResponse;
            }
            catch (WebException ex)
            {
                re = ex.Response as HttpWebResponse;
            }
            var s = re.GetResponseStream();
            var contenttype = re.ContentType.ToLower();
            contenttype = contenttype.IndexOf(";") > 0 ? contenttype.Split(';')[0] : contenttype;
            hd.StatusCode = (int)re.StatusCode;
            hd.ContentType = contenttype;
            if (hd.StatusCode == 200)
            {
                if (contenttype.StartsWith("text") ||
                    contenttype.IndexOf("json") > 0)
                {
                    StreamReader sr = new StreamReader(s, hp.ContentEncoding);
                    hd.Result = sr.ReadToEnd();
                }
                else
                {
                    if (contenttype.StartsWith("image")
                        || contenttype.StartsWith("audio")
                        || contenttype.StartsWith("video")
                        || contenttype == ResponseHeader_ContentType.doc
                        || contenttype == ResponseHeader_ContentType.xls
                        || contenttype == ResponseHeader_ContentType.xlsx
                        || contenttype == ResponseHeader_ContentType.pdf)
                    {
                        var cd = ComFunc.UrlDecode(ComFunc.nvl(re.Headers["Content-Disposition"]));
                        var filename = cd.ToLower().Replace("attachment;", "").Replace("filename=", "").Replace("\"", "").Trim();
                        hd.FileName = filename;
                        MemoryStream ms = new MemoryStream();
                        s.CopyTo(ms);
                        hd.Result = ComFunc.StreamToBytes(ms);
                    }
                    else
                    {
                        MemoryStream ms = new MemoryStream();
                        s.CopyTo(ms);
                        hd.Result = ComFunc.StreamToBytes(ms);
                    }
                }
            }
            else
            {
                if (s != null)
                {
                    if (contenttype.StartsWith("text") ||
                    contenttype.IndexOf("json") > 0)
                    {
                        StreamReader sr = new StreamReader(s, hp.ContentEncoding);
                        hd.Result = sr.ReadToEnd();
                    }
                    else
                    {
                        MemoryStream ms = new MemoryStream();
                        s.CopyTo(ms);
                        hd.Result = ms.ToArray();
                    }
                }
            }
            //Head处理
            foreach (string key in re.Headers.AllKeys)
            {
                hd.Header.SetValue(key, re.Headers[key]);
            }

            ProcessAfterRequest(hp, hd);
        }
        /// <summary>
        /// 请求后的特殊处理
        /// </summary>
        /// <param name="hp"></param>
        /// <param name="hd"></param>
        protected virtual void ProcessAfterRequest(HttpParameter hp,ResponseObject hd)
        {
            //do nothing
        }
        /// <summary>
        /// 发起request请求之前的处理
        /// </summary>
        /// <param name="hp"></param>
        /// <param name="hd"></param>
        protected virtual void ProcessBeforeRequest(HttpParameter hp, ResponseObject hd)
        {
            //do nothing
        }
        /// <summary>
        /// 獲得一個webrequest的實例
        /// </summary>
        /// <returns></returns>
        protected HttpWebRequest GetRequestInstance(HttpParameter hp, ResponseObject hd)
        {
            var handler = new HttpClientHandler();

            HttpWebRequest hr = (HttpWebRequest)WebRequest.Create(new Uri(hp.ToUrl));
            string cookieheader = "";
            if (hr.CookieContainer != null)
                cookieheader = hr.CookieContainer.GetCookieHeader(new Uri(hp.ToUrl));

            CookieContainer cookieCon = new CookieContainer();
            hr.CookieContainer = cookieCon;
            hr.CookieContainer.SetCookies(new Uri(hp.ToUrl), cookieheader);
            hr.Method = hp.RequestMethod;
            hr.Proxy = hp.HttpWebProxy;
            //if (_cert != null)
            //{
            //    hr.ClientCertificates.Add(_cert);
            //}
            //添加header
            foreach (var k in hp.Header.Keys)
            {
                if (k.ToLower() == "date")
                {
                    hr.Headers["Date"] = DateTimeStd.IsDateTime(hp.Header.GetValue(k)) ? DateTimeStd.ParseStd(ComFunc.nvl(hp.Header.GetValue(k))).Value.ToUniversalTime().ToString("r") : DateTime.Now.ToString("r");
                }
                else if (k.ToLower() == "content-length")
                {
                    hr.Headers["Content-Length"] = ComFunc.nvl(hp.Header.GetValue(k));
                }
                else if (k.ToLower() == "user-agent")
                {
                    hr.Headers["User-Agent"] = ComFunc.nvl(hp.Header.GetValue(k));
                }
                else
                {
                    hr.Headers[k] = ComFunc.nvl(hp.Header.GetValue(k));
                }

            }


            return hr;
        }

        /// <summary>
        /// 獲得postdata的串
        /// </summary>
        /// <returns></returns>
        private object GetPostDataData(HttpParameter hp, ResponseObject hd)
        {
            if (hp.ContentType.ToLower().IndexOf("/json") > 0)
            {
                return hp.PostData.ToJSONString();
            }
            else if (hp.ContentType == "multipart/form-data")
            {
                /*
                 *multipart/form-data的请求内容格式如下
Content-Type: multipart/form-data; boundary=AaB03x  
  
 --boundary  
 Content-Disposition: form-data; name="submit-name"  
  
 Larry  
 --boundary 
 Content-Disposition: form-data; name="file"; filename="file1.dat"  
 Content-Type: application/octet-stream  
  
 ... contents of file1.dat ...  
 --boundary--  
                 * */

                var boundary = String.Format("----------{0:N}", Guid.NewGuid());
                var _contenttype = hp.ContentType + ";boundary=" + boundary;

                Stream formDataStream = new System.IO.MemoryStream();

                foreach (var k in hp.PostData.Keys)
                {
                    var item = hp.PostData.GetValue(k);
                    if (item is FrameDLRObject)
                    {
                        dynamic dobj = item;
                        if (ComFunc.nvl(dobj.contenttype) == "application/octet-stream")
                        {
                            var name = ComFunc.nvl(dobj.name);
                            var filename = ComFunc.nvl(dobj.filename);
                            var filecontenttype = ComFunc.nvl(dobj.filecontenttype);
                            var filecontent = (byte[])dobj.formitem;

                            // Add just the first part of this param, since we will write the file data directly to the Stream
                            string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                                boundary,
                                name,
                                filename,
                                filecontenttype != "" ? filecontenttype : "application/octet-stream");

                            formDataStream.Write(hp.ContentEncoding.GetBytes(header), 0, hp.ContentEncoding.GetByteCount(header));

                            // Write the file data directly to the Stream, rather than serializing it to a string.
                            formDataStream.Write(filecontent, 0, filecontent.Length);
                        }
                        else
                        {
                            var name = ComFunc.nvl(dobj.name);
                            var formitem = ComFunc.nvl(dobj.formitem);
                            // Add just the first part of this param, since we will write the file data directly to the Stream
                            string header = string.Format("--{0}\r\nContent-Disposition: form-data; name={1}\r\n\r\n",
                                boundary,
                                name);

                            formDataStream.Write(hp.ContentEncoding.GetBytes(header), 0, hp.ContentEncoding.GetByteCount(header));
                            var bytes = Encoding.UTF8.GetBytes(formitem);
                            // Write the file data directly to the Stream, rather than serializing it to a string.
                            formDataStream.Write(bytes, 0, bytes.Length);
                        }
                    }
                }


                // Add the end of the request.  Start with a newline
                string footer = "\r\n--" + boundary + "--\r\n";
                formDataStream.Write(hp.ContentEncoding.GetBytes(footer), 0, hp.ContentEncoding.GetByteCount(footer));

                // Dump the Stream into a byte[]
                formDataStream.Position = 0;
                byte[] formData = new byte[formDataStream.Length];
                formDataStream.Read(formData, 0, formData.Length);
                formDataStream.Dispose();

                return formData;
            }//以put的方式发送文件内容
            else if (hp.ContentType.ToLower().StartsWith("put/"))
            {
                if (hp.PostData.Keys.Count > 0)
                {
                    var item = hp.PostData.GetValue(hp.PostData.Keys[0]);
                    if (item is FrameDLRObject)
                    {
                        dynamic dobj = item;
                        //只处理第一笔资料
                        //var name = ComFunc.nvl(dobj.name);
                        //var filename = ComFunc.nvl(dobj.filename);
                        //var filecontenttype = ComFunc.nvl(dobj.filecontenttype);
                        var filecontent = (byte[])dobj.formitem;

                        return filecontent;
                    }
                    else if (item is byte[])
                    {
                        return (byte[])item;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else if (hp.ContentType.ToLower().IndexOf("/xml") > 0)
            {
                return hp.PostData.ToXml();
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (string s in hp.PostData.Keys)
                {
                    sb.AppendFormat("&{0}={1}", s, hp.PostData.GetValue(s));
                }
                return sb.Length > 0 ? sb.ToString().Substring(1) : "";
            }

        }
    }
}
