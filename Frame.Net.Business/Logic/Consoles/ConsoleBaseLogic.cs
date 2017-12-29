using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.ResouceManage;
using EFFC.Frame.Net.Base.Token;
using EFFC.Frame.Net.Data.Parameters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Business.Logic.Consoles
{
    public abstract partial class ConsoleBaseLogic<P,D> : BaseLogic<P,D>
        where P:ConsoleParameter
        where D:DataCollection
    {
        public TextWriter ServerOut
        {
            get
            {
                return base.CallContext_Parameter.Out;
            }
        }
    }
}
