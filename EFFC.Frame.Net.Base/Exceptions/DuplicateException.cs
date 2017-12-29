using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFFC.Frame.Net.Base.Exceptions
{
    public class DuplicateException : Exception
    {
        public DuplicateException()
            : base()
        {
        }

        public DuplicateException(string msg)
            : base(msg)
        {
        }
    }
}
