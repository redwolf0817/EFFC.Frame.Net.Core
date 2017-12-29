using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.Constants;

namespace EFFC.Frame.Net.Base.Interfaces.System
{
    public interface ILogger
    {
        void WriteLog(LoggerLevel level, string message);
    }
}
