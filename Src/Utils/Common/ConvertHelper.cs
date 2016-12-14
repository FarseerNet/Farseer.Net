using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using FS.Cache;
using FS.Extends;
using FS.Infrastructure;

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
        /// <param name="sourceValue">要转换的源对象</param>
        /// <typeparam name="TReturn">基本类型</typeparam>
        public static TReturn ConvertType<TReturn>(object sourceValue) { return ConvertType(sourceValue, default(TReturn)); }
        /// <summary>
        ///     将对象转换为T类型
        /// </summary>
        /// <param name="sourceValue">要转换的源对象</param>
        /// <param name="defValue">转换失败时，代替的默认值</param>
        /// <typeparam name="TSource">转换前的类型</typeparam>
        /// <typeparam name="TReturn">转换后的类型</typeparam>
        public static TReturn ConvertType<TSource, TReturn>(TSource sourceValue, TReturn defValue)
        {
            if (sourceValue == null) { return defValue; }
            var val = ConvertType((object)sourceValue, typeof(TReturn));
            return val != null ? (TReturn)val : defValue;
        }

        /// <summary>
        ///     将值转换成类型对象的值
        /// </summary>
        /// <param name="sourceValue">原值</param>
        /// <param name="returnType">要转换的对象的类型</param>
        /// <param name="sourceType">原值的类型</param>
        public static object ConvertType(object sourceValue, Type returnType)
        {
            if (sourceValue == null) { return null; }
            // 获取非nullable类型
            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Nullable<>)) { returnType = Nullable.GetUnderlyingType(returnType); }

            var objString = sourceValue.ToString();

            // 对  List 类型处理
            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() != typeof(Nullable<>))
            {
                var returnGenericType = Extend.GetGenericType(returnType);
                // List参数类型
                switch (Type.GetTypeCode(returnGenericType))
                {
                    case TypeCode.Boolean: { return ToList(objString, false); }
                    case TypeCode.DateTime: { return ToList(objString, DateTime.MinValue); }
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Single: { return ToList(objString, 0m); }
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                    case TypeCode.UInt16: { return ToList<ushort>(objString); }
                    case TypeCode.UInt32: { return ToList<uint>(objString); }
                    case TypeCode.UInt64: { return ToList<ulong>(objString); }
                    case TypeCode.Int16: { return ToList<short>(objString); }
                    case TypeCode.Int64: { return ToList<long>(objString); }
                    case TypeCode.Int32: { return ToList(objString, 0); }
                    case TypeCode.Empty:
                    case TypeCode.Char:
                    case TypeCode.String: { return ToList(objString, ""); }
                }
            }

            // 枚举处理
            if (returnType.IsEnum)
            {
                if (sourceValue is bool)
                {
                    sourceValue = ((bool)sourceValue) ? 1 : 0;
                    objString = sourceValue.ToString();
                }
                return sourceValue is int ? Enum.ToObject(returnType, int.Parse(objString)) : Enum.Parse(returnType, objString, true);
            }

            // 简单类型转换
            var returnTypeCode = Type.GetTypeCode(returnType);
            var val = ConvertSimple(sourceValue, objString, returnType, returnTypeCode);
            if (val != null) { return val; }

            try { return Convert.ChangeType(sourceValue, returnType); }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 只做基础类型的转换
        /// </summary>
        private static object ConvertSimple(object sourceValue, string objString, Type returnType, TypeCode returnTypeCode)
        {
            if (string.IsNullOrWhiteSpace(objString)) { return null; }
            
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
                    case TypeCode.UInt64: return ((bool)sourceValue) ? 1 : 0;
                }
            }

            switch (returnTypeCode)
            {
                case TypeCode.Boolean: return !string.IsNullOrWhiteSpace(objString) && (objString == "1" || objString.ToLower().Equals("on") || objString.ToLower().Equals("true"));
                case TypeCode.Char: char outChar; if (char.TryParse(objString, out outChar)) { return outChar; } break;
                case TypeCode.SByte: sbyte outSbyte; if (sbyte.TryParse(objString, out outSbyte)) { return outSbyte; } break;
                case TypeCode.Byte: byte outByte; if (byte.TryParse(objString, out outByte)) { return outByte; } break;
                case TypeCode.Int16: short outInt16; if (short.TryParse(objString, out outInt16)) { return outInt16; } break;
                case TypeCode.UInt16: ushort outUInt16; if (ushort.TryParse(objString, out outUInt16)) { return outUInt16; } break;
                case TypeCode.Int32: int outInt32; if (int.TryParse(objString, out outInt32)) { return outInt32; } break;
                case TypeCode.UInt32: uint outUInt32; if (uint.TryParse(objString, out outUInt32)) { return outUInt32; } break;
                case TypeCode.Int64: long outInt64; if (long.TryParse(objString, out outInt64)) { return outInt64; } break;
                case TypeCode.UInt64: ulong outUInt64; if (ulong.TryParse(objString, out outUInt64)) { return outUInt64; } break;
                case TypeCode.Single: float outSingle; if (float.TryParse(objString, out outSingle)) { return outSingle; } break;
                case TypeCode.Double: double outDouble; if (double.TryParse(objString, out outDouble)) { return outDouble; } break;
                case TypeCode.Decimal: decimal outDecimal; if (decimal.TryParse(objString, out outDecimal)) { return outDecimal; } break;
                case TypeCode.DateTime: DateTime outDateTime; if (DateTime.TryParse(objString, out outDateTime)) { return outDateTime; } break;
                case TypeCode.Empty:
                case TypeCode.String: return objString;
                case TypeCode.Object:
                    {
                        if (returnType == typeof(Guid))
                        {
                            Guid guid;
                            if (Guid.TryParse(objString, out guid)) { return guid; }
                        }
                        break;
                    }
            }
            return null;
        }

        /// <summary>
        ///     判断是否T类型
        /// </summary>
        /// <param name="sourceValue">要判断的对象</param>
        /// <typeparam name="T">基本类型</typeparam>
        public static bool IsType<T>(object sourceValue)
        {
            if (sourceValue == null) { return false; }
            var returnType = typeof(T);
            // 获取非nullable类型
            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Nullable<>)) { returnType = Nullable.GetUnderlyingType(returnType); }
            var sourceType = sourceValue.GetType();

            // 相同类型，则直接返回原型
            if (returnType == sourceType || Type.GetTypeCode(sourceType) == Type.GetTypeCode(returnType)) { return true; }

            // 判断是否为泛型
            if (returnType.IsGenericType) { return sourceValue is T; }
            if (returnType.IsEnum) { return sourceValue is Enum; }

            var objString = sourceValue.ToString();
            var returnTypeCode = Type.GetTypeCode(returnType);
            switch (returnTypeCode)
            {
                case TypeCode.Boolean:
                    {
                        return !string.IsNullOrWhiteSpace(objString) && (objString.Equals("on") || objString == "1" || objString.Equals("true"));
                    }
                case TypeCode.Byte:
                    {
                        byte result;
                        return byte.TryParse(objString, out result);
                    }
                case TypeCode.Char:
                    {
                        char result;
                        return char.TryParse(objString, out result);
                    }
                case TypeCode.DateTime:
                    {
                        DateTime result;
                        return DateTime.TryParse(objString, out result);
                    }
                case TypeCode.Decimal:
                    {
                        decimal result;
                        return decimal.TryParse(objString, out result);
                    }
                case TypeCode.Double:
                    {
                        double result;
                        return double.TryParse(objString, out result);
                    }
                case TypeCode.Int16:
                    {
                        short result;
                        return short.TryParse(objString, out result);
                    }
                case TypeCode.Int32:
                    {
                        int result;
                        return int.TryParse(objString, out result);
                    }
                case TypeCode.Int64:
                    {
                        long result;
                        return long.TryParse(objString, out result);
                    }
                case TypeCode.SByte:
                    {
                        sbyte result;
                        return sbyte.TryParse(objString, out result);
                    }
                case TypeCode.Single:
                    {
                        float result;
                        return float.TryParse(objString, out result);
                    }
                case TypeCode.UInt16:
                    {
                        UInt16 result;
                        return ushort.TryParse(objString, out result);
                    }
                case TypeCode.UInt32:
                    {
                        uint result;
                        return uint.TryParse(objString, out result);
                    }
                case TypeCode.UInt64:
                    {
                        ulong result;
                        return ulong.TryParse(objString, out result);
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

        /// <summary>
        ///     将字符串转换成List型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="splitString">分隔符为NullOrEmpty时，则直接拆份为Char</param>
        /// <param name="defValue">默认值(单项转换失败时，默认值为NullOrEmpty时，则不添加，否则替换为默认值)</param>
        /// <typeparam name="T">基本类型</typeparam>
        public static List<T> ToList<T>(string str, T defValue = default(T), string splitString = ",")
        {
            if (string.IsNullOrWhiteSpace(str)) { return new List<T>(); }
            var returnType = typeof(T);
            var returnTypeCode = Type.GetTypeCode(returnType);
            string[] strArray;
            //判断是否带分隔符，如果没有。则直接拆份单个Char
            if (string.IsNullOrWhiteSpace(splitString))
            {
                strArray = new string[str.Length];
                for (var i = 0; i < str.Length; i++) { strArray[i] = str.Substring(i, 1); }
            }
            else
            {
                strArray = splitString.Length == 1 ? str.Split(splitString[0]) : str.Split(new[] { splitString }, StringSplitOptions.None);
            }
            var lst = new List<T>(strArray.Length);
            foreach (var item in strArray)
            {
                var val = ConvertSimple(item, item, returnType, returnTypeCode);
                lst.Add(val == null ? defValue : (T)val);
            }
            return lst;
        }

        /// <summary>
        ///     查找对象属性值
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="entity">当前实体类</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="defValue">默认值</param>
        public static T GetValue<TEntity, T>(TEntity entity, string propertyName, T defValue = default(T)) where TEntity : class
        {
            if (entity == null) { return defValue; }
            foreach (var property in entity.GetType().GetProperties())
            {
                if (property.Name != propertyName) { continue; }
                if (!property.CanRead) { return defValue; }
                return ConvertType(PropertyGetCacheManger.Cache(property, entity), defValue);
            }
            return defValue;
        }

        /// <summary>
        ///     比较两者是否相等，不考虑大小写,两边空格
        /// </summary>
        /// <param name="str">对比一</param>
        /// <param name="str2">对比二</param>
        /// <returns></returns>
        public static bool IsEquals(string str, string str2)
        {
            if (str == str2) { return true; }
            if (str == null || str2 == null) { return false; }
            if (str.Trim().Length != str2.Trim().Length) { return false; }
            return String.Compare(str.Trim(), str2.Trim(), StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        ///     将List转换成字符串
        /// </summary>
        /// <param name="lst">要拼接的LIST</param>
        /// <param name="sign">分隔符</param>
        public static string ToString(IEnumerable lst, string sign = ",")
        {
            if (lst == null) { return String.Empty; }
            var sb = new StringBuilder();
            foreach (var item in lst) { sb.Append(item + sign); }
            return sb.Length > 0 ? sb.Remove(sb.Length - sign.Length, sign.Length).ToString() : String.Empty;
        }
        public static MapingData[] DataReaderToDictionary(IDataReader reader)
        {
            // 获取数据的列并转成字典
            var dicColumns = new MapingData[reader.FieldCount];
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var mapData = dicColumns[i] = new MapingData();
                mapData.ColumnName = reader.GetName(i);
                mapData.DataList = new List<object>();
            }

            // 遍历行
            while (reader.Read())
            {
                // 当前记录的所有字段的值
                var arrVals = new object[reader.FieldCount];
                reader.GetValues(arrVals);
                // 遍历列
                for (var i = 0; i < dicColumns.Length; i++)
                {
                    dicColumns[i].DataList.Add(arrVals[i]);
                }
            }
            reader.Close();
            return dicColumns;
        }
        public static MapingData[] DataTableToDictionary(DataTable dt)
        {
            // 获取数据的列并转成字典
            var cols = dt.Columns;
            var dicColumns = new MapingData[cols.Count];
            for (var i = 0; i < cols.Count; i++)
            {
                var mapData = dicColumns[i] = new MapingData();
                mapData.ColumnName = cols[i].ColumnName;
                mapData.DataList = new List<object>(dt.Rows.Count);
            }
            // DataRowCollection转成DataRow[]
            var rows = new DataRow[dt.Rows.Count];
            dt.Rows.CopyTo(rows, 0);

            // 遍历DataRow
            foreach (var dr in rows)
            {
                var arrVals = dr.ItemArray;
                // 遍历列
                for (int i = 0; i < dicColumns.Length; i++)
                {
                    dicColumns[i].DataList.Add(arrVals[i]);
                }
            }
            return dicColumns;
        }
    }
}