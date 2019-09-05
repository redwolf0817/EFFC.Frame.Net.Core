using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Base.Constants;

namespace EFFC.Extends.LinqDLR2SQL
{
    /// <summary>
    /// linq表达式转为sql的table对象，该对象仅用于方便lamda表达式的创建作业，不具备扩展功能
    /// </summary>
    public class LinqDLRTable : LinqDLR2Sql<dynamic>
    {
        /// <summary>
        /// 生成一个LinqDLRTable的实例
        /// </summary>
        /// <param name="table">实际table的名称</param>
        /// <param name="aliasName">table的别名</param>
        /// <param name="sqlflags">sql相关操作符号</param>
        /// <returns>LinqDLRTable</returns>
        public static LinqDLRTable New<TColumn>(string table, string aliasName = "", SqlOperatorFlags sqlflags=null) where TColumn:LinqDLRColumn
        {
            var tn = aliasName == "" ? table : aliasName;
            sqlflags = sqlflags == null ? new SqlOperatorFlags() : sqlflags;
            LinqDLRTable rtn = New<LinqDLRTable>(new LamdaSQLObject<TColumn>(tn, sqlflags), table, aliasName, new GeneralLinqDLR2SQLGenerator(sqlflags));
            return rtn;
        }
    }
}
