using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;

namespace EFFC.Frame.Net.Rule.Data
{
    public partial class RuleData : DataCollection
    {
        /// <summary>
        /// 计算出的结果值
        /// </summary>
        public string EvaluateResult
        {
            get{return ComFunc.nvl(this["EvaluateResult"]);}
            set { this["EvaluateResult"] = value; }
        }
        /// <summary>
        /// 出错时的错误编号
        /// </summary>
        public string ErrorCode
        {
            get
            {
                return ComFunc.nvl(this["ErrorCode"]);
            }
            set
            {
                this["ErrorCode"] = value;
            }
        }
        /// <summary>
        /// 出错时的错误信息
        /// </summary>
        public string ErrorMsg
        {
            get
            {
                return ComFunc.nvl(this["ErrorMsg"]);
            }
            set
            {
                this["ErrorMsg"] = value;
            }
        }
        /// <summary>
        /// 错误类型
        /// </summary>
        public Type ErrorType
        {
            get
            {
                return (Type)(this["ErrorType"]);
            }
            set
            {
                this["ErrorType"] = value;
            }
        }
    }
}
