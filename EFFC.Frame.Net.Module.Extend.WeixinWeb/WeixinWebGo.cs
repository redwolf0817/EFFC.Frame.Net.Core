using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Module.Extend.WebGo.Datas;
using EFFC.Frame.Net.Module.Web.Parameters;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using System.IO;
using System.Xml;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Base.Parameter;
using Microsoft.AspNetCore.Http;
using EFFC.Frame.Net.Base.Module.Proxy;
using System.Security.Cryptography;
using EFFC.Frame.Net.Module.Extend.WeChat;

namespace EFFC.Frame.Net.Module.Extend.WeixinWeb
{
    public class WeixinWebGo: WebGo.WebGo
    {
        static Dictionary<string, FrameDLRObject> logmsg = new Dictionary<string, FrameDLRObject>();
        static object lockobj = new object();
        static string weixinhome = "weixinhome";
        static string weixinroothome = "~/";

        protected override void OnUsed(ProxyManager ma, dynamic options)
        {
            if (options != null)
            {
                if (ComFunc.nvl(options.WeixinHome) != "")
                {
                    weixinhome = options.WeixinHome;
                }
                if (ComFunc.nvl(options.WeixinRootHome) != "")
                {
                    weixinroothome = options.WeixinRootHome;
                }
            }

            GlobalCommon.Logger.WriteLog(LoggerLevel.INFO,
                        string.Format("WeixinWebGo起始页WeixinHome设定为{0},如要调整，请在ProxyManager.UseProxy中的options参数设定WeixinHome的值", weixinhome));
            GlobalCommon.Logger.WriteLog(LoggerLevel.INFO,
                        string.Format("WeixinWebGo的RootHome设定为{0},如要调整，请在ProxyManager.UseProxy中的options参数设定WeixinRootHome的值", weixinroothome));
            //添加微信远程呼叫模块
            DoAddProxy(ma, options);
        }
        protected override void LoadConfig(WebParameter p, GoData d)
        {
            base.LoadConfig(p, d);
            bool bvalue = true;
            foreach (var item in MyConfig.GetConfigurationList("Weixin"))
            {
                if (bool.TryParse(ComFunc.nvl(item.Value), out bvalue))
                {
                    p[DomainKey.CONFIG, item.Key] = bool.Parse(ComFunc.nvl(item.Value));
                }
                else if (DateTimeStd.IsDateTime(item.Value))
                {
                    p[DomainKey.CONFIG, item.Key] = DateTimeStd.ParseStd(item.Value).Value;
                }
                else
                {
                    p[DomainKey.CONFIG, item.Key] = ComFunc.nvl(item.Value);
                }
            }
        }
        protected override void ProcessRequestPath(WebParameter p, GoData d)
        {
            var context = p.CurrentHttpContext;
            //抓取请求资源
            p.RequestResourcePath = ComFunc.nvl(context.Request.Path).Replace("\\", "/").Replace(weixinroothome.Replace("~", ""), "");
            if (p.RequestResourceName == "" && p.RequestResourcePath == "/")
            {
                p.RequestResourcePath = p.RequestResourcePath + weixinhome;
            }

            var ext = Path.GetExtension(p.RequestResourcePath);
            if (ext != "")
            {
                string reqpath = Path.GetFileNameWithoutExtension(p.RequestResourcePath);
                string[] ss = reqpath.Split('.');
                p.Action = ss.Length > 1 ? ss[1] : "";
                p.RequestResourceName = ss.Length > 0 ? ss[0] : "";
                p.RequestResources = ss;
            }
            else
            {
                var turl = p.RequestResourcePath.Replace("~", "");
                turl = turl.StartsWith("/") ? turl.Substring(1) : turl;
                string[] ss = turl.Split('/');
                p.Action = ss.Length > 1 ? ss[1] : "";
                p.RequestResourceName = ss.Length > 0 ? ss[0] : "";
                p.RequestResources = ss;
            }
        }
        protected override void ProcessRequestInfo(WebParameter p, GoData d)
        {
            base.ProcessRequestInfo(p, d); ProcessRequestInfoWeixin(p, d);
        }

