using EFFC.Frame.Net.Base.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Data.LogicData
{
    public class ConsoleLogicData:DataCollection
    {
        public string[] Args
        {
            get;
            set;
        }

        public override object Clone()
        {
            return this.Clone<ConsoleLogicData>();
        }
    }
}
