using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using FS.Extends;

namespace FS.Utils.Common
{
    /// <summary>
    ///     解释字符串
    /// </summary>
    public static class Str
    {
        private static Regex _regexBr = new Regex(@"(\r\n)", RegexOptions.IgnoreCase);

        /// <summary>
        ///     返回字符串真实长度, 1个汉字长度为2
        /// </summary>
        /// <returns></returns>
        public static int Length(string str)
        {
            return Encoding.Default.GetBytes(str).Length;
        }

        /// <summary>
        ///     判断指定字符串在指定字符串数组中的位置
        /// </summary>
        /// <param name="searchText">要查找字符串</param>
        /// <param name="strArr">字符串数组</param>
        /// <param name="ignoreCase">是否不区分大小写, true为不区分, false为区分</param>
        /// <returns>字符串在指定字符串数组中的位置, 如不存在则返回-1</returns>
        public static int GetArrayIndex(string searchText, string[] strArr, bool ignoreCase)
        {
            for (var i = 0; i < strArr.Length; i++) { if (String.Compare(searchText, strArr[i], ignoreCase) == 0) { return i; } }
            return -1;
        }

        /// <summary>
        ///     删除字符串尾部的回车/换行/空格
        /// </summary>
        public static string RTrim(string str)
        {
            return str.TrimEnd(' ').DelEndOf("\r\n").DelEndOf("\r").DelEndOf("\n");
        }

        /// <summary>
        ///     获取指定标签内的内容
        /// </summary>
        /// <param name="str">内容</param>
        /// <param name="beforeTag">开始标签</param>
        /// <param name="endTag">结尾标签</param>
        /// <returns>有用信息</returns>
        public static string GetString(string str, string beforeTag, string endTag)
        {
            var s = GetStringArray(str, beforeTag, endTag);
            return s.Length == 0 ? String.Empty : s[0];
        }

        /// <summary>
        ///     获取指定标签内的内容
        /// </summary>
        /// <param name="str">要搜寻的字符串</param>
        /// <param name="beforeTag">开头的标签</param>
        /// <param name="endTag">结尾的标签</param>
        /// <returns></returns>
        public static string[] GetStringArray(string str, string beforeTag, string endTag)
        {
            var list = new List<string>();
            var bLen = beforeTag.Length;
            var eLen = endTag.Length;
            var temp = String.Empty;
            var index = 0;
            var indexBefore = 0;
            var indexEnd = 0;
            var findBefore = false;
            var findEnd = false;
            var cursor = 0;
            foreach (var c in str)
            {
                if (!findBefore)
                {
                    // 寻找开始的标签
                    if (index < bLen)
                    {
                        temp += c.ToString();
                        index++;
                    }
                    else
                    {
                        temp += c.ToString();
                        temp = temp.Substring(1);
                    }
                }
                else
                {
                    // 已经找到了开始的标签
                    if (index < eLen)
                    {
                        temp += c.ToString();
                        index++;
                    }
                    else
                    {
                        temp += c.ToString();
                        temp = temp.Substring(1);
                    }
                }
                cursor++;

                if (temp == beforeTag)
                {
                    findBefore = true;
                    indexBefore = cursor;
                    temp = "";
                    index = 0;
                }
                if (findBefore && temp == endTag)
                {
                    findEnd = true;
                    indexEnd = cursor;
                }
                if (!findBefore || !findEnd) continue;

                list.Add(str.Substring(indexBefore, indexEnd - indexBefore - eLen));
                findBefore = findEnd = false;
                indexBefore = indexEnd = 0;
            }
            var s = new string[list.Count];
            for (var i = 0; i < list.Count; i++)
                s[i] = list[i];
            return s;
        }

