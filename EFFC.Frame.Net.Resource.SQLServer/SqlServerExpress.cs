using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Unit.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Resource.SQLServer
{
    /// <summary>
    /// SqlServer的DBExpress解析器
    /// </summary>
    public class SqlServerExpress : SqlTypeDBExpress
    {
        protected override string ParameterFlag => "@";

        protected override string LinkFlag => "+";
    }
}
