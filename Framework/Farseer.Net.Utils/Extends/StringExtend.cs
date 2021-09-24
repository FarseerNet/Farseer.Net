using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using FS.Utils.Common;

// ReSharper disable once CheckNamespace

namespace FS.Extends
{
    /// <summary>
    ///     String扩展工具
    /// </summary>
    public static partial class UtilsExtend
    {
        /// <summary>
        ///     将字符串转换成List型
        /// </summary>
        /// <param name="str"> 要转换的字符串 </param>
        /// <param name="splitString"> 分隔符为NullOrEmpty时，则直接拆份为Char </param>
        /// <param name="defValue"> 默认值(单项转换失败时，默认值为NullOrEmpty时，则不添加，否则替换为默认值) </param>
        /// <typeparam name="T"> 基本类型 </typeparam>
        public static List<T> ToList<T>(this string str, T defValue, string splitString = ",")
        {
            return StringHelper.ToList(str, defValue, splitString);
        }

        /// <summary>
        ///     比较两者是否相等，不考虑大小写,两边空格
        /// </summary>
        /// <param name="str"> 对比一 </param>
        /// <param name="str2"> 对比二 </param>
        /// <returns> </returns>
        public static bool IsEquals(this string str, string str2)
        {
            return StringHelper.IsEquals(str, str2);
        }

        /// <summary>
        ///     直接返回分隔后的元素项
        /// </summary>
        /// <param name="arr"> 要分隔的字符串 </param>
        /// <param name="tag"> 分隔的标识 </param>
        /// <param name="index"> 元素下标 </param>
        /// <returns> </returns>
        public static string Split(this string arr, string tag, int index)
        {
            var arrs = arr.Split(new string[1] { tag }, StringSplitOptions.None);
            if (arrs.Length <= index)
            {
                return string.Empty;
            }

            return arrs[index];
        }

        /// <summary>
        ///     直接返回分隔后的元素项
        /// </summary>
        /// <param name="arr"> 要分隔的字符串 </param>
        /// <param name="tag"> 分隔的标识 </param>
        /// <param name="index"> 元素下标 </param>
        /// <returns> </returns>
        public static string Split(this string arr, char tag, int index)
        {
            var arrs = arr.Split(tag);
            if (arrs.Length <= index)
            {
                return string.Empty;
            }

            return arrs[index];
        }

        /// <summary>
        ///     删除指定最后的字符串
        /// </summary>
        /// <param name="str"> 要转换的字符串 </param>
        /// <param name="strChar"> 要删除的字符串 </param>
        public static string DelEndOf(this string str, string strChar)
        {
            if (string.IsNullOrWhiteSpace(str) || string.IsNullOrWhiteSpace(strChar))
            {
                return str;
            }

            var strLower     = str.ToLower();
            var strCharLower = strChar.ToLower();

            if (strLower.EndsWith(strCharLower))
            {
                var index = strLower.LastIndexOf(strCharLower, StringComparison.Ordinal);
                if (index > -1)
                {
                    str = str.Substring(0, index);
                }
            }

            return str;
        }

        /// <summary>
        ///     删除指定最后的字符串(直到找到为止)
        /// </summary>
        /// <param name="str"> 要转换的字符串 </param>
        /// <param name="strChar"> 要删除的字符串 </param>
        public static string DelLastOf(this string str, string strChar)
        {
            if (string.IsNullOrWhiteSpace(str) || string.IsNullOrWhiteSpace(strChar))
            {
                return str;
            }

            var index = str.LastIndexOf(strChar, StringComparison.Ordinal);
            return index > -1 ? str.Substring(0, index) : str;
        }

        /// <summary>
        ///     从字符串的指定位置截取指定长度的子字符串
        /// </summary>
        /// <param name="str"> 原字符串 </param>
        /// <param name="startIndex"> 子字符串的起始位置 </param>
        /// <param name="length"> 子字符串的长度(负数，则获取全部) </param>
        public static string SubString(this string str, int startIndex, int length = 0)
        {
            if (startIndex < 0)
            {
                return str;
            }

            if (str.Length <= startIndex)
            {
                return string.Empty;
            }

            if (length < 1)
            {
                return str.Substring(startIndex);
            }

            return str.Length < startIndex + length ? str.Substring(startIndex) : str.Substring(startIndex, length);
        }

        /// <summary>
        ///     截取到tag字符串
        /// </summary>
        /// <param name="str"> 原字符串 </param>
        /// <param name="tag"> 截取到的字符串 </param>
        public static string SubString(this string str, string tag)
        {
            return str.IndexOf(tag) > -1 ? str.Substring(0, str.IndexOf(tag)) : str;
        }