        protected virtual void ProcessRequestInfoWeixin(WebParameter p, GoData d)
        {
            //微信相关信息
            p.ExtentionObj.weixin = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            p.ExtentionObj.weixin.signature = ComFunc.nvl(p[DomainKey.QUERY_STRING, "signature"]);
            p.ExtentionObj.weixin.timestamp = ComFunc.nvl(p[DomainKey.QUERY_STRING, "timestamp"]);
            p.ExtentionObj.weixin.nonce = ComFunc.nvl(p[DomainKey.QUERY_STRING, "nonce"]);
            p.ExtentionObj.weixin.token = ComFunc.nvl(p[DomainKey.CONFIG, "weixin_token"]);
            p.ExtentionObj.weixin.encrypt_type = ComFunc.nvl(p[DomainKey.QUERY_STRING, "encrypt_type"]);
            p.ExtentionObj.weixin.encrypt_key = ComFunc.nvl(p[DomainKey.CONFIG, "weixin_encry_key"]);
            p.ExtentionObj.weixin.appid = ComFunc.nvl(p[DomainKey.CONFIG, "weixin_Appid"]);
            p.ExtentionObj.weixin.appsecret = ComFunc.nvl(p[DomainKey.CONFIG, "weixin_Appsecret"]);
            p.ExtentionObj.weixin.weixin_mch_ssl_path = ComFunc.nvl(p[DomainKey.CONFIG, "weixin_Mch_SSL_Path"]);
            p.ExtentionObj.weixin.weixin_mch_ssl_pass = ComFunc.nvl(p[DomainKey.CONFIG, "weixin_Mch_SSL_Pass"]);

            p.SetValue("logkey", DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            string content = string.Empty;
            if (CurrentContext.Request.Method.ToLower() == "post")
            {
                content = p.RequestContent;

                //如果内容为aes加密
                if (p.ExtentionObj.weixin.encrypt_type == "aes")
                {
                    WXBizMsgCrypt wxcpt = new WXBizMsgCrypt(p.ExtentionObj.weixin.token, p.ExtentionObj.weixin.encrypt_key, p.ExtentionObj.weixin.appid);
                    string msg = "";
                    var result = wxcpt.DecryptMsg(p.ExtentionObj.weixin.signature, p.ExtentionObj.weixin.timestamp, p.ExtentionObj.weixin.nonce, content, ref msg);
                    content = msg;
                }
                var contentobj = FrameDLRObject.IsXmlThen(content, null, FrameDLRFlags.SensitiveCase);

                if (contentobj != null)
                {
                    var root = (FrameDLRObject)contentobj.GetValue("xml");
                    foreach (var item in root.Items)
                    {
                        if (item.Key == "CreateTime")
                            p[DomainKey.POST_DATA, item.Key] = new DateTime(1970, 1, 1).AddSeconds(IntStd.IsNotIntThen(item.Value));
                        else
                            p[DomainKey.POST_DATA, item.Key] = item.Value;

                        if (item.Value is FrameDLRObject)
                        {
                            foreach (var sub in ((FrameDLRObject)item.Value).Items)
                            {
                                if (sub.Key == "CreateTime")
                                    p[DomainKey.POST_DATA, sub.Key] = new DateTime(1970, 1, 1).AddSeconds(IntStd.IsNotIntThen(sub.Value));
                                else
                                    p[DomainKey.POST_DATA, sub.Key] = sub.Value;
                            }
                        }

                    }
                }
                //微信推送过来的xml对象
                p.ExtentionObj.weixin.RecieveXMLObject = contentobj;
                //事件触发时的action处理
                if (ComFunc.nvl(p[DomainKey.POST_DATA, "MsgType"]) == "event")
                {
                    p.Action = "event_" + ComFunc.nvl(p[DomainKey.POST_DATA, "Event"]);
                }
                else
                {
                    //普通消息处理，action为消息类型
                    p.Action = "msg_" + ComFunc.nvl(p[DomainKey.POST_DATA, "MsgType"]);
                }
            }
            else
            {
                //action为api_valid的时候为微信服务器的验证请求
                p.Action = "api_valid";
            }
        }

        protected override void InvokeAction(WebParameter p, GoData d)
        {
            if (IsValid4Invoke(p,d))
                base.InvokeAction(p, d);

        }
        /// <summary>
        /// 业务逻辑执行之前的检核判定，如果为true，则执行业务逻辑模块，否则不执行
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        protected virtual bool IsValid4Invoke(WebParameter p,GoData d)
        {
            var isrun = true;
            if (!IsWeixinSignValid(p, d))
            {
                isrun = false;
            }
            return isrun;
        }
        protected override void FinishedProcess(WebParameter p, GoData d)
        {
            base.FinishedProcess(p, d);
            var logkey = ComFunc.nvl(p.GetValue("logkey"));
            if (logmsg.ContainsKey(logkey))
            {
                var dobj = (FrameDLRObject)logmsg[logkey];
                DebugLog(string.Format("标号{0}微信请求处理记录：\n{1}", logkey, dobj.ToJSONString()), p);
            }
        }
        protected override void OnError(Exception ex, ParameterStd p, DataCollection d)
        {
            //GlobalCommon.ExceptionProcessor.ProcessException(this, ex, p, d);
            string errorCode = "E-" + ComFunc.nvl(p[DomainKey.CONFIG, "Machine_No"]) + "-" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string errlog = "";
            if (ex.InnerException != null)
            {
                errlog = string.Format("错误编号：{0}，\n{1}\n{2}\nInnerException:{3}\n{4}", errorCode, ex.Message, ex.StackTrace, ex.InnerException.Message, ex.InnerException.StackTrace);
            }
            else
            {
                errlog = string.Format("错误编号：{0}，\n{1}\n{2}", errorCode, ex.Message, ex.StackTrace);
            }
            GlobalCommon.Logger.WriteLog(LoggerLevel.ERROR, errlog);

            var errormsg = "";
            var isdebug = p[DomainKey.CONFIG, "DebugMode"] == null ? false : (bool)p[DomainKey.CONFIG, "DebugMode"];
            if (isdebug)
            {
                errormsg = string.Format("出错了，{0}", errlog); ;
            }
            else
            {
                errormsg = string.Format("系统出错了，亲，请将错误编号（{0}）告知我们，我们会帮亲处理的哦！", errorCode);
            }
            var logkey = ComFunc.nvl(p.GetValue("logkey"));
            if (logmsg.ContainsKey(logkey))
            {
                var msgobj = (FrameDLRObject)logmsg[logkey];
                DebugLog(string.Format("标号{0}微信请求处理记录：\n{1}", logkey, msgobj.ToJSONString()), (WebParameter)p);
            }


            p.Resources.RollbackTransaction(p.CurrentTransToken);
            p.Resources.ReleaseAll();

            var dobj = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            dobj.ToUserName = p[DomainKey.POST_DATA, "FromUserName"];
            dobj.FromUserName = p[DomainKey.POST_DATA, "ToUserName"];
            dobj.CreateTime = DateTime.Now;
            dobj.MsgType = "text";
            dobj.Content = errormsg;
            dobj.FuncFlag = 0;
            var content = ToXml(dobj);
            //如果内容为aes加密
            if (p.ExtentionObj.weixin.encrypt_type == "aes")
            {
                DateTime createTime = dobj.CreateTime;
                int timeStamp = ToWeixinTime(createTime);
                Random random = new Random();
                string nonce = random.Next().ToString();

                WXBizMsgCrypt wxcpt = new WXBizMsgCrypt(p.ExtentionObj.weixin.token, p.ExtentionObj.weixin.encrypt_key, p.ExtentionObj.weixin.appid);
                string xmlEncrypt = "";
                //加密消息
                if (wxcpt.EncryptMsg(content, timeStamp.ToString(), nonce, ref xmlEncrypt) == WXBizMsgCrypt.WXBizMsgCryptErrorCode.WXBizMsgCrypt_OK)
                    content = xmlEncrypt;
            }
            var msgbytelength = Encoding.UTF8.GetByteCount(content);
            CurrentContext.Response.Headers.Add("Content-Length", new Microsoft.Extensions.Primitives.StringValues(msgbytelength + ""));
            CurrentContext.Response.ContentType = ResponseHeader_ContentType.xml + ";charset=utf-8";
            CurrentContext.Response.WriteAsync((string)content);
        }

        protected override void SetResponseContent(WebParameter p, GoData d)
        {
            if (d.ResponseData is FrameDLRObject)
            {
                
                var re = (FrameDLRObject)d.ResponseData;

                var content = ToXml(re);
                
                if (p.ExtentionObj.weixin.encrypt_type == "aes")
                {
                    var createTime = re.GetValue("CreateTime") == null ? DateTime.Now : (DateTime)re.GetValue("CreateTime");
                    int timeStamp = ToWeixinTime(createTime);
                    Random random = new Random();
                    string nonce = random.Next().ToString();

                    WXBizMsgCrypt wxcpt = new WXBizMsgCrypt(p.ExtentionObj.weixin.token, p.ExtentionObj.weixin.encrypt_key, p.ExtentionObj.weixin.appid);
                    string xmlEncrypt = "";
                    //加密消息
                    if (wxcpt.EncryptMsg(content, timeStamp.ToString(), nonce, ref xmlEncrypt) == WXBizMsgCrypt.WXBizMsgCryptErrorCode.WXBizMsgCrypt_OK)
                        content = xmlEncrypt;

                }
                
                var msgbytelength = Encoding.UTF8.GetByteCount(content);
                CurrentContext.Response.Headers.Add("Content-Length", msgbytelength + "");
                CurrentContext.Response.ContentType = ResponseHeader_ContentType.xml + ";charset=utf-8";
                CurrentContext.Response.StatusCode = 200;
                CurrentContext.Response.WriteAsync(content);
                
            }
            else
            {
                var msgbytelength = Encoding.UTF8.GetByteCount(ComFunc.nvl(d.ResponseData));
                CurrentContext.Response.Headers.Add("Content-Length", msgbytelength + "");
                if (d.ContentType == GoResponseDataType.String)
                {
                    CurrentContext.Response.ContentType = ResponseHeader_ContentType.html + ";charset=utf-8";
                }
                else
                {
                    CurrentContext.Response.ContentType = ResponseHeader_ContentType.xml + ";charset=utf-8";
                }
                CurrentContext.Response.StatusCode = 200;
                CurrentContext.Response.WriteAsync(ComFunc.nvl(d.ResponseData));
            }

        }
        private void DebugLog(string msg,WebParameter p)
        {
            var isdebug = p[DomainKey.CONFIG, "DebugMode"] == null ? false : (bool)p[DomainKey.CONFIG, "DebugMode"];
            if (isdebug)
            {
                GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, msg);
            }
        }
        
