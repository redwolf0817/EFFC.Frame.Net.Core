using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Module.Extend.WebGo.Logic;
using EFFC.Frame.Net.Module.Extend.WebGo.Datas;
using EFFC.Frame.Net.Module.Extend.WebGo.Parameters;
using EFFC.Frame.Net.Module.Extend.WeChat;

namespace EFFC.Frame.Net.Module.Extend.WeixinWeb.Logic
{
    /// <summary>
    /// 企业微信handler
    /// </summary>
    public abstract partial class QiyeWxGoLogic : GoLogic
    {
        static Dictionary<string, Dictionary<string, MethodInfo>> _method = new Dictionary<string, Dictionary<string, MethodInfo>>();
        static object lockobj = new object();

        /// <summary>
        /// 调用方法
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        protected override void DoProcess(GoBusiParameter p, GoBusiData d)
        {
            lock (lockobj)
            {
                // 如果方法列表数组没有就添加
                if (!_method.ContainsKey(Name))
                    _method.Add(Name, new Dictionary<string, MethodInfo>());

                if (_method[Name].Count <= 0)
                {
                    var t = GetType();
                    var methods = t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (var m in methods)
                    {
                        var parray = m.GetParameters();
                        if (parray.Length == 1 && (parray[0].ParameterType.FullName == typeof(LogicData).FullName))
                            _method[Name].Add(m.Name.ToLower(), m);
                    }
                }
            }

            LogicData ld = new LogicData();
            //添加querystring
            foreach (var s in p.WebParam.Domain(DomainKey.QUERY_STRING))
                ld.SetValue(s.Key, s.Value);

            //添加postback数据
            foreach (var s in p.WebParam.Domain(DomainKey.POST_DATA))
                ld.SetValue(s.Key, s.Value);

            //添加上传的文件
            foreach (var s in p.WebParam.Domain(DomainKey.UPDATE_FILE))
                ld.SetValue(s.Key, s.Value);

            //传入的参数
            foreach (var s in p.WebParam.Domain(DomainKey.INPUT_PARAMETER))
                ld.SetValue(s.Key, s.Value);

            //自定义的参数
            foreach (var s in p.WebParam.Domain(DomainKey.CUSTOMER_PARAMETER))
                ld.SetValue(s.Key, s.Value);

            object result = null;
            //如果没有action视为load
            p.CallAction = p.CallAction == "" ? "load" : p.CallAction;
            //呼叫方法
            if (_method[Name].ContainsKey(p.CallAction.ToLower()))
                ExposedObjectHelper.TryInvoke(_method[Name][p.CallAction.ToLower()], this, new object[] { ld }, out result);
            else
                GlobalCommon.Logger.WriteLog(LoggerLevel.ERROR, $"{Name}的Logic缺少名为{p.CallAction}的方法，请在{Name}中定义{p.CallAction}方法");
            //返回
            d.WebData.ResponseData = result;

            //如果是razorview
            if (d.WebData.ContentType == GoResponseDataType.RazorView)
            {
                if (d.WebData.ResponseData == null) d.WebData.ResponseData = FrameDLRObject.CreateInstance();
                if (d.WebData.MvcModuleData == null) d.WebData.MvcModuleData = d.WebData.ResponseData;
            }
        }
        /// <summary>
        /// 原有的,通过action执行对应方法的方法,现在已经不用了
        /// </summary>
        /// <param name="actionName"></param>
        /// <returns></returns>
        protected override Func<LogicData, object> GetFunction(string actionName)
        {
            //该方法不需要了
            return null;
        }


        #region Weixin预设方法定义

        /// <summary>
        /// 接入验证消息
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object api_valid(LogicData arg)
        {
            SetContentType(GoResponseDataType.String);
            string echostr = ComFunc.nvl(arg["echostr"]);
            WXBizMsgCrypt wxcpt = new WXBizMsgCrypt(Weixin.Token, Weixin.EncodingAESKey, Weixin.AppID);
            string msg = "";
            wxcpt.VerifyURL(Weixin.signature, Weixin.timestamp, Weixin.nonce, echostr, ref msg);
            return msg;
        }

