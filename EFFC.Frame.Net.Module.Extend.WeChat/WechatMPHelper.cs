using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Global;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.WeChat
{
    /// <summary>
    /// 微信小程序
    /// </summary>
    public class WechatMPHelper
    {
        /// <summary>
        /// 获取一个企业微信Helper的实例
        /// </summary>
        /// <param name="appid">微信的AppID，微信平台申请的账号</param>
        /// <param name="app_secret">微信的App密钥，微信平台申请的时候生成的</param>
        /// <param name="mch_ssl_path">微信商户的数字证书存放的物理路径，默认为空，为空的时候则无法使用微信商户的功能</param>
        /// <param name="mch_ssl_pass">微信商户数字证书的密码，默认为空，为空的时候则无法使用微信商户的功能</param>
        public WechatMPHelper(string appid, string app_secret, string mch_ssl_path = "", string mch_ssl_pass = "")
        {
            AppID = appid;
            AppSecret = app_secret;
            Weixin_Mch_Ssl_Path = mch_ssl_pass;
            Weixin_Mch_Ssl_Pass = mch_ssl_pass;
            if (!GlobalCommon.Proxys.HasProxy("weixinserver")) GlobalCommon.Proxys.UseProxy<WeixinHttpCallProxy>("weixinserver");
        }
        #region API地址
        /// <summary>
        /// 获取AccessToken的URL
        /// </summary>
        protected const string GetTokenUrl = @"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}";
        /// <summary>
        /// 登录凭证校验。通过 wx.login() 接口获得临时登录凭证 code 后传到开发者服务器调用此接口完成登录流程
        /// </summary>
        protected const string GetCode2SessionUrl = @"https://api.weixin.qq.com/sns/jscode2session?appid={0}&secret={1}&js_code={2}&grant_type=authorization_code";
        /// <summary>
        /// 用户支付完成后，获取该用户的 UnionId，无需用户授权
        /// </summary>
        protected const string GetUnionIDByPayUrl = @"https://api.weixin.qq.com/wxa/getpaidunionid?access_token={0}&openid={1}";
        /// <summary>
        /// 获取小程序新增或活跃用户的画像分布数据。时间范围支持昨天、最近7天、最近30天。其中，新增用户数为时间范围内首次访问小程序的去重用户数，活跃用户数为时间范围内访问过小程序的去重用户数
        /// </summary>
        protected const string GetUserPortraitUrl = @"https://api.weixin.qq.com/datacube/getweanalysisappiduserportrait?access_token={0}";
        /// <summary>
        /// 获取用户小程序访问分布数据
        /// </summary>
        protected const string GetVisitDistributionUrl = @"https://api.weixin.qq.com/datacube/getweanalysisappidvisitdistribution?access_token={0}";
        /// <summary>
        /// 访问页面。目前只提供按 page_visit_pv 排序的 top200。
        /// </summary>
        protected const string GetVisitPageUrl = @"https://api.weixin.qq.com/datacube/getweanalysisappidvisitpage?access_token={0}";
        /// <summary>
        /// 获取用户访问小程序数据概况
        /// </summary>
        protected const string GetDailySummaryUrl = @"https://api.weixin.qq.com/datacube/getweanalysisappiddailysummarytrend?access_token={0}";
        /// <summary>
        /// 下发客服当前输入状态给用户
        /// </summary>
        protected const string SetTypeUrl = @"https://api.weixin.qq.com/cgi-bin/message/custom/typing?access_token={0}";
        /// <summary>
        /// 把媒体文件上传到微信服务器。目前仅支持图片。用于发送客服消息或被动回复用户消息。
        /// </summary>
        protected const string UploadTempMediaUrl = @"https://api.weixin.qq.com/cgi-bin/media/upload?access_token={0}&type={0}";
        /// <summary>
        /// 获取客服消息内的临时素材。即下载临时的多媒体文件。目前小程序仅支持下载图片文件
        /// </summary>
        protected const string GetTempMediaUrl = @"https://api.weixin.qq.com/cgi-bin/media/get?access_token={0}&media_id={1}";
        /// <summary>
        /// 发送客服消息给用户
        /// </summary>
        protected const string SendCustomeServiceMsgUrl = @"https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token={0}";
        /// <summary>
        /// 发送模板消息
        /// </summary>
        protected const string SendTemplateMsgUrl = @"https://api.weixin.qq.com/cgi-bin/message/wxopen/template/send?access_token={0}";
        /// <summary>
        /// 获取小程序二维码，适用于需要的码数量较少的业务场景。通过该接口生成的小程序码，永久有效，有数量限制，
        /// </summary>
        protected const string CreateQRCodeUrl = @"https://api.weixin.qq.com/cgi-bin/wxaapp/createwxaqrcode?access_token={0}";
        /// <summary>
        /// 获取小程序码，适用于需要的码数量较少的业务场景。通过该接口生成的小程序码，永久有效，有数量限制
        /// </summary>
        protected const string GetQRCodeUrl = @"https://api.weixin.qq.com/wxa/getwxacode?access_token={0}";
        /// <summary>
        /// 获取小程序码，适用于需要的码数量极多的业务场景。通过该接口生成的小程序码，永久有效，数量暂无限制
        /// </summary>
        protected const string GetUnlimitedQRCodeUrl = @"https://api.weixin.qq.com/wxa/getwxacodeunlimit?access_token=ACCESS_TOKEN";
        #endregion

        #region 常用的属性

        #region 设定和验证类
        /// <summary>
        /// 企业微信的CropID, 这里和微信通用AppId
        /// </summary>
        public string AppID { get; protected set; }
        /// <summary>
        /// 企业微信,当前应用的Secret
        /// </summary>
        public string AppSecret { get; protected set; }
        /// <summary>
        /// 微信商户号的数字证书的物理路径
        /// </summary>
        public string Weixin_Mch_Ssl_Path { get; protected set; }
        /// <summary>
        /// 微信商户号的数字证书的密码
        /// </summary>
        public string Weixin_Mch_Ssl_Pass { get; protected set; }
        #endregion

        #region 公用类
        /// <summary>
        /// 获取微信的Access Token，用于与微信服务器进行信息交互
        /// </summary>
        public string Access_Token => GenAccess_Token(AppID, AppSecret);
        /// <summary>
        /// 当前新的微信时间戳
        /// </summary>
        public int NewTimsStamp => (int)(DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds;

        #endregion
        #region 底层呼叫方法
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
        protected RType CallWeixinServer<RType>(string url, string method = "POST", string contenttype = "text/json", FrameDLRObject header = null, FrameDLRObject data = null, bool isneedcert = false)
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
        protected FrameDLRObject CallWeixinServer(string url, string method = "POST", string contenttype = "text/json", FrameDLRObject header = null, FrameDLRObject data = null, bool isneedcert = false)
        {
            var result = CallWeixinServer<FrameDLRObject>(url, method, contenttype, header, data, isneedcert);
            if (result != null && ComFunc.nvl(result.GetValue("errcode")) != "" && ComFunc.nvl(result.GetValue("errcode")) != "0")
            {
                GlobalCommon.Logger.WriteLog(LoggerLevel.ERROR, $"WeixinMP CallWeixinServer Failed：{ComFunc.nvl(result.GetValue("errcode"))}-{ComFunc.nvl(result.GetValue("errmsg"))}");
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
        protected void CallWeixinServerAsync<RType>(string url, string method = "POST", string contenttype = "text/json", FrameDLRObject header = null, FrameDLRObject data = null, bool isneedcert = false, Action<object> callback = null, FrameDLRObject recorddata = null)
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
                    recorddata = recorddata == null ? FrameDLRObject.CreateInstance() : recorddata;
                    recorddata.SetValue("weixincallbackresult", r.Result);
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
        /// <param name="callback"></param>
        /// <param name="recorddata"></param>
        protected void CallWeixinServerAsync(string url, string method = "POST", string contenttype = "text/json", FrameDLRObject header = null, FrameDLRObject data = null, bool isneedcert = false, Action<object> callback = null, FrameDLRObject recorddata = null)
        {
            CallWeixinServerAsync<FrameDLRObject>(url, method, contenttype, header, data, isneedcert, callback, recorddata);
        }
        /// <summary>
        /// 根据AppID和appsecret获取对应微信号下的access_token
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="appsecret"></param>
        /// <returns></returns>
        public string GenAccess_Token(string appid, string appsecret)
        {
            if (GlobalCommon.ApplicationCache.Get(appid + "_weixinmp_access_token") == null)
            {
                GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, "***** WeixinMPHelper.cs GenAccess_Token start *********************************************************************************************");
                dynamic dobj = CallWeixinServer(string.Format(GetTokenUrl, appid, appsecret),"get");
                var token = ComFunc.nvl(dobj.access_token);
                var expireseconds = ComFunc.nvl(dobj.expires_in);
                GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, "appid: " + appid + ";  新Token:" + token + ";  expireseconds:" + expireseconds);
                //获取之后将超时时间缩短10秒，微信默认超时时间为7200秒，每获取一次就会重置该token
                if (token != "")
                    GlobalCommon.ApplicationCache.Set(appid + "_weixinmp_access_token", token, DateTime.Now.AddSeconds(IntStd.ParseStd(expireseconds).Value - 10));
                GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, "***** WeixinMPHelper.cs GenAccess_Token End *********************************************************************************************");

            }

            return ComFunc.nvl(GlobalCommon.ApplicationCache.Get(appid + "_weixin_access_token"));
        }

        /// <summary>
        /// 根据微信小程序平台提供的签名验证算法验证用户发来的数据是否有效
        /// </summary>
        /// <param name="rawData">公开的用户资料</param>
        /// <param name="signature">公开资料携带的签名信息</param>
        /// <param name="sessionKey">从服务端获取的SessionKey</param>
        /// <returns>True：资料有效，False：资料无效</returns>
        protected bool VaildateUserInfo(string rawData, string signature, string sessionKey)
        {
            //创建SHA1签名类
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            //编码用于SHA1验证的源数据
            byte[] source = Encoding.UTF8.GetBytes(rawData + sessionKey);
            //生成签名
            byte[] target = sha1.ComputeHash(source);
            //转化为string类型，注意此处转化后是中间带短横杠的大写字母，需要剔除横杠转小写字母
            string result = BitConverter.ToString(target).Replace("-", "").ToLower();
            //比对，输出验证结果
            return signature == result;
        }
        /// <summary>
        /// 根据微信小程序平台提供的解密算法解密数据
        /// </summary>
        /// <param name="encryptedData">加密数据</param>
        /// <param name="iv">初始向量</param>
        /// <param name="sessionKey">从服务端获取的SessionKey</param>
        /// <returns></returns>
        protected string Decrypt(string encryptedData, string iv, string sessionKey)
        {
            //创建解密器生成工具实例
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            //设置解密器参数
            aes.Mode = CipherMode.CBC;
            aes.BlockSize = 128;
            aes.Padding = PaddingMode.PKCS7;
            //格式化待处理字符串
            byte[] byte_encryptedData = Convert.FromBase64String(encryptedData);
            byte[] byte_iv = Convert.FromBase64String(iv);
            byte[] byte_sessionKey = Convert.FromBase64String(sessionKey);

            aes.IV = byte_iv;
            aes.Key = byte_sessionKey;
            //根据设置好的数据生成解密器实例
            ICryptoTransform transform = aes.CreateDecryptor();

            //解密
            byte[] final = transform.TransformFinalBlock(byte_encryptedData, 0, byte_encryptedData.Length);

            //生成结果
            string result = Encoding.UTF8.GetString(final);

            return result;

        }
        #endregion

        #endregion

        #region 方法
        /// <summary>
        /// 从加密信息中获取用户信息
        /// </summary>
        /// <param name="session_key"></param>
        /// <param name="rawData"></param>
        /// <param name="signature"></param>
        /// <param name="encryptedData"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public FrameDLRObject GetUserInfo(string session_key, string rawData,string signature,string encryptedData,string iv)
        {
            if (!VaildateUserInfo(rawData, signature, session_key))
            {
                return null;
            }
            var str = Decrypt(encryptedData, iv, session_key);
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, $"微信小程序校验解密后的串:{str}");
            return FrameDLRObject.IsJsonThen(str, null, FrameDLRFlags.SensitiveCase);
        }
        /// <summary>
        /// 登录凭证校验。通过 wx.login() 接口获得临时登录凭证 code 后传到开发者服务器调用此接口完成登录流程
        /// </summary>
        /// <param name="code">传入,Code</param>
        /// <returns></returns>
        public FrameDLRObject GetSessionByCode(string code)
        {
            /*  成功 rtn
                    {
                       openid	string	用户唯一标识
                        session_key	string	会话密钥
                        unionid	string	用户在开放平台的唯一标识符，在满足 UnionID 下发条件的情况下会返回，详见 UnionID 机制说明。
                        errcode	number	错误码
                        errmsg	string	错误信息
                    }
                失败{
                        errcode	number	错误码
                        errmsg	string	错误信息
                    }
             */
            FrameDLRObject obj = CallWeixinServer(string.Format(GetCode2SessionUrl, AppID, AppSecret, code),"get");
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "***********************GetSessionByCode result=" + obj.ToJSONString());
            return obj;
        }
        /// <summary>
        /// 用户支付完成后，获取该用户的 UnionId，无需用户授权
        /// </summary>
        /// <param name="openid">传入,支付用户唯一标识</param>
        /// <param name="transaction_id">微信支付订单号</param>
        /// <param name="mch_id">微信支付分配的商户号，和商户订单号配合使用</param>
        /// <param name="out_trade_no">微信支付商户订单号，和商户号配合使用</param>
        /// <returns></returns>
        public FrameDLRObject GetPaidUnionId(string openid, string transaction_id = "", string mch_id = "", string out_trade_no = "")
        {
            /*  成功 rtn
                    {
                       unionid	string	用户唯一标识，调用成功后返回
                        errcode	number	错误码
                        errmsg	string	错误信息
                    }
                失败{
                        errcode	number	错误码
                        errmsg	string	错误信息
                    }
             */
            var url = string.Format(GetUnionIDByPayUrl, AppSecret, openid);
            if (!string.IsNullOrEmpty(transaction_id))
            {
                url += $"&transaction_id={transaction_id}";
            }
            if (!string.IsNullOrEmpty(mch_id))
            {
                url += $"&mch_id={mch_id}";
            }
            if (!string.IsNullOrEmpty(out_trade_no))
            {
                url += $"&out_trade_no={out_trade_no}";
            }
            FrameDLRObject obj = CallWeixinServer(url, "get");
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "***********************GetPaidUnionId result=" + obj.ToJSONString());
            return obj;
        }
        /// <summary>
        /// 获取小程序新增或活跃用户的画像分布数据。时间范围支持昨天、最近7天、最近30天。其中，新增用户数为时间范围内首次访问小程序的去重用户数，活跃用户数为时间范围内访问过小程序的去重用户数
        /// </summary>
        /// <param name="begin">开始日期</param>
        /// <param name="end">结束日期，开始日期与结束日期相差的天数限定为0/6/29，分别表示查询最近1/7/30天数据，允许设置的最大值为昨日</param>
        /// <returns>结果参考https://developers.weixin.qq.com/miniprogram/dev/api-backend/analysis.getUserPortrait.html</returns>
        public FrameDLRObject GetUserPortrait(DateTime begin,DateTime end)
        {
            var url = string.Format(GetUserPortraitUrl, Access_Token);
            var postdata = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            postdata.begin_date = begin.ToString("yyyyMMdd");
            postdata.end_date = end.ToString("yyyyMMdd");
            FrameDLRObject obj = CallWeixinServer(url, "post","application/json",null, postdata);
            return obj;
        }
        /// <summary>
        /// 获取用户小程序访问分布数据
        /// </summary>
        /// <param name="begin">开始日期</param>
        /// <param name="end">结束日期，限定查询 1 天数据，允许设置的最大值为昨日。</param>
        /// <returns>结果参考https://developers.weixin.qq.com/miniprogram/dev/api-backend/analysis.getVisitDistribution.html</returns>
        public FrameDLRObject GetVisitDistribution(DateTime begin, DateTime end)
        {
            var url = string.Format(GetVisitDistributionUrl, Access_Token);
            var postdata = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            postdata.begin_date = begin.ToString("yyyyMMdd");
            postdata.end_date = end.ToString("yyyyMMdd");
            FrameDLRObject obj = CallWeixinServer(url, "post", "application/json", null, postdata);
            return obj;
        }
        /// <summary>
        /// 访问页面。目前只提供按 page_visit_pv 排序的 top200。
        /// </summary>
        /// <param name="begin">开始日期</param>
        /// <param name="end">结束日期，限定查询 1 天数据，允许设置的最大值为昨日。</param>
        /// <returns>结果参考https://developers.weixin.qq.com/miniprogram/dev/api-backend/analysis.getVisitPage.html</returns>
        public FrameDLRObject GetVisitPage(DateTime begin, DateTime end)
        {
            var url = string.Format(GetVisitPageUrl, Access_Token);
            var postdata = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            postdata.begin_date = begin.ToString("yyyyMMdd");
            postdata.end_date = end.ToString("yyyyMMdd");
            FrameDLRObject obj = CallWeixinServer(url, "post", "application/json", null, postdata);
            return obj;
        }
        /// <summary>
        /// 访问页面。目前只提供按 page_visit_pv 排序的 top200。
        /// </summary>
        /// <param name="begin">开始日期</param>
        /// <param name="end">结束日期，限定查询 1 天数据，允许设置的最大值为昨日。</param>
        /// <returns>结果参考https://developers.weixin.qq.com/miniprogram/dev/api-backend/analysis.getDailySummary.html</returns>
        public FrameDLRObject GetDailySummary(DateTime begin, DateTime end)
        {
            var url = string.Format(GetDailySummaryUrl, Access_Token);
            var postdata = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            postdata.begin_date = begin.ToString("yyyyMMdd");
            postdata.end_date = end.ToString("yyyyMMdd");
            FrameDLRObject obj = CallWeixinServer(url, "post", "application/json", null, postdata);
            return obj;
        }
        #endregion

        #region 客服消息
        /// <summary>
        /// 下发客服当前输入状态给用户
        /// </summary>
        /// <param name="openid">用户的 OpenID</param>
        /// <param name="command">命令 Typing	对用户下发"正在输入"状态;CancelTyping 取消对用户的"正在输入"状态</param>
        /// <returns></returns>
        public FrameDLRObject SetCustomerServiceType(string openid,string command= "Typing")
        {
            var url = string.Format(SetTypeUrl, Access_Token);
            var postdata = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            postdata.touser = openid;
            postdata.command = command;
            FrameDLRObject obj = CallWeixinServer(url, "post", "application/json", null, postdata);
            return obj;
        }
        /// <summary>
        /// 发送客服消息给用户-TXT
        /// </summary>
        /// <param name="touser">用户的 OpenID</param>
        /// <param name="content">文本消息内容</param>
        /// <returns></returns>
        public FrameDLRObject SendCustomerServiceMsg_Txt(string touser,string content)
        {
            var url = string.Format(SendCustomeServiceMsgUrl, Access_Token);
            var postdata = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            postdata.touser = touser;
            postdata.msgtype = "text";
            postdata.content = content;
            FrameDLRObject obj = CallWeixinServer(url, "post", "application/json", null, postdata);
            return obj;
        }
        /// <summary>
        /// 发送客服消息给用户-Link
        /// </summary>
        /// <param name="touser">用户的 OpenID</param>
        /// <param name="title">消息标题</param>
        /// <param name="description">图文链接消息</param>
        /// <param name="link_url">图文链接消息被点击后跳转的链接</param>
        /// <param name="thumb_url">图文链接消息的图片链接，支持 JPG、PNG 格式，较好的效果为大图 640 X 320，小图 80 X 80</param>
        /// <returns></returns>
        public FrameDLRObject SendCustomerServiceMsg_Link(string touser, string title,string description,string link_url,string thumb_url)
        {
            var url = string.Format(SendCustomeServiceMsgUrl, Access_Token);
            var postdata = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            postdata.touser = touser;
            postdata.msgtype = "link";
            postdata.link = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            postdata.link.title = title;
            postdata.link.description = description;
            postdata.link.url = url;
            postdata.link.title = thumb_url;
            FrameDLRObject obj = CallWeixinServer(url, "post", "application/json", null, postdata);
            return obj;
        }
        /// <summary>
        /// 发送客服消息给用户-MiniProgram
        /// </summary>
        /// <param name="touser">用户的 OpenID</param>
        /// <param name="title">消息标题</param>
        /// <param name="pagepath">小程序的页面路径，跟app.json对齐，支持参数，比如pages/index/index?foo=bar</param>
        /// <param name="thumb_media_id">小程序消息卡片的封面， image 类型的 media_id，通过 新增素材接口 上传图片文件获得，建议大小为 520*416</param>
        /// <returns></returns>
        public FrameDLRObject SendCustomerServiceMsg_MiniProgram(string touser,string title, string pagepath, string thumb_media_id)
        {
            var url = string.Format(SendCustomeServiceMsgUrl, Access_Token);
            var postdata = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            postdata.touser = touser;
            postdata.msgtype = "miniprogrampage";
            postdata.miniprogrampage = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            postdata.miniprogrampage.title = title;
            postdata.miniprogrampage.pagepath = pagepath;
            postdata.miniprogrampage.thumb_media_id = thumb_media_id;
            FrameDLRObject obj = CallWeixinServer(url, "post", "application/json", null, postdata);
            return obj;
        }
        /// <summary>
        /// 发送模板消息，相关参数和回传，查看https://developers.weixin.qq.com/miniprogram/dev/api-backend/templateMessage.send.html
        /// </summary>
        /// <param name="touser"></param>
        /// <param name="template_id"></param>
        /// <param name="form_id"></param>
        /// <param name="page"></param>
        /// <param name="emphasis_keyword"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public FrameDLRObject SendTemplateMsg(string touser, string template_id, string form_id, string page = "", string emphasis_keyword = "",params KeyValuePair<string,string>[] data)
        {
            var url = string.Format(SendCustomeServiceMsgUrl, Access_Token);
            var postdata = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            postdata.touser = touser;
            postdata.template_id = template_id;
            if (!string.IsNullOrEmpty(page))
                postdata.page = page;
            postdata.form_id = form_id;
            if (data != null)
            {
                FrameDLRObject d = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                foreach (var item in data)
                {
                    d.SetValue(item.Key, new
                    {
                        value = item.Value
                    });
                }
                postdata.data = d;
            }
                
            if (!string.IsNullOrEmpty(emphasis_keyword))
                postdata.emphasis_keyword = emphasis_keyword;
            FrameDLRObject obj = CallWeixinServer(url, "post", "application/json", null, postdata);
            return obj;
        }
        /// <summary>
        /// 获取小程序二维码，适用于需要的码数量较少的业务场景。通过该接口生成的小程序码，永久有效，有数量限制
        /// 参看https://developers.weixin.qq.com/miniprogram/dev/api-backend/wxacode.createQRCode.html
        /// </summary>
        /// <param name="path">扫码进入的小程序页面路径，最大长度 128 字节，不能为空；对于小游戏，可以只传入 query 部分，来实现传参效果，如：传入 "?foo=bar"，即可在 wx.getLaunchOptionsSync 接口中的 query 参数获取到 {foo:"bar"}</param>
        /// <param name="width">二维码的宽度，单位 px。最小 280px，最大 1280px</param>
        /// <returns></returns>
        public FrameDLRObject CreateQRCode(string path, double width = 430)
        {
            var url = string.Format(CreateQRCodeUrl, Access_Token);
            var postdata = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            postdata.path = path;
            postdata.width = width;
            FrameDLRObject obj = CallWeixinServer(url, "post", "application/json", null, postdata);
            return obj;
        }
        /// <summary>
        /// 获取小程序码，适用于需要的码数量较少的业务场景。通过该接口生成的小程序码，永久有效，有数量限制
        /// 参看https://developers.weixin.qq.com/miniprogram/dev/api-backend/wxacode.get.html
        /// </summary>
        /// <param name="path">扫码进入的小程序页面路径，最大长度 128 字节，不能为空；对于小游戏，可以只传入 query 部分，来实现传参效果，如：传入 "?foo=bar"，即可在 wx.getLaunchOptionsSync 接口中的 query 参数获取到 {foo:"bar"}</param>
        /// <param name="width">二维码的宽度，单位 px。最小 280px，最大 1280px</param>
        /// <param name="auto_color">自动配置线条颜色，如果颜色依然是黑色，则说明不建议配置主色调</param>
        /// <param name="r">auto_color 为 false 时生效，使用 rgb 设置颜色中的r 十进制表示</param>
        /// <param name="g">auto_color 为 false 时生效，使用 rgb 设置颜色中的g 十进制表示</param>
        /// <param name="b">auto_color 为 false 时生效，使用 rgb 设置颜色中的b 十进制表示</param>
        /// <param name="is_hyaline">是否需要透明底色，为 true 时，生成透明底色的小程序码</param>
        /// <returns></returns>
        public FrameDLRObject GetQRCode(string path, double width = 430, bool auto_color = false, string r = "0", string g = "0", string b = "0", bool is_hyaline = false)
        {
            var url = string.Format(GetQRCodeUrl, Access_Token);
            var postdata = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            postdata.path = path;
            postdata.width = width;
            postdata.auto_color = auto_color;
            postdata.line_color = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            postdata.line_color.r = r;
            postdata.line_color.g = g;
            postdata.line_color.b = b;
            postdata.is_hyaline = is_hyaline;
            FrameDLRObject obj = CallWeixinServer(url, "post", "application/json", null, postdata);
            return obj;
        }
        /// <summary>
        /// 获取小程序码，适用于需要的码数量极多的业务场景。通过该接口生成的小程序码，永久有效，数量暂无限制
        /// 参看https://developers.weixin.qq.com/miniprogram/dev/api-backend/wxacode.getUnlimited.html
        /// </summary>
        /// <param name="scene">最大32个可见字符，只支持数字，大小写英文以及部分特殊字符：!#$&'()*+,/:;=?@-._~，其它字符请自行编码为合法字符（因不支持%，中文无法使用 urlencode 处理，请使用其他编码方式）</param>
        /// <param name="path">扫码进入的小程序页面路径，最大长度 128 字节，不能为空；对于小游戏，可以只传入 query 部分，来实现传参效果，如：传入 "?foo=bar"，即可在 wx.getLaunchOptionsSync 接口中的 query 参数获取到 {foo:"bar"}</param>
        /// <param name="width">二维码的宽度，单位 px。最小 280px，最大 1280px</param>
        /// <param name="auto_color">自动配置线条颜色，如果颜色依然是黑色，则说明不建议配置主色调</param>
        /// <param name="r">auto_color 为 false 时生效，使用 rgb 设置颜色中的r 十进制表示</param>
        /// <param name="g">auto_color 为 false 时生效，使用 rgb 设置颜色中的g 十进制表示</param>
        /// <param name="b">auto_color 为 false 时生效，使用 rgb 设置颜色中的b 十进制表示</param>
        /// <param name="is_hyaline">是否需要透明底色，为 true 时，生成透明底色的小程序码</param>
        /// <returns></returns>
        public FrameDLRObject GetUnlimitedQRCode(string scene,string path, double width = 430, bool auto_color = false, string r = "0", string g = "0", string b = "0", bool is_hyaline = false)
        {
            var url = string.Format(GetUnlimitedQRCodeUrl, Access_Token);
            var postdata = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            postdata.scene = scene;
            postdata.path = path;
            postdata.width = width;
            postdata.auto_color = auto_color;
            postdata.line_color = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            postdata.line_color.r = r;
            postdata.line_color.g = g;
            postdata.line_color.b = b;
            postdata.is_hyaline = is_hyaline;
            FrameDLRObject obj = CallWeixinServer(url, "post", "application/json", null, postdata);
            return obj;
        }
        #endregion
    }
}
