using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Flow.Exceptions
{
    public class DuplicateFlowInstanceException:Exception
    {
        public DuplicateFlowInstanceException()
            : base()
        {
        }

        public DuplicateFlowInstanceException(string msg)
            : base(msg)
        {
        }
    }
}
