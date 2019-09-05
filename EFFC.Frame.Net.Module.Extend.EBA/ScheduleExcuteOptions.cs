using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EBA
{
    /// <summary>
    /// 任务执行的选项，该选项决定任务执行的方式
    /// </summary>
    public class ScheduleExcuteOptions
    {
        public ScheduleExcuteOptions()
        {
            IsImmediately = false;
            CallLogic = "";
            CallAction = "";
            IsNotExcute = false;
            CallGroup = "";
        }
        /// <summary>
        /// 是否立即执行
        /// </summary>
        public bool IsImmediately { get; set; }
        /// <summary>
        /// 指定要执行的logic
        /// </summary>
        public string CallLogic { get; set; }
        /// <summary>
        /// 指定要执行的action
        /// </summary>
        public string CallAction { get; set; }
        /// <summary>
        /// 执行是否不执行
        /// </summary>
        public bool IsNotExcute { get; set; }
        /// <summary>
        /// 指定要指定的组
        /// </summary>
        public string CallGroup { get; set; }
    }
}
