using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Razor
{
    /// <summary>
    /// Razor的异常
    /// </summary>
    public class RazorException:Exception
    {
        public RazorException()
            : base()
        {
        }

        public RazorException(string msg)
            : base(msg)
        {
        }
        public RazorException(string msg,Exception innerException) : base(msg, innerException)
        {

        }
    }
}
