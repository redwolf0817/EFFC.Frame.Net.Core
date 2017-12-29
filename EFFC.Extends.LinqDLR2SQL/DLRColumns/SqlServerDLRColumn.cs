using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Extends.LinqDLR2SQL.DLRColumns
{
    /// <summary>
    /// Sqlserver专用的DLRColumn
    /// </summary>
    public class SqlServerDLRColumn:LinqDLRColumn
    {
        public override string EqualFlag => "=";
        public override string GreaterEqualFlag => ">=";
        public override string GreaterFlag => ">";
        public override string LessEqualFlag => "<=";
        public override string LessFlag => "<";
        public override string LikeMatchFlag => "%";
        public override string LinkFlag => "+";
        public override string NotEqualFlag => "<>";
        public override string ParamFlag => "@";
    }
}
