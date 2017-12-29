using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Base.Exceptions
{
    public class SyncException:Exception
    {
        public SyncException()
            : base()
        {
        }

        public SyncException(string msg)
            : base(msg)
        {
        }
    }
}
