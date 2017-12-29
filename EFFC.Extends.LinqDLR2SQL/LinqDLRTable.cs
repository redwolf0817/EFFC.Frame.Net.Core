using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Extends.LinqDLR2SQL.DLRColumns;

namespace EFFC.Extends.LinqDLR2SQL
{
    /// <summary>
    /// linq表达式转为sql的table对象，该对象仅用于方便lamda表达式的创建作业，不具备扩展功能
    /// </summary>
    public class LinqDLRTable : LinqDLR2Sql<dynamic>
    {
        /// <summary>
        /// DB类型
        /// </summary>
        public enum DBType
        {
            /// <summary>
            /// 未指定类型
            /// </summary>
            None,
            /// <summary>
            /// SQLServer
            /// </summary>
            SqlServer,
            /// <summary>
            /// Sqlite
            /// </summary>
            Sqlite,
            /// <summary>
            /// MySQL
            /// </summary>
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
