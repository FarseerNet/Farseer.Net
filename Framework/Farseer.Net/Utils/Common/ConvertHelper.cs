using System;
using System.Reflection;
using FS.Extends;

namespace FS.Utils.Common
{
    /// <summary>
    ///     类型转换器
    /// </summary>
    public static class ConvertHelper
    {
        /// <summary>
        ///     将对象转换为T类型
        /// </summary>
        /// <param name="sourceValue"> 要转换的源对象 </param>
        /// <typeparam name="TReturn"> 基本类型 </typeparam>
        public static TReturn ConvertType<TReturn>(object sourceValue) => ConvertType(sourceValue: sourceValue, defValue: default(TReturn));

        /// <summary>
        ///     将对象转换为T类型
        /// </summary>
        /// <param name="sourceValue"> 要转换的源对象 </param>
        /// <param name="defValue"> 转换失败时，代替的默认值 </param>
        /// <typeparam name="TSource"> 转换前的类型 </typeparam>
        /// <typeparam name="TReturn"> 转换后的类型 </typeparam>
        public static TReturn ConvertType<TSource, TReturn>(TSource sourceValue, TReturn defValue)
        {
            if (sourceValue == null) return defValue;
            var val = ConvertType(sourceValue: sourceValue, returnType: typeof(TReturn));
            return val != null ? (TReturn)val : defValue;
        }

        /// <summary>
        ///     将值转换成类型对象的值
        /// </summary>
        /// <param name="sourceValue"> 原值 </param>
        /// <param name="returnType"> 要转换的对象的类型 </param>
        public static object ConvertType(object sourceValue, Type returnType)
        {
            if (sourceValue == null) return null;
            // 获取非nullable类型
            if (returnType.GetTypeInfo().IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Nullable<>)) returnType = Nullable.GetUnderlyingType(nullableType: returnType);

            var objString = sourceValue.ToString();

            // 对  List 类型处理
            if (returnType.GetTypeInfo().IsGenericType && returnType.GetGenericTypeDefinition() != typeof(Nullable<>))
            {
                var returnGenericType = returnType.GetGenericType();
                // List参数类型
                switch (Type.GetTypeCode(type: returnGenericType))
                {
                    case TypeCode.Boolean:
                    {
                        return StringHelper.ToList(str: objString, defValue: false);
                    }
                    case TypeCode.DateTime:
                    {
                        return StringHelper.ToList(str: objString, defValue: DateTime.MinValue);
                    }
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Single:
                    {
                        return StringHelper.ToList(str: objString, defValue: 0m);
                    }
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                    case TypeCode.UInt16:
                    {
                        return StringHelper.ToList<ushort>(str: objString);
                    }
                    case TypeCode.UInt32:
                    {
                        return StringHelper.ToList<uint>(str: objString);
                    }
                    case TypeCode.UInt64:
                    {
                        return StringHelper.ToList<ulong>(str: objString);
                    }
                    case TypeCode.Int16:
                    {
                        return StringHelper.ToList<short>(str: objString);
                    }
                    case TypeCode.Int64:
                    {
                        return StringHelper.ToList<long>(str: objString);
                    }
                    case TypeCode.Int32:
                    {
                        return StringHelper.ToList(str: objString, defValue: 0);
                    }
                    case TypeCode.Empty:
                    case TypeCode.Char:
                    case TypeCode.String:
                    {
                        return StringHelper.ToList(str: objString, defValue: "");
                    }
                }
            }

            // 枚举处理
            if (returnType.GetTypeInfo().IsEnum)
            {
                if (sourceValue is bool)
                {
                    sourceValue = (bool)sourceValue ? 1 : 0;
                    objString   = sourceValue.ToString();
                }

                return sourceValue is int ? Enum.ToObject(enumType: returnType, value: int.Parse(s: objString)) : Enum.Parse(enumType: returnType, value: objString, ignoreCase: true);
            }

            // 简单类型转换
            var returnTypeCode = Type.GetTypeCode(type: returnType);
            var val            = ConvertSimple(sourceValue: sourceValue, objString: objString, returnType: returnType, returnTypeCode: returnTypeCode);
            if (val != null) return val;

            try
            {
                return Convert.ChangeType(value: sourceValue, conversionType: returnType);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     只做基础类型的转换
        /// </summary>
        internal static object ConvertSimple(object sourceValue, string objString, Type returnType, TypeCode returnTypeCode)
        {
            if (string.IsNullOrWhiteSpace(value: objString)) return null;

            //  bool转数字
            if (sourceValue is bool)
            {
                switch (returnTypeCode)
                {
                    case TypeCode.Byte:
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.SByte:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64: return (bool)sourceValue ? 1 : 0;
                }
            }

            switch (returnTypeCode)
            {
                case TypeCode.Boolean: return !string.IsNullOrWhiteSpace(value: objString) && (objString == "1" || objString.ToLower().Equals(value: "on") || objString.ToLower().Equals(value: "true"));
                case TypeCode.Char:
                    char outChar;
                    if (char.TryParse(s: objString, result: out outChar)) return outChar;
                    break;
                case TypeCode.SByte:
                    sbyte outSbyte;
                    if (sbyte.TryParse(s: objString, result: out outSbyte)) return outSbyte;
                    break;
                case TypeCode.Byte:
                    byte outByte;
                    if (byte.TryParse(s: objString, result: out outByte)) return outByte;
                    break;
                case TypeCode.Int16:
                    short outInt16;
                    if (short.TryParse(s: objString, result: out outInt16)) return outInt16;
                    break;
                case TypeCode.UInt16:
                    ushort outUInt16;
                    if (ushort.TryParse(s: objString, result: out outUInt16)) return outUInt16;
                    break;
                case TypeCode.Int32:
                    int outInt32;
                    if (int.TryParse(s: objString, result: out outInt32)) return outInt32;
                    break;
                case TypeCode.UInt32:
                    uint outUInt32;
                    if (uint.TryParse(s: objString, result: out outUInt32)) return outUInt32;
                    break;
                case TypeCode.Int64:
                    long outInt64;
                    if (long.TryParse(s: objString, result: out outInt64)) return outInt64;
                    break;
                case TypeCode.UInt64:
                    ulong outUInt64;
                    if (ulong.TryParse(s: objString, result: out outUInt64)) return outUInt64;
                    break;
                case TypeCode.Single:
                    float outSingle;
                    if (float.TryParse(s: objString, result: out outSingle)) return outSingle;
                    break;
                case TypeCode.Double:
                    double outDouble;
                    if (double.TryParse(s: objString, result: out outDouble)) return outDouble;
                    break;
                case TypeCode.Decimal:
                    decimal outDecimal;
                    if (decimal.TryParse(s: objString, result: out outDecimal)) return outDecimal;
                    break;
                case TypeCode.DateTime:
                    DateTime outDateTime;
                    if (DateTime.TryParse(s: objString, result: out outDateTime)) return outDateTime;
                    break;
                case TypeCode.Empty:
                case TypeCode.String: return objString;
                case TypeCode.Object:
                {
                    if (returnType == typeof(Guid))
                    {
                        Guid guid;
                        if (Guid.TryParse(input: objString, result: out guid)) return guid;
                    }

                    break;
                }
            }

            return null;
        }
    }
}