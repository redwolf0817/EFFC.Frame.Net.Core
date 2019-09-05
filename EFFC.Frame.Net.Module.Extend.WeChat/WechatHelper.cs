using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Global;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Security;

namespace EFFC.Frame.Net.Module.Extend.WeChat
{
    public class WechatHelper
    {
        static object lockobj = new object();
        /// <summary>
        /// 获取一个微信Helper的实例
        /// </summary>
        /// <param name="appid">微信的AppID，微信平台申请的账号</param>
        /// <param name="app_secret">微信的App密钥，微信平台申请的时候生成的</param>
        /// <param name="token">微信的访问Token，微信平台申请的时候生成的</param>
        /// <param name="mch_id">微信商户的id，默认为空，为空的时候则无法使用微信商户的功能</param>
        /// <param name="mch_key">微信商户的密钥，默认为空，为空的时候则无法使用微信商户的功能</param>
        /// <param name="mch_ssl_path">微信商户的数字证书存放的物理路径，默认为空，为空的时候则无法使用微信商户的功能</param>
        /// <param name="mch_ssl_pass">微信商户数字证书的密码，默认为空，为空的时候则无法使用微信商户的功能</param>
        public WechatHelper(string appid,string app_secret,string token,string mch_id="",string mch_key="",string mch_ssl_path="",string mch_ssl_pass = "")
        {
            AppID = appid;
            AppSecret = app_secret;
            Mch_ID = mch_id;
            Mch_Key = mch_key;
            Weixin_Mch_Ssl_Path = mch_ssl_pass;
            Weixin_Mch_Ssl_Pass = mch_ssl_pass;
            Token = token;
            if (!GlobalCommon.Proxys.HasProxy("weixinserver")) GlobalCommon.Proxys.UseProxy<WeixinHttpCallProxy>("weixinserver");
        }
        /// <summary>
        /// 主微信号商户支付秘钥
        /// </summary>
        public string Mch_Key { get; protected set; }
        /// <summary>
        /// 主微信号的APPID
        /// </summary>
        public string AppID { get; protected set; }
        /// <summary>
        /// 主微信号商户ID
        /// </summary>
        public string Mch_ID { get; protected set; }
        /// <summary>
        /// 主微信接入时用的Token
        /// </summary>
        public string Token { get; protected set; }
        /// <summary>
        /// 主微信号的访问API使用的Access Token
        /// </summary>
        public string Access_Token
        {
            get
            {
                return GenAccess_Token(AppID, AppSecret);
            }
        }
        /// <summary>
        /// 主微信号的APPSecret
        /// </summary>
        public string AppSecret
        {
            get;
            protected set;
        }
        /// <summary>
        /// 微信商户号的数字证书的物理路径
        /// </summary>
        public string Weixin_Mch_Ssl_Path { get; protected set; }
        /// <summary>
        /// 微信商户号的数字证书的密码
        /// </summary>
        public string Weixin_Mch_Ssl_Pass { get; protected set; }
        /// <summary>
        /// 根据AppID和appsecret获取对应微信号下的access_token
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="appsecret"></param>
        /// <returns></returns>
        public string GenAccess_Token(string appid, string appsecret)
        {
            if (GlobalCommon.ApplicationCache.Get(appid + "_weixin_access_token") == null)
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
                    GlobalCommon.ApplicationCache.Set(appid + "_weixin_access_token", token, DateTime.Now.AddSeconds(IntStd.ParseStd(expireseconds).Value - 10));
                }
                GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, "***** GoLogic.Weixin.cs GenAccess_Token End *********************************************************************************************");

            }

            return ComFunc.nvl(GlobalCommon.ApplicationCache.Get(appid + "_weixin_access_token"));
        }
        /// <summary>
        /// 微信调用JSAPI需要用到的JSAPI_Ticket,用于做AES的数字签名
        /// </summary>
        public string Jsapi_ticket
        {
            get
            {
                if (GlobalCommon.ApplicationCache.Get("weixin_jsapi_ticket") == null)
                {
                    var result = CallWeixinServer(string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi", Access_Token));
                    dynamic dobj = (FrameDLRObject)result;
                    var ticket = ComFunc.nvl(dobj.ticket);
                    var expireseconds = ComFunc.nvl(dobj.expires_in);
                    if (ticket != "")
                    {
                        //获取之后将超时时间缩短10秒，微信默认超时时间为7200秒，每获取一次就会重置该token
                        GlobalCommon.ApplicationCache.Set("weixin_jsapi_ticket", ticket, DateTime.Now.AddSeconds(IntStd.ParseStd(expireseconds).Value - 10));
                    }
                }

                return ComFunc.nvl(GlobalCommon.ApplicationCache.Get("weixin_jsapi_ticket"));
            }
        }
        /// <summary>
        /// 获取Jsapi_ticket
        /// </summary>
        /// <param name="accesstoken"></param>
        /// <returns></returns>
        public string GenJsapi_ticket(string accesstoken)
        {
            if (GlobalCommon.ApplicationCache.Get(accesstoken + "_weixin_jsapi_ticket") == null)
            {
                var result = CallWeixinServer(string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi", Access_Token));
                dynamic dobj = (FrameDLRObject)result;
                var ticket = ComFunc.nvl(dobj.ticket);
                var expireseconds = ComFunc.nvl(dobj.expires_in);
                if (ticket != "")
                {
                    //获取之后将超时时间缩短10秒，微信默认超时时间为7200秒，每获取一次就会重置该token
                    GlobalCommon.ApplicationCache.Set(accesstoken + "_weixin_jsapi_ticket", ticket, DateTime.Now.AddSeconds(IntStd.ParseStd(expireseconds).Value - 10));
                }
            }

            return ComFunc.nvl(GlobalCommon.ApplicationCache.Get(accesstoken + "_weixin_jsapi_ticket"));
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
            var token = ComFunc.nvl(Token);

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
        /// 调用统一下单接口，获取预付单信息,H5支付
        /// </summary>
        /// <param name="notify_url">回调url</param>
        /// <param name="orderno">订单号</param>
        /// <param name="amount">金额</param>
        /// <param name="body">商品描述</param>
        /// <param name="attachdata">附加信息</param>
        /// <param name="ip">终端IP</param>
        /// <param name="goods_tags">商品标记</param>
        /// <param name="wap_url">WAP网站URL地址</param>
        /// <param name="wap_name">WAP 网站名</param>
        /// <returns></returns>
        public FrameDLRObject UnifiedOrder4MWEB(string notify_url, 
            string orderno, 
            double amount, 
            string body, 
            string attachdata,
            string ip, 
            string goods_tags,
            string wap_url,
            string wap_name)
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
            p.SetValue("trade_type", "MWEB");
            var h5_info = new
            {
                h5_info = new
                {
                    type = "Wap",
                    wap_url = wap_url,
                    wap_name = wap_name
                }
            };
            p.SetValue("scene_info", FrameDLRObject.CreateInstance(h5_info).tojsonstring());

            p.SetValue("sign", WXBizMsgCrypt.GenMD5SignString(p, Mch_Key));

            var result = CallWeixinServer(string.Format("https://api.mch.weixin.qq.com/pay/unifiedorder"), "", ResponseHeader_ContentType.xml, null, p);
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "UnifiedOrder4MWEB result:" + ((FrameDLRObject)result).ToJSONString());
            var rtn = FrameDLRObject.CreateInstance(@"{
issuccess:false,
msg:'',
prepay_id:'',
mweb_url:''
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
                        rtn.mweb_url = dresult.mweb_url;
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
        /// <param name="openid">接收人openid</param>
        /// <param name="totalamount">总额度</param>
        /// <param name="total_num">总数量</param>
        /// <param name="wishing">红包祝福语</param>
        /// <param name="act_name">活动名称</param>
        /// <param name="remark">备注</param>
        /// <param name="send_name">发送者名称</param>
        /// <param name="callback">回调方法</param>
        /// <param name="cachedata">回调时缓存用的数据</param>
        public void RedPacketAsync(string openid,
            double totalamount, int total_num,
            string wishing, string act_name, string remark,
            string send_name, Action<object> callback, FrameDLRObject cachedata)
        {
            RedPacketAsync(AppID, openid, totalamount, total_num, wishing, act_name, remark, send_name, callback, cachedata);
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
        /// <param name="callback">回调的logic.action</param>
        /// <param name="cachedata">回调中需要保留的数据</param>
        public void RedPacketAsync(string appid, string openid,
            double totalamount, int total_num,
            string wishing, string act_name, string remark,
            string send_name, Action<object> callback, FrameDLRObject cachedata)
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

            cachedata.SetValue("customercallback", callback);

            CallWeixinServerAsync(string.Format("https://api.mch.weixin.qq.com/mmpaymkttransfers/sendredpack"), "", ResponseHeader_ContentType.xml, null, p, true, callback, cachedata);
        }
        /// <summary>
        /// 获取异步回调结果集
        /// </summary>
        public dynamic CallBackResult
        {
            get;
            set;
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
        /// <param name="callback"></param>
        /// <param name="cachedata"></param>
        public void Pay2CustomerAsync(string appid, string openid, string username, double amount, string desc, bool isneed_name_check, Action<object> callback, FrameDLRObject cachedata)
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

            cachedata.SetValue("customercallback", callback);

            CallWeixinServerAsync(string.Format("https://api.mch.weixin.qq.com/mmpaymkttransfers/promotion/transfers"), "", ResponseHeader_ContentType.xml, null, p, true, callback, cachedata);
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
        [Obsolete("该方法已过期，不建议使用")]
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
        /// 发送模板消息
        /// </summary>
        /// <param name="touser_openid">接收人的openid</param>
        /// <param name="template_id">模板id</param>
        /// <param name="url">模板点击后跳转的url</param>
        /// <param name="miniprogram_appid">小程序appid，默认为空</param>
        /// <param name="miniprogram_pagepath">小程序的页面，默认为空</param>
        /// <param name="firstvalue">first的内容</param>
        /// <param name="remarkvalue">remark的内容</param>
        /// <param name="keywords">keywords的内容</param>
        /// <returns></returns>
        public dynamic SendTemplateMsg(string touser_openid, string template_id,
            string url = "",
            string miniprogram_appid = "",
            string miniprogram_pagepath = "",
            string firstvalue = "",
            string remarkvalue = "",
            params string[] keywords)
        {
            FrameDLRObject data = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            data.SetValue("first", new
            {
                value = firstvalue,
                color = ""
            });
            if(keywords!= null)
            {
                for(var i = 1; i <= keywords.Length; i++)
                {
                    data.SetValue($"keyword{i}", new
                    {
                        value = keywords[i - 1],
                        color = ""
                    });
                }
            }
            data.SetValue("remark", new
            {
                value = remarkvalue,
                color = ""
            });
            return SendTemplateMsg(touser_openid, template_id, url, miniprogram_appid, miniprogram_pagepath, data);
        }
        /// <summary>
        /// 发送模板消息
        /// </summary>
        /// <param name="touser_openid">接收人的openid</param>
        /// <param name="template_id">模板id</param>
        /// <param name="url">模板点击后跳转的url</param>
        /// <param name="miniprogram_appid">小程序appid，默认为空</param>
        /// <param name="miniprogram_pagepath">小程序的页面，默认为空</param>
        /// <param name="data">发送数据，格式如下：
        ///     {
        ///           "first": {
        ///               "value":"恭喜你购买成功！",
        ///              "color":"#173177"
        ///           },
        ///          "keyword1":{
        ///               "value":"巧克力",
        ///              "color":"#173177"
        ///           },
        ///          "keyword2": {
        ///              "value":"39.8元",
        ///              "color":"#173177"
        ///          },
        ///          "keyword3": {
        ///              "value":"2014年9月22日",
        ///              "color":"#173177"
        ///          },
        ///         "remark":{
        ///             "value":"欢迎再次购买！",
        ///             "color":"#173177"
        ///     }
        /// </param>
        /// <returns></returns>
        public dynamic SendTemplateMsg(string touser_openid, string template_id,
            string url = "",
            string miniprogram_appid = "",
            string miniprogram_pagepath = "",
            object data=null)
        {
            var content = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            content.touser = touser_openid;
            content.template_id = template_id;
            content.url = url;
            if (!string.IsNullOrEmpty(miniprogram_appid))
            {
                content.miniprogram = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                content.miniprogram.appid = miniprogram_appid;
                content.miniprogram.pagepath = miniprogram_pagepath;
            }
            content.data = data;

            var sendurl = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token=" + Access_Token;
            var result = CallWeixinServer(sendurl, "", ResponseHeader_ContentType.json, null, content, false);

            var rtn = FrameDLRObject.CreateInstance(@"{
issuccess:false,
msg:''
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
msg:''
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
        /// <param name="openid"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public dynamic SendCustomerServiceMsg_Text(string openid, string msg)
        {
            return SendCustomerServiceMsg_Text(Access_Token, openid, msg);
        }
        /// <summary>
        /// 客服消息接口，发送文本信息
        /// </summary>
        /// <param name="accesstoken"></param>
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
        /// <param name="openid"></param>
        /// <param name="media_id"></param>
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
        /// <param name="openid"></param>
        /// <param name="media_id"></param>
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
        /// <param name="openid"></param>
        /// <param name="title"></param>
        /// <param name="desc"></param>
        /// <param name="thumb_media_id"></param>
        /// <param name="media_id"></param>
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
        /// <param name="accesstoken"></param>
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
        /// <param name="items">单条消息结构如下：
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
        /// <param name="accesstoken">授权token</param>
        /// <param name="openid"></param>
        /// <param name="items">单条消息结构如下：
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

        private dynamic GenResponseObject(string touser,string fromuser)
        {
            var rtn = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            rtn.ToUserName = fromuser;
            rtn.FromUserName = touser;
            rtn.CreateTime = DateTime.Now;
            return rtn;
        }
        /// <summary>
        /// 被动消息回复，生成一个Text类型的回复数据
        /// </summary>
        /// <param name="content"></param>
        /// <param name="touser">接收消息的openid</param>
        /// <param name="fromuser">开发者微信号</param>
        /// <returns></returns>
        public FrameDLRObject GenResponseText(string content, string touser, string fromuser)
        {
            var rtn = GenResponseObject(touser,fromuser);
            rtn.MsgType = "text";
            rtn.Content = content;
            return rtn;
        }
        /// <summary>
        /// 被动消息回复，生成一个image类型的回复数据
        /// </summary>
        /// <param name="mediaid">通过素材管理接口上传多媒体文件，得到的id。</param>
        /// <param name="touser">接收消息的openid</param>
        /// <param name="fromuser">开发者微信号</param>
        /// <returns></returns>
        public FrameDLRObject GenResponseImage(string mediaid, string touser, string fromuser)
        {
            var rtn = GenResponseObject(touser, fromuser);
            rtn.MsgType = "image";
            rtn.Image = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            rtn.Image.MediaId = mediaid;
            return rtn;
        }
        /// <summary>
        /// 被动消息回复，生成一个image类型的回复数据
        /// </summary>
        /// <param name="mediaid">通过素材管理接口上传多媒体文件，得到的id。</param>
        /// <param name="touser">接收消息的openid</param>
        /// <param name="fromuser">开发者微信号</param>
        /// <returns></returns>
        public FrameDLRObject GenResponseVoice(string mediaid, string touser, string fromuser)
        {
            var rtn = GenResponseObject(touser, fromuser);
            rtn.MsgType = "voice";
            rtn.Voice = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            rtn.Voice.MediaId = mediaid;
            return rtn;
        }
        /// <summary>
        /// 被动消息回复，生成一个video类型的回复数据
        /// </summary>
        /// <param name="mediaid">通过素材管理接口上传多媒体文件，得到的id。</param>
        /// <param name="title">视频消息的标题</param>
        /// <param name="description">视频消息的描述</param>
        /// <param name="touser">接收消息的openid</param>
        /// <param name="fromuser">开发者微信号</param>
        /// <returns></returns>
        public FrameDLRObject GenResponseVideo(string mediaid, string title, string description, string touser, string fromuser)
        {
            var rtn = GenResponseObject(touser, fromuser);
            rtn.MsgType = "video";
            rtn.Video = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            rtn.Video.MediaId = mediaid;
            rtn.Video.Title = title;
            rtn.Video.Description = description;
            return rtn;
        }
        /// <summary>
        /// 被动消息回复，生成一个音乐消息类型的回复数据
        /// </summary>
        /// <param name="mediaid">缩略图的媒体id，通过素材管理接口上传多媒体文件，得到的id</param>
        /// <param name="title">音乐标题</param>
        /// <param name="description">音乐描述</param>
        /// <param name="MusicUrl">音乐链接</param>
        /// <param name="HQMusicUrl">高质量音乐链接，WIFI环境优先使用该链接播放音乐</param>
        /// <param name="touser">接收消息的openid</param>
        /// <param name="fromuser">开发者微信号</param>
        /// <returns></returns>
        public FrameDLRObject GenResponseMusic(string mediaid, string title, string description, string MusicUrl, string HQMusicUrl, string touser, string fromuser)
        {
            var rtn = GenResponseObject(touser, fromuser);
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
        /// 被动消息回复，生成一个图文消息类型的回复数据
        /// </summary>
        /// <param name="touser">接收消息的openid</param>
        /// <param name="fromuser">开发者微信号</param>
        /// <param name="items">数组，动态对象为敏感大小写
        /// 构成如下：
        /// Title：图文消息标题
        /// Description：图文消息描述
        /// PicUrl：图片链接，支持JPG、PNG格式，较好的效果为大图360*200，小图200*200
        /// Url：点击图文消息跳转链接
        /// </param>
        /// <returns></returns>
        public FrameDLRObject GenResponseNews(string touser, string fromuser,params FrameDLRObject[] items)
        {
            var rtn = GenResponseObject(touser, fromuser);
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
            if (ComFunc.nvl(GlobalCommon.ApplicationCache.Get("l2s_" + longurl)) != "")
            {
                return ComFunc.nvl(GlobalCommon.ApplicationCache.Get("l2s_" + longurl));
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
                        GlobalCommon.ApplicationCache.Set("l2s_" + longurl, dresult.short_url, new TimeSpan(3, 0, 0, 0));
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

        /// <summary>
        /// 获取用户的明细信息
        /// </summary>
        /// <param name="openid">要查询用户的openid号</param>
        /// <returns></returns>
        public FrameDLRObject GetUserInfo(string openid)
        {
            return GetUserInfo(openid, Access_Token);
        }
        /// <summary>
        /// 获取用户的明细信息
        /// </summary>
        /// <param name="openid">要查询用户的openid号</param>
        /// <param name="access_token">访问微信的AccessToken</param>
        /// <returns></returns>
        public FrameDLRObject GetUserInfo(string openid,string access_token)
        {
            return CallWeixinServer(string.Format("https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}&lang=zh_CN", access_token, openid));
        }
        /// <summary>
        /// 获取用户的openid-微信网页授权方式
        /// </summary>
        /// <param name="code">微信提供换取token的code</param>
        /// <param name="access_token">传出的access token</param>
        /// <param name="refresh_token">传出的refresh token</param>
        /// <param name="openid">用户openid</param>
        /// <param name="expires_in">超时秒数</param>
        /// <param name="errcode">出错时微信返回的错误代码</param>
        /// <param name="errmsg">出错时微信返回的错误信息</param>
        /// <returns></returns>
        public bool GetOpenID4PageAuth(string code, ref string access_token, ref string refresh_token, ref string openid, ref int expires_in,ref string errcode,ref string errmsg)
        {
            dynamic result = CallWeixinServer(string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code", AppID, AppSecret, code));
            if (ComFunc.nvl(result.errcode) == "")
            {
                access_token = ComFunc.nvl(result.access_token);
                refresh_token = ComFunc.nvl(result.refresh_token);
                expires_in = IntStd.IsNotIntThen(result.expires_in, 100);
                openid = ComFunc.nvl(result.openid);
                return true;
            }
            else
            {
                errcode = ComFunc.nvl(result.errcode);
                errmsg = ComFunc.nvl(result.errmsg);
                return false;
            }
        }
        /// <summary>
        /// 获取用户的信息-微信网页授权方式，该方法调用之前需先调用GetOpenID4PageAuth来获取对应的access_token,refresh_token等参数
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="refresh_token"></param>
        /// <param name="expires_in"></param>
        /// <param name="open_id"></param>
        /// <returns></returns>
        public FrameDLRObject GetUserInfo4PageAuthBy(string access_token,string refresh_token,int expires_in, string open_id)
        {
            dynamic result = CallWeixinServer(string.Format("https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang=zh_CN", access_token, open_id), "get");
            return result;
        }
        /// <summary>
        /// 获取用户的明细信息-微信网页授权方式
        /// </summary>
        /// <param name="code"></param>
        /// <returns>如果成功则返回登录用户的信息资料，否则返回错误信息格式为微信返回的错误信息，{"errcode":40029,"errmsg":"invalid code"}</returns>
        public FrameDLRObject GetUserInfo4PageAuth(string code)
        {
            string key = $"GetUserInfo4PageAuth_{code}";
            lock (lockobj)
            {
                if (GlobalCommon.ApplicationCache.Get(key) != null)
                {
                    return (FrameDLRObject)GlobalCommon.ApplicationCache.Get(key);
                }
                var access_token = "";
                var refresh_token = "";
                var expires_in = 100;
                var openid = "";
                var errcode = "";
                var errmsg = "";
                if (GetOpenID4PageAuth(code,ref access_token,ref refresh_token,ref openid,ref expires_in,ref errcode,ref errmsg))
                {
                    expires_in = IntStd.IsNotIntThen(expires_in, 100);
                    dynamic result1 = GetUserInfo4PageAuthBy(access_token, refresh_token, expires_in, openid);
                    if (ComFunc.nvl(result1.errcode) == "")
                    {
                        GlobalCommon.ApplicationCache.Set(key, result1, DateTime.Now.AddSeconds(expires_in - 100));
                        return result1;
                    }
                    else
                    {
                        return result1;
                    }
                }
                else
                {
                    var rtn = FrameDLRObject.CreateInstance();
                    rtn.errcode = errcode;
                    rtn.errmsg = errmsg;
                    return rtn;
                }
            }
        }
        /// <summary>
        /// 创建自定义菜单
        /// </summary>
        /// <param name="menu_json">菜单内容，格式参考https://mp.weixin.qq.com/wiki?t=resource/res_main&id=mp1421141013</param>
        /// <returns></returns>
        public FrameDLRObject CreateMenu(FrameDLRObject menu_json)
        {
            var url = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token="+ Access_Token;
            return CallWeixinServer(url, "post", "text/json", null, menu_json);
            
        }
        /// <summary>
        /// 查询自定义菜单
        /// </summary>
        /// <param name="menu_json">菜单内容，格式参考https://mp.weixin.qq.com/wiki?t=resource/res_main&id=mp1421141013</param>
        /// <returns></returns>
        public FrameDLRObject GetMenu()
        {
            var url = "https://api.weixin.qq.com/cgi-bin/menu/get?access_token=" + Access_Token;
            return CallWeixinServer(url, "get");

        }
        /// <summary>
        /// 查询自定义菜单
        /// </summary>
        /// <param name="menu_json">菜单内容，格式参考https://mp.weixin.qq.com/wiki?t=resource/res_main&id=mp1421141013</param>
        /// <returns></returns>
        public FrameDLRObject DeleteMenu()
        {
            var url = "https://api.weixin.qq.com/cgi-bin/menu/delete?access_token=" + Access_Token;
            return CallWeixinServer(url, "get");

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
        public RType CallWeixinServer<RType>(string url, string method = "POST", string contenttype = "text/json", FrameDLRObject header = null, FrameDLRObject data = null, bool isneedcert = false)
        {
            var result = GlobalCommon.Proxys["weixinserver"].CallModule<RType>(new
            {
                Url = url,
                ContentEncoding = Encoding.UTF8,
                ContentType = string.IsNullOrEmpty(contenttype) ? "text/json" : contenttype,
                Certificate = isneedcert ? new X509Certificate2(Weixin_Mch_Ssl_Path, Weixin_Mch_Ssl_Pass, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet) : null,
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
        public FrameDLRObject CallWeixinServer(string url, string method = "POST", string contenttype = "text/json", FrameDLRObject header = null, FrameDLRObject data = null, bool isneedcert = false)
        {
            var result = CallWeixinServer<FrameDLRObject>(url, method, contenttype, header, data, isneedcert);
            if(result != null && ComFunc.nvl(result.GetValue("errcode")) != "" && ComFunc.nvl(result.GetValue("errcode")) != "0")
            {
                GlobalCommon.Logger.WriteLog(LoggerLevel.ERROR, $"CallWeixinServer Failed：{ComFunc.nvl(result.GetValue("errcode"))}-{ComFunc.nvl(result.GetValue("errmsg"))}");
            }
            return result;
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
        /// <param name="callback"></param>
        /// <param name="recorddata"></param>
        public void CallWeixinServerAsync<RType>(string url, string method = "POST", 
            string contenttype = "text/json", FrameDLRObject header = null, FrameDLRObject data = null, bool isneedcert = false, 
            Action<object> callback = null, FrameDLRObject recorddata = null)
        {
            var result = GlobalCommon.Proxys["weixinserver"].CallModule<RType>(new
            {
                Url = url,
                ContentEncoding = Encoding.UTF8,
                ContentType = string.IsNullOrEmpty(contenttype) ? "text/json" : contenttype,
                Certificate = isneedcert ? new X509Certificate2(Weixin_Mch_Ssl_Path, Weixin_Mch_Ssl_Pass, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet) : null,
                RequestMethod = string.IsNullOrEmpty(method) ? "POST" : method,
                PostData = data,
                Header = header

            });
            if (callback != null)
            {
                result.ContinueWith(r =>
                {
                    //string[] la = callbackLA.Split('.');
                    //var logic = "";
                    //var action = "";
                    //if (la.Length > 0)
                    //{
                    //    logic = la[0];
                    //}
                    //if (la.Length > 1)
                    //{
                    //    action = la[1];
                    //}
                    recorddata = recorddata == null ? FrameDLRObject.CreateInstance() : recorddata;
                    recorddata.SetValue("weixincallbackresult", r.Result);
                    //_logic.OuterInterface.CallLocalLogic(logic, action, recorddata);

                    callback.Invoke(recorddata);
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
        public void CallWeixinServerAsync(string url, string method = "POST", string contenttype = "text/json", FrameDLRObject header = null, FrameDLRObject data = null, bool isneedcert = false, Action<object> callback = null, FrameDLRObject recorddata = null)
        {
            CallWeixinServerAsync<FrameDLRObject>(url, method, contenttype, header, data, isneedcert, callback, recorddata);
        }
    }
}
