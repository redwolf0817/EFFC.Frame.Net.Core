using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Parameter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EFFC.Frame.Net.Base.Module
{
    public abstract class HttpRemoteAsyncProxy<P, D> : IModularAsyncProxy<P, D>
        where P : ParameterStd
        where D : DataCollection
    {
        
        string _url = "";
        Encoding _encoding = Encoding.UTF8;
        string _requestmethod = "post";
        string _contenttype = "application/x-www-form-urlencoded";
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

        public bool BeginCallModule(P p, D d, Action<P, D> callback)
        {
            try
            {
                this.ProcessBeforeRequest(p, d);
                //尝试解决速度慢的问题
                System.Net.ServicePointManager.DefaultConnectionLimit = 100;
                HttpWebRequest hr = GetRequestInstance();

                if (_contenttype.ToLower().StartsWith("put/"))
                {
                    hr.ContentType = _contenttype.Substring(4);
                }
                else
                {
                    hr.ContentType = _contenttype;
                }
                hr.Method = _requestmethod;

                FrameDLRObject requeststate = FrameDLRObject.CreateInstance();
                requeststate.SetValue("request", hr);
                requeststate.SetValue("p", p);
                requeststate.SetValue("d", d);
                requeststate.SetValue("callback", callback);
                var dt1 = DateTime.Now;
                var async = hr.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), requeststate);
                var dt2 = DateTime.Now;
                p.ExtentionObj.asynccallcost = (dt2 - dt1).TotalMilliseconds;
                p.ExtentionObj.async = async;
                return true;
            }
            finally
            {
                p.Resources.ReleaseAll();
            }
        }

       

        private void GetRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            FrameDLRObject stateobj = (FrameDLRObject)asynchronousResult.AsyncState;
            P p = (P)stateobj.GetValue("p");
            D d = (D)stateobj.GetValue("d");
            HttpWebRequest request = (HttpWebRequest)stateobj.GetValue("request");
            Stream postStream = null;
            try
            {

                // End the operation
                postStream = request.EndGetRequestStream(asynchronousResult);
                object postdatastr = GetPostDataData();

                if (request.Method.ToLower() == "post")
                {
                    if (postdatastr != null)
                    {
                        request.ContentType = _contenttype;
                        if (_contenttype.ToLower().IndexOf("/json") > 0)
                        {
                            //提交请求
                            var streamWriter = new StreamWriter(postStream);
                            streamWriter.Write(postdatastr);
                            streamWriter.Flush();
                        }
                        else if (_contenttype.ToLower().IndexOf("multipart/form-data") > 0)
                        {
                            byte[] postdata = (byte[])postdatastr;
                            //request.ContentLength = postdata.Length;
                            //提交请求
                            postStream.Write(postdata, 0, postdata.Length);
                            postStream.Flush();
                        }
                        else
                        {
                            byte[] postdatabyte = _encoding.GetBytes(postdatastr.ToString());
                            //request.ContentLength = postdatabyte.Length;
                            //提交请求
                            postStream.Write(postdatabyte, 0, postdatabyte.Length);
                            postStream.Flush();
                        }


                    }
                }
                else if (request.Method.ToLower() == "put")
                {
                    if (postdatastr != null && postdatastr is byte[])
                    {
                        var postdatabyte = (byte[])postdatastr;
                        postStream.Write(postdatabyte, 0, postdatabyte.Length);
                        postStream.Flush();
                    }
                }
                // Start the asynchronous operation to get the response
                request.BeginGetResponse(new AsyncCallback(GetResponseCallback), stateobj);
            }
            catch (Exception ex)
            {
                OnError(ex, p, d);
            }
            finally
            {
                if (postStream != null)
                {
                    postStream.Close();
                }

                p.Resources.ReleaseAll();
            }
        }

        private void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            FrameDLRObject stateobj = (FrameDLRObject)asynchronousResult.AsyncState;
            HttpWebRequest request = (HttpWebRequest)stateobj.GetValue("request");
            P p = (P)stateobj.GetValue("p");
            D d = (D)stateobj.GetValue("d");
            Action<P, D> callbakc = (Action<P, D>)stateobj.GetValue("callback");
            HttpWebResponse re = null;
            Stream streamResponse = null;
            StreamReader streamRead = null;
            // End the operation
            try
            {
                try
                {
                    re = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
                }
                catch (WebException ex)
                {
                    re = ex.Response as HttpWebResponse;
                }
                
                var s = re.GetResponseStream();
                var responseobj = FrameDLRObject.CreateInstance();
                var contenttype = re.ContentType.ToLower();
                contenttype = contenttype.IndexOf(";") > 0 ? contenttype.Split(';')[0] : contenttype;
                responseobj.contenttype = contenttype;
                responseobj.statuscode = (int)re.StatusCode;
                if (responseobj.statuscode == 200)
                {
                    if (contenttype.ToLower().StartsWith("text") ||
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
                            var cd = HttpUtility.UrlDecode(ComFunc.nvl(re.Headers.Get("Content-Disposition")), Encoding.UTF8);
                            var filename = cd.ToLower().Replace("attachment;", "").Replace("filename=", "").Replace("\"","").Trim();
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
                foreach (string key in re.Headers.Keys)
                {
                    header.SetValue(key, re.Headers[key]);
                }
                responseobj.header = header;
                this.ProcessAfterRequest(responseobj, p, d, callbakc);
            }
            catch (Exception ex)
            {
                OnError(ex, p, d);
            }
            finally
            {
                if (streamResponse != null)
                {
                    streamResponse.Close();
                }
                if (streamRead != null)
                {
                    streamRead.Close();
                }
                if (re != null)
                {
                    re.Close();
                }

                p.Resources.ReleaseAll();
            }
        }

        public virtual void OnError(Exception ex, P p, D data)
        {
            foreach (var t in p.TransTokenList.Items)
            {
                p.Resources.RollbackTransaction(t);
            }
        }

        /// <summary>
        /// 獲得一個webrequest的實例
        /// </summary>
        /// <returns></returns>
        protected virtual HttpWebRequest GetRequestInstance()
        {
            HttpWebRequest hr = (HttpWebRequest)WebRequest.Create(new Uri(_url));
            string cookieheader = "";
            if (hr.CookieContainer != null)
                cookieheader = hr.CookieContainer.GetCookieHeader(new Uri(_url));

            CookieContainer cookieCon = new CookieContainer();
            hr.CookieContainer = cookieCon;
            hr.CookieContainer.SetCookies(new Uri(_url), cookieheader);

            hr.KeepAlive = false;
            hr.Method = _requestmethod;
            hr.Proxy = _proxy;
            if (_cert != null)
            {
                hr.ClientCertificates.Add(_cert);
            }
            //添加header
            foreach (var k in _header.Keys)
            {
                if (k.ToLower() == "date")
                {
                    hr.Date = DateTimeStd.IsDateTime(_header.GetValue(k)) ? DateTimeStd.ParseStd(ComFunc.nvl(_header.GetValue(k))).Value : DateTime.Now;
                }
                else if (k.ToLower() == "content-length")
                {
                    hr.ContentLength = long.Parse(ComFunc.nvl(_header.GetValue(k)));
                }
                else if (k.ToLower() == "user-agent")
                {
                    hr.UserAgent = ComFunc.nvl(_header.GetValue(k));
                }
                else
                {
                    if (ComFunc.nvl(hr.Headers[k]) != "")
                    {
                        hr.Headers[k] = ComFunc.nvl(_header.GetValue(k));
                    }
                    else
                    {
                        hr.Headers.Add(k, ComFunc.nvl(_header.GetValue(k)));
                    }
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
                formDataStream.Close();

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
        protected abstract void ProcessAfterRequest(FrameDLRObject responsestring, P p, D d, Action<P, D> callback);

        /// <summary>
        /// 结束处理-不做任何处理动作
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public virtual bool EndCallModule(P p, D d)
        {
            //do nothing
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="data"></param>
        public void WaitMe(P p, D data)
        {
            if (p.ExtentionObj.async != null && p.ExtentionObj.async is IAsyncResult)
            {
                ((IAsyncResult)p.ExtentionObj.async).AsyncWaitHandle.WaitOne();
            }
        }
    }
}
