using EFFC.Frame.Net.Base.Interfaces.System;
using System;
using System.Collections.Generic;
using System.Text;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Global;

namespace EFFC.Frame.Net.Module.Extend.WebGo
{
    public class WebGoExceptionProcessor : IExceptionProcess
    {
        public void ProcessException(object sender, Exception ex, ParameterStd p, DataCollection d)
        {
            GlobalCommon.Logger.WriteLog(Base.Constants.LoggerLevel.ERROR, ex.Message);
        }
    }
}
