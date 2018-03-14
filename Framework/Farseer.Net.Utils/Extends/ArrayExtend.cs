using System;
using System.Text;
using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace

namespace FS.Extends
{
    public static partial class UtilsExtend
    {
        /// <summary>
        ///     指定清除标签的内容
        /// </summary>
        /// <param name="strs">内容</param>
        /// <param name="tag">标签</param>
        /// <param name="options">选项</param>
        public static string[] Remove(this string[] strs, string tag, RegexOptions options = RegexOptions.None)
        {
            for (var i = 0; i < strs.Length; i++) { strs[i] = strs[i].Remove(tag, options); }
            return strs;
        }

        /// <summary>
        ///     将任何数组转换成用符号连接的字符串
        /// </summary>
        /// <param name="obj">任何对象</param>
        /// <param name="func">传入要在转换过程中要执行的方法</param>
        /// <param name="sign">分隔符</param>
        /// <typeparam name="T">基本对象</typeparam>
        public static string ToString<T>(this T[] obj, string sign = ",", Func<T, string> func = null)
        {
            if (obj == null || obj.Length == 0) { return string.Empty; }

            var str = new StringBuilder();
            foreach (var t in obj)
            {
                if (func == null) { str.Append(sign + t); }
                else
                { str.Append(sign + func(t)); }
            }

            return str.ToString().Substring(sign.Length);
        }
    }
}