using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;
using EFFC.Frame.Net.Base.Data.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Data.FlowData
{
    public class FlowInstanceReport:DataCollection
    {
        FrameDLRObject _ext = FrameDLRObject.CreateInstance();
        /// <summary>
        /// 执行完成之后的当前Step
        /// </summary>
        public string ResultStep
        {
            get
            {
                return ComFunc.nvl(GetValue("ResultStep"));
            }
            set
            {
                SetValue("ResultStep", value);
            }
        }
        /// <summary>
        /// 执行后的状态
        /// </summary>
        public FlowStateType ResultState
        {
            get
            {
                return GetValue<FlowStateType>("ResultState");
            }
            set
            {
                SetValue("ResultState", value);
            }
        }
    }
}