        /// <summary>
        ///     获取指定标签内的内容 （注意 "@@-@" 代表一切任意字符  返回空串 表示没有找到）
        /// </summary>
        /// <param name="sourceCode">获取的代码</param>
        /// <param name="startTag">开始的代码</param>
        /// <param name="endTag">结束的代码</param>
        /// <returns></returns>
        public static string GetTag(string sourceCode, string startTag, string endTag)
        {
            //  "@@-@" 代表一切任意字符
            //  "[\v\f\r\n]+"  代表 回车符、换行符、空格等

            try
            {
                var start = Regex.Escape(Regex.Replace(startTag, "[\v\f\r\n]+", "", RegexOptions.IgnoreCase)).Replace("@@-@", ".{0,}?");
                var end = Regex.Escape(Regex.Replace(endTag, "[\v\f\r\n]+", "", RegexOptions.IgnoreCase)).Replace("@@-@", ".{0,}?");
                var reg = new Regex(start + ".{0,}?" + end, RegexOptions.IgnoreCase);
                var mc = reg.Matches(Regex.Replace(sourceCode, "[\v\f\r\n]+", "", RegexOptions.IgnoreCase));

                foreach (Match match in mc) { foreach (Group group in match.Groups) { foreach (Capture capture in @group.Captures) { return capture.Value; } } }

                return "";
            }
            catch {
                return "";
            }
        }

        /// <summary>
        ///     获取指定标签内的内容（注意 "@@-@" 代表一切任意字符  返回空串 表示没有找到）
        /// </summary>
        /// <param name="sourceCode">原文内容</param>
        /// <param name="content">要替换的内容</param>
        /// <param name="startTag">开始标记</param>
        /// <param name="endTag">结束标记</param>
        /// <returns></returns>
        public static string[] GetTagArray(string sourceCode, string content, string startTag, string endTag)
        {
            //  "@@-@" 代表一切任意字符
            //  "[\v\f\r\n]+"  代表 回车符、换行符、空格等
            var list = new List<string>();
            try
            {
                var start = Regex.Escape(Regex.Replace(startTag, "[\v\f\r\n]+", "", RegexOptions.IgnoreCase)).Replace("@@-@", ".{0,}?");
                var end = Regex.Escape(Regex.Replace(endTag, "[\v\f\r\n]+", "", RegexOptions.IgnoreCase)).Replace("@@-@", ".{0,}?");
                var reg = new Regex(start + ".{0,}?" + end, RegexOptions.IgnoreCase);
                var mc = reg.Matches(Regex.Replace(sourceCode, "[\v\f\r\n]+", content, RegexOptions.IgnoreCase));

                foreach (Match match in mc) { foreach (Group group in match.Groups) { foreach (Capture capture in @group.Captures) { list.Add(capture.Value); } } }
            }
            catch {
                return null;
            }
            return list.ToArray();
        }

        /// <summary>
        ///     指定索引范围，替换字段串
        /// </summary>
        /// <returns></returns>
        public static string ReplaceString(string source, string str, int startIndex, int endIndex)
        {
            if (startIndex == -1 || endIndex == -1) { return source; }
            return source.Substring(0, startIndex) + str + source.Substring(endIndex);
        }

        /// <summary>
        ///     自定义的替换字符串函数
        /// </summary>
        public static string ReplaceString(string sourceString, string searchString, string replaceString, bool isCaseInsensetive)
        {
            return Regex.Replace(sourceString, Regex.Escape(searchString), replaceString, isCaseInsensetive ? RegexOptions.IgnoreCase : RegexOptions.None);
        }

        /// <summary>
        ///     删除指定开头的字符串
        /// </summary>
        public static string DelStartsOf(string str, string strChar)
        {
            if (str.ToLower().StartsWith(strChar.ToLower()))
            {
                var index = str.ToLower().IndexOf(strChar.ToLower());
                if (index > -1) { str = str.Substring(index + strChar.Length); }
            }
            return str;
        }

        /// <summary>
        ///     位数补齐,不足长度，前面补0
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="enoughString">不足时，字符串补充</param>
        /// <param name="lenght">长度</param>
        /// <returns></returns>
        public static string NumberString(int value, int lenght, string enoughString = "0")
        {
            var str = value.ToString();
            while (str.Length < lenght) { str = enoughString + str; }
            return str;
        }

        /// <summary>
        ///     转换标签，将, ; ，；|｜转换成 tag
        /// </summary>
        public static string ConvertTag(string str, string tag = ",")
        {
            var reg = new Regex(@"[,;，；|｜ \t、　]");
            str = reg.Replace(str, tag);
            var lst = str.ToList("", tag);
            lst.RemoveAll(String.IsNullOrWhiteSpace);
            return ClearRepeatContinuousTag(lst.ToString(tag), tag).Trim();
        }

