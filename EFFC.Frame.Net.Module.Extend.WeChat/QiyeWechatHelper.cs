using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Global;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Linq;

namespace EFFC.Frame.Net.Module.Extend.WeChat
{
    /// <summary>
    /// 企业微信号开发API
    /// </summary>
    public class QiyeWechatHelper
    {
        /// <summary>
        /// 获取一个企业微信Helper的实例
        /// </summary>
        /// <param name="appid">微信的AppID，微信平台申请的账号</param>
        /// <param name="app_secret">微信的App密钥，微信平台申请的时候生成的</param>
        /// <param name="token">微信的访问Token，微信平台申请的时候生成的</param>
        /// <param name="agentid">企业应用ID号</param>
        /// <param name="encodingAESKey">微信加密密钥</param>
        /// <param name="mch_ssl_path">微信商户的数字证书存放的物理路径，默认为空，为空的时候则无法使用微信商户的功能</param>
        /// <param name="mch_ssl_pass">微信商户数字证书的密码，默认为空，为空的时候则无法使用微信商户的功能</param>
        public QiyeWechatHelper(string appid, string app_secret, string token, string encodingAESKey,string agentid, string mch_ssl_path = "", string mch_ssl_pass = "")
        {
            AppID = appid;
            AppSecret = app_secret;
            Weixin_Mch_Ssl_Path = mch_ssl_pass;
            Weixin_Mch_Ssl_Pass = mch_ssl_pass;
            Token = token;
            AgentId = agentid;
            EncodingAESKey = encodingAESKey;
            if (!GlobalCommon.Proxys.HasProxy("weixinserver")) GlobalCommon.Proxys.UseProxy<WeixinHttpCallProxy>("weixinserver");
        }
        #region API地址
        /// <summary>
        /// 获取AccessToken的URL
        /// </summary>
        protected const string GetTokenUrl = @"https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={0}&corpsecret={1}";
        /// <summary>
        /// 通过AccessToken获得JSDKTicket的URL
        /// </summary>
        protected const string GetJsApiTicketUrl = @"https://qyapi.weixin.qq.com/cgi-bin/get_jsapi_ticket?access_token={0}";
        /// <summary>
        /// 通过AccessToken和MediaId下载媒体的URL
        /// </summary>
        protected const string DownloadMediaUrl = @"https://qyapi.weixin.qq.com/cgi-bin/media/get?access_token={0}&media_id={1}";
        /// <summary>
        /// 通过AccessToekn和Code获得登入用户信息和Ticket的URL
        /// </summary>
        protected const string GetUserTicketUrl = @"https://qyapi.weixin.qq.com/cgi-bin/user/getuserinfo?access_token={0}&code={1}";
        /// <summary>
        /// 通过AccessToken和UserId得到用户详细信息
        /// </summary>
        protected const string GetUserDetailUrl = @"https://qyapi.weixin.qq.com/cgi-bin/user/get?access_token={0}&userid={1}";
        /// <summary>
        /// 通过AccessToken发送应用消息
        /// </summary>
        protected const string SendMsgUrl = @"https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token={0}";
        /// <summary>
        /// 获取部门下用户列表
        /// </summary>
        protected const string GetUsersByDeptIDUrl = @"https://qyapi.weixin.qq.com/cgi-bin/user/simplelist?access_token={0}&department_id={1}&fetch_child={2}";
        /// <summary>
        /// 获取部门下用户明细列表
        /// </summary>
        protected const string GetUsersDetailByDeptIDUrl = @"https://qyapi.weixin.qq.com/cgi-bin/user/list?access_token={0}&department_id={1}&fetch_child={2}";
        /// <summary>
        /// 获取部门列表
        /// </summary>
        protected const string GetDeptListUrl = @"https://qyapi.weixin.qq.com/cgi-bin/department/list?access_token={0}";
        #endregion

        #region 常用的属性

        #region 设定和验证类
        /// <summary>
        /// 企业微信的CropID, 这里和微信通用AppId
        /// </summary>
        public string AppID { get; protected set; }

        /// <summary>
        /// 企业微信当前应用的AgentId,(微信无此概念)
        /// </summary>
        public string AgentId { get; protected set; }

