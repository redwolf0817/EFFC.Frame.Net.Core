using EFFC.Frame.Net.Module.Business.Logic;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Module.Business.Parameters;
using EFFC.Frame.Net.Module.Business.Datas;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Module.Extend.WebGo.Datas;
using EFFC.Frame.Net.Module.Extend.WebGo.Parameters;
using EFFC.Frame.Net.Module.Web.Datas;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Module.Web.Logic;

namespace EFFC.Frame.Net.Module.Extend.WebGo.Logic
{
    public abstract partial class GoLogic : WebBaseLogic<GoBusiParameter,GoBusiData>
    {
        FrameDLRObject _loginInfo = null;
        protected abstract Func<LogicData, object> GetFunction(string actionName);
        protected override void DoProcess(GoBusiParameter p, GoBusiData d)
        {
            var func = GetFunction(p.CallAction);
            if (func != null)
            {
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
                ld.WebSocketRecieveData = p.WebParam[DomainKey.POST_DATA, "ws_data"];
                d.WebData.ResponseData = func(ld);

                if (d.WebData.ContentType == GoResponseDataType.RazorView)
                {
                    if (d.WebData.ResponseData == null) d.WebData.ResponseData = FrameDLRObject.CreateInstance();
                    if (d.WebData.MvcModuleData == null) d.WebData.MvcModuleData = d.WebData.ResponseData;
                }
            }
        }

        public string[] RequestResources
        {
            get
            {
                if (CallContext_Parameter.WebParam.RequestResources != null)
                {
                    var copy = new string[CallContext_Parameter.WebParam.RequestResources.Length];
                    CallContext_Parameter.WebParam.RequestResources.CopyTo(copy, 0);
                    return copy;
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 登陆者的信息
        /// </summary>
        /// <returns></returns>
        public dynamic LoginInfo
        {
            get
            {
                if (_loginInfo == null && CallContext_Parameter.WebParam.LoginInfo != null)
                    _loginInfo = (FrameDLRObject)CallContext_Parameter.WebParam.LoginInfo.Clone();

                return _loginInfo;
            }
        }
        /// <summary>
        /// 更新LoginInfo
        /// </summary>
        /// <param name="logininfo"></param>
        public void UpdateLoginInfo(FrameDLRObject logininfo)
        {
            //if (this.Name == "login")
            CallContext_Parameter.WebParam.LoginInfo = logininfo;
        }
        /// <summary>
        /// 判断本次请求是否为ajax异步调用
        /// </summary>
        public bool IsAjaxAsync
        {
            get
            {
                return (bool)CallContext_Parameter["IsAjaxAsync"];
            }
        }
        /// <summary>
        /// 判断本次请求是否为websocket方式
        /// </summary>
        public bool IsWebSocket
        {
            get
            {
                return CallContext_Parameter.WebParam.IsWebSocket;
            }
        }
        /// <summary>
        /// 手工设定host view文件路径
        /// 格式：~/xxx.js
        /// </summary>
        /// <param name="path"></param>
        public void SetHostViewPath(string path)
        {
            CallContext_DataCollection.WebData.ExtentionObj.hostviewpath = path;
        }

        /// <summary>
        /// 设定responsedata的数据类型
        /// </summary>
        /// <param name="type"></param>
        public void SetContentType(GoResponseDataType type)
        {
            this.CallContext_DataCollection.WebData.ContentType = type;
        }

        /// <summary>
        /// Response跳转
        /// </summary>
        /// <param name="touri"></param>
        public void RedirectTo(string touri)
        {
            this.CallContext_DataCollection.WebData.RedirectUri = touri;//HttpUtility.UrlEncode(touri, Encoding.UTF8);
        }

        /// <summary>
        /// Response跳转
        /// </summary>
        /// <param name="touri"></param>
        /// <param name="encoder"></param>
        public void RedirectTo(string touri, Encoding encoder)
        {
            this.CallContext_DataCollection.WebData.RedirectUri = ComFunc.UrlEncode(touri);
        }

        /// <summary>
        /// 设定下载文件的名称
        /// </summary>
        /// <param name="filename"></param>
        public void SetDownLoadFileName(string filename)
        {
            this.CallContext_DataCollection["__download_filename__"] = filename;
        }
        /// <summary>
        /// 判断当前系统是否处于Debug开发模式
        /// </summary>
        public bool IsDebug
        {
            get
            {
                if (ComFunc.nvl(Configs["DebugMode"]) == "")
                {
                    return false;
                }
                else
                {
                    return bool.Parse(ComFunc.nvl(Configs["DebugMode"]));
                }
            }
        }

    }
}
