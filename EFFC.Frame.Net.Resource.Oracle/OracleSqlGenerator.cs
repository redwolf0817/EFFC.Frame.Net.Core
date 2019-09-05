using EFFC.Frame.Net.Base.ResouceManage.DB;
using EFFC.Frame.Net.Unit.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Base.ResouceManage.DB
{
    /// <summary>
    /// Oracle专属linqDLR2sql生成器
    /// </summary>
    public class OracleSqlGenerator : DaoSqlGenerator
    {
        /// <summary>
        /// Oracle专属linqDLR2sql生成器
        /// </summary>
        public OracleSqlGenerator() : base(new OracleOperatorFlags())
        {
        }
    }
}
