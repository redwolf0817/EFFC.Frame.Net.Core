using System;
using System.Collections.Generic;
using System.Text;
using Test.Linq2SQL.DLRColumns;

namespace Test.Linq2SQL
{
    /// <summary>
    /// linq表达式转为sql的table对象
    /// </summary>
    public class LinqDLRTable : LinqDLR2Sql<dynamic>
    {
        public enum DBType
        {
            None,
            SqlServer,
            Sqlite,
            MySql
        }

        /// <summary>
        /// 生成一个LinqDLRTable的实例
        /// </summary>
        /// <param name="table">实际table的名称</param>
        /// <param name="aliasName">table的别名</param>
        /// <param name="dbtype">数据库类型</param>
        /// <returns>LinqDLRTable</returns>
        public static LinqDLRTable New(string table, string aliasName = "", DBType dbtype = DBType.None)
        {
            var tn = aliasName == "" ? table : aliasName;
            LinqDLRTable rtn = null;
            switch (dbtype)
            {
                case DBType.SqlServer:
                    rtn = New<LinqDLRTable>(new LamdaSQLObject<SqlServerDLRColumn>(tn), table, aliasName);
                    break;
                case DBType.Sqlite:
                    rtn = New<LinqDLRTable>(new LamdaSQLObject<SqliteDLRColumn>(tn), table, aliasName);
                    break;
                case DBType.MySql:
                    rtn = New<LinqDLRTable>(new LamdaSQLObject<MysqlDLRColumn>(tn), table, aliasName);
                    break;
                default:
                    rtn = New<LinqDLRTable>(new LamdaSQLObject<LinqDLRColumn>(tn), table, aliasName);
                    break;
            }
            return rtn;
        }
    }
}
