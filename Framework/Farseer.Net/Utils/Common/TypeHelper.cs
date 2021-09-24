// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-03-02 18:06
// ********************************************

using System;
using System.Collections.Concurrent;

namespace FS.Utils.Common
{
    /// <summary>
    ///     类型帮助器
    /// </summary>
    public class TypeHelper
    {
        private static readonly ConcurrentDictionary<Type, Type> DicGenericType       = new();
        private static readonly ConcurrentDictionary<Type, Type> DicNullableArguments = new();

        /// <summary>
        ///     获取List的元素类型
        /// </summary>
        /// <param name="type"> 可空类型的Type </param>
        public static Type GetGenericType(Type type)
        {
            Type resultType;
            if (!DicGenericType.TryGetValue(key: type, value: out resultType))
            {
                var genericArguments = type.GetGenericArguments();
                resultType = type.IsGenericType && genericArguments.Length > 0 ? genericArguments[0] : type;
                DicGenericType.TryAdd(key: type, value: resultType);
            }

            return resultType;
        }

        /// <summary>
        ///     获取非空类型的真实Type
        /// </summary>
        /// <param name="type"> 可空类型的Type </param>
        public static Type GetNullableArguments(Type type)
        {
            Type resultType;
            if (!DicNullableArguments.TryGetValue(key: type, value: out resultType))
            {
                resultType = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(nullableType: type) : type;
                DicNullableArguments.TryAdd(key: type, value: resultType);
            }

            return resultType;
        }

        /// <summary>
        ///     判断是否T类型
        /// </summary>
        /// <param name="sourceValue"> 要判断的对象 </param>
        /// <typeparam name="T"> 基本类型 </typeparam>
        public static bool IsType<T>(object sourceValue)
        {
            if (sourceValue == null) return false;
            var returnType = typeof(T);
            // 获取非nullable类型
            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Nullable<>)) returnType = Nullable.GetUnderlyingType(nullableType: returnType);
            var sourceType                                                                                          = sourceValue.GetType();

            // 相同类型，则直接返回原型
            if (returnType == sourceType || Type.GetTypeCode(type: sourceType) == Type.GetTypeCode(type: returnType)) return true;

            // 判断是否为泛型
            if (returnType.IsGenericType) return sourceValue is T;
            if (returnType.IsEnum) return sourceValue is Enum;

            var objString      = sourceValue.ToString();
            var returnTypeCode = Type.GetTypeCode(type: returnType);
            switch (returnTypeCode)
            {
                case TypeCode.Boolean:
                {
                    return !string.IsNullOrWhiteSpace(value: objString) && (objString.Equals(value: "on") || objString == "1" || objString.Equals(value: "true"));
                }
                case TypeCode.Byte:
                {
                    byte result;
                    return byte.TryParse(s: objString, result: out result);
                }
                case TypeCode.Char:
                {
                    char result;
                    return char.TryParse(s: objString, result: out result);
                }
                case TypeCode.DateTime:
                {
                    DateTime result;
                    return DateTime.TryParse(s: objString, result: out result);
                }
                case TypeCode.Decimal:
                {
                    decimal result;
                    return decimal.TryParse(s: objString, result: out result);
                }
                case TypeCode.Double:
                {
                    double result;
                    return double.TryParse(s: objString, result: out result);
                }
                case TypeCode.Int16:
                {
                    short result;
                    return short.TryParse(s: objString, result: out result);
                }
                case TypeCode.Int32:
                {
                    int result;
                    return int.TryParse(s: objString, result: out result);
                }
                case TypeCode.Int64:
                {
                    long result;
                    return long.TryParse(s: objString, result: out result);
                }
                case TypeCode.SByte:
                {
                    sbyte result;
                    return sbyte.TryParse(s: objString, result: out result);
                }
                case TypeCode.Single:
                {
                    float result;
                    return float.TryParse(s: objString, result: out result);
                }
                case TypeCode.UInt16:
                {
                    ushort result;
                    return ushort.TryParse(s: objString, result: out result);
                }
                case TypeCode.UInt32:
                {
                    uint result;
                    return uint.TryParse(s: objString, result: out result);
                }
                case TypeCode.UInt64:
                {
                    ulong result;
                    return ulong.TryParse(s: objString, result: out result);
                }
                case TypeCode.Empty:
                case TypeCode.String:
                {
                    return true;
                }
                case TypeCode.Object:
                {
                    break;
                }
            }

            return sourceType == returnType;
        }
    }
}