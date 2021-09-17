using System;
using System.Collections.Concurrent;
using System.Data;
using System.Reflection;
using FS.Utils.Common;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    /// <summary>
    /// 扩展方法类
    /// </summary>
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

        private static readonly ConcurrentDictionary<Type, Type> DicNullableArguments = new();
        /// <summary>
        /// 获取非空类型的真实Type
        /// </summary>
        /// <param name="type">可空类型的Type</param>
        public static Type GetNullableArguments(this Type type)
        {
            Type resultType;
            if (!DicNullableArguments.TryGetValue(type, out resultType))
            {
                resultType = type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(type) : type;
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
                var typeInfo = type.GetTypeInfo();
                var genericArguments = typeInfo.GetGenericArguments();
                resultType = typeInfo.IsGenericType && genericArguments.Length > 0 ? genericArguments[0] : type;
                DicGenericType.TryAdd(type, resultType);
            }
            return resultType;
        }


        /// <summary>
        ///     判断IDataReader是否存在某列
        /// </summary>
        public static bool HaveName(this IDataReader reader, string name)
        {
            for (var i = 0; i < reader.FieldCount; i++) { if (StringHelper.IsEquals(reader.GetName(i), name)) { return true; } }
            return false;
        }
        
        
        /// <summary>
        /// 不要使用.NET CORE 的HashCode
        /// </summary>
        public static int HashCode(this string str)
        {
            unchecked
            {
                var hash1 = (5381 << 16) + 5381;
                var hash2 = hash1;

                for (var i = 0; i < str.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }
    }
}