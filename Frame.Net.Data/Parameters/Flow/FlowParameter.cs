using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Parameter;
using EFFC.Frame.Net.Base.ResouceManage;
using EFFC.Frame.Net.Base.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFFC.Frame.Net.Data.Parameters.Flow
{
    public class FlowParameter : ParameterStd
    {
        /// <summary>
        /// 请求的流程名称
        /// </summary>
        public string FlowName
        {
            get
            {
                return ComFunc.nvl(GetValue(DomainKey.INPUT_PARAMETER, "flowname"));
            }
            set
            {
                SetValue(DomainKey.INPUT_PARAMETER, "flowname", value);
            }
        }
        /// <summary>
        /// 请求的流程实例
        /// </summary>
        public string FlowInstanceID
        {
            get
            {
                return ComFunc.nvl(GetValue(DomainKey.INPUT_PARAMETER, "flowinstanceid"));
            }
            set
            {
                SetValue(DomainKey.INPUT_PARAMETER, "flowinstanceid", value);
            }
        }
        /// <summary>
        /// 指定跳到指定步骤
        /// </summary>
        public string JumpStep
        {
            get
            {
                return ComFunc.nvl(GetValue(DomainKey.INPUT_PARAMETER, "jumpstep"));
            }
            set
            {
                SetValue(DomainKey.INPUT_PARAMETER, "jumpstep", value);
            }
        }
        
        /// <summary>
        /// 当前被呼叫的logic名称
        /// </summary>
        public string CalledLogicName
        {
            get
            {
                return ComFunc.nvl(GetValue(ParameterKey.LOGIC));
            }
            set
            {
                SetValue(ParameterKey.LOGIC, value);
            }
        }
        bool _iscontinue = true;
        /// <summary>
        /// 决定后面流程是否继续执行
        /// </summary>
        public bool IsContinue
        {
            get
            {
                return _iscontinue;
            }
            set
            {
                _iscontinue = value;
            }
        }

        public override object Clone()
        {
            return this.Clone<FlowParameter>();
        }
    }
}
