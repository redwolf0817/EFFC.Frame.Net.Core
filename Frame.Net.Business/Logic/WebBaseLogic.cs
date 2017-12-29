using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Interfaces;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.ResouceManage;
using EFFC.Frame.Net.Base.ResouceManage.DB;
using EFFC.Frame.Net.Base.Token;
using EFFC.Frame.Net.Base.Unit;
using EFFC.Frame.Net.Data;
using EFFC.Frame.Net.Data.LogicData;
using EFFC.Frame.Net.Data.Parameters;
using EFFC.Frame.Net.Data.UnitData;
using EFFC.Frame.Net.Data.WebData;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Business.Engine;

namespace EFFC.Frame.Net.Business.Logic
{
    public abstract partial class WebBaseLogic<P, D> : BaseLogic<P,D>
        where P : WebParameter
        where D : WebBaseData
    {
        private LoginUserData _loginInfo = null;

        public string Action
        {
            get { return base.CallContext_Parameter.Action; }
        }
        

        protected override void DoProcess(P p, D d)
        {
            LogicData ld = new LogicData();
            //统一写入INPUT_PARAMETER，可以提高logic的通用性
            //添加querystring
            foreach (var s in p.Domain(DomainKey.QUERY_STRING))
            {
                ld.SetValue(s.Key, s.Value);
            }
            //添加postback数据
            foreach (var s in p.Domain(DomainKey.POST_DATA))
            {
                ld.SetValue(s.Key, s.Value);
            }
            //添加上传的文件
            foreach (var s in p.Domain(DomainKey.UPDATE_FILE))
            {
                ld.SetValue(s.Key, s.Value);
            }
            //传入的参数
            foreach (var s in p.Domain(DomainKey.INPUT_PARAMETER))
            {
                ld.SetValue(s.Key, s.Value);
            }
            //自定义的参数
            foreach (var s in p.Domain(DomainKey.CUSTOMER_PARAMETER))
            {
                ld.SetValue(s.Key, s.Value);
            }
            ld.WebSocketRecieveData = p[DomainKey.POST_DATA, "ws_data"];

            DoInvoke(p, d, ld);
        }

        protected abstract void DoInvoke(P p, D d, LogicData ld);

        public string[] RequestResources
        {
            get
            {
                if (CallContext_Parameter.RequestResources != null)
                {
                    var copy = new string[CallContext_Parameter.RequestResources.Length];
                    CallContext_Parameter.RequestResources.CopyTo(copy, 0);
                    return copy;
                }
                else
                {
                    return null;
                }
            }
        }
        
        /// <summary>
        /// 開啟事務
        /// </summary>
        public void BeginTrans()
        {
            CallContext_ResourceManage.BeginTransaction(CallContext_CurrentToken);
        }
        /// <summary>
        /// 提交事務
        /// </summary>
        public void CommitTrans()
        {
            CallContext_ResourceManage.CommitTransaction(CallContext_CurrentToken);
        }
        /// <summary>
        /// 回滾事務
        /// </summary>
        public void RollBack()
        {
            CallContext_ResourceManage.RollbackTransaction(CallContext_CurrentToken);
        }

        /// <summary>
        /// 登陆者的信息
        /// </summary>
        /// <returns></returns>
        public LoginUserData LoginInfo
        {
            get
            {
                if (_loginInfo == null && CallContext_Parameter.LoginInfo != null)
                    _loginInfo = CallContext_Parameter.LoginInfo.Clone<LoginUserData>();

                return _loginInfo;
            }
        }
        /// <summary>
        /// 更新LoginInfo
        /// </summary>
        /// <param name="logininfo"></param>
        public void UpdateLoginInfo(LoginUserData logininfo)
        {
            //if (this.Name == "login")
            CallContext_Parameter.LoginInfo = logininfo;
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
                return CallContext_Parameter.IsWebSocket;
            }
        }
        /// <summary>
        /// 手工设定host view文件路径
        /// 格式：~/xxx.js
        /// </summary>
        /// <param name="path"></param>
        public void SetHostViewPath(string path)
        {
            CallContext_DataCollection.ExtentionObj.hostviewpath = path;
        }
        /// <summary>
        /// 当前系统使用的HostView的渲染引擎
        /// </summary>
        public HostJsView CurrentHostViewEngine
        {
            get
            {
                if (CallContext_Parameter.ExtentionObj.hostviewengine != null && CallContext_Parameter.ExtentionObj.hostviewengine is HostJsView)
                {
                    return (HostJsView)CallContext_Parameter.ExtentionObj.hostviewengine;
                }
                else
                {
                    var rtn = new HostJsView();
                    CallContext_Parameter.ExtentionObj.hostviewengine = rtn;
                    return rtn;
                }
            }
        }
    }
}