        private bool IsWeixinSignValid(WebParameter p,GoData d)
        {
            var rtn = true;
            string token = p.ExtentionObj.weixin.token;
            string signature = p.ExtentionObj.weixin.signature;
            string timestamp = p.ExtentionObj.weixin.timestamp;
            string nonce = p.ExtentionObj.weixin.nonce;
            if (string.IsNullOrWhiteSpace(signature) || string.IsNullOrWhiteSpace(timestamp) || string.IsNullOrWhiteSpace(nonce))
            {
                rtn = false;
                var dobj = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                dobj.ToUserName = p[DomainKey.POST_DATA, "FromUserName"];
                dobj.FromUserName = p[DomainKey.POST_DATA, "ToUserName"];
                dobj.CreateTime = DateTime.Now;
                dobj.MsgType = "text";
                dobj.Content = "检验无效，微信请求参数不正确";
                dobj.FuncFlag = 0;
                d.ResponseData = dobj;
            }
            else
            {
                string[] infos = new string[] { token, timestamp, nonce };
                Array.Sort<string>(infos);
                string info = string.Format("{0}{1}{2}", infos[0], infos[1], infos[2]);
                rtn = string.Compare(signature, GetSha1Hash(info, Encoding.ASCII), true) == 0;
                if (!rtn)
                {
                    var dobj = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                    dobj.ToUserName = p[DomainKey.POST_DATA, "FromUserName"];
                    dobj.FromUserName = p[DomainKey.POST_DATA, "ToUserName"];
                    dobj.CreateTime = DateTime.Now;
                    dobj.MsgType = "text";
                    dobj.Content = "检验无效，不是微信访问接口";
                    dobj.FuncFlag = 0;
                    d.ResponseData = dobj;
                }
            }

            return rtn;
        }
        private string GetSha1Hash(string input, Encoding encoding)
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(input) && encoding != null)
            {
                byte[] bytes = encoding.GetBytes(input);
                //SHA1 sha1 = new SHA1CryptoServiceProvider();
                SHA1 sha1 = SHA1.Create();
                byte[] result = sha1.ComputeHash(bytes);
                foreach (byte b in result)
                    sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }
        private string ToXml(FrameDLRObject obj)
        {

            XmlDocument doc = ComFunc.GetSafeXmlInstance();
            XmlElement root = doc.CreateElement("xml");
            doc.AppendChild(root);
            foreach (var k in obj.Keys)
            {
                ToXmlElem(doc, root, k, obj.GetValue(k));
            }
            return doc.InnerXml;
        }

