using EFFC.Frame.Net.Base.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFFC.Frame.Net.Base.Interfaces.Core
{
    public interface ITransaction
    {
        /// <summary>
        /// 开启事务处理
        /// </summary>
        void BeginTransaction(FrameIsolationLevel level);
        /// <summary>
        /// 提交事务处理
        /// </summary>
        void CommitTransaction();
        /// <summary>
        /// 回滚事务处理
        /// </summary>
        void RollbackTransaction();
    }
}
