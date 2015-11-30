using System;
using FS.Utils.Common;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    public static class OtherExtend
    {
        /// <summary>
        ///     不足total长度时，向左边补0
        /// </summary>
        /// <param name="sourceValue">要转换的值</param>
        /// <param name="total">补0的长度</param>
        public static string PadLeft<T>(this T sourceValue, int total) where T : struct
        {
            return sourceValue.ToString().PadLeft(total, '0');
        }
        /// <summary>
        ///     不足total长度时，向左边补0
        /// </summary>
        /// <param name="sourceValue">要转换的值</param>
        /// <param name="total">补0的长度</param>
        public static string PadLeft<T>(this T? sourceValue, int total) where T : struct
        {
            return sourceValue.GetValueOrDefault().ToString().PadLeft(total, '0');
        }

        /// <summary>
        ///     不足total长度时，向右边补0
        /// </summary>
        /// <param name="sourceValue">要转换的值</param>
        /// <param name="total">补0的长度</param>
        public static string PadRight<T>(this T sourceValue, int total) where T : struct
        {
            return sourceValue.ToString().PadRight(total, '0');
        }
        /// <summary>
        ///     不足total长度时，向右边补0
        /// </summary>
        /// <param name="sourceValue">要转换的值</param>
        /// <param name="total">补0的长度</param>
        public static string PadRight<T>(this T? sourceValue, int total) where T : struct
        {
            return sourceValue.GetValueOrDefault().ToString().PadRight(total, '0');
        }

        /// <summary>
        ///     不足total长度时，向左边补0
        /// </summary>
        /// <param name="sourceValue">要转换的值</param>
        /// <param name="total">补0的长度</param>
        public static string PadLeft(this string sourceValue, int total)
        {
            return sourceValue.PadLeft(total, '0');
        }
        /// <summary>
        ///     不足total长度时，向右边补0
        /// </summary>
        /// <param name="sourceValue">要转换的值</param>
        /// <param name="total">补0的长度</param>
        public static string PadRight(this string sourceValue, int total)
        {
            return sourceValue.PadRight(total, '0');
        }
    }
}