        /// <summary>
        ///     只出现一次标签，之后的都删除
        ///     如：AABBAABBCCAA，清除AA后：AABBBBCC
        /// </summary>
        public static string ClearRepeatTag(string str, string tag)
        {
            var strs = new StringBuilder();
            var isRepeat = false;
            foreach (var s in str.Split(new string[1] {tag}, StringSplitOptions.None))
            {
                strs.Append(s);
                if (isRepeat) continue;
                strs.Append(tag);
                isRepeat = true;
            }
            return strs.ToString().DelEndOf(tag);
        }

        /// <summary>
        ///     指定的标签连续出现时，只保留一个
        ///     如：AABBAAAABBAACC，清除重复连续后：AABBAABBAACC
        /// </summary>
        public static string ClearRepeatContinuousTag(string str, string tag)
        {
            var strs = new StringBuilder();
            var isRepeat = false;
            foreach (var s in str.Split(new string[1] {tag}, StringSplitOptions.None))
            {
                if (!String.IsNullOrWhiteSpace(s))
                {
                    strs.Append(s);
                    isRepeat = false;
                }

                if (isRepeat) continue;
                strs.Append(tag);
                isRepeat = true;
            }
            return strs.ToString().DelEndOf(tag);
        }

        /// <summary>
        ///     检测Email
        /// </summary>
        /// <param name="strEmail"></param>
        /// <returns></returns>
        public static string GetEmailHostName(string strEmail)
        {
            return strEmail.IndexOf("@") < 0 ? "" : strEmail.Substring(strEmail.LastIndexOf("@")).ToLower();
        }

        /// <summary>
        ///     字符串转换
        /// </summary>
        /// <param name="oenc">源编码</param>
        /// <param name="tenc">转换之后的编码</param>
        /// <param name="source">源字符串</param>
        public static string ConvertEncoding(string source, Encoding oenc, Encoding tenc)
        {
            var utfbytes = oenc.GetBytes(source);
            return tenc.GetString(utfbytes);
        }

        /// <summary>
        ///     将字符串转换为Color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color ToColor(string color)
        {
            int red, green, blue = 0;
            char[] rgb;
            color = color.TrimStart('#');
            color = Regex.Replace(color.ToLower(), "[g-zG-Z]", "");
            switch (color.Length)
            {
                case 3:
                    rgb = color.ToCharArray();
                    red = Convert.ToInt32(rgb[0].ToString() + rgb[0].ToString(), 16);
                    green = Convert.ToInt32(rgb[1].ToString() + rgb[1].ToString(), 16);
                    blue = Convert.ToInt32(rgb[2].ToString() + rgb[2].ToString(), 16);
                    return Color.FromArgb(red, green, blue);
                case 6:
                    rgb = color.ToCharArray();
                    red = Convert.ToInt32(rgb[0].ToString() + rgb[1].ToString(), 16);
                    green = Convert.ToInt32(rgb[2].ToString() + rgb[3].ToString(), 16);
                    blue = Convert.ToInt32(rgb[4].ToString() + rgb[5].ToString(), 16);
                    return Color.FromArgb(red, green, blue);
                default:
                    return Color.FromName(color);
            }
        }

        /// <summary>
        ///     返回指定IP是否在指定的IP数组所限定的范围内, IP数组内的IP地址可以使用*表示该IP段任意, 例如192.168.1.*
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="iparray"></param>
        /// <returns></returns>
        public static bool InIPArray(string ip, string[] iparray)
        {
            var userip = ip.ToArray(".", null);

            foreach (var s in iparray)
            {
                var tmpip = s.ToArray(".", null);
                var r = 0;
                for (var i = 0; i < tmpip.Length; i++)
                {
                    if (tmpip[i] == "*") { return true; }

                    if (userip.Length <= i) { break; }
                    if (tmpip[i] == userip[i]) { r++; }
                    else
                    { break; }
                }
                if (r == 4) { return true; }
            }
            return false;
        }

        /// <summary>
        ///     反转字符串
        /// </summary>
        /// <param name="input">要反转字符串</param>
        /// <returns></returns>
        public static string Reverse(string input)
        {
            var chars = input.ToUpper().ToCharArray();
            var length = chars.Length;
            for (var index = 0; index < length/2; index++)
            {
                var c = chars[index];
                chars[index] = chars[length - 1 - index];
                chars[length - 1 - index] = c;
            }
            return new String(chars);
        }
    }
}