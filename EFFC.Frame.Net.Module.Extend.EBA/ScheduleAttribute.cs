using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EBA
{
    /// <summary>
    /// 执行进度表属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class ScheduleAttribute:Attribute
    {
        /// <summary>
        /// 下次启动时间
        /// </summary>
        public abstract DateTime GetNextStartTime(DateTime pretime);
    }
}