        #region 消息类
        /// <summary>
        /// 接受消息—文本消息,复写它以实现自己逻辑
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object msg_text(LogicData arg)
        {
            var rtn = Weixin.GenResponseText("接收到一条文本消息");
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "这里是msg_text:" + rtn.ToJSONString());
            return rtn;
        }

        /// <summary>
        /// 接受消息—图片消息,复写它以实现自己逻辑
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object msg_image(LogicData arg)
        {
            var rtn = Weixin.GenResponseText("接收到一条图片消息");
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "这里是msg_image" + rtn.ToJSONString());
            return rtn;
        }

        /// <summary>
        /// 接受消息—语音消息,复写它以实现自己逻辑
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object msg_voice(LogicData arg)
        {
            var rtn = Weixin.GenResponseText("接收到一条语音消息");
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "这里是msg_voice" + rtn.ToJSONString());
            return rtn;
        }

        /// <summary>
        /// 接受消息—视频消息,复写它以实现自己逻辑
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object msg_video(LogicData arg)
        {
            var rtn = Weixin.GenResponseText("接收到一条视频消息");
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "这里是msg_video" + rtn.ToJSONString());
            return rtn;
        }

        /// <summary>
        /// 接受消息—位置消息,复写它以实现自己逻辑
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object msg_location(LogicData arg)
        {
            var rtn = Weixin.GenResponseText("接收到一条位置消息");
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "这里是msg_location" + rtn.ToJSONString());
            return rtn;
        }

        /// <summary>
        /// 接受消息—链接消息,复写它以实现自己逻辑
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object msg_link(LogicData arg)
        {
            var rtn = Weixin.GenResponseText("接收到一条链接消息");
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "这里是msg_link" + rtn.ToJSONString());
            return rtn;
        }
        #endregion

        #region 事件类

        /// <summary>
        /// 接受事件—[订阅],复写它以实现自己逻辑
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object event_subscribe(LogicData arg)
        {
            var rtn = Weixin.GenResponseText("接收到[订阅]事件");
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "这里是[event_subscribe]:" + rtn.ToJSONString());
            return rtn;
        }

        /// <summary>
        /// 接受事件—[取消订阅],复写它以实现自己逻辑
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object event_unsubscribe(LogicData arg)
        {
            var rtn = Weixin.GenResponseText("接收到[取消订阅]事件");
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "这里是[event_unsubscribe]:" + rtn.ToJSONString());
            return rtn;
        }
        /// <summary>
        /// 接受事件—[点击菜单拉取消息的事件],复写它以实现自己逻辑
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object event_click(LogicData arg)
        {
            var rtn = Weixin.GenResponseText("接收到[点击菜单拉取消息的事件]事件");
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "这里是[event_click]:" + rtn.ToJSONString());
            return rtn;
        }
        /// <summary>
        /// 接受事件—[点击菜单跳转链接的事件推送],复写它以实现自己逻辑
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object event_view(LogicData arg)
        {
            var rtn = Weixin.GenResponseText("接收到[点击菜单跳转链接的事件推送]事件");
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "这里是[event_view]:" + rtn.ToJSONString());
            return rtn;
        }
        /// <summary>
        /// 接受事件—[扫码推事件的事件推送],复写它以实现自己逻辑
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object event_scancode_push(LogicData arg)
        {
            var rtn = Weixin.GenResponseText("接收到[扫码推事件的事件推送]事件");
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "这里是[event_scancode_push]:" + rtn.ToJSONString());
            return rtn;
        }
        /// <summary>
        /// 接受事件—[扫码推事件且弹出“消息接收中”提示框的事件推送],复写它以实现自己逻辑
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object event_scancode_waitmsg(LogicData arg)
        {
            var rtn = Weixin.GenResponseText("接收到[扫码推事件且弹出“消息接收中”提示框的事件推送]事件");
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "这里是[event_scancode_waitmsg]:" + rtn.ToJSONString());
            return rtn;
        }
        /// <summary>
        /// 接受事件—[弹出系统拍照发图的事件推送],复写它以实现自己逻辑
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object event_pic_sysphoto(LogicData arg)
        {
            var rtn = Weixin.GenResponseText("接收到[弹出系统拍照发图的事件推送]事件");
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "这里是[event_pic_sysphoto]:" + rtn.ToJSONString());
            return rtn;
        }
        /// <summary>
        /// 接受事件—[弹出拍照或者相册发图的事件推送],复写它以实现自己逻辑
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object event_pic_photo_or_album(LogicData arg)
        {
            var rtn = Weixin.GenResponseText("接收到[弹出拍照或者相册发图的事件推送]事件");
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "这里是[event_pic_photo_or_album]:" + rtn.ToJSONString());
            return rtn;
        }
        /// <summary>
        /// 接受事件—[弹出微信相册发图器的事件推送],复写它以实现自己逻辑
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object event_pic_weixin(LogicData arg)
        {
            var rtn = Weixin.GenResponseText("接收到[弹出拍照或者相册发图的事件推送]事件");
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "这里是[event_pic_weixin]:" + rtn.ToJSONString());
            return rtn;
        }
        /// <summary>
        /// 接受事件—[弹出地理位置选择器的事件推送],复写它以实现自己逻辑
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object event_location_select(LogicData arg)
        {
            var rtn = Weixin.GenResponseText("接收到[弹出地理位置选择器的事件推送]事件");
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "这里是[event_location_select]:" + rtn.ToJSONString());
            return rtn;
        }

        /// <summary>
        /// 接受事件—[进入应用],复写它以实现自己逻辑
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object event_enter_agent(LogicData arg)
        {
            var rtn = Weixin.GenResponseText("接收到[进入应用]事件");
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "这里是[event_enter_agent]:" + rtn.ToJSONString());
            return rtn;
        }

        /// <summary>
        /// 接受事件—[上报地理位置],复写它以实现自己逻辑
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object event_location(LogicData arg)
        {
            var rtn = Weixin.GenResponseText("接收到[上报地理位置]事件");
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "这里是[event_location]:" + rtn.ToJSONString());
            return rtn;
        }

        /// <summary>
        /// 接受事件—[异步任务完成事件推送],复写它以实现自己逻辑
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object event_batch_job_result(LogicData arg)
        {
            //这里要看JobType,目前分别有：sync_user(增量更新成员)、 replace_user(全量覆盖成员）、invite_user(邀请成员关注）、replace_party(全量覆盖部门)
            var rtn = Weixin.GenResponseText("接收到[异步任务完成事件推送]事件");
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "这里是[event_batch_job_result]:" + rtn.ToJSONString());
            return rtn;
        }

        /// <summary>
        /// 接受事件—[通讯录变更]或[更新成员事件],复写它以实现自己逻辑
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object event_change_contact(LogicData arg)
        {
            //这里具体要看ChangeType,可能会是create_user,update_user,delete_user,create_party,update_party,delete_party,update_tag
            var rtn = Weixin.GenResponseText("接收到[通讯录变更]事件");
            GlobalCommon.Logger.WriteLog(LoggerLevel.DEBUG, "这里是[event_change_contact]:" + rtn.ToJSONString());
            return rtn;
        }



        #endregion
        /// <summary>
        /// 通过这个方法.传入当前URL可以得到jsdk所需的config
        /// </summary>
        /// <param name="arg"> url参数 是必须的</param>
        /// <returns></returns>
        protected virtual object GetJsdkInfo(LogicData arg)
        {
            var url = ComFunc.UrlDecode(ComFunc.nvl(arg["url"])).Replace(" ", "+");
            var nonstr = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);
            var timestamp = Weixin.NewTimsStamp;
            var sign = Weixin.GenJSAPISignKey(nonstr, timestamp, url, Weixin.Jsapi_ticket);

            var rtn = FrameDLRObject.CreateInstance(true, "");
            rtn.appId = Weixin.AppID;
            rtn.timestamp = timestamp;
            rtn.nonceStr = nonstr;
            rtn.signature = sign;
            rtn.jsapi_ticket = Weixin.Jsapi_ticket; // 应该没用

            return rtn;
        }
        #endregion
    }
}
