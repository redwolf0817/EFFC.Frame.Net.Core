﻿using System;
using System.Text;
using System.Net;
using System.IO;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Base.Module
{
    public abstract class HttpRemoteProxy<P, D> : IModularProxy<P, D>
        where P : ParameterStd
        where D : DataCollection
    {
        string _url = "";
        Encoding _encoding = Encoding.UTF8;
        string _requestmethod = "POST";
        string _contenttype = "text/json";
        FrameDLRObject _postdata = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
        FrameDLRObject _header = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
        X509Certificate2 _cert = null;
        IWebProxy _proxy = null;
        /// <summary>
        /// 设置请求的Encode,默认为utf8
        /// </summary>
        /// <param name="encode"></param>
        protected void SetEncoding(Encoding encode)
        {
            _encoding = encode;
        }
        /// <summary>
        /// 设置请求的Method,默认为post
        /// </summary>
        /// <param name="request_method">post,get</param>
        protected void SetRequestMethod(string request_method)
        {
            _requestmethod = request_method;
        }
        /// <summary>
        /// 设置请求的Content type,默认为application/x-www-form-urlencoded
        /// 如果method|verb为PUT时，请添加前缀“PUT/”
        /// </summary>
        /// <param name="content_type"></param>
        protected void SetContentType(string content_type)
        {
            _contenttype = content_type;
        }
        /// <summary>
        /// 设置请求的URL
        /// </summary>
        /// <param name="url"></param>
        protected void SetRequestURL(string url)
        {
            _url = url;
        }
        /// <summary>
        /// 设置ssl的数字证书
        /// </summary>
        /// <param name="cert"></param>
        protected void SetX509Certificate2(X509Certificate2 cert)
        {
            _cert = cert;
        }
        /// <summary>
        /// 设置网络代理，跟网络性能有关，设为null则为关闭代理，默认关闭代理
        /// </summary>
        /// <param name="proxy"></param>
        protected void SetProxy(IWebProxy proxy)
        {
            _proxy = proxy;
        }
        /// <summary>
        /// 添加post数据,如果contenttype为multipart/form-data则value的数据类型为FrameDLRObject，格式为
        /// {
        ///     name:参数名称
        ///     contenttype：参数数据类型，普通值类型则可不填写，文件类型则必须填为application/octet-stream
        ///     filename:只有contenttype=application/octet-stream时该栏位才有效，文件名称
        ///     formitem：数据值，如果contenttype=application/octet-stream时，则数据为byte[]
        /// }
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        protected void AddPostData(string key, object value)
        {
            _postdata.SetValue(key, value);
        }
        /// <summary>
        /// 添加header
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        protected void AddHeader(string key, string value)
        {
            _header.SetValue(key, value);
        }

        public virtual bool CallModule(P p, D d)
        {
            this.ProcessBeforeRequest(p, d);
            HttpWebRequest hr = GetRequestInstance();

            object postdatastr = GetPostDataData();
            if (_contenttype.ToLower().StartsWith("put/"))
            {
                hr.ContentType = _contenttype.Substring(4);
            }
            else
            {
                hr.ContentType = _contenttype;
            }
            
            if (postdatastr != null)
            {
                
                if (hr.Method.ToLower() == "post")
                {
                    if (_contenttype.ToLower().IndexOf("/json") > 0)
                    {
                        //提交请求
                        var t = hr.GetRequestStreamAsync();
                        Task.WaitAll(t);
                        var streamWriter = new StreamWriter(t.Result);
                        streamWriter.Write(postdatastr);
                        streamWriter.Flush();
                        streamWriter.Dispose();
                    }
                    else if (_contenttype.ToLower().IndexOf("multipart/form-data") >= 0)
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
            var responseobj = FrameDLRObject.CreateInstance();
            var contenttype = re.ContentType.ToLower();
            contenttype = contenttype.IndexOf(";") > 0 ? contenttype.Split(';')[0] : contenttype;
            responseobj.statuscode = (int)re.StatusCode;
            responseobj.contenttype = contenttype;
            if (responseobj.statuscode == 200)
            {
                if (contenttype.StartsWith("text") ||
                    contenttype.IndexOf("json") > 0)
                {
                    StreamReader sr = new StreamReader(s, _encoding);
                    responseobj.content = sr.ReadToEnd();
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
                        responseobj.contenttype = re.ContentType;
                        responseobj.filename = filename;
                        MemoryStream ms = new MemoryStream();
                        s.CopyTo(ms);
                        responseobj.content = ms.ToArray();
                    }
                    else
                    {
                        MemoryStream ms = new MemoryStream();
                        s.CopyTo(ms);
                        responseobj.content = ms.ToArray();
                    }
                }
            }
            else
            {
                if (s != null)
                {
                    MemoryStream ms = new MemoryStream();
                    s.CopyTo(ms);
                    responseobj.content = ms.ToArray();
                }
            }
            //Head处理
            FrameDLRObject header = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            foreach (string key in re.Headers.AllKeys)
            {
                header.SetValue(key, re.Headers[key]);
            }
            responseobj.header = header;
            this.ProcessAfterRequest(responseobj, p, d);
            return true;
        }

        public virtual void OnError(Exception ex, P p, D data)
        {
            throw ex;
        }

        /// <summary>
        /// 獲得一個webrequest的實例
        /// </summary>
        /// <returns></returns>
        protected HttpWebRequest GetRequestInstance()
        {
            HttpWebRequest hr = (HttpWebRequest)WebRequest.Create(new Uri(_url));
            string cookieheader = "";
            if (hr.CookieContainer != null)
                cookieheader = hr.CookieContainer.GetCookieHeader(new Uri(_url));

            CookieContainer cookieCon = new CookieContainer();
            hr.CookieContainer = cookieCon;
            hr.CookieContainer.SetCookies(new Uri(_url), cookieheader);
            hr.Method = _requestmethod;
            hr.Proxy = _proxy;
            //if (_cert != null)
            //{
            //    hr.ClientCertificates.Add(_cert);
            //}
            //添加header
            foreach (var k in _header.Keys)
            {
                if (k.ToLower() == "date")
                {
                    hr.Headers["Date"] = DateTimeStd.IsDateTime(_header.GetValue(k)) ? DateTimeStd.ParseStd(ComFunc.nvl(_header.GetValue(k))).Value.ToUniversalTime().ToString("r") : DateTime.Now.ToString("r");
                }
                else if (k.ToLower() == "content-length")
                {
                    hr.Headers["Content-Length"] = ComFunc.nvl(_header.GetValue(k));
                }
                else if (k.ToLower() == "user-agent")
                {
                    hr.Headers["User-Agent"] = ComFunc.nvl(_header.GetValue(k));
                }
                else
                {
                    hr.Headers[k] = ComFunc.nvl(_header.GetValue(k));
                }

            }

            return hr;
        }

        /// <summary>
        /// 獲得postdata的串
        /// </summary>
        /// <returns></returns>
        private object GetPostDataData()
        {
            if (_contenttype.ToLower().IndexOf("/json") > 0)
            {
                return _postdata.ToJSONString();
            }
            else if (_contenttype == "multipart/form-data")
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
                _contenttype = _contenttype + ";boundary=" + boundary;

                Stream formDataStream = new System.IO.MemoryStream();

                foreach (var k in _postdata.Keys)
                {
                    var item = _postdata.GetValue(k);
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

                            formDataStream.Write(_encoding.GetBytes(header), 0, _encoding.GetByteCount(header));

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

                            formDataStream.Write(_encoding.GetBytes(header), 0, _encoding.GetByteCount(header));
                            var bytes = Encoding.UTF8.GetBytes(formitem);
                            // Write the file data directly to the Stream, rather than serializing it to a string.
                            formDataStream.Write(bytes, 0, bytes.Length);
                        }
                    }
                }


                // Add the end of the request.  Start with a newline
                string footer = "\r\n--" + boundary + "--\r\n";
                formDataStream.Write(_encoding.GetBytes(footer), 0, _encoding.GetByteCount(footer));

                // Dump the Stream into a byte[]
                formDataStream.Position = 0;
                byte[] formData = new byte[formDataStream.Length];
                formDataStream.Read(formData, 0, formData.Length);
                formDataStream.Dispose();

                return formData;
            }//以put的方式发送文件内容
            else if (_contenttype.ToLower().StartsWith("put/"))
            {
                if (_postdata.Keys.Count > 0)
                {
                    var item = _postdata.GetValue(_postdata.Keys[0]);
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
            else if (_contenttype.ToLower().IndexOf("/xml") > 0)
            {
                return _postdata.ToXml();
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (string s in _postdata.Keys)
                {
                    sb.AppendFormat("&{0}={1}", s, _postdata.GetValue(s));
                }
                return sb.Length > 0 ? sb.ToString().Substring(1) : "";
            }

        }

        protected abstract void ProcessBeforeRequest(P p, D d);
        protected abstract void ProcessAfterRequest(FrameDLRObject responseobj, P p, D d);
    }
}
