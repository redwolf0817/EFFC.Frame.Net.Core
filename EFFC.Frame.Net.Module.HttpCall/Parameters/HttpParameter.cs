using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Parameter;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace EFFC.Frame.Net.Module.HttpCall.Parameters
{
    public class HttpParameter:ParameterStd
    {
        FrameDLRObject _postdata = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
        FrameDLRObject _header = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
        public HttpParameter()
        {
            ContentEncoding = Encoding.UTF8;
            RequestMethod = "POST";
            ContentType = "text/json";
            HttpWebProxy = null;
        }
        /// <summary>
        /// 要访问的url地址
        /// </summary>
        public string ToUrl
        {
            get;
            set;
        }
        /// <summary>
        /// 内容编码
        /// </summary>
        public Encoding ContentEncoding
        {
            get;
            set;
        }
        /// <summary>
        /// http method
        /// </summary>
        public string RequestMethod
        {
            get;
            set;
        }
        /// <summary>
        /// Http Content-type
        /// </summary>
        public string ContentType
        {
            get;
            set;
        }
        /// <summary>
        /// 数字证书
        /// </summary>
        public X509Certificate2 Certificate
        {
            get;
            set;
        }
        /// <summary>
        /// 网络请求代理设定
        /// </summary>
        public IWebProxy HttpWebProxy
        {
            get;
            set;
        }
        /// <summary>
        /// 要提交的数据
        /// </summary>
        public FrameDLRObject PostData
        {
            get
            {
                return _postdata;
            }
        }
        /// <summary>
        /// Http请求的header数据
        /// </summary>
        public FrameDLRObject Header
        {
            get
            {
                return _header;
            }
        }
    }
}
