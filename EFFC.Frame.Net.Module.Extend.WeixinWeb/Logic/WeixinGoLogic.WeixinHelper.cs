using EFFC.Frame.Net.Module.Extend.WebGo.Logic;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Module.Extend.WebGo.Datas;
using EFFC.Frame.Net.Module.Extend.WebGo.Parameters;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Base.Data.Base;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using EFFC.Frame.Net.Module.Business.Parameters;
using EFFC.Frame.Net.Module.Web.Parameters;

namespace EFFC.Frame.Net.Module.Extend.WeixinWeb.Logic
{
    public abstract partial class WeixinGoLogic
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
        public class WeixinHelper
        {
            WeixinGoLogic _logic;

            public WeixinHelper(WeixinGoLogic logic)
            {
                _logic = logic;
            }
            /// <summary>
            /// 主微信号的APPID
            /// </summary>
            public string AppID
            {
                get
                {
                    return ComFunc.nvl(_logic.CallContext_Parameter.WebParam.ExtentionObj.weixin.appid);
                }
            }
            /// <summary>
            /// 主微信号的APPSecret
            /// </summary>
            public string AppSecret
            {
                get
                {
                    return ComFunc.nvl(_logic.CallContext_Parameter.WebParam.ExtentionObj.weixin.appsecret);
                }
            }
            /// <summary>
            /// 主微信号商户ID
            /// </summary>
            public string Mch_ID
            {
                get
                {
                    return ComFunc.nvl(_logic.CallContext_Parameter.WebParam.ExtentionObj.weixin.mch_id);
                }
            }
            /// <summary>
            /// 主微信号商户支付秘钥
            /// </summary>
            public string Mch_Key
            {
                get
                {
                    return ComFunc.nvl(_logic.CallContext_Parameter.WebParam.ExtentionObj.weixin.mch_key);
                }
            }
            /// <summary>
            /// 微信发过来的随机验证码Nonce_str
            /// </summary>
            public string Nonce_str
            {
                get
                {
                    return ComFunc.nvl(_logic.CallContext_Parameter.WebParam.ExtentionObj.weixin.nonce);
                }
            }
            /// <summary>
            /// 请求的用户OpenID
            /// </summary>
            public string FromUserName
            {
                get
                {
                    return ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "FromUserName"]);
                }
            }
            /// <summary>
            /// 请求的ToUserName
            /// </summary>
            public string ToUserName
            {
                get
                {
                    return ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "ToUserName"]);
                }
            }
            /// <summary>
            /// 请求的CreateTime
            /// </summary>
            public DateTime CreateTime
            {
                get
                {
                    return (DateTime)_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "CreateTime"];
                }
            }
            /// <summary>
            /// 请求的MsgType
            /// </summary>
            public string MsgType
            {
                get
                {
                    return ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "MsgType"]);
                }
            }

            /// <summary>
            /// 请求的MsgType
            /// </summary>
            public string Content
            {
                get
                {
                    return ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "Content"]);
                }
            }

            /// <summary>
            /// 请求的MsgType
            /// </summary>
            public string MsgId
            {
                get
                {
                    return ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "MsgId"]);
                }
            }
            /// <summary>
            /// 请求的PicUrl
            /// </summary>
            public string PicUrl
            {
                get
                {
                    return ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "PicUrl"]);
                }
            }
            /// <summary>
            /// 请求的MediaId
            /// </summary>
            public string MediaId
            {
                get
                {
                    return ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "MediaId"]);
                }
            }
            /// <summary>
            /// 请求的语音Format，语音格式，如amr，speex等
            /// </summary>
            public string Format
            {
                get
                {
                    return ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "Format"]);
                }
            }
            /// <summary>
            /// 请求的语音识别结果，使用UTF8编码
            /// </summary>
            public string Recognition
            {
                get
                {
                    return ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "Recognition"]);
                }
            }
            /// <summary>
            /// 视频消息缩略图的媒体id，可以调用多媒体文件下载接口拉取数据。
            /// </summary>
            public string ThumbMediaId
            {
                get
                {
                    return ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "ThumbMediaId"]);
                }
            }
            /// <summary>
            /// 地理位置维度
            /// </summary>
            public double Location_X
            {
                get
                {
                    return DoubleStd.ParseStd(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "Location_X"]).Value;
                }
            }
            /// <summary>
            /// 地理位置经度
            /// </summary>
            public double Location_Y
            {
                get
                {
                    return DoubleStd.ParseStd(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "Location_Y"]).Value;
                }
            }
            /// <summary>
            /// 地图缩放大小
            /// </summary>
            public double Scale
            {
                get
                {
                    return DoubleStd.ParseStd(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "Scale"]).Value;
                }
            }
            /// <summary>
            /// 地理位置信息
            /// </summary>
            public string Label
            {
                get
                {
                    return ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "Label"]);
                }
            }
            /// <summary>
            /// 消息标题
            /// </summary>
            public string MessageTitle
            {
                get
                {
                    return ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "Title"]);
                }
            }
            /// <summary>
            /// 消息描述
            /// </summary>
            public string MessageDescription
            {
                get
                {
                    return ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "Description"]);
                }
            }
            /// <summary>
            /// 消息链接
            /// </summary>
            public string MessageUrl
            {
                get
                {
                    return ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "Url"]);
                }
            }
            /// <summary>
            /// 事件类型，
            /// subscribe(订阅)、unsubscribe(取消订阅)、SCAN（取消关注）、LOCATION（上报地理位置）、CLICK（点击菜单）
            /// </summary>
            public string Event
            {
                get
                {
                    return ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "Event"]);
                }
            }
            /// <summary>
            /// 事件KEY值
            /// </summary>
            public string EventKey
            {
                get
                {
                    //返回的是POSTDATA里面有"EventKey"的数据
                    return ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "EventKey"]);
                }
            }
            /// <summary>
            /// 二维码的ticket，可用来换取二维码图片
            /// </summary>
            public string Ticket
            {
                get
                {
                    return ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "Ticket"]);
                }
            }

            /// <summary>
            /// 上报事件中的地理位置纬度
            /// </summary>
            public double Latitude
            {
                get
                {
                    return DoubleStd.ParseStd(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "Latitude"]).Value;
                }
            }
            /// <summary>
            /// 上报事件中的地理位置经度
            /// </summary>
            public double Longitude
            {
                get
                {
                    return DoubleStd.ParseStd(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "Longitude"]).Value;
                }
            }
            /// <summary>
            /// 上报事件中的地理位置精度
            /// </summary>
            public double Precision
            {
                get
                {
                    return DoubleStd.ParseStd(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "Precision"]).Value;
                }
            }
            /// <summary>
            /// 微信支付授权访问目录
            /// </summary>
            public string Pay_AuthDirectory
            {
                get
                {
                    return ComFunc.nvl(_logic.CallContext_Parameter.WebParam.ExtentionObj.weixin.weixin_authpaydir);
                }
            }
            /// <summary>
            /// 获取微信的Access Token，用于与微信服务器进行信息交互
            /// </summary>
            public string Access_Token
            {
                get
                {
                    return GenAccess_Token(AppID, AppSecret);
                }
            }

            /// <summary>
            /// 扫描信息
            /// </summary>
            public string ScanCodeInfo
            {
                get
                {
                    return ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "ScanCodeInfo"]);
                }
            }
            /// <summary>
            /// 扫描类型，一般是qrcode
            /// </summary>
            public string ScanType
            {
                get
                {
                    return ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "ScanType"]);
                }
            }
            /// <summary>
            /// 扫描结果，即二维码对应的字符串信息
            /// </summary>
            public string ScanResult
            {
                get
                {
                    return ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "ScanResult"]);
                }
            }
            /// <summary>
            /// 根据AppID和appsecret获取对应微信号下的access_token
            /// </summary>
            /// <param name="appid"></param>
            /// <param name="appsecret"></param>
            /// <returns></returns>
            public string GenAccess_Token(string appid, string appsecret)
            {
                if (_logic.CacheHelper.GetCache(appid + "_weixin_access_token") == null)
                {
                    GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, "***** GoLogic.Weixin.cs GenAccess_Token start *********************************************************************************************");

                    var result = CallWeixinServer(string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", appid, appsecret));
                    dynamic dobj = (FrameDLRObject)result;
                    var token = ComFunc.nvl(dobj.access_token);
                    var expireseconds = ComFunc.nvl(dobj.expires_in);
                    GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, "APP_ID: " + appid + ";  新Token:" + token + ";  expireseconds:" + expireseconds);
                    if (token != "")
                    {
                        //获取之后将超时时间缩短10秒，微信默认超时时间为7200秒，每获取一次就会重置该token
                        _logic.CacheHelper.SetCache(appid + "_weixin_access_token", token, DateTime.Now.AddSeconds(IntStd.ParseStd(expireseconds).Value - 10));
                    }
                    GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, "***** GoLogic.Weixin.cs GenAccess_Token End *********************************************************************************************");

                }

                return ComFunc.nvl(_logic.CacheHelper.GetCache(appid + "_weixin_access_token"));
            }
            /// <summary>
            /// 微信调用JSAPI需要用到的JSAPI_Ticket,用于做AES的数字签名
            /// </summary>
            public string Jsapi_ticket
            {
                get
                {
                    if (_logic.CacheHelper.GetCache("weixin_jsapi_ticket") == null)
                    {
                        var result = CallWeixinServer(string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi", Access_Token));
                        dynamic dobj = (FrameDLRObject)result;
                        var ticket = ComFunc.nvl(dobj.ticket);
                        var expireseconds = ComFunc.nvl(dobj.expires_in);
                        if (ticket != "")
                        {
                            //获取之后将超时时间缩短10秒，微信默认超时时间为7200秒，每获取一次就会重置该token
                            _logic.CacheHelper.SetCache("weixin_jsapi_ticket", ticket, DateTime.Now.AddSeconds(IntStd.ParseStd(expireseconds).Value - 10));
                        }
                    }

                    return ComFunc.nvl(_logic.CacheHelper.GetCache("weixin_jsapi_ticket"));
                }
            }
            /// <summary>
            /// 获取Jsapi_ticket
            /// </summary>
            /// <param name="accesstoken"></param>
            /// <returns></returns>
            public string GenJsapi_ticket(string accesstoken)
            {
                if (_logic.CacheHelper.GetCache(accesstoken + "_weixin_jsapi_ticket") == null)
                {
                    var result = CallWeixinServer(string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi", Access_Token));
                    dynamic dobj = (FrameDLRObject)result;
                    var ticket = ComFunc.nvl(dobj.ticket);
                    var expireseconds = ComFunc.nvl(dobj.expires_in);
                    if (ticket != "")
                    {
                        //获取之后将超时时间缩短10秒，微信默认超时时间为7200秒，每获取一次就会重置该token
                        _logic.CacheHelper.SetCache(accesstoken + "_weixin_jsapi_ticket", ticket, DateTime.Now.AddSeconds(IntStd.ParseStd(expireseconds).Value - 10));
                    }
                }

                return ComFunc.nvl(_logic.CacheHelper.GetCache(accesstoken + "_weixin_jsapi_ticket"));
            }
            /// <summary>
            /// 当前新的微信时间戳
            /// </summary>
            public int NewTimsStamp
            {
                get
                {
                    DateTime baseTime = new DateTime(1970, 1, 1);
                    return (int)(DateTime.Now - baseTime).TotalSeconds;
                }
            }
            /// <summary>
            /// 生成微信支付通知的url，走wx请求时使用
            /// </summary>
            /// <param name="url"></param>
            /// <returns></returns>
            public string GenPayNotify_url(string url)
            {
                var sb = new StringBuilder();
                var timestamp = ComFunc.nvl(NewTimsStamp);
                var nonce = ComFunc.nvl(ComFunc.Random(6));
                var token = ComFunc.nvl(_logic.CallContext_Parameter.WebParam.ExtentionObj.weixin.token);

                var sign = WXBizMsgCrypt.GenWeixinVisitSign(token, timestamp, nonce);
                var str = Path.GetFileNameWithoutExtension(url);
                var extname = Path.GetExtension(url);
                var strwithext = Path.GetFileName(url);

                var strarr = str.Split('.');
                string s = string.Format("signature={0}&timestamp={1}&nonce={2}", sign, timestamp, nonce);

                var encryptstr = ComFunc.AESEncrypt(s).Replace("/", "_2F_").Replace("+", "_2B_");
                encryptstr = ComFunc.UrlEncode(encryptstr);

                str += ".pay." + encryptstr;
                var newurl = url.Replace(strwithext, str + extname);
                return newurl;
            }

            /// <summary>
            /// 获取SHA算法的Sign签名,ASCII编码
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public string GenSHASignKey(FrameDLRObject obj)
            {
                return WXBizMsgCrypt.GenSHASignKey(obj, Encoding.ASCII);
            }
            /// <summary>
            /// 获取JSAPI的SignKey
            /// </summary>
            /// <param name="noncestr">随机串，与config中的一致</param>
            /// <param name="timestamp">时间戳，与config中的一致</param>
            /// <param name="url">要调用jsapi的页面</param>
            /// <returns></returns>
            public string GenJSAPISignKey(string noncestr, int timestamp, string url)
            {
                return GenJSAPISignKey(noncestr, timestamp, url, Jsapi_ticket);
            }
            /// <summary>
            /// 获取JSAPI的SignKey
            /// </summary>
            /// <param name="noncestr">随机串，与config中的一致</param>
            /// <param name="timestamp">时间戳，与config中的一致</param>
            /// <param name="url">要调用jsapi的页面</param>
            /// <param name="jsapi_ticket">要调用的jsapi_ticket</param>
            /// <returns></returns>
            public string GenJSAPISignKey(string noncestr, int timestamp, string url, string jsapi_ticket)
            {
                var p = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                p.jsapi_ticket = jsapi_ticket;
                p.noncestr = noncestr;
                p.timestamp = timestamp;
                p.url = url.IndexOf("#") >= 0 ? url.Substring(0, url.IndexOf("#")) : url;

                return GenSHASignKey(p);
            }
            /// <summary>
            /// 根据参数采用MD5算法生成数字签名
            /// </summary>
            /// <param name="obj">参数集，该参数集不可包含数字签名的栏位</param>
            /// <returns></returns>
            public string GenMD5SignString(FrameDLRObject obj)
            {
                return WXBizMsgCrypt.GenMD5SignString(obj, Mch_Key);
            }
            /// <summary>
            /// 生成JSAPI的Config参数
            /// </summary>
            /// <param name="debug"></param>
            /// <param name="appid"></param>
            /// <param name="noncestr"></param>
            /// <param name="timestamp"></param>
            /// <param name="url"></param>
            /// <param name="jsapilist"></param>
            /// <returns></returns>
            public FrameDLRObject GenJSAPIConfigParameters(bool debug, string appid, string noncestr, int timestamp, string url, string[] jsapilist)
            {
                var rtn = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                rtn.debug = debug;
                rtn.appId = appid;
                rtn.timestamp = timestamp;
                rtn.nonceStr = noncestr;
                rtn.jsApiList = jsapilist;
                rtn.signature = GenJSAPISignKey(rtn.nonceStr, rtn.timestamp, url);
                return rtn;
            }
            /// <summary>
            /// 生成JSAPI的Config参数
            /// </summary>
            /// <param name="debug"></param>
            /// <param name="noncestr"></param>
            /// <param name="timestamp"></param>
            /// <param name="url"></param>
            /// <returns></returns>
            public FrameDLRObject GenJSAPIConfigParameters(bool debug, string noncestr, int timestamp, string url)
            {
                return GenJSAPIConfigParameters(debug, AppID, noncestr, timestamp, url, new string[] { "onMenuShareTimeline",
"onMenuShareAppMessage",
"onMenuShareQQ",
"onMenuShareWeibo",
"onMenuShareQZone",
"startRecord",
"stopRecord",
"onVoiceRecordEnd",
"playVoice",
"pauseVoice",
"stopVoice",
"onVoicePlayEnd",
"uploadVoice",
"downloadVoice",
"chooseImage",
"previewImage",
"uploadImage",
"downloadImage",
"translateVoice",
"getNetworkType",
"openLocation",
"getLocation",
"hideOptionMenu",
"showOptionMenu",
"hideMenuItems",
"showMenuItems",
"hideAllNonBaseMenuItem",
"showAllNonBaseMenuItem",
"closeWindow",
"scanQRCode",
"chooseWXPay",
"openProductSpecificView",
"addCard",
"chooseCard",
"openCard" });
            }
            /// <summary>
            /// 调用统一下单接口，获取预付单信息
            /// </summary>
            /// <param name="notify_url">回调url</param>
            /// <param name="openid">用户的openid</param>
            /// <param name="orderno">订单号</param>
            /// <param name="amount">金额</param>
            /// <param name="body">商品描述</param>
            /// <param name="attachdata">附加信息</param>
            /// <returns></returns>
            public FrameDLRObject UnifiedOrder4JSAPI(string notify_url, string openid, string orderno, double amount, string body, string attachdata, string goods_tags = "趋动")
            {
                FrameDLRObject p = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                p.SetValue("appid", AppID);
                p.SetValue("mch_id", Mch_ID);
                p.SetValue("nonce_str", Guid.NewGuid().ToString().Replace("-", ""));
                p.SetValue("body", body);
                p.SetValue("attach", attachdata);
                p.SetValue("out_trade_no", orderno);
                p.SetValue("total_fee", amount * 100);
                p.SetValue("spbill_create_ip", "8.8.8.8");
                var now = DateTime.Now;
                p.SetValue("time_start", now.ToString("yyyyMMddHHmmss"));
                p.SetValue("time_expire", now.AddMinutes(15).ToString("yyyyMMddHHmmss"));
                p.SetValue("goods_tag", goods_tags);
                p.SetValue("notify_url", notify_url);
                p.SetValue("trade_type", "JSAPI");
                p.SetValue("openid", openid);

                p.SetValue("sign", WXBizMsgCrypt.GenMD5SignString(p, Mch_Key));

                var result = CallWeixinServer(string.Format("https://api.mch.weixin.qq.com/pay/unifiedorder"), "", ResponseHeader_ContentType.xml, null, p);
                GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "UnifiedOrder4JSAPI result:" + ((FrameDLRObject)result).ToJSONString());
                var rtn = FrameDLRObject.CreateInstance(@"{
issuccess:false,
msg:'',
prepay_id:''
}");
                if (result is FrameDLRObject)
                {
                    dynamic dresult = (FrameDLRObject)result;
                    if (dresult.return_code == "SUCCESS")
                    {
                        rtn.msg = dresult.return_msg;
                        if (dresult.result_code == "SUCCESS")
                        {
                            rtn.issuccess = true;
                            rtn.prepay_id = dresult.prepay_id;
                        }
                    }
                }

                return rtn;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="transaction_id">微信的订单号，优先使用 </param>
            /// <param name="out_trade_no">商户系统内部的订单号</param>
            /// <returns></returns>
            public FrameDLRObject QueryUnifiedOrder(string transaction_id, string out_trade_no)
            {
                FrameDLRObject p = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                p.SetValue("appid", AppID);
                p.SetValue("mch_id", Mch_ID);
                p.SetValue("transaction_id", transaction_id);
                p.SetValue("out_trade_no", out_trade_no);
                p.SetValue("nonce_str", Guid.NewGuid().ToString().Replace("-", ""));

                p.SetValue("sign", WXBizMsgCrypt.GenMD5SignString(p, Mch_Key));
                var result = CallWeixinServer(string.Format("https://api.mch.weixin.qq.com/pay/orderquery"), "", ResponseHeader_ContentType.xml, null, p);
                var rtn = FrameDLRObject.CreateInstance(@"{
issuccess:false,
msg:'',
}");
                if (result is FrameDLRObject)
                {
                    dynamic dresult = (FrameDLRObject)result;
                    if (dresult.return_code == "SUCCESS")
                    {
                        rtn.msg = dresult.return_msg;
                        if (dresult.result_code == "SUCCESS")
                        {
                            rtn.issuccess = true;
                            rtn.trade_state = dresult.trade_state;
                            rtn.trade_type = dresult.trade_type;
                            rtn.bank_type = dresult.bank_type;
                            rtn.total_fee = DecimalStd.IsDecimal(dresult.total_fee) ? DecimalStd.ParseStd(dresult.total_fee).Value / 100 : 0.0;
                            rtn.coupon_fee = dresult.coupon_fee;
                            rtn.fee_type = dresult.fee_type;
                            rtn.transaction_id = dresult.transaction_id;
                            rtn.out_trade_no = dresult.out_trade_no;
                            rtn.attach = dresult.attach;
                        }
                    }
                }

                return rtn;
            }
            /// <summary>
            /// 申请退款
            /// </summary>
            /// <param name="transaction_id">微信交易单号</param>
            /// <param name="out_trade_no">订单号</param>
            /// <param name="out_refund_no">退款单号</param>
            /// <param name="amount">退款金额</param>
            /// <returns></returns>
            public FrameDLRObject RefundOrder(string transaction_id, string out_trade_no, string out_refund_no, decimal amount, string fundtype = "CNY")
            {
                FrameDLRObject p = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                p.SetValue("appid", AppID);
                p.SetValue("mch_id", Mch_ID);
                p.SetValue("device_info", "8.8.8.8");
                p.SetValue("transaction_id", transaction_id);
                p.SetValue("out_trade_no", out_trade_no);
                p.SetValue("out_refund_no", out_refund_no);
                p.SetValue("total_fee", (int)(amount * 100));
                p.SetValue("refund_fee", (int)(amount * 100));
                p.SetValue("refund_fee_type", fundtype);
                p.SetValue("op_user_id", Mch_ID);
                p.SetValue("nonce_str", Guid.NewGuid().ToString().Replace("-", ""));
                p.SetValue("sign", WXBizMsgCrypt.GenMD5SignString(p, Mch_Key));


                var result = CallWeixinServer(string.Format("https://api.mch.weixin.qq.com/secapi/pay/refund"), "", ResponseHeader_ContentType.xml, null, p, true);
                var rtn = FrameDLRObject.CreateInstance(@"{
issuccess:false,
msg:'',
}");
                if (result is FrameDLRObject)
                {
                    GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "refund result:" + ((FrameDLRObject)result).ToJSONString());
                    dynamic dresult = (FrameDLRObject)result;
                    if (dresult.return_code == "SUCCESS")
                    {
                        rtn.msg = dresult.return_msg;
                        if (dresult.result_code == "SUCCESS")
                        {
                            rtn.issuccess = true;
                            rtn.refund_id = dresult.refund_id;
                            rtn.refund_channel = dresult.refund_channel;
                        }
                        else
                        {
                            rtn.msg = dresult.err_code_des;
                        }
                    }
                }

                return rtn;
            }

            /// <summary>
            /// 发放微信红包
            /// </summary>
            /// <param name="openid">对方openid</param>
            /// <param name="minamount">最小金额</param>
            /// <param name="maxamount">最大金额</param>
            /// <param name="totalamount">总金额</param>
            /// <param name="total_num">总人数</param>
            /// <param name="wishing">祝福语</param>
            /// <param name="act_name">活动名称</param>
            /// <param name="remark">备注</param>
            /// <param name="logo_imgurl">商户logo的url</param>
            /// <param name="share_content">分享文案</param>
            /// <param name="share_url">分享链接</param>
            /// <param name="share_imgurl">分享的图片</param>
            /// <param name="nick_name">提供方名称</param>
            /// <param name="send_name">商户名称</param>
            /// <returns></returns>
            public FrameDLRObject RedPacket(string openid,
                double totalamount, int total_num,
                string wishing, string act_name, string remark,
                string send_name)
            {
                return RedPacket(AppID, openid, totalamount, total_num, wishing, act_name, remark, send_name);
            }
            /// <summary>
            /// 发放微信红包
            /// </summary>
            /// <param name="appid">对方所在的公众号id</param>
            /// <param name="openid">对方openid</param>
            /// <param name="totalamount">资金总额</param>
            /// <param name="total_num">发送总量</param>
            /// <param name="wishing">祝福语</param>
            /// <param name="act_name">活动名称</param>
            /// <param name="remark">备注</param>
            /// <param name="send_name">商户名称</param>
            /// <returns></returns>
            public FrameDLRObject RedPacket(string appid, string openid,
                double totalamount, int total_num,
                string wishing, string act_name, string remark,
                string send_name)
            {
                var mch_billno = Mch_ID + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "0";
                FrameDLRObject p = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                p.SetValue("nonce_str", Guid.NewGuid().ToString().Replace("-", ""));
                p.SetValue("mch_billno", mch_billno);
                p.SetValue("mch_id", Mch_ID);
                p.SetValue("wxappid", appid);
                p.SetValue("send_name", send_name);
                p.SetValue("re_openid", openid);
                p.SetValue("client_ip", "8.8.8.8");
                p.SetValue("total_amount", (int)(totalamount * 100));
                p.SetValue("total_num", total_num);
                p.SetValue("wishing", wishing);
                p.SetValue("act_name", act_name);
                p.SetValue("remark", remark);
                p.SetValue("sign", WXBizMsgCrypt.GenMD5SignString(p, Mch_Key));

                var result = CallWeixinServer(string.Format("https://api.mch.weixin.qq.com/mmpaymkttransfers/sendredpack"), "", ResponseHeader_ContentType.xml, null, p, true);

                var rtn = FrameDLRObject.CreateInstance(@"{
issuccess:false,
error_code:'',
msg:'',
}");
                rtn.mch_billno = mch_billno;
                if (result is FrameDLRObject)
                {
                    GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "RedPacket result:" + ((FrameDLRObject)result).ToJSONString());
                    dynamic dresult = (FrameDLRObject)result;
                    if (dresult.return_code == "SUCCESS")
                    {
                        rtn.msg = dresult.return_msg;
                        if (dresult.result_code == "SUCCESS")
                        {
                            rtn.issuccess = true;

                        }
                        else
                        {
                            rtn.error_code = dresult.err_code;
                            rtn.msg = dresult.err_code_des;
                        }
                    }
                    else
                    {
                        rtn.error_code = dresult.err_code;
                        rtn.msg = dresult.err_code_des;
                    }
                }

                return rtn;
            }
            /// <summary>
            /// 发放微信红包
            /// </summary>
            /// <param name="openid">对方openid</param>
            /// <param name="minamount">最小金额</param>
            /// <param name="maxamount">最大金额</param>
            /// <param name="totalamount">总金额</param>
            /// <param name="total_num">总人数</param>
            /// <param name="wishing">祝福语</param>
            /// <param name="act_name">活动名称</param>
            /// <param name="remark">备注</param>
            /// <param name="logo_imgurl">商户logo的url</param>
            /// <param name="share_content">分享文案</param>
            /// <param name="share_url">分享链接</param>
            /// <param name="share_imgurl">分享的图片</param>
            /// <param name="nick_name">提供方名称</param>
            /// <param name="send_name">商户名称</param>
            /// <returns></returns>
            public void RedPacketAsync(string openid,
                double totalamount, int total_num,
                string wishing, string act_name, string remark,
                string send_name, string callbackLA, FrameDLRObject cachedata)
            {
                RedPacketAsync(AppID, openid, totalamount, total_num, wishing, act_name, remark, send_name, callbackLA, cachedata);
            }
            /// <summary>
            /// 发放微信红包
            /// </summary>
            /// <param name="appid">对方所在的appid</param>
            /// <param name="openid">对方openid</param>
            /// <param name="totalamount">总金额</param>
            /// <param name="total_num">总数</param>
            /// <param name="wishing">祝福语</param>
            /// <param name="act_name">活动名称</param>
            /// <param name="remark">备注</param>
            /// <param name="send_name">商户名称</param>
            /// <param name="callbackLA">回调的logic.action</param>
            /// <param name="cachedata">回调中需要保留的数据</param>
            public void RedPacketAsync(string appid, string openid,
                double totalamount, int total_num,
                string wishing, string act_name, string remark,
                string send_name, string callbackLA, FrameDLRObject cachedata)
            {
                var mch_billno = Mch_ID + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "0";
                FrameDLRObject p = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                p.SetValue("nonce_str", Guid.NewGuid().ToString().Replace("-", ""));
                p.SetValue("mch_billno", mch_billno);
                p.SetValue("mch_id", Mch_ID);
                p.SetValue("wxappid", appid);
                p.SetValue("send_name", send_name);
                p.SetValue("re_openid", openid);
                p.SetValue("client_ip", "8.8.8.8");
                p.SetValue("total_amount", (int)(totalamount * 100));
                p.SetValue("total_num", total_num);
                p.SetValue("wishing", wishing);
                p.SetValue("act_name", act_name);
                p.SetValue("remark", remark);
                p.SetValue("sign", WXBizMsgCrypt.GenMD5SignString(p, Mch_Key));

                cachedata.SetValue("customercallback", callbackLA);

                CallWeixinServerAsync(string.Format("https://api.mch.weixin.qq.com/mmpaymkttransfers/sendredpack"),"", ResponseHeader_ContentType.xml, null, p, true, callbackLA, cachedata);
            }
            /// <summary>
            /// 获取异步回调结果集
            /// </summary>
            public dynamic CallBackResult
            {
                get
                {
                    return _logic.CallContext_Parameter.WebParam.GetValue(DomainKey.CUSTOMER_PARAMETER, "weixincallbackresult");
                }
            }
            /// <summary>
            /// 获取红包异步回调结果集
            /// </summary>
            /// <returns></returns>
            public dynamic GetRedPacketCallbackResult()
            {

                dynamic dresult = CallBackResult;
                var rtn = FrameDLRObject.CreateInstance(@"{
issuccess:false,
error_code:'',
msg:'',
}");
                if (dresult != null)
                {
                    GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "RedPacket result:" + ((FrameDLRObject)dresult).ToJSONString());
                    if (dresult.return_code == "SUCCESS")
                    {
                        rtn.msg = dresult.return_msg;
                        if (dresult.result_code == "SUCCESS")
                        {
                            rtn.issuccess = true;
                        }
                        else
                        {
                            rtn.error_code = dresult.err_code;
                            rtn.msg = dresult.err_code_des;
                        }
                    }
                    else
                    {
                        rtn.error_code = dresult.err_code;
                        rtn.msg = dresult.err_code_des;
                    }
                }
                else
                {
                    rtn.msg = "Nothing returned";
                }

                return rtn;

            }
            /// <summary>
            /// 分裂红包
            /// </summary>
            /// <param name="openid"></param>
            /// <param name="totalamount"></param>
            /// <param name="total_num"></param>
            /// <param name="wishing"></param>
            /// <param name="act_name"></param>
            /// <param name="remark"></param>
            /// <param name="send_name"></param>
            /// <returns></returns>
            public FrameDLRObject GroupRedPacket(string openid,
                double totalamount, int total_num,
                string wishing, string act_name, string remark,
                string send_name)
            {
                var mch_billno = Mch_ID + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "0";
                FrameDLRObject p = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                p.SetValue("nonce_str", Guid.NewGuid().ToString().Replace("-", ""));
                p.SetValue("mch_billno", mch_billno);
                p.SetValue("mch_id", Mch_ID);
                p.SetValue("wxappid", AppID);
                p.SetValue("send_name", send_name);
                p.SetValue("re_openid", openid);
                p.SetValue("amt_type", "ALL_RAND");
                p.SetValue("total_amount", (int)(totalamount * 100));
                p.SetValue("total_num", total_num);
                p.SetValue("wishing", wishing);
                p.SetValue("act_name", act_name);
                p.SetValue("remark", remark);
                p.SetValue("sign", WXBizMsgCrypt.GenMD5SignString(p, Mch_Key));

                var result = CallWeixinServer(string.Format("https://api.mch.weixin.qq.com/mmpaymkttransfers/sendgroupredpack"), "", ResponseHeader_ContentType.xml, null, p, true);
                var rtn = FrameDLRObject.CreateInstance(@"{
issuccess:false,
error_code:'',
msg:'',
}");
                rtn.mch_billno = mch_billno;
                if (result is FrameDLRObject)
                {
                    GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "RedPacket result:" + ((FrameDLRObject)result).ToJSONString());
                    dynamic dresult = (FrameDLRObject)result;
                    if (dresult.return_code == "SUCCESS")
                    {
                        rtn.msg = dresult.return_msg;
                        if (dresult.result_code == "SUCCESS")
                        {
                            rtn.issuccess = true;

                        }
                        else
                        {
                            rtn.error_code = dresult.err_code;
                            rtn.msg = dresult.err_code_des;
                        }
                    }
                    else
                    {
                        rtn.error_code = dresult.err_code;
                        rtn.msg = dresult.err_code_des;
                    }
                }

                return rtn;
            }
            /// <summary>
            /// 支付金额给用户
            /// </summary>
            /// <param name="appid">收款方所在的微信号</param>
            /// <param name="openid">收款方所在微信的openid</param>
            /// <param name="username">收款方的姓名</param>
            /// <param name="amount">金额，</param>
            /// <param name="desc"></param>
            /// <param name="isneed_name_check"></param>
            /// <returns></returns>
            public dynamic Pay2Customer(string appid, string openid, string username, double amount, string desc, bool isneed_name_check)
            {
                var mch_billno = Mch_ID + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "0";
                FrameDLRObject p = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                p.SetValue("mch_appid", appid);
                p.SetValue("mchid", Mch_ID);
                p.SetValue("nonce_str", Guid.NewGuid().ToString().Replace("-", ""));
                p.SetValue("partner_trade_no", mch_billno);
                p.SetValue("openid", openid);
                p.SetValue("check_name", isneed_name_check ? "FORCE_CHECK" : "NO_CHECK");
                p.SetValue("re_user_name", username);
                p.SetValue("amount", (int)(amount * 100));
                p.SetValue("desc", desc);
                p.SetValue("spbill_create_ip", "8.8.8.8");
                p.SetValue("sign", WXBizMsgCrypt.GenMD5SignString(p, Mch_Key));

                var result = CallWeixinServer(string.Format("https://api.mch.weixin.qq.com/mmpaymkttransfers/promotion/transfers"), "", ResponseHeader_ContentType.xml, null, p, true);

                var rtn = FrameDLRObject.CreateInstance(@"{
issuccess:false,
error_code:'',
msg:'',
}");
                rtn.mch_billno = mch_billno;
                if (result is FrameDLRObject)
                {
                    GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "Pay2Customer result:" + ((FrameDLRObject)result).ToJSONString());
                    dynamic dresult = (FrameDLRObject)result;
                    if (dresult.return_code == "SUCCESS")
                    {
                        rtn.msg = dresult.return_msg;
                        if (dresult.result_code == "SUCCESS")
                        {
                            rtn.issuccess = true;

                        }
                        else
                        {
                            rtn.error_code = dresult.err_code;
                            rtn.msg = dresult.err_code_des;
                        }
                    }
                    else
                    {
                        rtn.error_code = dresult.err_code;
                        rtn.msg = dresult.err_code_des;
                    }
                }

                return rtn;
            }
            /// <summary>
            /// 付款给用户，异步调用
            /// </summary>
            /// <param name="appid"></param>
            /// <param name="openid"></param>
            /// <param name="username"></param>
            /// <param name="amount"></param>
            /// <param name="desc"></param>
            /// <param name="isneed_name_check"></param>
            /// <param name="callbackLA"></param>
            public void Pay2CustomerAsync(string appid, string openid, string username, double amount, string desc, bool isneed_name_check, string callbackLA, FrameDLRObject cachedata)
            {
                var mch_billno = Mch_ID + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "0";
                FrameDLRObject p = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                p.SetValue("mch_appid", appid);
                p.SetValue("mchid", Mch_ID);
                p.SetValue("nonce_str", Guid.NewGuid().ToString().Replace("-", ""));
                p.SetValue("partner_trade_no", mch_billno);
                p.SetValue("openid", openid);
                p.SetValue("check_name", isneed_name_check ? "FORCE_CHECK" : "NO_CHECK");
                p.SetValue("re_user_name", username);
                p.SetValue("amount", (int)(amount * 100));
                p.SetValue("desc", desc);
                p.SetValue("spbill_create_ip", "8.8.8.8");
                p.SetValue("sign", WXBizMsgCrypt.GenMD5SignString(p, Mch_Key));

                cachedata.SetValue("customercallback", callbackLA);

                CallWeixinServerAsync(string.Format("https://api.mch.weixin.qq.com/mmpaymkttransfers/promotion/transfers"),"", ResponseHeader_ContentType.xml,null, p, true, callbackLA, cachedata);
            }
            /// <summary>
            /// 生成一个临时二维码
            /// </summary>
            /// <param name="accesstoken"></param>
            /// <param name="scene_id"></param>
            /// <returns></returns>
            public dynamic CreateTempQR(string accesstoken, string scene_id)
            {
                var url = "https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token=" + accesstoken;
                dynamic re = CallWeixinServer(url, "", "", null, FrameDLRObject.CreateInstance(@"{
            expire_seconds: 0,
            action_name: 'QR_LIMIT_SCENE',
            action_info: {
            scene: {
                scene_str:'" + scene_id + @"',
                }
            }
        }"));
                var rtn = FrameDLRObject.CreateInstance(@"{
issuccess:false,
msg:''
}");
                if (re != null)
                {
                    if (ComFunc.nvl(re.errcode) == "")
                    {
                        rtn.ticket = ComFunc.nvl(re.ticket);
                        rtn.qrurl = "https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=" + ComFunc.UrlEncode(rtn.ticket);
                        rtn.expire_seconds = re.expire_seconds;
                        rtn.issuccess = true;
                        rtn.msg = "二维码生成成功";
                    }
                    else
                    {
                        rtn.issuccess = false;
                        rtn.msg = re.errmsg;
                    }

                }
                else
                {
                    rtn.issuccess = false;
                    rtn.msg = "二维码生成失败";
                }

                return rtn;
            }
            /// <summary>
            /// 向微信服务器上传素材
            /// </summary>
            /// <param name="stream"></param>
            /// <param name="filename"></param>
            /// <param name="type">媒体文件类型，分别有图片（image）、语音（voice）、视频（video）和缩略图（thumb）</param>
            /// <returns></returns>
            public dynamic UploadMedia(Stream stream, string filename, string type)
            {
                var rtn = FrameDLRObject.CreateInstance(@"{
issuccess:false,
msg:'',
}");

                var url = string.Format("https://api.weixin.qq.com/cgi-bin/media/upload?access_token={0}&type={1}", Access_Token, type);
                var p = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                p.filename = filename;
                p.name = "media";
                p.filecontenttype = Path.GetExtension(filename);
                p.filecontent = ComFunc.ConvertToBinary(stream);
                var result = CallWeixinServer(url, "", "multipart/form-data", null, p, false);

                if (result is FrameDLRObject)
                {
                    GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "SendTemplateMsg result:" + ((FrameDLRObject)result).ToJSONString());
                    dynamic dresult = (FrameDLRObject)result;
                    if (ComFunc.nvl(dresult.errcode) == "")
                    {
                        rtn.issuccess = true;
                        rtn.type = dresult.type;
                        rtn.media_id = dresult.media_id;
                    }
                    else
                    {
                        rtn.issuccess = false;
                        rtn.msg = dresult.errmsg;
                    }
                }

                return rtn;
            }
            /// <summary>
            /// 上传本地文件
            /// </summary>
            /// <param name="filepath"></param>
            /// <param name="type"></param>
            /// <returns></returns>
            public dynamic UploadMedia(string filepath, string type)
            {
                var rtn = FrameDLRObject.CreateInstance(@"{
issuccess:false,
msg:'',
}");
                if (!File.Exists(filepath))
                {
                    rtn.issuccess = false;
                    rtn.msg = "文件不存在";
                    return rtn;
                }

                var stream = File.Open(filepath, FileMode.Open);
                var filename = Path.GetFileName(filepath);
                return UploadMedia(stream, filename, type);

            }
            /// <summary>
            /// 下载临时媒体文件
            /// </summary>
            /// <param name="mediaid"></param>
            /// <returns></returns>
            public dynamic DownloadMedia(string mediaid)
            {
                var rtn = FrameDLRObject.CreateInstance(@"{
issuccess:false,
msg:'',
}");
                var url = string.Format("http://api.weixin.qq.com/cgi-bin/media/get?access_token={0}&media_id={1}", Access_Token, mediaid);
                dynamic result = CallWeixinServer(url);
                if (ComFunc.nvl(result.errcode) == "")
                {
                    rtn.issuccess = true;
                    rtn.content = result.content;
                    rtn.filename = result.filename;
                    rtn.contenttype = result.contenttype;
                }
                else
                {
                    rtn.issuccess = false;
                    rtn.msg = result.errmsg;
                }

                return rtn;
            }
            /// <summary>
            /// 发送模板消息
            /// </summary>
            /// <param name="p">消息内容参数，格式件http://mp.weixin.qq.com/wiki/17/304c1885ea66dbedf7dc170d84999a9d.html#.E8.8E.B7.E5.BE.97.E6.A8.A1.E6.9D.BFID</param>
            /// <returns>格式：{
            ///issuccess:false,
            ///error_code:'',
            ///msg:'',
            ///}</returns>
            public dynamic SendTemplateMsg(FrameDLRObject p)
            {
                var url = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token=" + Access_Token;
                var result = CallWeixinServer(url, "", ResponseHeader_ContentType.json, null, p, false);

                var rtn = FrameDLRObject.CreateInstance(@"{
issuccess:false,
error_code:'',
msg:'',
}");
                if (result is FrameDLRObject)
                {
                    GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "SendTemplateMsg result:" + ((FrameDLRObject)result).ToJSONString());
                    dynamic dresult = (FrameDLRObject)result;
                    if (dresult.errcode == 0)
                    {
                        rtn.issuccess = true;
                        rtn.msg = dresult.errmsg;

                    }
                    else
                    {
                        rtn.issuccess = false;
                        rtn.msg = dresult.errmsg;
                    }
                }

                return rtn;
            }
            /// <summary>
            /// 发送客服消息，用户请求48小时内可以使用
            /// </summary>
            /// <param name="p"></param>
            /// <returns></returns>
            public dynamic SendCustomerServiceMsg(FrameDLRObject p)
            {
                var accesstoken = ComFunc.nvl(p.GetValue("access_token"));
                accesstoken = accesstoken == "" ? Access_Token : accesstoken;
                var url = "https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token=" + accesstoken;
                p.Remove("access_token");
                var result = CallWeixinServer(url, "", ResponseHeader_ContentType.json, null, p, false);

                var rtn = FrameDLRObject.CreateInstance(@"{
issuccess:false,
error_code:'',
msg:'',
}");
                if (result is FrameDLRObject)
                {
                    GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "SendCustomerServiceMsg result:" + ((FrameDLRObject)result).ToJSONString());
                    dynamic dresult = (FrameDLRObject)result;
                    if (ComFunc.nvl(dresult.errcode) == "0")
                    {
                        rtn.issuccess = true;
                        rtn.msg = dresult.errmsg;

                    }
                    else
                    {
                        rtn.issuccess = false;
                        rtn.msg = dresult.errmsg;
                    }
                }

                return rtn;
            }
            /// <summary>
            /// 客服消息接口，发送文本信息
            /// </summary>
            /// <param name="msg"></param>
            /// <returns></returns>
            public dynamic SendCustomerServiceMsg_Text(string openid, string msg)
            {
                return SendCustomerServiceMsg_Text(Access_Token, openid, msg);
            }
            /// <summary>
            /// 客服消息接口，发送文本信息
            /// </summary>
            /// <param name="appid"></param>
            /// <param name="openid"></param>
            /// <param name="msg"></param>
            /// <returns></returns>
            public dynamic SendCustomerServiceMsg_Text(string accesstoken, string openid, string msg)
            {
                var p = FrameDLRObject.CreateInstance(@"{
    access_token:'" + accesstoken + @"',
    touser:'" + openid + @"',
    msgtype:'text',
    text:
    {
         content:'" + msg + @"'
    }
}", FrameDLRFlags.SensitiveCase);

                return SendCustomerServiceMsg(p);
            }
            /// <summary>
            /// 客服消息接口，发送图片信息
            /// </summary>
            /// <param name="msg"></param>
            /// <returns></returns>
            public dynamic SendCustomerServiceMsg_Pic(string openid, string media_id)
            {
                return SendCustomerServiceMsg_Pic(Access_Token, openid, media_id);
            }
            /// <summary>
            /// 客服消息接口，发送图片信息
            /// </summary>
            /// <param name="accesstoken"></param>
            /// <param name="openid"></param>
            /// <param name="media_id"></param>
            /// <returns></returns>
            public dynamic SendCustomerServiceMsg_Pic(string accesstoken, string openid, string media_id)
            {
                var p = FrameDLRObject.CreateInstance(@"{
    access_token:'" + accesstoken + @"',
    touser:'" + openid + @"',
    msgtype:'image',
    image:
    {
      media_id:'" + media_id + @"'
    }
}", FrameDLRFlags.SensitiveCase);

                return SendCustomerServiceMsg(p);
            }
            /// <summary>
            /// 客服消息接口，发送语音消息
            /// </summary>
            /// <param name="msg"></param>
            /// <returns></returns>
            public dynamic SendCustomerServiceMsg_Voice(string openid, string media_id)
            {
                return SendCustomerServiceMsg_Voice(Access_Token, openid, media_id);
            }
            /// <summary>
            /// 客服消息接口，发送语音消息
            /// </summary>
            /// <param name="accesstoken"></param>
            /// <param name="openid"></param>
            /// <param name="media_id"></param>
            /// <returns></returns>
            public dynamic SendCustomerServiceMsg_Voice(string accesstoken, string openid, string media_id)
            {
                var p = FrameDLRObject.CreateInstance(@"{
    access_token:'" + accesstoken + @"',
    touser:'" + openid + @"',
    msgtype:'voice',
    voice:
    {
      media_id:'" + media_id + @"'
    }
}", FrameDLRFlags.SensitiveCase);

                return SendCustomerServiceMsg(p);
            }
            /// <summary>
            /// 客服消息接口，发送视频消息
            /// </summary>
            /// <param name="msg"></param>
            /// <returns></returns>
            public dynamic SendCustomerServiceMsg_Video(string openid, string title, string desc, string thumb_media_id, string media_id)
            {
                return SendCustomerServiceMsg_Video(Access_Token, openid, title, desc, thumb_media_id, media_id);
            }
            /// <summary>
            /// 客服消息接口，发送视频消息
            /// </summary>
            /// <param name="accesstoken"></param>
            /// <param name="openid"></param>
            /// <param name="title"></param>
            /// <param name="desc"></param>
            /// <param name="thumb_media_id"></param>
            /// <param name="media_id"></param>
            /// <returns></returns>
            public dynamic SendCustomerServiceMsg_Video(string accesstoken, string openid, string title, string desc, string thumb_media_id, string media_id)
            {
                var p = FrameDLRObject.CreateInstance(@"{
                                access_token:'" + accesstoken + @"',
                                touser:'" + openid + @"',
                                msgtype:'video',
                                video:
                                {
                                    media_id:'" + media_id + @"',
                                    thumb_media_id:'" + thumb_media_id + @"',
                                    title:'" + title + @"',
                                    description:'" + desc + @"'
                                }
                            }", FrameDLRFlags.SensitiveCase);

                return SendCustomerServiceMsg(p);
            }
            /// <summary>
            /// 客服消息接口，发送音乐消息
            /// </summary>
            /// <param name="openid"></param>
            /// <param name="title"></param>
            /// <param name="desc"></param>
            /// <param name="thumb_media_id"></param>
            /// <param name="musicurl"></param>
            /// <param name="hqmusicurl"></param>
            /// <returns></returns>
            public dynamic SendCustomerServiceMsg_Music(string openid, string title, string desc, string thumb_media_id, string musicurl, string hqmusicurl)
            {
                return SendCustomerServiceMsg_Music(Access_Token, openid, title, desc, thumb_media_id, musicurl, hqmusicurl);
            }
            /// <summary>
            /// 客服消息接口，发送音乐消息
            /// </summary>
            /// <param name="openid"></param>
            /// <param name="title"></param>
            /// <param name="desc"></param>
            /// <param name="thumb_media_id"></param>
            /// <param name="musicurl"></param>
            /// <param name="hqmusicurl"></param>
            /// <returns></returns>
            public dynamic SendCustomerServiceMsg_Music(string accesstoken, string openid, string title, string desc, string thumb_media_id, string musicurl, string hqmusicurl)
            {
                var p = FrameDLRObject.CreateInstance(@"{
                                access_token:'" + accesstoken + @"',
                                touser:'" + openid + @"',
                                msgtype:'music',
                                music:
                                {
                                     title:'" + title + @"',
                                     description:'" + desc + @"',
                                     musicurl:'" + musicurl + @"',
                                     hqmusicurl:'" + hqmusicurl + @"',
                                     thumb_media_id:'" + thumb_media_id + @"' 
                                }
                            }", FrameDLRFlags.SensitiveCase);

                return SendCustomerServiceMsg(p);
            }
            /// <summary>
            /// 客服消息接口，发送图文消息 图文消息条数限制在10条以内，注意，如果图文数超过10，则将会无响应。
            /// </summary>
            /// <param name="openid"></param>
            /// <param name="articles">单条消息结构如下：
            /// {
            /// "title":"Happy Day",
            /// "description":"Is Really A Happy Day",
            /// "url":"URL",
            /// "picurl":"PIC_URL"
            ///},
            /// </param>
            /// <returns></returns>
            public dynamic SendCustomerServiceMsg_News(string openid, params FrameDLRObject[] items)
            {
                var p = FrameDLRObject.CreateInstance(@"{
                                touser:'" + openid + @"',
                                msgtype:'news',
                                news:
                                {
                                }
                            }", FrameDLRFlags.SensitiveCase);
                p.news.articles = items;
                return SendCustomerServiceMsg(p);
            }
            /// <summary>
            /// 客服消息接口，发送图文消息 图文消息条数限制在10条以内，注意，如果图文数超过10，则将会无响应。
            /// </summary>
            /// <param name="openid"></param>
            /// <param name="articles">单条消息结构如下：
            /// {
            /// "title":"Happy Day",
            /// "description":"Is Really A Happy Day",
            /// "url":"URL",
            /// "picurl":"PIC_URL"
            ///},
            /// </param>
            /// <returns></returns>
            public dynamic SendCustomerServiceMsg_News(string accesstoken, string openid, params FrameDLRObject[] items)
            {
                var p = FrameDLRObject.CreateInstance(@"{
                                touser:'" + openid + @"',
                                msgtype:'news',
                                news:
                                {
                                }
                            }", FrameDLRFlags.SensitiveCase);
                p.news.articles = items;
                return SendCustomerServiceMsg(p);
            }

            private dynamic GenResponseObject()
            {
                var rtn = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                rtn.ToUserName = FromUserName;
                rtn.FromUserName = ToUserName;
                rtn.CreateTime = DateTime.Now;
                return rtn;
            }
            /// <summary>
            /// 生成一个Text类型的回复数据
            /// </summary>
            /// <param name="content"></param>
            /// <returns></returns>
            public FrameDLRObject GenResponseText(string content)
            {
                var rtn = GenResponseObject();
                rtn.MsgType = "text";
                rtn.Content = content;
                return rtn;
            }
            /// <summary>
            /// 生成一个image类型的回复数据
            /// </summary>
            /// <param name="mediaid">通过素材管理接口上传多媒体文件，得到的id。</param>
            /// <returns></returns>
            public FrameDLRObject GenResponseImage(string mediaid)
            {
                var rtn = GenResponseObject();
                rtn.MsgType = "image";
                rtn.Image = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                rtn.Image.MediaId = mediaid;
                return rtn;
            }
            /// <summary>
            /// 生成一个image类型的回复数据
            /// </summary>
            /// <param name="mediaid">通过素材管理接口上传多媒体文件，得到的id。</param>
            /// <returns></returns>
            public FrameDLRObject GenResponseVoice(string mediaid)
            {
                var rtn = GenResponseObject();
                rtn.MsgType = "voice";
                rtn.Voice = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                rtn.Voice.MediaId = mediaid;
                return rtn;
            }
            /// <summary>
            /// 生成一个video类型的回复数据
            /// </summary>
            /// <param name="mediaid">通过素材管理接口上传多媒体文件，得到的id。</param>
            /// <param name="title">视频消息的标题</param>
            /// <param name="description">视频消息的描述</param>
            /// <returns></returns>
            public FrameDLRObject GenResponseVideo(string mediaid, string title, string description)
            {
                var rtn = GenResponseObject();
                rtn.MsgType = "video";
                rtn.Video = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                rtn.Video.MediaId = mediaid;
                rtn.Video.Title = title;
                rtn.Video.Description = description;
                return rtn;
            }
            /// <summary>
            /// 生成一个音乐消息类型的回复数据
            /// </summary>
            /// <param name="mediaid">缩略图的媒体id，通过素材管理接口上传多媒体文件，得到的id</param>
            /// <param name="title">音乐标题</param>
            /// <param name="description">音乐描述</param>
            /// <param name="MusicUrl">音乐链接</param>
            /// <param name="HQMusicUrl">高质量音乐链接，WIFI环境优先使用该链接播放音乐</param>
            /// <returns></returns>
            public FrameDLRObject GenResponseMusic(string mediaid, string title, string description, string MusicUrl, string HQMusicUrl)
            {
                var rtn = GenResponseObject();
                rtn.MsgType = "music";
                rtn.Music = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                rtn.Music.Title = title;
                rtn.Music.Description = description;
                rtn.Music.MusicUrl = MusicUrl;
                rtn.Music.HQMusicUrl = HQMusicUrl;
                rtn.Music.ThumbMediaId = mediaid;
                return rtn;
            }
            /// <summary>
            /// 生成一个图文消息类型的回复数据
            /// </summary>
            /// <param name="items">数组，动态对象为敏感大小写
            /// 构成如下：
            /// Title：图文消息标题
            /// Description：图文消息描述
            /// PicUrl：图片链接，支持JPG、PNG格式，较好的效果为大图360*200，小图200*200
            /// Url：点击图文消息跳转链接
            /// </param>
            /// <returns></returns>
            public FrameDLRObject GenResponseNews(params FrameDLRObject[] items)
            {
                var rtn = GenResponseObject();
                rtn.MsgType = "news";
                rtn.ArticleCount = items.Length;
                rtn.Articles = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                rtn.Articles.item = items;
                return rtn;
            }
            /// <summary>
            /// 长链接转短链接
            /// </summary>
            /// <param name="longurl"></param>
            /// <param name="accesstoken"></param>
            /// <returns></returns>
            public string Long2ShortUrl(string longurl, string accesstoken)
            {
                if (ComFunc.nvl(_logic.CacheHelper.GetCache("l2s_" + longurl)) != "")
                {
                    return ComFunc.nvl(_logic.CacheHelper.GetCache("l2s_" + longurl));
                }
                else
                {
                    var url = @"https://api.weixin.qq.com/cgi-bin/shorturl?access_token=" + accesstoken;
                    var postdata = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                    postdata.action = "long2short";
                    postdata.long_url = longurl;
                    var result = CallWeixinServer(url, "Post", ResponseHeader_ContentType.json, null, postdata, false);

                    if (result is FrameDLRObject)
                    {
                        GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "Long2ShortUrl result:" + ((FrameDLRObject)result).ToJSONString());
                        dynamic dresult = (FrameDLRObject)result;
                        if (dresult.errcode == 0)
                        {
                            _logic.CacheHelper.SetCache("l2s_" + longurl, dresult.short_url, new TimeSpan(3, 0, 0, 0));
                            return ComFunc.nvl(dresult.short_url);
                        }
                    }

                    return "";
                }
            }
            /// <summary>
            /// 长链接转短链接
            /// </summary>
            /// <param name="longurl"></param>
            /// <returns></returns>
            public string Long2ShortUrl(string longurl)
            {
                return Long2ShortUrl(longurl, Access_Token);
            }


            public FrameDLRObject GetUserInfo(string openid)
            {
                var rtn = GenResponseObject();
                dynamic info = CallWeixinServer(string.Format("https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}&lang=zh_CN", Access_Token, openid));

                rtn.nickname = ComFunc.nvl(info.nickname);//用户昵称
                rtn.sex = IntStd.IsInt(info.sex) ? IntStd.ParseStd(info.sex).Value : 0;//用户性别：值为1时是男性，值为2时是女性，值为0时是未知
                rtn.headurl = ComFunc.nvl(info.headimgurl);//用户头像链接
                rtn.language = ComFunc.nvl(info.language);//用户的语言，简体中文为zh_CN
                rtn.city = ComFunc.nvl(info.city);//用户所在城市
                rtn.province = ComFunc.nvl(info.province);//用户所在省份
                rtn.country = ComFunc.nvl(info.country);//用户所在国家
                rtn.remark = ComFunc.nvl(info.remark);//公众号运营者对粉丝的备注，公众号运营者可在微信公众平台用户管理界面对粉丝添加备注
                rtn.subscribe_time = IntStd.IsInt(info.subscribe_time) ? new DateTime(1970, 1, 1).AddSeconds(IntStd.ParseStd(info.subscribe_time).Value) : DateTime.MinValue;//用户关注时间，如果用户曾多次关注，则取最后关注时间
                return rtn;
            }
            /// <summary>
            /// 呼叫微信服务
            /// </summary>
            /// <typeparam name="RType">返回值类型，FrameDLRObject、string、stream或者指定的model</typeparam>
            /// <param name="url"></param>
            /// <param name="method"></param>
            /// <param name="contenttype"></param>
            /// <param name="header"></param>
            /// <param name="data"></param>
            /// <param name="isneedcert"></param>
            /// <returns></returns>
            private RType CallWeixinServer<RType>(string url, string method = "POST", string contenttype = "text/json", FrameDLRObject header = null, FrameDLRObject data = null, bool isneedcert = false)
            {
                var result = GlobalCommon.Proxys["weixinserver"].CallModule<RType>(new
                {
                    Url = url,
                    ContentEncoding = Encoding.UTF8,
                    ContentType = string.IsNullOrEmpty(contenttype) ? "text/json" : contenttype,
                    Certificate = isneedcert ? new X509Certificate2(_logic.CallContext_Parameter.WebParam.ExtentionObj.weixin.weixin_mch_ssl_path, _logic.CallContext_Parameter.WebParam.ExtentionObj.weixin.weixin_mch_ssl_pass, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet) : null,
                    RequestMethod = string.IsNullOrEmpty(method) ? "POST" : method,
                    PostData = data,
                    Header = header

                }).GetAwaiter().GetResult();
                return result;
            }
            /// <summary>
            /// 呼叫微信服务
            /// </summary>
            /// <param name="url"></param>
            /// <param name="method"></param>
            /// <param name="contenttype"></param>
            /// <param name="header"></param>
            /// <param name="data"></param>
            /// <param name="isneedcert"></param>
            /// <returns></returns>
            private FrameDLRObject CallWeixinServer(string url, string method = "POST", string contenttype = "text/json", FrameDLRObject header = null, FrameDLRObject data = null, bool isneedcert = false)
            {
                return CallWeixinServer<FrameDLRObject>(url, method, contenttype, header, data, isneedcert);
            }
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
                var result = GlobalCommon.Proxys["weixinserver"].CallModule<RType>(new
                {
                    Url = url,
                    ContentEncoding = Encoding.UTF8,
                    ContentType = string.IsNullOrEmpty(contenttype) ? "text/json" : contenttype,
                    Certificate = isneedcert ? new X509Certificate2(_logic.CallContext_Parameter.WebParam.ExtentionObj.weixin.weixin_mch_ssl_path, _logic.CallContext_Parameter.WebParam.ExtentionObj.weixin.weixin_mch_ssl_pass, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet) : null,
                    RequestMethod = string.IsNullOrEmpty(method) ? "POST" : method,
                    PostData = data,
                    Header = header

                });
                if (!string.IsNullOrEmpty(callbackLA))
                {
                    result.ContinueWith(r =>
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
                        recorddata = recorddata == null ? FrameDLRObject.CreateInstance() : recorddata;
                        recorddata.SetValue("weixincallbackresult", r.Result);
                        _logic.OuterInterface.CallLocalLogic(logic, action, recorddata);
                    });
                }
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
        }
    }
}
