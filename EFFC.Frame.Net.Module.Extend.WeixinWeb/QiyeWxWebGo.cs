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
    public class QiyeWxWebGo : WeixinWebGo
    {
        protected override bool IsValid4Invoke(WebParameter p, GoData d)
        {
            //企业微信的来源校验不同
            return true;
        }
        protected override void ProcessRequestInfoWeixin(WebParameter p, GoData d)
        {
            //微信相关信息
            p.ExtentionObj.weixin = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            // 微信企业号的加密签名为msg_signature而不是signature,所以得重写
            p.ExtentionObj.weixin.signature = ComFunc.nvl(p[DomainKey.QUERY_STRING, "msg_signature"]);
            p.ExtentionObj.weixin.timestamp = ComFunc.nvl(p[DomainKey.QUERY_STRING, "timestamp"]);
            p.ExtentionObj.weixin.nonce = ComFunc.nvl(p[DomainKey.QUERY_STRING, "nonce"]);
            p.ExtentionObj.weixin.token = ComFunc.nvl(p[DomainKey.CONFIG, "weixin_token"]);
            p.ExtentionObj.weixin.encrypt_type = ComFunc.nvl(p[DomainKey.QUERY_STRING, "encrypt_type"]) == "" ? "aes" : ComFunc.nvl(p[DomainKey.QUERY_STRING, "encrypt_type"]);
            p.ExtentionObj.weixin.encrypt_key = ComFunc.nvl(p[DomainKey.CONFIG, "weixin_encry_key"]);
            p.ExtentionObj.weixin.appid = ComFunc.nvl(p[DomainKey.CONFIG, "weixin_Appid"]);
            p.ExtentionObj.weixin.appsecret = ComFunc.nvl(p[DomainKey.CONFIG, "weixin_Appsecret"]);
            p.ExtentionObj.weixin.weixin_mch_ssl_path = ComFunc.nvl(p[DomainKey.CONFIG, "weixin_Mch_SSL_Path"]);
            p.ExtentionObj.weixin.weixin_mch_ssl_pass = ComFunc.nvl(p[DomainKey.CONFIG, "weixin_Mch_SSL_Pass"]);
            // 20171124 Ge.Song 针对企业微信添加AgentId
            p.ExtentionObj.weixin.agentid = ComFunc.nvl(p[DomainKey.CONFIG, "weixin_AgentId"]);

            p.SetValue("logkey", DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            string content = string.Empty;
            if (CurrentContext.Request.Method.ToLower() == "post")
            {
                content = p.RequestContent;
                WXBizMsgCrypt wxcpt = new WXBizMsgCrypt(p.ExtentionObj.weixin.token, p.ExtentionObj.weixin.encrypt_key, p.ExtentionObj.weixin.appid);
                string msg = "";
                wxcpt.DecryptMsg(p.ExtentionObj.weixin.signature, p.ExtentionObj.weixin.timestamp, p.ExtentionObj.weixin.nonce, content, ref msg);
                content = msg;

                if (content != "")
                {
                    XmlDocument doc = new XmlDocument();
                    //防范xxe攻击
                    doc.XmlResolver = null;
                    doc.LoadXml(content);
                    var root = doc.FirstChild;
                    foreach (XmlNode node in root.ChildNodes)
                    {
                        // 一般来说企业微信事件只会回传AgentID,ToUserName,Encrypt
                        p[DomainKey.POST_DATA, node.Name] = node.Name == "CreateTime"
                                                            ? (object) new DateTime(1970, 1, 1).AddSeconds(int.Parse(node.InnerText))
                                                            : node.InnerText;
                        if (node.HasChildNodes)
                        {
                            foreach (XmlNode sub in node.ChildNodes)
                            {
                                if (node.Name == "CreateTime")
                                    p[DomainKey.POST_DATA, sub.Name] = new DateTime(1970, 1, 1).AddSeconds(int.Parse(sub.InnerText));
                                else
                                    p[DomainKey.POST_DATA, sub.Name] = sub.InnerText;
                            }
                        }
                    }
                }
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

        
    }
}
