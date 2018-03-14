using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace

namespace FS.Extends
{
    /// <summary>
    ///     其它扩展，夫归类的扩展
    /// </summary>
    public static partial class UtilsExtend
    {
        /// <summary>
        ///     获取短日期
        /// </summary>
        /// <param name="time">时间对象</param>
        /// <param name="format">格式化时间格式</param>
        public static string ToShortString(this DateTime time, string format = "yyyy-MM-dd")
        {
            var date = time.ToString(format);
            return date.Contains("0001") || date.Contains("1900") ? string.Empty : date;
        }

        /// <summary>
        ///     获取短日期
        /// </summary>
        /// <param name="time">时间对象</param>
        /// <param name="format">格式化时间格式</param>
        public static string ToShortString(this DateTime? time, string format = "yyyy-MM-dd")
        {
            return time.GetValueOrDefault().ToShortString(format);
        }

        /// <summary>
        ///     获取短日期
        /// </summary>
        /// <param name="time">时间对象</param>
        /// <param name="addDay">添加天数</param>
        public static DateTime ToShortDate(this DateTime time, double addDay)
        {
            return time.AddDays(addDay).Date;
        }

        /// <summary>
        ///     获取短日期
        /// </summary>
        /// <param name="time">时间对象</param>
        public static DateTime ToShortDate(this DateTime time)
        {
            return time.Date;
        }

        /// <summary>
        ///     获取短日期
        /// </summary>
        /// <param name="time">时间对象</param>
        public static DateTime ToShortDate(this DateTime? time)
        {
            return time.GetValueOrDefault().Date;
        }

        /// <summary>
        ///     获取短日期
        /// </summary>
        /// <param name="time">时间对象</param>
        /// <param name="addDay">添加天数</param>
        public static DateTime ToShortDate(this DateTime? time, double addDay)
        {
            return time.GetValueOrDefault().AddDays(addDay).Date;
        }

        /// <summary>
        ///     获取长日期
        /// </summary>
        /// <param name="time">时间对象</param>
        /// <param name="format">格式化时间格式</param>
        public static string ToLongString(this DateTime time, string format = "yyyy-MM-dd HH:mm")
        {
            var date = time.ToString(format);
            return date.Contains("0001") || date.Contains("1900") ? string.Empty : date;
        }

        /// <summary>
        ///     获取长日期
        /// </summary>
        /// <param name="time">时间对象</param>
        /// <param name="format">格式化时间格式</param>
        public static string ToLongString(this DateTime? time, string format = "yyyy-MM-dd HH:mm")
        {
            return time.GetValueOrDefault().ToLongString(format);
        }

        /// <summary>
        ///     获取当前日期的第一天
        /// </summary>
        /// <param name="dt">时间</param>
        /// <returns></returns>
        public static DateTime GetFirstDate(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1);
        }

        /// <summary>
        ///     返回当前日期的最后一天
        /// </summary>
        /// <param name="dt">时间</param>
        public static DateTime GetLastDate(this DateTime dt)
        {
            return dt.GetFirstDate().AddMonths(1).AddDays(-1);
        }

        /// <summary>
        ///     返回当前日期的最后一天
        /// </summary>
        /// <param name="dt">时间</param>
        public static int GetLastDay(this DateTime dt)
        {
            return dt.GetLastDate().Day;
        }

        /// <summary>
        ///     返回当前的所有天数
        /// </summary>
        /// <param name="dt">时间</param>
        public static List<int> GetAllDay(this DateTime dt)
        {
            var lst = new List<int>();
            for (var i = 1; i <= dt.GetLastDay(); i++) { lst.Add(i); }
            return lst;
        }

        /// <summary>
        ///     日期转换成时间戳
        /// </summary>
        /// <param name="dt">时间</param>
        public static long ToTimestamp(this DateTime dt)
        {
            var time = Convert.ToDateTime("1970-1-1 08:00:00");
            var ts = dt.Subtract(time);

            return (long)ts.TotalSeconds;
        }
    }
}