        /// <summary>
        /// 企业微信,当前应用的Secret
        /// </summary>
        public string AppSecret { get; protected set; }
        /// <summary>
        /// 企业号 加密用的token
        /// </summary>
        public string Token { get; protected set; }
        /// <summary>
        /// 企业微信, 加密用的EncodingAESKey
        /// </summary>
        public string EncodingAESKey { get; protected set; }
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
        /// 微信调用JSAPI需要用到的JSAPI_Ticket,用于做AES的数字签名
        /// </summary>
        public string Jsapi_ticket => GenJsapi_ticket(Access_Token);
        /// <summary>
        /// 当前新的微信时间戳
        /// </summary>
        public int NewTimsStamp => (int)(DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds;

        #endregion

        #endregion

        #region 方法
        /// <summary>
        /// 下载临时媒体文件
        /// </summary>
        /// <param name="mediaid"></param>
        /// <returns></returns>
        public dynamic DownloadMedia(string mediaid)
        {
            var rtn = FrameDLRObject.CreateInstance(false, "");
            var url = string.Format(DownloadMediaUrl, Access_Token, mediaid);
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
        /// 生成一个Text类型的回复数据
        /// </summary>
        /// <param name="content"></param>
        /// <param name="touser">接收者的openid</param>
        /// <param name="fromuser">发送微信号的id</param>
        /// <returns></returns>
        public FrameDLRObject GenResponseText(string content,string touser,string fromuser)
        {
            var rtn = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
            rtn.ToUserName = fromuser;
            rtn.FromUserName = touser;
            rtn.CreateTime = DateTime.Now;
            rtn.MsgType = "text";
            rtn.Content = content;
            return rtn;
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
        /// 登入时通过Code获取到登入人的UserID(一般来说这个就够了)
        /// </summary>
        /// <param name="code">传入, code</param>
        /// <param name="userTicket">传出, ticketId可以让你在7200s之内获取更加详细的用户信息</param>
        /// <param name="deviceId">传出,登入用设备ID</param>
        /// <returns>userId</returns>
        public string GetUserIdByCode(string code, ref string userTicket, ref string deviceId)
        {
            string openId = "";
            return GetUserIdByCode(code, ref userTicket, ref deviceId, ref openId);
        }
        /// <summary>
        /// 登入时通过Code获取到登入人的UserID
        /// </summary>
        /// <param name="code">传入,Code</param>
        /// <param name="userTicket">传出,ticketId可以让你在7200s之内获取更加详细的用户信息</param>
        /// <param name="deviceId">传出,登入用设备ID</param>
        /// <param name="openId">非企业成员OpenID</param>
        /// <returns></returns>
        public string GetUserIdByCode(string code, ref string userTicket, ref string deviceId, ref string openId)
        {
            /*  企业成员rtn
                    {
                       "errcode": 0,
                       "errmsg": "ok",
                       "UserId":"USERID",
                       "DeviceId":"DEVICEID",
                       "user_ticket": "USER_TICKET"，
                       "expires_in":7200
                    }
             */
            /*非企业成员Rtn
                {
                  "errcode": 0,
                  "errmsg": "ok",
                  "OpenId":"OPENID",
                  "DeviceId":"DEVICEID"
                }
                */
            FrameDLRObject obj = CallWeixinServer(string.Format(GetUserTicketUrl, Access_Token, code));
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "***********************" + obj.ToJSONString());
            dynamic dobj = obj;
            string userId = ComFunc.nvl(dobj.userid);
            deviceId = ComFunc.nvl(dobj.DeviceId);
            userTicket = ComFunc.nvl(dobj.user_ticket);
            openId = ComFunc.nvl(dobj.OpenId);
            return userId;
        }
        /// <summary>
        /// 通过userId 得到用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public FrameDLRObject GetUserDetail(string userId)
        {
            return CallWeixinServer(string.Format(GetUserDetailUrl, Access_Token, userId));
        }
        /// <summary>
        /// 获取部门下成员的列表
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="is_fetch_child"></param>
        /// <returns></returns>
        public List<object> GetUsersByDeptID(string deptid,bool is_fetch_child = false)
        {
            var rtn = new List<object>();
            dynamic obj = CallWeixinServer(string.Format(GetUsersByDeptIDUrl, Access_Token, deptid, is_fetch_child ? 1 : 0), "GET");
            if(obj!= null)
            {
                if(ComFunc.nvl(obj.errcode) == "0")
                {
                    if(obj.userlist is object[])
                    {
                        rtn = ((object[])obj.userlist).ToList();
                    }
                    else
                    {
                        rtn = obj.userlist;
                    }
                }
            }

            return rtn;
        }
        /// <summary>
        /// 根据部门ID获取用户明细信息列表
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="is_fetch_child"></param>
        /// <returns></returns>
        public List<object> GetUsersDetailByDeptID(string deptid, bool is_fetch_child = false)
        {
            var rtn = new List<object>();
            dynamic obj = CallWeixinServer(string.Format(GetUsersDetailByDeptIDUrl, Access_Token, deptid, is_fetch_child ? 1 : 0), "GET");
            if (obj != null)
            {
                if (ComFunc.nvl(obj.errcode) == "0")
                {
                    if (obj.userlist is object[])
                    {
                        rtn = ((object[])obj.userlist).ToList();
                    }
                    else
                    {
                        rtn = obj.userlist;
                    }
                }
            }

            return rtn;
        }
        /// <summary>
        /// 获取部门列表
        /// </summary>
        /// <param name="deptid">部门id。获取指定部门及其下的子部门。 如果不填，默认获取全量组织架构</param>
        /// <returns></returns>
        public List<object> GetDeptList(string deptid="")
        {
            var rtn = new List<object>();
            var url = string.Format(GetDeptListUrl, Access_Token);
            if (deptid != "")
            {
                url += "&id=" + deptid;
            }
            dynamic obj = CallWeixinServer(url, "GET");
            if (obj != null)
            {
                if (ComFunc.nvl(obj.errcode) == "0")
                {
                    if (obj.department is object[])
                    {
                        rtn = ((object[])obj.department).ToList();
                    }
                    else
                    {
                        rtn = obj.department;
                    }
                }
            }

            return rtn;
        }
        /// <summary>
        /// 发送文本消息
        /// </summary>
        /// <param name="toUsers">成员ID列表（消息接收者，多个接收者用‘|’或','分隔，最多支持1000个）。特殊情况：指定为@all，则向该企业应用的全部成员发送</param>
        /// <param name="toPartyIds">部门ID列表，多个接收者用‘|’或','分隔，最多支持100个。当touser为@all时忽略本参数</param>
        /// <param name="totag">标签ID列表，多个接收者用‘|’或','分隔，最多支持100个。当touser为@all时忽略本参数</param>
        /// <param name="content">消息内容，最长不超过2048个字节</param>
        /// <param name="isSafe">表示是否是保密消息，默认false</param>
        /// <returns></returns>
        public FrameDLRObject SendTxtMsg(string toUsers,string toPartyIds,string totag,string content,bool isSafe=false)
        {
            FrameDLRObject obj = CallWeixinServer(string.Format(SendMsgUrl, Access_Token), "POST", "text/json", null, FrameDLRObject.CreateInstance(new
            {
                touser = toUsers.Replace(",", "|"),
                toparty = toPartyIds.Replace(",", "|"),
                totag = totag.Replace(",", ""),
                msgtype = "text",
                agentid = AgentId,
                text = new
                {
                    content
                },
                safe = isSafe ? 1 : 0
            }, FrameDLRFlags.SensitiveCase));

            return obj;

        }
        /// <summary>
        /// 图片消息
        /// </summary>
        /// <param name="toUsers">成员ID列表（消息接收者，多个接收者用‘|’或','分隔，最多支持1000个）。特殊情况：指定为@all，则向该企业应用的全部成员发送</param>
        /// <param name="toPartyIds">部门ID列表，多个接收者用‘|’或','分隔，最多支持100个。当touser为@all时忽略本参数</param>
        /// <param name="totag">标签ID列表，多个接收者用‘|’或','分隔，最多支持100个。当touser为@all时忽略本参数</param>
        /// <param name="media_id">文件id，可以调用上传临时素材接口获取</param>
        /// <param name="isSafe">表示是否是保密消息，默认false</param>
        /// <returns></returns>
        public FrameDLRObject SendImageMsg(string toUsers, string toPartyIds, string totag, string media_id, bool isSafe = false)
        {
            FrameDLRObject obj = CallWeixinServer(string.Format(SendMsgUrl, Access_Token), "POST", "text/json", null, FrameDLRObject.CreateInstance(new
            {
                touser = toUsers.Replace(",", "|"),
                toparty = toPartyIds.Replace(",", "|"),
                totag = totag.Replace(",", ""),
                msgtype = "image",
                agentid = AgentId,
                image = new
                {
                    media_id
                },
                safe = isSafe ? 1 : 0
            }, FrameDLRFlags.SensitiveCase));

            return obj;

        }
        /// <summary>
        /// 语音消息
        /// </summary>
        /// <param name="toUsers">成员ID列表（消息接收者，多个接收者用‘|’或','分隔，最多支持1000个）。特殊情况：指定为@all，则向该企业应用的全部成员发送</param>
        /// <param name="toPartyIds">部门ID列表，多个接收者用‘|’或','分隔，最多支持100个。当touser为@all时忽略本参数</param>
        /// <param name="totag">标签ID列表，多个接收者用‘|’或','分隔，最多支持100个。当touser为@all时忽略本参数</param>
        /// <param name="media_id">文件id，可以调用上传临时素材接口获取</param>
        /// <param name="isSafe">表示是否是保密消息，默认false</param>
        /// <returns></returns>
        public FrameDLRObject SendVoiceMsg(string toUsers, string toPartyIds, string totag, string media_id, bool isSafe = false)
        {
            FrameDLRObject obj = CallWeixinServer(string.Format(SendMsgUrl, Access_Token), "POST", "text/json", null, FrameDLRObject.CreateInstance(new
            {
                touser = toUsers.Replace(",", "|"),
                toparty = toPartyIds.Replace(",", "|"),
                totag = totag.Replace(",", ""),
                msgtype = "voice",
                agentid = AgentId,
                voice = new
                {
                    media_id
                },
                safe = isSafe ? 1 : 0
            }, FrameDLRFlags.SensitiveCase));

            return obj;

        }
        /// <summary>
        /// 视频消息
        /// </summary>
        /// <param name="toUsers">成员ID列表（消息接收者，多个接收者用‘|’或','分隔，最多支持1000个）。特殊情况：指定为@all，则向该企业应用的全部成员发送</param>
        /// <param name="toPartyIds">部门ID列表，多个接收者用‘|’或','分隔，最多支持100个。当touser为@all时忽略本参数</param>
        /// <param name="totag">标签ID列表，多个接收者用‘|’或','分隔，最多支持100个。当touser为@all时忽略本参数</param>
        /// <param name="media_id">文件id，可以调用上传临时素材接口获取</param>
        /// <param name="title">视频消息的标题，不超过128个字节，超过会自动截断</param>
        /// <param name="description">视频消息的描述，不超过512个字节，超过会自动截断</param>
        /// <param name="isSafe">表示是否是保密消息，默认false</param>
        /// <returns></returns>
        public FrameDLRObject SendVideoMsg(string toUsers, string toPartyIds, string totag, string media_id,string title="",string description="", bool isSafe = false)
        {
            FrameDLRObject obj = CallWeixinServer(string.Format(SendMsgUrl, Access_Token), "POST", "text/json", null, FrameDLRObject.CreateInstance(new
            {
                touser = toUsers.Replace(",", "|"),
                toparty = toPartyIds.Replace(",", "|"),
                totag = totag.Replace(",", ""),
                msgtype = "video",
                agentid = AgentId,
                video = new
                {
                    media_id,
                    title,
                    description
                },
                safe = isSafe ? 1 : 0
            }, FrameDLRFlags.SensitiveCase));

            return obj;

        }
        /// <summary>
        /// 文件消息
        /// </summary>
        /// <param name="toUsers">成员ID列表（消息接收者，多个接收者用‘|’或','分隔，最多支持1000个）。特殊情况：指定为@all，则向该企业应用的全部成员发送</param>
        /// <param name="toPartyIds">部门ID列表，多个接收者用‘|’或','分隔，最多支持100个。当touser为@all时忽略本参数</param>
        /// <param name="totag">标签ID列表，多个接收者用‘|’或','分隔，最多支持100个。当touser为@all时忽略本参数</param>
        /// <param name="media_id">文件id，可以调用上传临时素材接口获取</param>
        /// <param name="isSafe">表示是否是保密消息，默认false</param>
        /// <returns></returns>
        public FrameDLRObject SendFileMsg(string toUsers, string toPartyIds, string totag, string media_id, bool isSafe = false)
        {
            FrameDLRObject obj = CallWeixinServer(string.Format(SendMsgUrl, Access_Token), "POST", "text/json", null, FrameDLRObject.CreateInstance(new
            {
                touser = toUsers.Replace(",", "|"),
                toparty = toPartyIds.Replace(",", "|"),
                totag = totag.Replace(",", ""),
                msgtype = "file",
                agentid = AgentId,
                file = new
                {
                    media_id
                },
                safe = isSafe ? 1 : 0
            }, FrameDLRFlags.SensitiveCase));

            return obj;

        }
        /// <summary>
        /// 文本卡片消息，卡片消息的展现形式非常灵活，支持使用br标签或者空格来进行换行处理，也支持使用div标签来使用不同的字体颜色，目前内置了3种文字颜色：灰色(gray)、高亮(highlight)、默认黑色(normal)，将其作为div标签的class属性即可
        /// </summary>
        /// <param name="toUsers">成员ID列表（消息接收者，多个接收者用‘|’或','分隔，最多支持1000个）。特殊情况：指定为@all，则向该企业应用的全部成员发送</param>
        /// <param name="toPartyIds">部门ID列表，多个接收者用‘|’或','分隔，最多支持100个。当touser为@all时忽略本参数</param>
        /// <param name="totag">标签ID列表，多个接收者用‘|’或','分隔，最多支持100个。当touser为@all时忽略本参数</param>
        /// <param name="title">标题，不超过128个字节，超过会自动截断</param>
        /// <param name="description">描述，不超过512个字节，超过会自动截断,可以内嵌html标签</param>
        /// <param name="url">点击后跳转的链接。</param>
        /// <param name="btntxt">按钮文字。 默认为“详情”， 不超过4个文字，超过自动截断。</param>
        /// <returns></returns>
        public FrameDLRObject SendTextCardMsg(string toUsers, string toPartyIds, string totag, string title,string description,string url,string btntxt="详情")
        {
            FrameDLRObject obj = CallWeixinServer(string.Format(SendMsgUrl, Access_Token), "POST", "text/json", null, FrameDLRObject.CreateInstance(new
            {
                touser = toUsers.Replace(",", "|"),
                toparty = toPartyIds.Replace(",", "|"),
                totag = totag.Replace(",", "|"),
                msgtype = "textcard",
                agentid = AgentId,
                textcard = new
                {
                    title,
                    description,
                    url,
                    btntxt
                }
            }, FrameDLRFlags.SensitiveCase));

            return obj;

        }
        /// <summary>
        /// 发送图文消息
        /// </summary>
        /// <param name="toUsers">成员ID列表（消息接收者，多个接收者用‘|’或','分隔，最多支持1000个）。特殊情况：指定为@all，则向该企业应用的全部成员发送</param>
        /// <param name="toPartyIds">部门ID列表，多个接收者用‘|’或','分隔，最多支持100个。当touser为@all时忽略本参数</param>
        /// <param name="totag">标签ID列表，多个接收者用‘|’或','分隔，最多支持100个。当touser为@all时忽略本参数</param>
        /// <param name="articles">格式如下，{
        ///      "title" : "中秋节礼品领取",
        ///       "description" : "今年中秋节公司有豪礼相送",
        ///        "url" : "URL",
        ///      "picurl" : "http://res.mail.qq.com/node/ww/wwopenmng/images/independent/doc/test_pic_msg1.png",
        ///       "btntxt":"更多"
        ///   }</param>
        /// <returns></returns>
        public FrameDLRObject SendNewsMsg(string toUsers, string toPartyIds, string totag, List<object> articles)
        {
            FrameDLRObject obj = CallWeixinServer(string.Format(SendMsgUrl, Access_Token), "POST", "text/json", null, FrameDLRObject.CreateInstance(new
            {
                touser = toUsers.Replace(",", "|"),
                toparty = toPartyIds.Replace(",", "|"),
                totag = totag.Replace(",", "|"),
                msgtype = "news",
                agentid = AgentId,
                news = new
                {
                    articles
                }
            }, FrameDLRFlags.SensitiveCase));

            return obj;

        }
        /// <summary>
        /// 发送图文消息，mpnews类型的图文消息，跟普通的图文消息一致，唯一的差异是图文内容存储在企业微信。多次发送mpnews，会被认为是不同的图文，阅读、点赞的统计会被分开计算。
        /// </summary>
        /// <param name="toUsers">成员ID列表（消息接收者，多个接收者用‘|’或','分隔，最多支持1000个）。特殊情况：指定为@all，则向该企业应用的全部成员发送</param>
        /// <param name="toPartyIds">部门ID列表，多个接收者用‘|’或','分隔，最多支持100个。当touser为@all时忽略本参数</param>
        /// <param name="totag">标签ID列表，多个接收者用‘|’或','分隔，最多支持100个。当touser为@all时忽略本参数</param>
        /// <param name="articles">{
        ///      "title": "Title", 
        ///      "thumb_media_id": "MEDIA_ID",
        ///       "author": "Author",
        ///       "content_source_url": "URL",
        ///       "content": "Content",
        ///       "digest": "Digest description"
        /// }</param>
        /// <param name="isSafe">表示是否是保密消息，默认false</param>
        /// <returns></returns>
        public FrameDLRObject SendMPNewsMsg(string toUsers, string toPartyIds, string totag, List<object> articles, bool isSafe = false)
        {
            FrameDLRObject obj = CallWeixinServer(string.Format(SendMsgUrl, Access_Token), "POST", "text/json", null, FrameDLRObject.CreateInstance(new
            {
                touser = toUsers.Replace(",", "|"),
                toparty = toPartyIds.Replace(",", "|"),
                totag = totag.Replace(",", "|"),
                msgtype = "mpnews",
                agentid = AgentId,
                mpnews = new
                {
                    articles
                },
                safe = isSafe ? 1 : 0
            }, FrameDLRFlags.SensitiveCase));

            return obj;

        }
        /// <summary>
        /// 小程序通知消息
        /// </summary>
        /// <param name="toUser">成员ID，可以是userid或者加密后的userid</param>
        /// <param name="appid">小程序appid，必须是与当前小程序应用关联的小程序</param>
        /// <param name="content_items">消息内容键值对，最多允许10个item，格式如下
        /// {
        ///       "key": "会议室",
        ///        "value": "402"
        /// }
        /// </param>
        /// <param name="page"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="emphasis_first_item"></param>
        /// <returns></returns>
        public FrameDLRObject SendMiniMsg(string toUser,string appid,List<object> content_items, string page="", string title="", string description="", bool emphasis_first_item=false)
        {
            FrameDLRObject obj = CallWeixinServer(string.Format(SendMsgUrl, Access_Token), "POST", "text/json", null, FrameDLRObject.CreateInstance(new
            {
                touser = toUser,
                msgtype = "miniprogram_notice",
                miniprogram_notice = new
                {
                    appid,
                    page,
                    title,
                    description,
                    emphasis_first_item,
                    content_item = content_items
                }
            }, FrameDLRFlags.SensitiveCase));

            return obj;

        }
        #endregion

        #region 底层呼叫方法
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
                dynamic dobj = CallWeixinServer(string.Format(GetTokenUrl, appid, appsecret));
                var token = ComFunc.nvl(dobj.access_token);
                var expireseconds = ComFunc.nvl(dobj.expires_in);
                GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, "corpid: " + appid + ";  新Token:" + token + ";  expireseconds:" + expireseconds);
                //获取之后将超时时间缩短10秒，微信默认超时时间为7200秒，每获取一次就会重置该token
                if (token != "")
                    GlobalCommon.ApplicationCache.Set(appid + "_weixin_access_token", token, DateTime.Now.AddSeconds(IntStd.ParseStd(expireseconds).Value - 10));
                GlobalCommon.Logger.WriteLog(LoggerLevel.INFO, "***** GoLogic.Weixin.cs GenAccess_Token End *********************************************************************************************");

            }

