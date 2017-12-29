using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Base.Exceptions
{
    public class FrameException:Exception
    {
        public FrameException()
            : base()
        {
        }

        public FrameException(string msg)
            : base(msg)
        {
        }
    }
}
