using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Base.ResouceManage.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Data.Parameters
{
    public class DODParameter:ParameterStd
    {
        public string PropertyName
        {
            get
            {
                return ComFunc.nvl(GetValue("PropertyName"));
            }
            set
            {
                SetValue("PropertyName", value);
            }
        }

        public string FlowInstanceID
        {
            get
            {
                return ComFunc.nvl(GetValue("FlowInstanceID"));
            }
            set
            {
                SetValue("FlowInstanceID", value);
            }
        }

        public override object Clone()
        {
            return this.Clone<DODParameter>();
        }
    }
}
