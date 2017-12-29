using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Module;
using EFFC.Frame.Net.Base.Token;
using EFFC.Frame.Net.Data.FlowData;
using EFFC.Frame.Net.Data.Parameters.Flow;
using EFFC.Frame.Net.Flow.Interfaces;
using EFFC.Frame.Net.Flow.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Flow.Core
{
    public abstract class FlowBaseStep : IStep<FlowParameter, FlowData>
    {
        FlowParameter _p = null;
        FlowData _d = null;
        public FlowStateType Run(FlowParameter p, FlowData d)
        {
            _p = p;
            _d = d;

            return ExcuteStep(p, d);
        }

        protected abstract FlowStateType ExcuteStep(FlowParameter p, FlowData d);
        public abstract string StepUID
        {
            get;
        }
        /// <summary>
        /// 開啟事務
        /// </summary>
        public void BeginTrans()
        {
            _p.Resources.BeginTransaction(_p.CurrentTransToken);
        }
        public TransactionToken BeginTrans(FrameIsolationLevel level)
        {
            TransactionToken newtoken = TransactionToken.NewToken();
            newtoken.Begin();
            _p.TransTokenList.Add(newtoken);
            return newtoken;
        }
        /// <summary>
        /// 提交事務
        /// </summary>
        public void CommitTrans()
        {
            _p.Resources.CommitTransaction(_p.CurrentTransToken);
        }
        public void CommitTrans(TransactionToken token)
        {
            _p.Resources.CommitTransaction(token);
        }
        /// <summary>
        /// 回滾事務
        /// </summary>
        public void RollBack()
        {
            _p.Resources.RollbackTransaction(_p.CurrentTransToken);
        }

        public void RollBack(TransactionToken token)
        {
            _p.Resources.RollbackTransaction(token);
        }

        protected void CallLogic(string logic, string action, params KeyValuePair<string,object>[] param)
        {
            var copyp = _p.Clone<FlowParameter>();
            copyp.CalledLogicName = logic;
            copyp.SetValue(ParameterKey.ACTION, action);
            if (param != null)
            {
                foreach (var val in param)
                {
                    copyp.SetValue(DomainKey.CUSTOMER_PARAMETER, val.Key, val.Value);
                }
            }
            ModuleProxyManager.Call<FlowBusinessProxy, FlowParameter, FlowData>(copyp, _d);
        }

        protected void CallLogicSingleton(string logic, string action, params KeyValuePair<string, object>[] param)
        {
            var copyp = _p.Clone<FlowParameter>();
            copyp.CalledLogicName = logic;
            copyp.SetValue(ParameterKey.ACTION, action);
            if (param != null)
            {
                foreach (var val in param)
                {
                    copyp.SetValue(DomainKey.CUSTOMER_PARAMETER, val.Key, val.Value);
                }
            }
            ModuleProxyManager.Call<FlowBusinessSingletonProxy, FlowParameter, FlowData>(copyp, _d);
        }

        protected FrameDLRObject CallLogicAsync(string logic, string action, Action<FlowParameter, FlowData> callback, params KeyValuePair<string, object>[] param)
        {
            dynamic rtn = FrameDLRObject.CreateInstance();
            var copyp = _p.Clone<FlowParameter>();
            copyp.CalledLogicName = logic;
            copyp.SetValue(ParameterKey.ACTION, action);
            if (param != null)
            {
                foreach (var val in param)
                {
                    copyp.SetValue(DomainKey.CUSTOMER_PARAMETER, val.Key, val.Value);
                }
            }
            var proxy = ModuleProxyManager.BeginCall<FlowBusinessProxy, FlowParameter, FlowData>(copyp, _d, callback);
            rtn.callproxy = proxy;
            rtn.callparameter = copyp;

            return rtn;
        }
        protected FrameDLRObject CallLogicSingletonAsync(string logic, string action, Action<FlowParameter, FlowData> callback, params KeyValuePair<string, object>[] param)
        {
            dynamic rtn = FrameDLRObject.CreateInstance();
            var copyp = _p.Clone<FlowParameter>();
            copyp.CalledLogicName = logic;
            copyp.SetValue(ParameterKey.ACTION, action);
            if (param != null)
            {
                foreach (var val in param)
                {
                    copyp.SetValue(DomainKey.CUSTOMER_PARAMETER, val.Key, val.Value);
                }
            }
            var proxy = ModuleProxyManager.BeginCall<FlowBusinessSingletonProxy, FlowParameter, FlowData>(copyp, _d, callback);
            rtn.callproxy = proxy;
            rtn.callparameter = copyp;

            return rtn;
        }

        protected void WaitCallLogicAsync(dynamic callresult)
        {
            if (callresult == null || callresult.callproxy == null || callresult.callparameter == null) return;
            var proxy = (IModularAsyncProxy<FlowParameter, FlowData>)callresult.callproxy;
            var callp = (FlowParameter)callresult.callparameter;
            ModuleProxyManager.EndCall<IModularAsyncProxy<FlowParameter, FlowData>, FlowParameter, FlowData>(proxy, callp, _d);
        }

    }
}
