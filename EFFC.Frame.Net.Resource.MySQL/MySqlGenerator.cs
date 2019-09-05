using EFFC.Frame.Net.Unit.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Base.ResouceManage.DB
{
    /// <summary>
    /// Mysql专属linqDLR2sql生成器
    /// </summary>
    public class MySqlGenerator : DaoSqlGenerator
    {
        /// <summary>
        /// Mysql专属linqDLR2sql生成器
        /// </summary>
        public MySqlGenerator() : base(new MySQLOperatorFlags())
        {
        }
    }
}
