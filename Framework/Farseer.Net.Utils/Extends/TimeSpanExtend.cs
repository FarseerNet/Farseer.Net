using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace

namespace FS.Extends
{
    /// <summary>
    ///     其它扩展，夫归类的扩展
    /// </summary>
    public static partial class TimeSpanExtend
    {
        /// <summary>
        /// 返回时间中文的描述
        /// </summary>
        public static string GetDateDesc(this TimeSpan ts)
        {
            if (ts.TotalDays >= 1)
            {
                return $"{ts.TotalDays.ConvertType(0)} 天 {ts.Hours.ConvertType(0)} 小时";
            }

            if (ts.TotalHours >= 1)
            {
                return $"{ts.TotalHours.ConvertType(0)} 小时 {ts.Minutes.ConvertType(0)} 分";
            }

            if (ts.TotalMinutes >= 1)
            {
                return $"{ts.TotalMinutes.ConvertType(0)} 分 {ts.Seconds.ConvertType(0)} 秒";
            }

            return $"{ts.TotalSeconds.ConvertType(0)} 秒";
        }
    }
}