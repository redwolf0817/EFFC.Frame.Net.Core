using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Interfaces.System;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Global;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EWRA
{
    public class EWRAExceptionProcessor : IExceptionProcess
    {
        public void ProcessException(object sender, Exception ex, ParameterStd p, DataCollection d)
        {
            GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.ERROR, ex.Message);
        }
    }
}
