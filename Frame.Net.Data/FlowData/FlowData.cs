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
    public class FlowData:DataCollection
    {
        /// <summary>
        /// 流程处理的状态
        /// </summary>
        public bool IsSuccess
        {
            get
            {
                return (bool)GetValue("__is_success__");
            }
            set
            {
                SetValue("__is_success__", value);
            }
        }
        /// <summary>
        /// 流程处理的结果信息
        /// </summary>
        public string Message
        {
            get
            {
                return ComFunc.nvl(GetValue("__message__"));
            }
            set
            {
                SetValue("__message__", value);
            }
        }
        /// <summary>
        /// 流程处理的结果内容
        /// </summary>
        public FrameDLRObject ResultContent
        {
            get
            {
                return (FrameDLRObject)GetValue("__result_content__");
            }
            set
            {
                SetValue("__result_content__", value);
            }
        }
        /// <summary>
        /// 当前流程处理的数据结果信息
        /// </summary>
        public FlowInstanceInfo CurrentInfo
        {
            get
            {
                return (FlowInstanceInfo)GetValue("__current_instance_info__");
            }
            set
            {
                SetValue("__current_instance_info__", value);
            }
        }
    }

    public class FlowInstanceInfo
    {
        FrameDLRObject _ext;
        FlowStateType _state = FlowStateType.None;
        /// <summary>
        /// 流程名称
        /// </summary>
        public string FlowName { get; set; }
        /// <summary>
        /// 实例名称
        /// </summary>
        public string InstanceID { get; set; }
        /// <summary>
        /// 流程版本号1
        /// </summary>
        public FlowVersion FlowVersion { get; set; }
        /// <summary>
        /// 流程状态
        /// </summary>
        public FlowStateType FlowState
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
            }
        }
        /// <summary>
        /// 流程step
        /// </summary>
        public string StepID { get; set; }
        /// <summary>
        /// 扩展数据
        /// </summary>
        public dynamic ExtionObj
        {
            get
            {
                if (_ext == null)
                {
                    _ext = FrameDLRObject.CreateInstance();
                }

                return _ext;
            }
        }
    }
}