            return ComFunc.nvl(GlobalCommon.ApplicationCache.Get(appid + "_weixin_access_token"));
        }
        /// <summary>
        /// 生成JSDK签名所需的ticket(有效期7200)
        /// </summary>
        /// <param name="accesstoken"></param>
        /// <returns></returns>
        public string GenJsapi_ticket(string accesstoken)
        {
            if (GlobalCommon.ApplicationCache.Get(accesstoken + "_weixin_jsapi_ticket") == null)
            {
                var result = CallWeixinServer(string.Format(GetJsApiTicketUrl, Access_Token));
                dynamic dobj = (FrameDLRObject)result;
                var ticket = ComFunc.nvl(dobj.ticket);
                var expireseconds = ComFunc.nvl(dobj.expires_in);
                //获取之后将超时时间缩短10秒，微信默认超时时间为7200秒，每获取一次就会重置该token
                if (ticket != "")
                    GlobalCommon.ApplicationCache.Set(accesstoken + "_weixin_jsapi_ticket", ticket, DateTime.Now.AddSeconds(IntStd.ParseStd(expireseconds).Value - 10));
            }
            return ComFunc.nvl(GlobalCommon.ApplicationCache.Get(accesstoken + "_weixin_jsapi_ticket"));
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
        /// 获取SHA算法的Sign签名,ASCII编码
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string GenSHASignKey(FrameDLRObject obj)
        {
            return WXBizMsgCrypt.GenSHASignKey(obj, Encoding.ASCII);
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
        #endregion
    }
}
