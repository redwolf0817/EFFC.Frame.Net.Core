using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EFFC.Frame.Net.Data.RuleData
{
    public class ObjectResult
    {
        public ObjectResult()
        {
            DataType = typeof(string);
        }
        public Type DataType { get; set; }
        public object DataValue { get; set; }
    }
}
