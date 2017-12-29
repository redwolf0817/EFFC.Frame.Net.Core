using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.Interfaces.DataConvert;

namespace EFFC.Frame.Net.Data.RuleData
{
    public class RuleJsonModel<T>
    {
        public string ErrorCode { get; set; }
        public Type ErrorType { get; set; }
        public string ErrorMessage { get; set; }
        public T Content { get; set; }
    }
}
