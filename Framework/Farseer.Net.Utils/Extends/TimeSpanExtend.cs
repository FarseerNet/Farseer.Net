using System;

// ReSharper disable once CheckNamespace

namespace FS.Extends
{
    /// <summary>
    ///     其它扩展，夫归类的扩展
    /// </summary>
    public static class TimeSpanExtend
    {
        /// <summary>
        ///     返回时间中文的描述
        /// </summary>
        public static string GetDateDesc(this TimeSpan ts)
        {
            if (ts.TotalDays >= 1) return $"{ts.TotalDays.ConvertType(defValue: 0)} 天 {ts.Hours.ConvertType(defValue: 0)} 小时";

            if (ts.TotalHours >= 1) return $"{ts.TotalHours.ConvertType(defValue: 0)} 小时 {ts.Minutes.ConvertType(defValue: 0)} 分";

            if (ts.TotalMinutes >= 1) return $"{ts.TotalMinutes.ConvertType(defValue: 0)} 分 {ts.Seconds.ConvertType(defValue: 0)} 秒";

            return $"{ts.TotalSeconds.ConvertType(defValue: 0)} 秒";
        }
    }
}