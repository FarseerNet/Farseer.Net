using System;

// ReSharper disable once CheckNamespace

namespace FS.Extends
{
    /// <summary>
    ///     其它扩展，夫归类的扩展
    /// </summary>
    public static partial class Extend
    {
        /// <summary>
        ///     日期转换成时间戳（秒）
        /// </summary>
        /// <param name="dt"> 时间 </param>
        public static long ToTimestamp(this DateTime dt)
        {
            var time = new DateTime(year: 1970, month: 1, day: 1).Add(value: TimeZoneInfo.Local.BaseUtcOffset);
            var ts   = dt.Subtract(value: time);

            return (long)ts.TotalSeconds;
        }

        /// <summary>
        ///     日期转换成时间戳（毫秒，13位）
        /// </summary>
        /// <param name="dt"> 时间 </param>
        public static long ToTimestamps(this DateTime dt)
        {
            var time = new DateTime(year: 1970, month: 1, day: 1).Add(value: TimeZoneInfo.Local.BaseUtcOffset);
            var ts   = dt.Subtract(value: time);

            return (long)ts.TotalMilliseconds;
        }

        /// <summary>
        ///     时间戳转化成为本地时间（秒）
        /// </summary>
        /// <param name="timestamp"> 时间戳（秒） </param>
        /// <returns> </returns>
        public static DateTime ToTimestamp(this long timestamp) => new DateTime(year: 1970, month: 1, day: 1).Add(value: TimeZoneInfo.Local.BaseUtcOffset).AddSeconds(value: timestamp);

        /// <summary>
        ///     时间戳转化成为本地时间（毫秒）
        /// </summary>
        /// <param name="timestamp"> 时间戳（毫秒） </param>
        /// <returns> </returns>
        public static DateTime ToTimestamps(this long timestamp) => new DateTime(year: 1970, month: 1, day: 1).Add(value: TimeZoneInfo.Local.BaseUtcOffset).AddMilliseconds(value: timestamp);
    }
}