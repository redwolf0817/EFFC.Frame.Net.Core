using EFFC.Frame.Net.Module.Extend.WebGo.Logic;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Module.Extend.WebGo.Datas;
using EFFC.Frame.Net.Module.Extend.WebGo.Parameters;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Common;
using System.Reflection;
using EFFC.Frame.Net.Global;
using System.IO;
using EFFC.Frame.Net.Base.Data.Base;

namespace EFFC.Frame.Net.Module.Extend.WeixinWeb.Logic
{
    public abstract partial class WeixinGoLogic : GoLogic
    {
        static Dictionary<string, Dictionary<string, MethodInfo>> _method = new Dictionary<string, Dictionary<string, MethodInfo>>();
        static object lockobj = new object();
        protected override void DoProcess(GoBusiParameter p, GoBusiData d)
        {
            lock (lockobj)
            {
                if (!_method.ContainsKey(this.Name))
                {
                    _method.Add(this.Name, new Dictionary<string, MethodInfo>());
                }
                if (_method[this.Name].Count <= 0)
                {
                    var t = this.GetType();
                    var methods = t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (var m in methods)
                    {
                        var parray = m.GetParameters();
                        if (parray.Length == 1 && (parray[0].ParameterType.FullName == typeof(LogicData).FullName))
                        {
                            _method[this.Name].Add(m.Name.ToLower(), m);
                        }
                    }
                }
            }

            LogicData ld = new LogicData();
            //添加querystring
            foreach (var s in p.WebParam.Domain(DomainKey.QUERY_STRING))
            {
                ld.SetValue(s.Key, s.Value);
            }
            //添加postback数据
            foreach (var s in p.WebParam.Domain(DomainKey.POST_DATA))
            {
                ld.SetValue(s.Key, s.Value);
            }
            //添加上传的文件
            foreach (var s in p.WebParam.Domain(DomainKey.UPDATE_FILE))
            {
                ld.SetValue(s.Key, s.Value);
            }
            //传入的参数
            foreach (var s in p.WebParam.Domain(DomainKey.INPUT_PARAMETER))
            {
                ld.SetValue(s.Key, s.Value);
            }
            //自定义的参数
            foreach (var s in p.WebParam.Domain(DomainKey.CUSTOMER_PARAMETER))
            {
                ld.SetValue(s.Key, s.Value);
            }
            object result = null;
            p.CallAction = p.CallAction == "" ? "load" : p.CallAction;
            if (_method[Name].ContainsKey(p.CallAction.ToLower()))
            {

                if (!ExposedObjectHelper.TryInvoke(_method[Name][p.CallAction.ToLower()], this, new object[] { ld }, out result))
                {
                    throw new Exception($"方法{Name}.{p.CallAction}执行失败");
                }
            }
            else
            {
                GlobalCommon.Logger.WriteLog(LoggerLevel.ERROR, $"{Name}的Logic缺少名为{p.CallAction}的方法，请在{Name}中定义{p.CallAction}方法");
            }
            d.WebData.ResponseData = result;

            if (d.WebData.ContentType == GoResponseDataType.RazorView)
            {
                if (d.WebData.ResponseData == null) d.WebData.ResponseData = FrameDLRObject.CreateInstance();
                if (d.WebData.MvcModuleData == null) d.WebData.MvcModuleData = d.WebData.ResponseData;
            }
        }



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
            return arg["echostr"];
        }
        /// <summary>
        /// 接受消息—文本消息
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object msg_text(LogicData arg)
        {
            return Weixin.GenResponseText("接收到一条文本消息");
        }
        /// <summary>
        /// 接受消息—语音消息
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object msg_voice(LogicData arg)
        {
            /*以下代码用于作为多媒体资料获取的sample代码
            var re = Weixin.DownloadMedia(Weixin.MediaId);
            if (re.issuccess)
            {
                if(!Directory.Exists( ServerInfo.ServerRootPath + "/Weixin_files/"))
                {
                    Directory.CreateDirectory(ServerInfo.ServerRootPath + "/Weixin_files/");
                }
                File.WriteAllBytes(ServerInfo.ServerRootPath + "/Weixin_files/" + re.filename, re.content);
            }
            */
            return Weixin.GenResponseText("接收到一条语音消息");
        }
        /// <summary>
        /// 接受消息—图像消息
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object msg_image(LogicData arg)
        {
            /*以下代码用于作为多媒体资料获取的sample代码
            var re = Weixin.DownloadMedia(Weixin.MediaId);
            if (re.issuccess)
            {
                if(!Directory.Exists( ServerInfo.ServerRootPath + "/Weixin_files/"))
                {
                    Directory.CreateDirectory(ServerInfo.ServerRootPath + "/Weixin_files/");
                }
                File.WriteAllBytes(ServerInfo.ServerRootPath + "/Weixin_files/" + re.filename, re.content);
            }
            */
            return Weixin.GenResponseText("接收到一条图片消息");
        }
        /// <summary>
        /// 接受消息—视频消息
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object msg_video(LogicData arg)
        {
            /*以下代码用于作为多媒体资料获取的sample代码
            var re = Weixin.DownloadMedia(Weixin.MediaId);
            if (re.issuccess)
            {
                if(!Directory.Exists( ServerInfo.ServerRootPath + "/Weixin_files/"))
                {
                    Directory.CreateDirectory(ServerInfo.ServerRootPath + "/Weixin_files/");
                }
                File.WriteAllBytes(ServerInfo.ServerRootPath + "/Weixin_files/" + re.filename, re.content);
            }
            */
            return Weixin.GenResponseText("接收到一条视频消息");
        }
        /// <summary>
        /// 接受消息—小视频消息
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object msg_hortvideo(LogicData arg)
        {
            /*以下代码用于作为多媒体资料获取的sample代码
            var re = Weixin.DownloadMedia(Weixin.MediaId);
            if (re.issuccess)
            {
                if(!Directory.Exists( ServerInfo.ServerRootPath + "/Weixin_files/"))
                {
                    Directory.CreateDirectory(ServerInfo.ServerRootPath + "/Weixin_files/");
                }
                File.WriteAllBytes(ServerInfo.ServerRootPath + "/Weixin_files/" + re.filename, re.content);
            }
            */
            return Weixin.GenResponseText("接收到一条小视频消息");
        }
        /// <summary>
        /// 接受消息—地理位置
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object msg_location(LogicData arg)
        {
            return Weixin.GenResponseText($"接收到一条地理位置消息:经度{Weixin.Location_X},纬度{Weixin.Location_Y},地图缩放{Weixin.Scale},位置{Weixin.Label}");
        }
        /// <summary>
        /// 接受消息-链接
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object msg_link(LogicData arg)
        {
            return Weixin.GenResponseText("接收到一条链接消息");
        }
        /// <summary>
        /// 事件-用户未关注时，进行关注后
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object event_subscribe(LogicData arg)
        {
            dynamic re = Weixin.GetUserInfo(Weixin.FromUserName);
            //            return Weixin.GenResponseNews(FrameDLRObject.CreateInstanceFromat(@"{
            //title:{0},
            //picurl:{1}
            //}", $"新用户\"{re.nickname}\"关注", re.headurl));
            return Weixin.GenResponseText($"新用户<img src='{re.headurl}'/>\"{re.nickname}\"关注");
        }
        /// <summary>
        /// 事件- 用户已关注时，进行关注后
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object event_scan(LogicData arg)
        {
            return Weixin.GenResponseText($"老用户\"{Weixin.FromUserName}\"关注");
        }
        /// <summary>
        /// 事件- 用户取消关注
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object event_unsubscribe(LogicData arg)
        {
            return Weixin.GenResponseText("用户取消关注");
        }
        /// <summary>
        /// /事件-上报地理位置
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object event_location(LogicData arg)
        {
            return Weixin.GenResponseText("上报地理位置");
        }
        /// <summary>
        /// 事件-点击菜单
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object event_click(LogicData arg)
        {
            return Weixin.GenResponseText($"点击了菜单{Weixin.EventKey}");
        }
        /// <summary>
        /// 事件-点击菜单跳转链接时
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object event_view(LogicData arg)
        {
            return Weixin.GenResponseText($"点击了菜单的跳转链接{Weixin.EventKey}");
        }

        /// <summary>
        /// 事件-点击菜单扫描一个二维码
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected virtual object event_scancode_waitmsg(LogicData arg)
        {
            return Weixin.GenResponseText($"点击菜单扫描了某个二维码{Weixin.EventKey}");
        }
        #endregion
    }
}
