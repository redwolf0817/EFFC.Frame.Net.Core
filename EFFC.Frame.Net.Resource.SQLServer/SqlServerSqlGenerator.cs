using EFFC.Extends.LinqDLR2SQL;
using EFFC.Frame.Net.Unit.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Resource.SQLServer
{
    /// <summary>
    /// SQLServer专属linqDLR2sql生成器
    /// </summary>
    public class SqlServerSqlGenerator : DaoSqlGenerator
    {
        /// <summary>
        /// SQLServer专属linqDLR2sql生成器
        /// </summary>
        public SqlServerSqlGenerator() : base(new SqlServerOperatorFlags())
        {
        }
    }
}
