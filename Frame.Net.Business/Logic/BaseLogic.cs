using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Base.ResouceManage;
using EFFC.Frame.Net.Base.Token;
using System;
using System.Threading;

namespace EFFC.Frame.Net.Business.Logic
{
    public abstract partial class BaseLogic<P,D>:ILogic
        where P:ParameterStd
        where D:DataCollection
    {
        AsyncLocal<ParameterStd> _asynp = new AsyncLocal<ParameterStd>();
        AsyncLocal<DataCollection> _asynd = new AsyncLocal<DataCollection>();
        AsyncLocal<ResourceManage> _asynrm = new AsyncLocal<ResourceManage>();
        AsyncLocal<TransactionToken> _asyntt = new AsyncLocal<TransactionToken>();

        public abstract string Name
        {
            get;
        }

        public void process(Base.Parameter.ParameterStd p, Base.Data.DataCollection d)
        {
            //多线程状态下，各种资源类不可作为属性field共享，否则会发生一致性的问题
            //将传入的资源和参数放入到call context的共享环境中提供可靠的参数和资源访问
            var _rm = p.GetValue<ResourceManage>(ParameterKey.RESOURCE_MANAGER);
            var _token = p.GetValue<TransactionToken>(ParameterKey.TOKEN);
            try
            {
                _asynp.Value = p;
                _asynd.Value = d;
                _asynrm.Value = _rm;
                _asyntt.Value = _token;

                DoProcess((P)p, (D)d);

                if (_token.CurrentStatus == TransactionToken.TransStatus.Begin)
                    CommitTrans();

            }
            catch (Exception ex)
            {
                RollBack();
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                _rm.Release(_token);

            }
        }
        /// <summary>
        /// 提供call context环境下的参数资源集访问
        /// </summary>
        public P CallContext_Parameter
        {
            get
            {
                return (P)_asynp.Value;
            }
            set
            {
                _asynp.Value = value;
            }
        }
        /// <summary>
        /// 提供call context环境下的数据集访问
        /// </summary>
        public D CallContext_DataCollection
        {
            get
            {
                return (D)_asynd.Value;
            }
            set
            {
                _asynd.Value = value;
            }
        }
        /// <summary>
        /// 提供call context环境下的当前事务token访问
        /// </summary>
        public TransactionToken CallContext_CurrentToken
        {
            get
            {
                return (TransactionToken)_asyntt.Value;
            }
            set
            {
                _asyntt.Value = value;
            }
        }
        /// <summary>
        /// 提供call context环境下的当前资源管理器访问
        /// </summary>
        public ResourceManage CallContext_ResourceManage
        {
            get
            {
                return (ResourceManage)_asynrm.Value;
            }
            set
            {
                _asynrm.Value = value;
            }
        }


        protected abstract void DoProcess(P p, D d);

        /// <summary>
        /// 開啟事務
        /// </summary>
        public void BeginTrans()
        {
            CallContext_ResourceManage.BeginTransaction(CallContext_CurrentToken);
        }
        public TransactionToken BeginTrans(FrameIsolationLevel level)
        {
            TransactionToken newtoken = TransactionToken.NewToken();
            newtoken.Begin();
            CallContext_Parameter.TransTokenList.Add(newtoken);
            return newtoken;
        }
        /// <summary>
        /// 提交事務
        /// </summary>
        public void CommitTrans()
        {
            CallContext_ResourceManage.CommitTransaction(CallContext_CurrentToken);
        }
        public void CommitTrans(TransactionToken token)
        {
            CallContext_ResourceManage.CommitTransaction(token);
        }
        /// <summary>
        /// 回滾事務
        /// </summary>
        public void RollBack()
        {
            CallContext_ResourceManage.RollbackTransaction(CallContext_CurrentToken);
        }

        public void RollBack(TransactionToken token)
        {
            CallContext_ResourceManage.RollbackTransaction(token);
        }
        /// <summary>
        /// 创建一个资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T CreateResource<T>() where T : IResourceEntity
        {
            return CallContext_ResourceManage.CreateInstance<T>();
        }
    }
}
