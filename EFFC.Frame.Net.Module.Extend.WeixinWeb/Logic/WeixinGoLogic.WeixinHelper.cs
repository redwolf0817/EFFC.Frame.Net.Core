using System;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Module.Extend.WeChat;
using System.Collections.Generic;
using System.Linq;

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
        public class WeixinHelper:WechatHelper
        {
            WeixinGoLogic _logic;

            public WeixinHelper(WeixinGoLogic logic):base("","","")
            {
                _logic = logic;
                AppID = ComFunc.nvl(_logic.CallContext_Parameter.WebParam.ExtentionObj.weixin.appid);
                AppSecret = ComFunc.nvl(_logic.CallContext_Parameter.WebParam.ExtentionObj.weixin.appsecret);
                Token = ComFunc.nvl(_logic.CallContext_Parameter.WebParam.ExtentionObj.weixin.token);
                Mch_ID = ComFunc.nvl(_logic.CallContext_Parameter.WebParam.ExtentionObj.weixin.mch_id);
                Mch_Key = ComFunc.nvl(_logic.CallContext_Parameter.WebParam.ExtentionObj.weixin.mch_key);
                Weixin_Mch_Ssl_Path = ComFunc.nvl(_logic.CallContext_Parameter.WebParam.ExtentionObj.weixin.weixin_mch_ssl_path);
                Weixin_Mch_Ssl_Pass = ComFunc.nvl(_logic.CallContext_Parameter.WebParam.ExtentionObj.weixin.weixin_mch_ssl_pass);
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
            /// 请求的Msg
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
            /// 扫描信息
            /// </summary>
            public string ScanCodeInfo
            {
                get
                {
                    //返回的是POSTDATA里面有"ScanCodeInfo"的数据
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
                    //返回的是POSTDATA里面有"ScanType"的数据
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
                    //返回的是POSTDATA里面有"ScanResult"的数据
                    return ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, "ScanResult"]);
                }
            }
            /// <summary>
            /// 从微信服务器接收到的推送消息内容(xml格式)，已转为对象
            /// </summary>
            public FrameDLRObject RecieveMsgObject
            {
                get
                {

                    return _logic.CallContext_Parameter.WebParam.ExtentionObj.weixin.RecieveXMLObject == null ? FrameDLRObject.CreateInstance() : (FrameDLRObject)_logic.CallContext_Parameter.WebParam.ExtentionObj.weixin.RecieveXMLObject;
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
            /// 根据key从微信服务器传来的请求message中获取对应的值
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public string GetMsgByKey(string key)
            {
                return ComFunc.nvl(_logic.CallContext_Parameter.WebParam[DomainKey.POST_DATA, key]);
            }

            /// <summary>
            /// 被动消息回复，生成一个Text类型的回复数据
            /// </summary>
            /// <param name="content"></param>
            /// <returns></returns>
            public FrameDLRObject GenResponseText(string content)
            {
                return GenResponseText(content, ToUserName, FromUserName);
            }
            /// <summary>
            /// 被动消息回复，生成一个image类型的回复数据
            /// </summary>
            /// <param name="mediaid">通过素材管理接口上传多媒体文件，得到的id。</param>
            /// <returns></returns>
            public FrameDLRObject GenResponseImage(string mediaid)
            {
                return GenResponseImage(mediaid,FromUserName,FromUserName);
            }
            /// <summary>
            /// 被动消息回复，生成一个image类型的回复数据
            /// </summary>
            /// <param name="mediaid">通过素材管理接口上传多媒体文件，得到的id。</param>
            /// <returns></returns>
            public FrameDLRObject GenResponseVoice(string mediaid)
            {
                return GenResponseVoice(mediaid, ToUserName, FromUserName);
            }
            /// <summary>
            /// 被动消息回复，生成一个video类型的回复数据
            /// </summary>
            /// <param name="mediaid">通过素材管理接口上传多媒体文件，得到的id。</param>
            /// <param name="title">视频消息的标题</param>
            /// <param name="description">视频消息的描述</param>
            /// <returns></returns>
            public FrameDLRObject GenResponseVideo(string mediaid, string title, string description)
            {
                return GenResponseVideo(mediaid, title, description, ToUserName, FromUserName);
            }
            /// <summary>
            /// 被动消息回复，生成一个音乐消息类型的回复数据
            /// </summary>
            /// <param name="mediaid">缩略图的媒体id，通过素材管理接口上传多媒体文件，得到的id</param>
            /// <param name="title">音乐标题</param>
            /// <param name="description">音乐描述</param>
            /// <param name="MusicUrl">音乐链接</param>
            /// <param name="HQMusicUrl">高质量音乐链接，WIFI环境优先使用该链接播放音乐</param>
            /// <returns></returns>
            public FrameDLRObject GenResponseMusic(string mediaid, string title, string description, string MusicUrl, string HQMusicUrl)
            {
                return GenResponseMusic(mediaid,title,description,MusicUrl,HQMusicUrl,ToUserName,FromUserName);
            }
            /// <summary>
            /// 被动消息回复，生成一个图文消息类型的回复数据
            /// </summary>
            /// <param name="items">数组，动态对象为敏感大小写
            /// 构成如下：
            /// Title：图文消息标题
            /// Description：图文消息描述
            /// PicUrl：图片链接，支持JPG、PNG格式，较好的效果为大图360*200，小图200*200
            /// Url：点击图文消息跳转链接
            /// </param>
            /// <returns></returns>
            public FrameDLRObject GenResponseNews(params object[] items)
            {
                if (items != null)
                {
                    return base.GenResponseNews(ToUserName, FromUserName, items.Select(obj=>(FrameDLRObject)FrameDLRObject.CreateInstance(obj,FrameDLRFlags.SensitiveCase)).ToArray());
                }
                else
                {
                    return base.GenResponseNews(ToUserName, FromUserName);
                }
                
            }

        }
    }
}
