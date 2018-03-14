using System;
using System.Text;

// ReSharper disable once CheckNamespace

namespace FS.Extends
{
    /// <summary>
    ///     格式化变量
    /// </summary>
    public static partial class UtilsExtend
    {
        /// <summary>
        ///     数字格式化,将转换成1000,10
        /// </summary>
        public static string Format(this int number, bool isHaveTag = true, int len = 2)
        {
            var str = String.Empty;
            if (isHaveTag) { str = "￥"; }
            return str + number.ToString($"n{len}");
        }

        /// <summary>
        ///     数字格式化,将转换成1000,10
        /// </summary>
        public static string Format(this int? number, bool isHaveTag = true, int len = 2)
        {
            return Format(number.GetValueOrDefault(), isHaveTag, len);
        }

        /// <summary>
        ///     数字格式化,将转换成1000,10
        /// </summary>
        public static string Format(this uint number, bool isHaveTag = true, int len = 2)
        {
            var str = String.Empty;
            if (isHaveTag) { str = "￥"; }
            return str + number.ToString($"n{len}");
        }

        /// <summary>
        ///     数字格式化,将转换成1000,10
        /// </summary>
        public static string Format(this uint? number, bool isHaveTag = true, int len = 2)
        {
            return Format(number.GetValueOrDefault(), isHaveTag, len);
        }

        /// <summary>
        ///     数字格式化,将转换成1000,10
        /// </summary>
        public static string Format(this decimal number, bool isHaveTag = true, int len = 2)
        {
            var str = String.Empty;
            if (isHaveTag) { str = "￥"; }
            return str + number.ToString($"n{len}");
        }

        /// <summary>
        ///     数字格式化,将转换成1000,10
        /// </summary>
        public static string Format(this decimal? number, bool isHaveTag = true, int len = 2)
        {
            return Format(number.GetValueOrDefault(), isHaveTag, len);
        }

        /// <summary>
        ///     数字格式化,将转换成1000,10
        /// </summary>
        public static string Format(this double number, bool isHaveTag = true, int len = 2)
        {
            var str = String.Empty;
            if (isHaveTag) { str = "￥"; }
            return str + number.ToString($"n{len}");
        }

        /// <summary>
        ///     数字格式化,将转换成1000,10
        /// </summary>
        public static string Format(this double? number, bool isHaveTag = true, int len = 2)
        {
            return Format(number.GetValueOrDefault(), isHaveTag, len);
        }

        /// <summary>
        ///     格式化字节数字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string FormatBytesStr(int bytes)
        {
            if (bytes > 1073741824) { return ((double)(bytes / 1073741824)).ToString("0") + "G"; }
            if (bytes > 1048576) { return ((double)(bytes / 1048576)).ToString("0") + "M"; }
            if (bytes > 1024) { return ((double)(bytes / 1024)).ToString("0") + "K"; }
            return bytes.ToString() + "Bytes";
        }

        /// <summary>
        ///     当NullOrEmpty，用新的字符串代替，否则用原来的。
        /// </summary>
        /// <param name="obj">要检测的值</param>
        /// <param name="newString">要替换的新字符串</param>
        public static string IsNullOrEmpty<T>(this T? obj, string newString) where T : struct
        {
            return (obj == null || obj.ToString().IsNullOrEmpty()) ? newString : obj.ToString();
        }

        /// <summary>
        ///     当不为NullOrEmpty，用新的字符串代替，否则用原来的。
        /// </summary>
        /// <param name="obj">要检测的值</param>
        /// <param name="newString">要替换的新字符串</param>
        public static string IsNotNullOrEmpty<T>(this T? obj, string newString) where T : struct
        {
            return (obj == null || obj.ToString().IsNullOrEmpty()) ? obj.ToString() : newString;
        }

        /// <summary>
        /// 根据给定的范围，超出则返回notRangeVal,则否返回inRangeVal
        /// </summary>
        /// <param name="t">要判断的值</param>
        /// <param name="min">范围值</param>
        /// <param name="max">范围值</param>
        /// <param name="notRangeVal">超出则返回notRangeVal</param>
        /// <param name="inRangeVal">没有超出则返回inRangeVal</param>
        public static int Range(this int t, int min, int max, int notRangeVal, int inRangeVal) => t >= min && t <= max ? inRangeVal : notRangeVal;
        /// <summary>
        /// 根据给定的范围，超出则返回notRangeVal,则否返回inRangeVal
        /// </summary>
        /// <param name="t">要判断的值</param>
        /// <param name="min">范围值</param>
        /// <param name="max">范围值</param>
        /// <param name="notRangeVal">超出则返回notRangeVal</param>
        /// <param name="inRangeVal">没有超出则返回inRangeVal</param>
        public static int Range(this int? t, int min, int max, int notRangeVal, int inRangeVal) => Range(t.GetValueOrDefault(), min, max, notRangeVal, inRangeVal);
        /// <summary>
        /// 根据给定的范围，超出则返回notRangeVal
        /// </summary>
        /// <param name="t">要判断的值</param>
        /// <param name="min">范围值</param>
        /// <param name="max">范围值</param>
        /// <param name="notRangeVal">超出则返回notRangeVal</param>
        public static int Range(this int t, int min, int max, int notRangeVal) => t >= min && t <= max ? t : notRangeVal;
        /// <summary>
        /// 根据给定的范围，超出则返回notRangeVal
        /// </summary>
        /// <param name="t">要判断的值</param>
        /// <param name="min">范围值</param>
        /// <param name="max">范围值</param>
        /// <param name="notRangeVal">超出则返回notRangeVal</param>
        public static int Range(this int? t, int min, int max, int notRangeVal) => Range(t.GetValueOrDefault(), min, max, notRangeVal, t.GetValueOrDefault());

        /// <summary>
        /// 根据给定的范围，是否在Min和Max范围内
        /// </summary>
        /// <param name="t">要判断的值</param>
        /// <param name="min">范围值</param>
        /// <param name="max">范围值</param>
        public static bool IsRange(this int t, int min, int max) => t >= min && t <= max;
        /// <summary>
        /// 根据给定的范围，是否在Min和Max范围内
        /// </summary>
        /// <param name="t">要判断的值</param>
        /// <param name="min">范围值</param>
        /// <param name="max">范围值</param>
        public static bool IsRange(this int? t, int min, int max) => IsRange(t.GetValueOrDefault(), min, max);

        /// <summary>
        /// 迭代指定次数标签
        /// </summary>
        public static string ForEach(this int t, string tag)
        {
            var str = new StringBuilder();
            for (var i = 0; i < t; i++) { str.Append(tag); }
            return str.ToString();
        }
        /// <summary>
        /// 迭代指定次数标签
        /// </summary>
        public static void ForEach(this int t, Action<int> act)
        {
            for (var i = 0; i < t; i++) { act(i); }
        }
    }
}