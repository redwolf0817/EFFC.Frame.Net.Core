using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EBA.Attributes
{
    /// <summary>
    /// 每xx（秒，分，小时，天）执行一次
    /// </summary>
    public class EBAFrequenceAttribute : ScheduleAttribute
    {
        /// <summary>
        /// 频率类型
        /// </summary>
        public enum FrequenceType
        {
            /// <summary>
            /// 秒
            /// </summary>
            Second,
            /// <summary>
            /// 分
            /// </summary>
            Minute,
            /// <summary>
            /// 小时
            /// </summary>
            Hour,
            /// <summary>
            /// 天
            /// </summary>
            Day
        }
        double seconds = 0;
        public EBAFrequenceAttribute(FrequenceType type, double unit)
        {
            switch (type)
            {
                case FrequenceType.Second:
                    seconds = unit;
                    break;
                case FrequenceType.Minute:
                    seconds = unit * 60;
                    break;
                case FrequenceType.Hour:
                    seconds = unit * 60 * 60;
                    break;
                case FrequenceType.Day:
                    seconds = unit * 60 * 60 * 24;
                    break;
            }
        }
        public override DateTime GetNextStartTime(DateTime pretime)
        {
            return pretime.AddSeconds(seconds);
        }
    }
}
