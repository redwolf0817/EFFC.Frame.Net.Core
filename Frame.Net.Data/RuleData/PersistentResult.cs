using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Constants;
using EFFC.Frame.Net.Base.Data;

namespace EFFC.Frame.Net.Data.RuleData
{
    /// <summary>
    /// 持久化的结果
    /// </summary>
    public class PersistentResult:DataCollection
    {
        /// <summary>
        /// 状态标记
        /// </summary>
        public StateDefine StateFlag
        {
            get
            {
                return (StateDefine)this["StateFlag"];
            }
            set
            {
                this["StateFlag"] = value;
            }
        }

        /// <summary>
        /// 当前Step的UID
        /// </summary>
        public string CurrentStepUID
        {
            get
            {
                return ComFunc.nvl(this["CurrentStepUID"]);
            }
            set
            {
                this["CurrentStepUID"] = value;
            }
        }
    }
}
