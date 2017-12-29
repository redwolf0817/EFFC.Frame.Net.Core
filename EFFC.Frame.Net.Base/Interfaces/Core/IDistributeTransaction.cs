using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.Token;

namespace EFFC.Frame.Net.Base.Interfaces.Core
{
    public interface IDistributeTransaction
    {
        /// <summary>
        /// 开启事务处理
        /// </summary>
        void BeginTransaction(TransactionToken token);
        /// <summary>
        /// 提交事务处理
        /// </summary>
        void CommitTransaction(TransactionToken token);
        /// <summary>
        /// 回滚事务处理
        /// </summary>
        void RollbackTransaction(TransactionToken token);
    }
}