        /// <summary>
        ///     将字符串转换成Array型
        /// </summary>
        /// <param name="str"> 要转换的字符串 </param>
        /// <param name="splitString"> 分隔符为NullOrEmpty时，则直接拆份为Char </param>
        /// <param name="defValue"> 默认值(单项转换失败时，默认值为NullOrEmpty时，则不添加，否则替换为默认值) </param>
        /// <typeparam name="T"> 基本类型 </typeparam>
        public static T[] ToArray<T>(this string str, T defValue, string splitString = ",")
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            var strArray = new string[str.Length];
            //判断是否带分隔符，如果没有。则直接拆份单个Char
            if (string.IsNullOrEmpty(splitString))
            {
                var c = str.ToCharArray();
                for (var i = 0; i < c.Length; i++)
                {
                    strArray[i] = ConvertHelper.ConvertType(c[i], "");
                }
            }

            else
            {
                strArray = Regex.Split(str, Regex.Escape(splitString), RegexOptions.IgnoreCase);
            }

            var lst = new T[strArray.Length];

            for (var i = 0; i < strArray.Length; i++)
            {
                if (TypeHelper.IsType<T>(strArray[i]))
                {
                    lst[i] = ConvertHelper.ConvertType(strArray[i], default(T));
                }
                else if (defValue != null)
                {
                    lst[i] = defValue;
                }
            }

            return lst;
        }

