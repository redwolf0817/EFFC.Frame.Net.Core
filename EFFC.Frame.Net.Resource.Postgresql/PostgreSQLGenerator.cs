using EFFC.Frame.Net.Unit.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Resource.Postgresql
{
    /// <summary>
    /// PostgreSQL专属linqDLR2sql生成器
    /// </summary>
    public class PostgreSQLGenerator : DaoSqlGenerator
    {
        /// <summary>
        /// PostgreSQL专属linqDLR2sql生成器
        /// </summary>
        public PostgreSQLGenerator() : base(new PostgreSqlOperatorFlags())
        {
        }
    }
}
