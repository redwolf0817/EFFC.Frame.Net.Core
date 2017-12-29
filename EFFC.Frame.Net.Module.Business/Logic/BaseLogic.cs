using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Interfaces.Core;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Base.ResouceManage;
using EFFC.Frame.Net.Base.Token;
using EFFC.Frame.Net.Module.Business.Datas;
using EFFC.Frame.Net.Module.Business.Parameters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace EFFC.Frame.Net.Module.Business.Logic
{
    /// <summary>
    /// Logic的基类
    /// </summary>
    public abstract partial class BaseLogic<PType,DType>
        where PType:ParameterStd
        where DType:DataCollection
    {
        AsyncLocal<PType> _asynp = new AsyncLocal<PType>();
        AsyncLocal<DType> _asynd = new AsyncLocal<DType>();
        AsyncLocal<ResourceManage> _asynrm = new AsyncLocal<ResourceManage>();
        AsyncLocal<TransactionToken> _asyntt = new AsyncLocal<TransactionToken>();

        string _name = "";
        /// <summary>
        /// logic的名称，busi模块根据该属性来调用对应的logic
        /// </summary>
        public virtual string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    _name = this.GetType().Name.ToLower();
                    if (_name.EndsWith("logic")) _name = _name.Substring(0, _name.Length - 5);
                    if (_name.EndsWith("go")) _name = _name.Substring(0, _name.Length - 2);
                    if (_name.EndsWith("view")) _name = _name.Substring(0, _name.Length - 4);
                }

                return _name;
            }
        }
        protected abstract void DoProcess(PType p, DType d);
        public void process(PType p, DType d)
        {
            //多线程状态下，各种资源类不可作为属性field共享，否则会发生一致性的问题
            //将传入的资源和参数放入到call context的共享环境中提供可靠的参数和资源访问
            var _rm = p.GetValue<ResourceManage>(ParameterKey.RESOURCE_MANAGER);
            var _token = p.GetValue<TransactionToken>(ParameterKey.TOKEN);

            _asynp.Value = p;
            _asynd.Value = d;
            _asynrm.Value = _rm;
            _asyntt.Value = _token;



            DoProcess(p, d);

        }
        /// <summary>
        /// 提供call context环境下的参数资源集访问
        /// </summary>
        public PType CallContext_Parameter
        {
            get
            {
                return _asynp.Value;
            }
            set
            {
                _asynp.Value = value;
            }
        }
        /// <summary>
        /// 提供call context环境下的数据集访问
        /// </summary>
        public DType CallContext_DataCollection
        {
            get
            {
                return _asynd.Value;
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
                return _asyntt.Value;
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
                return _asynrm.Value;
            }
            set
            {
                _asynrm.Value = value;
            }
        }
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

        /// <summary>
        /// 創建一個普通的資源對象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T NewResourceEntity<T>() where T : IResourceEntity
        {
            return CallContext_ResourceManage.CreateInstance<T>(GetHashCode().ToString());
        }
        /// <summary>
        /// 創建一個事務資源對象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T NewTransResourceEntity<T>() where T : IResourceEntity, ITransaction
        {
            return NewTransResourceEntity<T>(CallContext_CurrentToken);
        }
        /// <summary>
        /// 創建一個事務資源對象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token">指定的事务</param>
        /// <returns></returns>
        public T NewTransResourceEntity<T>(TransactionToken token) where T : IResourceEntity, ITransaction
        {
            return CallContext_ResourceManage.CreateInstance<T>(token);
        }

    }
}
