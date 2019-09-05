using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Module.Business.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EConsole.Parameters
{
    public class ConsoleParameter: BusiModuleParameter
    {
        /// <summary>
        /// DBConnectionString
        /// </summary>
        public string DBConnectionString
        {
            get
            {
                return ComFunc.nvl(this[DomainKey.CONFIG, ParameterKey.DBCONNECT_STRING]);
            }
            set
            {
                this[DomainKey.CONFIG, ParameterKey.DBCONNECT_STRING] = value;
            }
        }
    }
}
