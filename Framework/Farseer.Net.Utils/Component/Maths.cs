// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2016-08-18 15:39
// ********************************************

using System;

namespace FS.Utils.Component
{
    /// <summary>
    ///     Math的扩展补充
    /// </summary>
    public static class Maths
    {
        /// <summary>
        ///     返回最大的时间
        /// </summary>
        public static DateTime Max(DateTime? val1, DateTime? val2) => (val1.GetValueOrDefault() >= val2.GetValueOrDefault() ? val1 : val2).GetValueOrDefault();

        /// <summary>
        ///     返回最小的时间
        /// </summary>
        public static DateTime Min(DateTime? val1, DateTime? val2) => (val1.GetValueOrDefault() < val2.GetValueOrDefault() ? val1 : val2).GetValueOrDefault();

        /// <summary>
        ///     两个值不相等，并且第二个值不为null，则返回第二值
        /// </summary>
        public static string WhenNotNull(string val1, string val2) => val2 != val1 && !string.IsNullOrEmpty(value: val2) ? val2 : val1;

        /// <summary>
        ///     两个值不相等，并且第二个值大于checkVal，则返回第二值
        /// </summary>
        public static int WhenGreaterThan(int? val1, int? val2, int checkVal = 0) => (val2.GetValueOrDefault() != val1.GetValueOrDefault() && val2.GetValueOrDefault() > checkVal ? val2 : val1).GetValueOrDefault();

        /// <summary>
        ///     两个值不相等，并且第二个值小于checkVal，则返回第二值
        /// </summary>
        public static int WhenLessThan(int? val1, int? val2, int checkVal = 0) => (val2.GetValueOrDefault() != val1.GetValueOrDefault() && val2.GetValueOrDefault() < checkVal ? val2 : val1).GetValueOrDefault();
    }
}