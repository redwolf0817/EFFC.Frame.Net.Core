using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Flow.Exceptions
{
    public class InvalidFlowException:Exception
    {
        public InvalidFlowException()
            : base()
        {
        }

        public InvalidFlowException(string msg)
            : base(msg)
        {
        }
    }
}
