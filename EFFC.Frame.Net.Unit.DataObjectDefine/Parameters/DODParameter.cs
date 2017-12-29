using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Parameter;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Unit.DataObjectDefine.Parameters
{
    /// <summary>
    /// DataObject的参数集定义
    /// </summary>
    public class DODParameter : ParameterStd
    {
        /// <summary>
        /// DOD Unit所在的Assembly名称
        /// </summary>
        public string UnitAssembly
        {
            get;
            set;
        }

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
            return this.DeepCopy<DODParameter>();
        }
    }
}
