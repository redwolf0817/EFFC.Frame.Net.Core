using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Base.Exceptions
{
    public class TypeRequiredException: Exception
    {
        public TypeRequiredException()
            : base()
        {
        }

        public TypeRequiredException(string msg)
            : base(msg)
        {
        }
    }
}
