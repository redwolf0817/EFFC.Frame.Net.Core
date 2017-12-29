using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFFC.Frame.Net.Base.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException()
            : base()
        {
        }

        public NotFoundException(string msg)
            : base(msg)
        {
        }
    }
}
