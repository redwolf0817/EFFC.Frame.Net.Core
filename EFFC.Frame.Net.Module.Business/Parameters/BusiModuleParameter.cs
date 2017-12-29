using EFFC.Frame.Net.Base.Parameter;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Business.Parameters
{
    /// <summary>
    /// 业务处理模块专用参数集
    /// </summary>
    public class BusiModuleParameter:ParameterStd
    {
        /// <summary>
        /// 要调用的logic名称
        /// </summary>
        public string CallLogicName
        {
            get;
            set;
        }
        /// <summary>
        /// 要调用的action
        /// </summary>
        public string CallAction
        {
            get;
            set;
        }

        public override object Clone()
        {
            var rtn = base.DeepCopy<BusiModuleParameter>();
            return rtn;
        }
    }
}
