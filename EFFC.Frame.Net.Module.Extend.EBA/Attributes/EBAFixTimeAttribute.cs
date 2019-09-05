using EFFC.Frame.Net.Base.Common;
using EFFC.Frame.Net.Base.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFFC.Frame.Net.Module.Extend.EBA.Attributes
{
    /// <summary>
    /// 在固定周期的时间点运行
    /// </summary>
    public class EBAFixTimeAttribute : ScheduleAttribute
    {
        int _month = 0;
        int _day = 0;
        int _hour = 0;
        int _minute = 0;
        int _second = 0;
        /// <summary>
        /// 约定一个周期的固定时间点运行
        /// </summary>
        /// <param name="month">1-12，-1表示忽略</param>
        /// <param name="day">1-31，-1表示忽略</param>
        /// <param name="hour">0-23，-1表示忽略</param>
        /// <param name="minute">0-59,-1表示忽略</param>
        /// <param name="second">0-59，-1表示忽略</param>
        public EBAFixTimeAttribute(int month=-1,int day= -1, int hour= -1, int minute= -1, int second= -1)
        {
            _month = month;
            _day = day;
            _hour = hour;
            _minute = minute;
            _second = second;
        }
        public override DateTime GetNextStartTime(DateTime pretime)
        {
            var firsttime = GetSettingTime();
            if (GetSettingTime() != DateTime.MinValue) return firsttime;

            var maxday = 0;

            var year = 0;
            var month = 0;
            var day = 0;
            var hour = 0;
            var minute = 0;
            var second = 0;
            
            var rtn = DateTime.MaxValue;
            if (_month >= 1 && _month <= 12)
            {
                year = _month <= pretime.Month ? pretime.AddYears(1).Year : pretime.Year;
                month = _month;
                maxday = new DateTimeStd(year, month, 1).DayOfMonth;
                if (_day > maxday)
                {
                    day = maxday;
                }
                else
                {
                    day = _day > 0 ? _day : pretime.Day;
                }
                hour = _hour >= 0 && _hour <= 59 ? _hour : pretime.Hour;
                minute = _minute >= 0 && _minute <= 59 ? _minute : pretime.Minute;
                second = _second >= 0 && _second <= 59 ? _second : pretime.Second;

                rtn = new DateTime(year, month, day, hour, minute, second);
                return (rtn - pretime).TotalSeconds <= 0 ? DateTime.MaxValue : rtn;
            }



            if (_day > 0)
            {
                var tmp = pretime;
                if (_day > 0) tmp = pretime.AddMonths(1);
                year = tmp.Year;
                month = tmp.Month;
                maxday = new DateTimeStd(year, month, 1).DayOfMonth;

                day = _day > maxday ? maxday : _day;
                hour = _hour >= 0 && _hour <= 59 ? _hour : pretime.Hour;
                minute = _minute >= 0 && _minute <= 59 ? _minute : pretime.Minute;
                second = _second >= 0 && _second <= 59 ? _second : pretime.Second;

                rtn = new DateTime(year, month, day, hour, minute, second);
                return (rtn - pretime).TotalSeconds <= 0 ? DateTime.MaxValue : rtn;
            }
            if (_hour >= 0 && _hour <= 59)
            {
                var tmp = pretime.AddDays(1);
                year = tmp.Year;
                month = tmp.Month;
                day = tmp.Day;
                hour = _hour;
                minute = _minute >= 0 && _minute <= 59 ? _minute : pretime.Minute;
                second = _second >= 0 && _second <= 59 ? _second : pretime.Second;

                rtn = new DateTime(year, month, day, hour, minute, second);
                return (rtn - pretime).TotalSeconds <= 0 ? DateTime.MaxValue : rtn;
            }
            if (_minute >= 0 && _minute <= 59)
            {
                var tmp = pretime.AddHours(1);
                year = tmp.Year;
                month = tmp.Month;
                day = tmp.Day;
                hour = tmp.Hour;
                minute = _minute;
                second = _second >= 0 && _second <= 59 ? _second : pretime.Second;

                rtn = new DateTime(year, month, day, hour, minute, second);
                return (rtn - pretime).TotalSeconds <= 0 ? DateTime.MaxValue : rtn;
            }
            if (_second >= 0 && _second <= 59)
            {
                var tmp = pretime.AddMinutes(1);
                year = tmp.Year;
                month = tmp.Month;
                day = tmp.Day;
                hour = tmp.Hour;
                minute = tmp.Minute;
                second = _second;

                rtn = new DateTime(year, month, day, hour, minute, second);
                return (rtn - pretime).TotalSeconds <= 0 ? DateTime.MaxValue : rtn;
            }

            return DateTime.MaxValue;
        }

        private DateTime GetSettingTime()
        {
            var maxday = 0;
            var dt = DateTime.Now;
            var year = dt.Year;
            var month = _month > 0 && _month < 13 ? _month : dt.Month;
            maxday = new DateTimeStd(year, month, 1).DayOfMonth;

            var day = _day>0 && _day <=maxday?_day: dt.Day;
            var hour = _hour >= 0 && _hour <= 23 ? _hour : dt.Hour;
            var minute = _minute >= 0 && _minute <= 59 ? _minute : dt.Hour;
            var second = _second >= 0 && _second <= 59 ? _second : dt.Hour;
            var rtn = new DateTime(year, month, day, hour, minute, second);
            if (rtn > DateTime.Now)
            {
                return rtn;
            }
            else
            {
                return DateTime.MinValue;
            }
        }
    }
}
