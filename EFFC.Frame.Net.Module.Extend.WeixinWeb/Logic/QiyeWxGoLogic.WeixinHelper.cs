using EFFC.Frame.Net.Base.Common;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Module.Extend.WeChat;

namespace EFFC.Frame.Net.Module.Extend.WeixinWeb.Logic
{
    public abstract partial class QiyeWxGoLogic
    {
        WeixinHelper _weixin = null;
        public WeixinHelper Weixin
        {
            get
            {
                if (_weixin == null) _weixin = new WeixinHelper(this);
                return _weixin;
            }
        }
        /// <summary>
        /// 微信号的一些底层方法
        /// </summary>
        public class WeixinHelper:QiyeWechatHelper
        {
            QiyeWxGoLogic _logic;
            /// <summary>
            /// 初始化一个微信号的底层方法
            /// </summary>
            /// <param name="logic"></param>
            public WeixinHelper(QiyeWxGoLogic logic):base("","","","","")
            {
                _logic = logic;
                AppID = ComFunc.nvl(_logic.CallContext_Parameter.WebParam.ExtentionObj.weixin.appid);
                AppSecret = ComFunc.nvl(_logic.CallContext_Parameter.WebParam.ExtentionObj.weixin.appsecret);
                Token = ComFunc.nvl(_logic.CallContext_Parameter.WebParam.ExtentionObj.weixin.token);
                Weixin_Mch_Ssl_Path = ComFunc.nvl(_logic.CallContext_Parameter.WebParam.ExtentionObj.weixin.weixin_mch_ssl_path);
                Weixin_Mch_Ssl_Pass = ComFunc.nvl(_logic.CallContext_Parameter.WebParam.ExtentionObj.weixin.weixin_mch_ssl_pass);
                EncodingAESKey = ComFunc.nvl(_logic.CallContext_Parameter.WebParam.ExtentionObj.weixin.encrypt_key);
                AgentId = ComFunc.nvl(_logic.CallContext_Parameter.WebParam.ExtentionObj.weixin.agentid);
            }
           
            #region 常用的属性

            #region 设定和验证类
            public string signature => ComFunc.nvl(_logic.CallContext_Parameter.WebParam.ExtentionObj.weixin.signature);
            public string timestamp => ComFunc.nvl(_logic.CallContext_Parameter.WebParam.ExtentionObj.weixin.timestamp);
            public string nonce => ComFunc.nvl(_logic.CallContext_Parameter.WebParam.ExtentionObj.weixin.nonce);
            #endregion

            #region 公用类
            /// <summary>
            /// 请求的用户OpenID
            /// </summary>
            public string FromUserName => ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "FromUserName"]);

            /// <summary>
            /// 请求的ToUserName
            /// </summary>
            public string ToUserName => ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "ToUserName"]);
            /// <summary>
            /// 请求的CreateTime
            /// </summary>
            public string CreateTime => ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "CreateTime"]);
            /// <summary>
            /// 请求的MsgType
            /// </summary>
            public string MsgType => ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "MsgType"]);
            /// <summary>
            /// 事件类型，
            /// subscribe(订阅)、unsubscribe(取消订阅)、SCAN（取消关注）、LOCATION（上报地理位置）、CLICK（点击菜单）
            /// </summary>
            public string Event => ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "Event"]);
            /// <summary>
            /// 事件KEY值
            /// </summary>
            public string EventKey => ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "EventKey"]);
            /// <summary>
            /// 请求的Content
            /// </summary>
            public string Content => ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "Content"]);


            #endregion

            #endregion

            #region 方法
            /// <summary>
            /// 生成一个Text类型的回复数据
            /// </summary>
            /// <param name="content"></param>
            /// <returns></returns>
            public FrameDLRObject GenResponseText(string content)
            {
                return GenResponseText(content, ToUserName, FromUserName);
            }
            #endregion

            #region 底层呼叫方法
            /// <summary>
            /// 异步调用微信服务
            /// </summary>
            /// <typeparam name="RType"></typeparam>
            /// <param name="url"></param>
            /// <param name="method"></param>
            /// <param name="contenttype"></param>
            /// <param name="header"></param>
            /// <param name="data"></param>
            /// <param name="isneedcert"></param>
            /// <param name="callbackLA"></param>
            /// <param name="recorddata"></param>
            private void CallWeixinServerAsync<RType>(string url, string method = "POST", string contenttype = "text/json", FrameDLRObject header = null, FrameDLRObject data = null, bool isneedcert = false, string callbackLA = "", FrameDLRObject recorddata = null)
            {
                base.CallWeixinServerAsync<RType>(url, method, contenttype, header, data, isneedcert, (arg) =>
                {
                    string[] la = callbackLA.Split('.');
                    var logic = "";
                    var action = "";
                    if (la.Length > 0)
                    {
                        logic = la[0];
                    }
                    if (la.Length > 1)
                    {
                        action = la[1];
                    }
                    _logic.OuterInterface.CallLocalLogic(logic, action, (dynamic)arg);
                }, recorddata);
            }
            /// <summary>
            /// 异步调用微信服务
            /// </summary>
            /// <param name="url"></param>
            /// <param name="method"></param>
            /// <param name="contenttype"></param>
            /// <param name="header"></param>
            /// <param name="data"></param>
            /// <param name="isneedcert"></param>
            /// <param name="callbackLA"></param>
            /// <param name="recorddata"></param>
            private void CallWeixinServerAsync(string url, string method = "POST", string contenttype = "text/json", FrameDLRObject header = null, FrameDLRObject data = null, bool isneedcert = false, string callbackLA = "", FrameDLRObject recorddata = null)
            {
                CallWeixinServerAsync<FrameDLRObject>(url, method, contenttype, header, data, isneedcert, callbackLA, recorddata);
            }
            #endregion
        }
    }
}
