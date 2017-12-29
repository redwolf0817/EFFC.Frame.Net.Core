using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Base.Exceptions
{
    public class ItemExistsException:Exception
    {
         public ItemExistsException()
            : base()
        {
        }

         public ItemExistsException(string msg)
            : base(msg)
        {
        }
    }
}
