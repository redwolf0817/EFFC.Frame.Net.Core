using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.ResouceManage;
using EFFC.Frame.Net.Base.Token;
using EFFC.Frame.Net.Global;
using EFFC.Frame.Net.Module.Extend.WebGo.Datas;
using EFFC.Frame.Net.Module.Extend.WebGo.Parameters;
using EFFC.Frame.Net.Module.Web.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.WebGo.Logic
{
    public abstract partial class GoLogic
    {
        OuterInterfaceHelper _oifh = null;
        public OuterInterfaceHelper OuterInterface
        {
            get
            {
                if (_oifh == null) _oifh = new OuterInterfaceHelper(this);
                return _oifh;
            }


        }
        public class OuterInterfaceHelper
        {
            GoLogic _logic = null;

            public OuterInterfaceHelper(GoLogic logic)
            {
                _logic = logic;
            }
            /// <summary>
            /// 呼叫本地logic
            /// </summary>
            /// <param name="logic"></param>
            /// <param name="action"></param>
            /// <param name="p"></param>
            /// <returns></returns>
            public object CallLocalLogic(string logic, string action, params KeyValuePair<string, object>[] p)
            {
                FrameDLRObject dp = FrameDLRObject.CreateInstance(FrameDLRFlags.SensitiveCase);
                foreach (var item in p)
                {
                    dp.SetValue(item.Key, item.Value);
                }
                return CallLocalLogic(logic, action, dp);
            }

            /// <summary>
            /// 呼叫本地logic
            /// </summary>
            /// <param name="logic"></param>
            /// <param name="action"></param>
            /// <param name="p"></param>
            /// <returns></returns>
            public object CallLocalLogic(string logic, string action, FrameDLRObject p)
            {
                var copyp = (WebParameter)_logic.CallContext_Parameter.WebParam.Clone();
                var copyd = (GoData)_logic.CallContext_DataCollection.WebData.Clone();
                copyp.RequestResourceName = logic;
                copyp.Action = action;
                ResourceManage rema = new ResourceManage();
                copyp.SetValue(ParameterKey.RESOURCE_MANAGER, rema);
                var defaulttoken = TransactionToken.NewToken();
                copyp.TransTokenList.Add(defaulttoken);
                copyp.SetValue(ParameterKey.TOKEN, defaulttoken);
                copyp.SetValue("IsAjaxAsync", false);
                if(p != null)
                {
                    foreach(var key in p.Keys)
                    {
                        copyp.SetValue(DomainKey.CUSTOMER_PARAMETER, key, p.GetValue(key));
                    }
                }

                return CallLocalLogic(logic, action, copyp, copyd);
            }
            /// <summary>
            /// 呼叫本地logic
            /// </summary>
            /// <param name="logic"></param>
            /// <param name="action"></param>
            /// <param name="p"></param>
            /// <param name="d"></param>
            /// <returns></returns>
            private object CallLocalLogic(string logic, string action, WebParameter p, GoData d)
            {
                object od = d;
                GlobalCommon.Proxys["gobusi"].CallModule(ref od, p);
                return d.ResponseData;
            }



            
        }
    }
}
