using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Base.ResouceManage;
using EFFC.Frame.Net.Base.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Business.Logic
{
    public abstract partial class BaseLogic<P,D>:ILogic
        where P:ParameterStd
        where D:DataCollection
    {
        //protected P _wp = null;
        //protected D _wd = null;
        //protected ResourceManage _rm = null;
        //protected TransactionToken _token = null;
        protected const string CALL_CONTEXT_PARAMETER = "__parameter__";
        protected const string CALL_CONTEXT_DATACOLLECTION = "__datacollection__";
        protected const string CALL_CONTEXT_RESOURCEMANAGER = "__resource_manager__";
        protected const string CALL_CONTEXT_TRANSTOKEN = "__trans_token__";

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
                CallContext.SetData(Name + CALL_CONTEXT_PARAMETER, p);
                CallContext.SetData(Name + CALL_CONTEXT_DATACOLLECTION, d);
                CallContext.SetData(Name + CALL_CONTEXT_RESOURCEMANAGER, _rm);
                CallContext.SetData(Name + CALL_CONTEXT_TRANSTOKEN, _token);

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
                return (P)CallContext.GetData(Name + CALL_CONTEXT_PARAMETER);
            }
        }
        /// <summary>
        /// 提供call context环境下的数据集访问
        /// </summary>
        public D CallContext_DataCollection
        {
            get
            {
                return (D)CallContext.GetData(Name + CALL_CONTEXT_DATACOLLECTION);
            }
        }
        /// <summary>
        /// 提供call context环境下的当前事务token访问
        /// </summary>
        public TransactionToken CallContext_CurrentToken
        {
            get
            {
                return (TransactionToken)CallContext.GetData(Name + CALL_CONTEXT_TRANSTOKEN);
            }
        }
        /// <summary>
        /// 提供call context环境下的当前资源管理器访问
        /// </summary>
        public ResourceManage CallContext_ResourceManage
        {
            get
            {
                return (ResourceManage)CallContext.GetData(Name + CALL_CONTEXT_RESOURCEMANAGER);
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
