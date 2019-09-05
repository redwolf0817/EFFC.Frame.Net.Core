using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace EFFC.Frame.Net.Module.HttpCall
{
    /// <summary>
    /// 简易Rest服务调用Helper
    /// </summary>
    public class SimpleRestCallHelper
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SimpleRestCallHelper()
        {
            ConnectTimeOut = 600000;
            ReadWriteTimeout = 12000000;
        }
        /// <summary>
        /// 链接超时设定，单位毫秒，默认600000
        /// </summary>
        protected int ConnectTimeOut { get; set;}
        /// <summary>
        /// 读写超时设定，单位毫秒，默认12000000
        /// </summary>
        protected int ReadWriteTimeout { get; set; }
        /// <summary>
        /// 采用Get方式请求资料
        /// </summary>
        /// <param name="url"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public virtual string Get(string url,object header=null)
        {
            var rtn = "";
            var dt = DateTime.Now;
            HttpWebRequest req = null;
            try
            {
                req = (HttpWebRequest)WebRequest.Create(url);
                if (header != null)
                {
                    FrameDLRObject dheader = FrameDLRObject.CreateInstance(header, Base.Constants.FrameDLRFlags.SensitiveCase);
                    foreach (var item in dheader.Items)
                    {
                        req.Headers.Add(item.Key, ComFunc.nvl(item.Value));
                    }
                }

                req.Method = "GET";
                req.Timeout = ConnectTimeOut;
                req.ReadWriteTimeout = ReadWriteTimeout;
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                Console.WriteLine($"send cast time:{(DateTime.Now - dt).TotalMilliseconds}ms");
                StreamReader sr = new StreamReader(res.GetResponseStream(), System.Text.Encoding.UTF8);
                rtn = sr.ReadToEnd();
                sr.Close();
                res.Close();
            }
            catch (Exception ex)
            {
                if (ex is WebException)
                {
                    var rep = ((WebException)ex).Response;
                    if (rep != null)
                    {
                        var str = new StreamReader(rep.GetResponseStream(), System.Text.Encoding.UTF8).ReadToEnd();
                        rtn = str;
                    }
                    else
                    {
                        rtn = $"Failed:请求{url}失败，信息：{ex.Message}";
                    }
                }
                else
                {
                    rtn = $"Failed:请求{url}失败，信息：{ex.Message}";
                }
            }
            finally
            {
                req = null;
            }

            return rtn;
        }
        /// <summary>
        /// 执行接口发送操作,POST,PATCH,PUT操作
        /// </summary>
        /// <param name="url">请求的连接</param>
        /// <param name="data">数据，string类型，可以为json或xml或其他格式的数据文本</param>
        /// <param name="header">request的header信息</param>
        /// <param name="method">POST,PATCH,PUT</param>
        /// <param name="contentType">默认为application/json;charset=utf-8</param>
        /// <param name="encode">默认为utf8</param>
        /// <returns></returns>
        public string SendHttpRequest(string url, string data, object header = null, string method = "POST", string contentType = "application/json;charset=utf-8", Encoding encode = null)
        {
            encode = encode == null ? Encoding.UTF8 : encode;
            var rtn = "";
            var dt = DateTime.Now;
            HttpWebRequest req = null;
            try
            {
                req = (HttpWebRequest)WebRequest.Create(url);
                if (header != null)
                {
                    FrameDLRObject dheader = FrameDLRObject.CreateInstance(header, Base.Constants.FrameDLRFlags.SensitiveCase);
                    foreach (var item in dheader.Items)
                    {
                        req.Headers.Add(item.Key, ComFunc.nvl(item.Value));
                    }

                }

                byte[] requestBytes = encode.GetBytes(data);
                req.Method = method;
                req.ContentType = contentType;
                req.ContentLength = requestBytes.Length;
                req.Timeout = ConnectTimeOut;
                req.ReadWriteTimeout = ReadWriteTimeout;
                Stream requestStream = req.GetRequestStream();
                requestStream.Write(requestBytes, 0, requestBytes.Length);
                requestStream.Close();
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                Console.WriteLine($"send cast time:{(DateTime.Now - dt).TotalMilliseconds}ms");
                StreamReader sr = new StreamReader(res.GetResponseStream(), System.Text.Encoding.UTF8);
                rtn = sr.ReadToEnd();
                requestStream = null;
                sr.Close();
                res.Close();
            }
            catch (Exception ex)
            {
                if (ex is WebException)
                {
                    var rep = ((WebException)ex).Response;
                    if (rep != null)
                    {
                        var str = new StreamReader(rep.GetResponseStream(), System.Text.Encoding.UTF8).ReadToEnd();
                        rtn = str;
                    }
                    else
                    {
                        rtn = $"Failed:请求{url}失败，信息：{ex.Message}";
                    }
                }
                else
                {
                    rtn = $"Failed:请求{url}失败，信息：{ex.Message}";
                }
            }
            finally
            {
                req = null;
            }

            return rtn;
        }
        /// <summary>
        /// 执行Rest接口发送操作,POST,PATCH,PUT操作，基于json的数据结构
        /// </summary>
        /// <param name="url">目标地址</param>
        /// <param name="data">请求的参数，json格式</param>
        /// <param name="header">请求的header</param>
        /// <param name="method">请求的method,POST,PATCH,PUT</param>
        /// <returns></returns>
        public virtual string Send(string url, object data,object header=null, string method = "POST")
        {
            FrameDLRObject content = (FrameDLRObject)FrameDLRObject.CreateInstance(data, FrameDLRFlags.SensitiveCase);
            return SendHttpRequest(url, content.ToJSONString(), header, method);
        }
    }
}
