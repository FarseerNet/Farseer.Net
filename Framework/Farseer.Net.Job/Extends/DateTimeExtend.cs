using System;

// ReSharper disable once CheckNamespace

namespace FS.Extends
{
    /// <summary>
    ///     其它扩展，夫归类的扩展
    /// </summary>
    public static partial class UtilsExtend
    {
        /// <summary>
        ///     日期转换成时间戳（秒）
        /// </summary>
        /// <param name="dt">时间</param>
        public static long ToTimestamp(this DateTime dt)
        {
            var time = new DateTime(1970, 1, 1).Add(TimeZoneInfo.Local.BaseUtcOffset);
            var ts   = dt.Subtract(time);

            return (long)ts.TotalSeconds;
        }

        /// <summary>
        ///     日期转换成时间戳（毫秒）
        /// </summary>
        /// <param name="dt">时间</param>
        public static long ToTimestamps(this DateTime dt)
        {
            var time = new DateTime(1970, 1, 1).Add(TimeZoneInfo.Local.BaseUtcOffset);
            var ts   = dt.Subtract(time);

            return (long)ts.TotalMilliseconds;
        }
        
        /// <summary>
        /// 时间戳转化成为本地时间（秒）
        /// </summary>
        /// <param name="timestamp">时间戳（秒）</param>
        /// <returns></returns>
        public static DateTime ToTimestamp(this long timestamp)
        {
            return new DateTime(1970, 1, 1).Add(TimeZoneInfo.Local.BaseUtcOffset).AddSeconds(timestamp);
        }

        /// <summary>
        /// 时间戳转化成为本地时间（毫秒）
        /// </summary>
        /// <param name="timestamp">时间戳（毫秒）</param>
        /// <returns></returns>
        public static DateTime ToTimestamps(this long timestamp)
        {
            return new DateTime(1970, 1, 1).Add(TimeZoneInfo.Local.BaseUtcOffset).AddMilliseconds(timestamp);
        }
    }
}