using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Base.Interfaces.DataConvert
{
    public interface IJSONable
    {

        dynamic ToJSONObject();
        string ToJSONString();
    }
}
