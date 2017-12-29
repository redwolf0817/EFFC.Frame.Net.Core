using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Unit.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EFFC.Frame.Net.Resource.Sqlite
{
    /// <summary>
    /// Sqlite的dbexpress解析器
    /// </summary>
    public class SqliteExpress : SqlTypeDBExpress
    {
        const string pflag = "$";
        const string linkflag = "||";

        protected override string ParameterFlag => "$";

        protected override string LinkFlag => "||";
    }
}
