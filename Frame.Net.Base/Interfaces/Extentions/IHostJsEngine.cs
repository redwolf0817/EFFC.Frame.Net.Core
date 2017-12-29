using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Base.Interfaces.Extentions
{
    interface IHostJsEngine
    {
        void Evaluate(string script, params KeyValuePair<string, object>[] kvp);
        object GetOutObject(string key);
    }
}
