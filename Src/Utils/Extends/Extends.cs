using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        private static readonly ConcurrentDictionary<Type, Type> DicNullableArguments = new ConcurrentDictionary<Type, Type>();
        /// <summary>
        /// 获取非空类型的真实Type
        /// </summary>
        /// <param name="type">可空类型的Type</param>
        public static Type GetNullableArguments(this Type type)
        {
            Type resultType;
            if (!DicNullableArguments.TryGetValue(type, out resultType))
            {
                resultType = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(type) : type;
                DicNullableArguments.TryAdd(type, resultType);
            }
            return resultType;
        }

        private static readonly ConcurrentDictionary<Type, Type> DicGenericType = new ConcurrentDictionary<Type, Type>();
        /// <summary>
        /// 获取List的元素类型
        /// </summary>
        /// <param name="type">可空类型的Type</param>
        public static Type GetGenericType(this Type type)
        {
            Type resultType;
            if (!DicGenericType.TryGetValue(type, out resultType))
            {
                var genericArguments = type.GetGenericArguments();
                resultType = type.IsGenericType && genericArguments.Length > 0 ? genericArguments[0] : type;
                DicGenericType.TryAdd(type, resultType);
            }
            return resultType;
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