        /// <summary>
        ///     指定清除标签的内容
        /// </summary>
        /// <param name="str"> 内容 </param>
        /// <param name="tag"> 标签 </param>
        /// <param name="options"> 选项 </param>
        public static string Remove(this string str, string tag, RegexOptions options = RegexOptions.None)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }

            return tag.IsNullOrEmpty() ? str : Regex.Replace(str, tag, "", options);
        }

        /// <summary>
        ///     替换字符串
        /// </summary>
        /// <param name="str"> 数据源 </param>
        /// <param name="tag"> 要搜索的字符串 </param>
        /// <param name="newString"> 代替的数据 </param>
        /// <param name="options"> 选项 </param>
        /// <returns> </returns>
        public static string Replace(this string str, string tag, string newString, RegexOptions options)
        {
            return Regex.Replace(str, tag.Replace("|", "\\|"), newString, options);
        }

        /// <summary>
        ///     将字符串转换成List型
        /// </summary>
        /// <param name="str"> 要转换的字符串 </param>
        /// <param name="type"> </param>
        /// <param name="splitString"> 分隔符为NullOrEmpty时，则直接拆份为Char </param>
        public static List<Enum> ToEnumList(this string str, Type type, string splitString = ",")
        {
            var lst = new List<Enum>();
            if (string.IsNullOrWhiteSpace(str))
            {
                return lst;
            }

            //判断是否带分隔符，如果没有。则直接拆份单个Char
            if (splitString.Len() == 0)
            {
                for (var i = 0; i < str.Length; i++)
                {
                    lst.Add((Enum)str.Substring(i, 1).ConvertType(type));
                }
            }
            else
            {
                var strArray = splitString.Length == 1 ? str.Split(splitString[0]) : str.Split(splitString);
                foreach (var item in strArray)
                {
                    lst.Add((Enum)item.ConvertType(type));
                }
            }

            return lst;
        }

        /// <summary>
        ///     是否不为Null或者Empty
        /// </summary>
        /// <param name="str"> 要判断的字符串 </param>
        public static bool IsHaving(this string str) => str != null && str.Trim().Length > 0;

        /// <summary>
        ///     是否为Null或者Empty或者空白字符
        /// </summary>
        /// <param name="str"> 要判断的字符串 </param>
        public static bool IsNullOrEmpty(this string str) => string.IsNullOrWhiteSpace(str);

        /// <summary>
        ///     对比开头字符是否一致
        /// </summary>
        /// <param name="str"> 原字符串 </param>
        /// <param name="value"> 要对比的字符串 </param>
        /// <returns> </returns>
        public static bool IsStartsWith(this string str, string value)
        {
            return !string.IsNullOrWhiteSpace(str) && str.ToLower().StartsWith(value.ToLower());
        }

        /// <summary>
        ///     对比开头字符是否一致
        /// </summary>
        /// <param name="str"> 原字符串 </param>
        /// <param name="value"> 要对比的字符串 </param>
        /// <returns> </returns>
        public static bool IsEndsWith(this string str, string value)
        {
            return !string.IsNullOrWhiteSpace(str) && str.ToLower().EndsWith(value.ToLower());
        }

        /// <summary>
        ///     获取json
        /// </summary>
        /// <param name="json"> json字符串 </param>
        public static Dictionary<TKey, TValue> GetJson<TKey, TValue>(this string json)
        {
            var dic = new Dictionary<TKey, TValue>();
            if (json.IsNullOrEmpty())
            {
                return dic;
            }

            foreach (var item in json.Split('&'))
            {
                var s = item.Split('=');
                if (s.Length != 2)
                {
                    continue;
                }

                var key   = ConvertHelper.ConvertType<TKey>(s[0]);
                var value = ConvertHelper.ConvertType<TValue>(s[1]);

                dic[key] = value;
            }

            return dic;
        }

        /// <summary>
        ///     获取json值
        /// </summary>
        /// <param name="json"> json字符串 </param>
        /// <param name="key"> 对应的键值 </param>
        public static string GetJsonValue(this string json, object key)
        {
            return GetJsonValue(json, key, string.Empty);
        }

        /// <summary>
        ///     获取json值
        /// </summary>
        /// <param name="json"> json字符串 </param>
        /// <param name="key"> 对应的键值 </param>
        /// <param name="defValue"> 默认值 </param>
        public static T GetJsonValue<T>(this string json, object key, T defValue)
        {
            if (json.IsNullOrEmpty())
            {
                return defValue;
            }

            var reg   = new Regex(string.Format(@"(?<=\b{0}\b=)[^&]*", key), RegexOptions.IgnoreCase);
            var match = reg.Match(json);
            if (match != null && match.Success)
            {
                return ConvertHelper.ConvertType(match.Value, defValue);
            }

            return defValue;
        }

        /// <summary>
        ///     获取json Key
        /// </summary>
        /// <param name="json"> json字符串 </param>
        /// <param name="value"> 对应的键值 </param>
        public static string GetJsonKey(this string json, object value)
        {
            return GetJsonKey(json, value, string.Empty);
        }

        /// <summary>
        ///     获取json Key
        /// </summary>
        /// <param name="json"> json字符串 </param>
        /// <param name="value"> 对应的值 </param>
        /// <param name="defValue"> 默认值 </param>
        public static T GetJsonKey<T>(this string json, object value, T defValue)
        {
            if (json.IsNullOrEmpty())
            {
                return defValue;
            }

            var reg   = new Regex(string.Format(@"\b[^=^&]*\b(?=\b={0}\b)", value), RegexOptions.IgnoreCase);
            var match = reg.Match(json);
            if (match != null && match.Success)
            {
                return ConvertHelper.ConvertType(match.Value, defValue);
            }

            return defValue;
        }

        /// <summary>
        ///     获取json值
        /// </summary>
        /// <param name="json"> json字符串 </param>
        /// <param name="key"> 对应的键值 </param>
        /// <param name="value"> 值 </param>
        public static string SetJsonValue(this string json, object key, object value)
        {
            if (json.IsNullOrEmpty())
            {
                json = string.Empty;
            }

            if (value == null)
            {
                value = string.Empty;
            }

            var type = value.GetType();
            if (type.Name.IsEquals("Nullable`1"))
            {
                type = Nullable.GetUnderlyingType(type);
            }

            if (value is Enum)
            {
                value = (int)value;
            }

            if (value is IList)
            {
                value = ((IList)value).ToString(",");
            }

            if (!json.IsNullOrEmpty())
            {
                var reg = new Regex(string.Format(@"\b{0}\b=[^&]*[&^&]*", key), RegexOptions.IgnoreCase);
                json = reg.Replace(json, "");
            }

            if (json.Length > 0 && !json.EndsWith("&"))
            {
                json += "&";
            }

            return json + string.Format("{0}={1}", key, value);
        }

        /// <summary>
        ///     分隔字符串
        /// </summary>
        /// <param name="str"> 字符串 </param>
        /// <param name="splitString"> 分隔符号 </param>
        /// <returns> </returns>
        public static string[] Split(this string str, string splitString = ",")
        {
            if (splitString.Len() == 1)
            {
                return str.Split(splitString[0]);
            }

            return str.Split(new string[1] { splitString }, StringSplitOptions.None);
        }

        /// <summary>
        ///     是否包括str2字段串
        /// </summary>
        /// <param name="str"> 源字符串 </param>
        /// <param name="str2"> 要包括的字符串 </param>
        public static bool IsContains(this string str, string str2)
        {
            return str.IndexOf(str2, StringComparison.OrdinalIgnoreCase) > -1;
            //Regex regex = new Regex(str, RegexOptions.IgnoreCase);
            //return regex.IsMatch(str2);
        }

        /// <summary>
        ///     返回字符串的长度（用于SQL的比较）
        /// </summary>
        /// <param name="str"> 要比较的字段值 </param>
        /// <returns> </returns>
        public static int Len(this string str)
        {
            if (str == null)
            {
                return 0;
            }

            return str.Length;
        }

        /// <summary>
        ///     去除前后字符串
        /// </summary>
        /// <param name="source"> 要去除的字符源 </param>
        /// <param name="leftIndex"> 开始位置 </param>
        /// <param name="rightIndex"> 结束位置 </param>
        public static string CutTrim(this string source, int leftIndex, int rightIndex = 0)
        {
            if (leftIndex > 0)
            {
                source = source.SubString(leftIndex);
            }

            if (rightIndex > 0)
            {
                return source.SubString(0, source.Length - rightIndex);
            }

            return source;
        }

        /// <summary>
        ///     去除前后字符串
        /// </summary>
        /// <param name="source"> 要去除的字符源 </param>
        /// <param name="leftIndex"> 开始位置 </param>
        /// <param name="rightStr"> 结束位置字符串 </param>
        public static string CutTrim(this string source, int leftIndex, string rightStr = null)
        {
            if (leftIndex > 0)
            {
                source = source.SubString(leftIndex);
            }

            if (!string.IsNullOrWhiteSpace(rightStr))
            {
                return source.SubString(0, source.LastIndexOf(rightStr));
            }

            return source;
        }

        /// <summary>
        ///     去除前后字符串
        /// </summary>
        /// <param name="source"> 要去除的字符源 </param>
        /// <param name="leftStr"> 开始位置字符串 </param>
        /// <param name="rightStr"> 结束位置字符串 </param>
        public static string CutTrim(this string source, string leftStr, string rightStr = null)
        {
            if (!string.IsNullOrWhiteSpace(leftStr))
            {
                source = source.SubString(source.IndexOf(leftStr) + leftStr.Length);
            }

            if (!string.IsNullOrWhiteSpace(rightStr))
            {
                return source.SubString(0, source.LastIndexOf(rightStr));
            }

            return source;
        }

        /// <summary>
        ///     去除前后字符串
        /// </summary>
        /// <param name="source"> 要去除的字符源 </param>
        /// <param name="leftStr"> 开始位置字符串 </param>
        /// <param name="rightIndex"> 结束位置 </param>
        public static string CutTrim(this string source, string leftStr, int rightIndex = 0)
        {
            if (!string.IsNullOrWhiteSpace(leftStr))
            {
                source = source.SubString(source.IndexOf(leftStr) + leftStr.Length);
            }

            if (rightIndex > 0)
            {
                return source.SubString(0, source.Length - rightIndex);
            }

            return source;
        }

        /// <summary>
        ///     截取字符串长，超过指定长度，用tag代替。
        ///     一个汉字，长度为2
        /// </summary>
        /// <param name="source"> 要截取的字符串 </param>
        /// <param name="length"> 截取长度(一个汉字，长度为2) </param>
        /// <param name="tag"> 超出长度时显示的字符 </param>
        /// <param name="isAlwaysShowTag"> 如果没有超出，是否也显示 </param>
        /// <returns> </returns>
        public static string CutString(this string source, int length, string tag = "", bool isAlwaysShowTag = false)
        {
            var str = new StringBuilder();
            foreach (var item in source)
            {
                if (Encoding.Default.GetBytes(str.ToString()).Length >= length)
                {
                    return str + tag;
                }

                str.Append(item);
            }

            if (isAlwaysShowTag)
            {
                str.Append(tag);
            }

            return str.ToString();
        }

        /// <summary>
        ///     当NullOrEmpty，用新的字符串代替，否则用原来的。
        /// </summary>
        /// <param name="str"> 要检测的值 </param>
        /// <param name="newString"> 要替换的新字符串 </param>
        public static string WhileNullOrEmpty(this string str, string newString)
        {
            return string.IsNullOrWhiteSpace(str) ? newString : str;
        }

        /// <summary>
        ///     当不为NullOrEmpty，用新的字符串代替，否则用原来的。
        /// </summary>
        /// <param name="str"> 要检测的值 </param>
        /// <param name="newString"> 要替换的新字符串 </param>
        public static string WhileNotNullOrEmpty(this string str, string newString)
        {
            return string.IsNullOrWhiteSpace(str) ? str : newString;
        }

        /// <summary>
        ///     判断ID是否存在于str
        /// </summary>
        /// <param name="str"> </param>
        /// <param name="ID"> </param>
        /// <returns> </returns>
        public static bool Contains(this string str, int? ID)
        {
            return ("," + str + ",").Contains("," + ID.ToString() + ",");
            //return str.ToList(0).Contains(ID);
        }

        /// <summary>
        ///     迭代字符
        /// </summary>
        /// <param name="str"> 要迭代的字符串 </param>
        /// <param name="count"> 次数 </param>
        public static string For(this string str, int count = 1)
        {
            var strs = new StringBuilder();
            for (var i = 0; i < count; i++)
            {
                strs.Append(str);
            }

            return strs.ToString();
        }
    }
}