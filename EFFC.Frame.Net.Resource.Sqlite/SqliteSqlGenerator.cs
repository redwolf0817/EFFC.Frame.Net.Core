using EFFC.Frame.Net.Unit.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Resource.Sqlite
{
    /// <summary>
    /// Sqlite专属linqDLR2sql生成器
    /// </summary>
    public class SqliteSqlGenerator : DaoSqlGenerator
    {
        /// <summary>
        /// Sqlite专属linqDLR2sql生成器
        /// </summary>
        public SqliteSqlGenerator() : base(new SqliteOperatorFlags())
        {
        }
    }
}
