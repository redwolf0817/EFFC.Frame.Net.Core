using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EFFC.Extends.LinqDLR2SQL
{
    /// <summary>
    /// LinqDLR2SQL用于sql生成的扩展接口
    /// </summary>
    public class GeneralLinqDLR2SQLGenerator : LinqDLR2SQLGenerator
    {
        SqlOperatorFlags _sqlflags = null;
        public override SqlOperatorFlags SqlFlags { get => _sqlflags; }
        public GeneralLinqDLR2SQLGenerator(SqlOperatorFlags sqlflags)
        {
            _sqlflags = sqlflags == null ? new SqlOperatorFlags() : sqlflags;
        }
    }


}
