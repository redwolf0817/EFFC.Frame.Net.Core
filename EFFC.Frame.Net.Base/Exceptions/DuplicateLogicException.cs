using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFFC.Frame.Net.Base.Exceptions
{
    public class DuplicateLogicException : Exception
    {
        public DuplicateLogicException()
            : base()
        {
        }

        public DuplicateLogicException(string msg)
            : base(msg)
        {
        }
    }
}
