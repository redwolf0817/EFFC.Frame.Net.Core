using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using EFFC.Frame.Net.Unit.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Resource.Postgresql
{
    public class PostgreSqlExpress : SqlTypeDBExpress
    {
        protected override string ParameterFlag => "@";

        protected override string LinkFlag => "||";
        protected override string Column_Quatation => "\"{0}\"";

       

    }
}
