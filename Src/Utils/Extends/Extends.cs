using System;
using System.Data;
using FS.Utils.Common;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    public static partial class Extend
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

        /// <summary>
        /// 获取非空类型的真实Type
        /// </summary>
        /// <param name="type">可空类型的Type</param>
        public static Type GetNullableArguments(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) { return Nullable.GetUnderlyingType(type); }
            return type;
        }

        /// <summary>
        /// 获取List的元素类型
        /// </summary>
        /// <param name="type">可空类型的Type</param>
        public static Type GetGenericType(this Type type)
        {
            var genericArguments = type.GetGenericArguments();
            if (type.IsGenericType && genericArguments.Length > 0) { return genericArguments[0]; }
            return type;
        }

        /// <summary>
        ///     判断IDataReader是否存在某列
        /// </summary>
        public static bool HaveName(this IDataReader reader, string name)
        {
            for (var i = 0; i < reader.FieldCount; i++) { if (ConvertHelper.IsEquals(reader.GetName(i), name)) { return true; } }
            return false;
        }
    }
}