        private void ToXmlElem(XmlDocument doc, XmlElement parent, string name, object obj)
        {


            if (obj is int
                    || obj is long
                    || obj is double
                    || obj is float
                    || obj is decimal)
            {
                XmlElement elem = doc.CreateElement(name);
                elem.AppendChild(doc.CreateTextNode(obj.ToString()));
                parent.AppendChild(elem);
            }
            else if (obj is DateTime)
            {
                var dt = (DateTime)obj;
                XmlElement elem = doc.CreateElement(name);
                elem.AppendChild(doc.CreateTextNode(ToWeixinTime(dt).ToString()));
                parent.AppendChild(elem);
            }
            else if (obj is string)
            {
                XmlElement elem = doc.CreateElement(name);
                elem.AppendChild(doc.CreateCDataSection(obj.ToString()));
                parent.AppendChild(elem);
            }
            else if (obj is FrameDLRObject)
            {
                var dobj = (FrameDLRObject)obj;
                XmlElement elem = doc.CreateElement(name);
                foreach (var k in dobj.Keys)
                {
                    ToXmlElem(doc, elem, k, dobj.GetValue(k));
                }
                parent.AppendChild(elem);
            }
            else if (obj is Dictionary<string, object>)
            {
                var dobj = (Dictionary<string, object>)obj;
                XmlElement elem = doc.CreateElement(name);
                foreach (var k in dobj)
                {
                    ToXmlElem(doc, elem, k.Key, k.Value);
                }
                parent.AppendChild(elem);
            }
            else if (obj is object[])
            {
                var arr = (object[])obj;
                foreach (var item in arr)
                {
                    var elemitem = doc.CreateElement(name);
                    if (item is FrameDLRObject)
                    {
                        var dobj = (FrameDLRObject)item;
                        foreach (var k in dobj.Keys)
                        {
                            ToXmlElem(doc, elemitem, k, dobj.GetValue(k));
                        }
                    }
                    else if (item is Dictionary<string, object>)
                    {
                        var dobj = (Dictionary<string, object>)item;
                        foreach (var k in dobj)
                        {
                            ToXmlElem(doc, elemitem, k.Key, k.Value);
                        }
                    }
                    else
                    {
                        elemitem.AppendChild(doc.CreateCDataSection(ComFunc.nvl(item)));
                    }
                    parent.AppendChild(elemitem);
                }

            }
            else
            {
                XmlElement elem = doc.CreateElement(name);
                elem.AppendChild(doc.CreateCDataSection(ComFunc.nvl(obj)));
                parent.AppendChild(elem);
            }
        }

        /// <summary>
        /// 返回微信时间（距1970年1月1日0点的秒数）
        /// </summary>
        /// <param name="dt">时间</param>
        /// <returns>返回微信时间</returns>
        public static int ToWeixinTime(DateTime dt)
        {
            DateTime baseTime = new DateTime(1970, 1, 1);
            return (int)(dt - baseTime).TotalSeconds;
        }
    }
}
