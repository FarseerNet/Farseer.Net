using System;
using System.Text;

namespace FS.Utils.Component
{
    /// <summary>
    ///     对StringBuilder再封装
    /// </summary>
    public class StrPlus
    {
        // Fields
        private StringBuilder _str = new StringBuilder();

        /// <summary>
        ///     对StringBuilder再封装
        /// </summary>
        public StrPlus(string strValue)
        {
            _str = new StringBuilder(value: strValue);
        }

        /// <summary>
        ///     对StringBuilder再封装
        /// </summary>
        public StrPlus()
        {
        }

        /// <summary>
        ///     返回指定位置的字符
        /// </summary>
        public char this[int index]
        {
            get => _str[index: index];
            set => _str[index: index] = value;
        }

        /// <summary>
        ///     返回所有字符串
        /// </summary>
        public string Value => _str.ToString();

        /// <summary>
        ///     获取当前 System.String 对象中的字符数。
        /// </summary>
        public int Length => _str.ToString().Length;

        /// <summary>
        ///     给定类型，返回DbExecutor
        /// </summary>
        public static implicit operator string(StrPlus sp) => sp.Value;

        /// <summary>
        ///     给定类型，返回DbExecutor
        /// </summary>
        public static implicit operator StrPlus(string s) => new StrPlus(strValue: s);

        /// <summary>
        ///     添加字符串
        /// </summary>
        public void Append(string Text, int spaceNum = 0)
        {
            _str.Append(value: Space(spaceNum: spaceNum) + Text);
        }

        /// <summary>
        ///     添加字符串
        /// </summary>
        public void AppendLine(string text = "", int spaceNum = 0)
        {
            Append(Text: text, spaceNum: spaceNum);
            Append(Text: "\r\n");
        }

        /// <summary>
        ///     添加字符串
        /// </summary>
        /// <param name="format"> </param>
        /// <param name="args"> </param>
        public void AppendFormat(string format, params object[] args)
        {
            if (args != null && args.Length > 0) format = string.Format(format: format, args: args);
            Append(Text: format);
        }

        /// <summary>
        ///     添加字符串
        /// </summary>
        /// <param name="format"> </param>
        /// <param name="args"> </param>
        public void AppendFormatLine(string format, params object[] args)
        {
            AppendFormat(format: format, args: args);
            AppendLine();
        }

        /// <summary>
        ///     添加字符串
        /// </summary>
        /// <param name="spaceNum"> </param>
        /// <param name="format"> </param>
        /// <param name="args"> </param>
        public void AppendFormatLine(string format, int spaceNum = 0, params object[] args)
        {
            AppendFormat(format: format, spaceNum, args);
            AppendLine();
        }

        /// <summary>
        ///     删除指定最后的字符串
        /// </summary>
        public string DelLastChar(string strChar)
        {
            var s      = _str.ToString();
            var length = s.LastIndexOf(value: strChar);
            if (length > 0)
            {
                _str = new StringBuilder();
                _str.Append(value: s.Substring(startIndex: 0, length: length));
            }

            return _str.ToString();
        }

        /// <summary>
        ///     移出指定数量的字符串
        /// </summary>
        /// <param name="start"> </param>
        /// <param name="num"> </param>
        public void Remove(int start, int num)
        {
            _str.Remove(startIndex: start, length: num);
        }

        /// <summary>
        ///     制表符
        /// </summary>
        public string Space(int spaceNum)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < spaceNum; i++) builder.Append(value: "\t");
            return builder.ToString();
        }

        /// <summary>
        ///     转换为String类型
        /// </summary>
        public override string ToString() => _str.ToString();

        /// <summary>
        ///     清除所有字符串
        /// </summary>
        public void Clear()
        {
            _str = new StringBuilder();
        }

        /// <summary>
        ///     从此实例检索子字符串。子字符串从指定的字符位置开始且具有指定的长度。
        /// </summary>
        /// <param name="startIndex"> 此实例中子字符串的起始字符位置（从零开始）。 </param>
        /// <param name="length"> 子字符串中的字符数。 </param>
        public string Substring(int startIndex, int length = 0)
        {
            if (length == 0) return _str.ToString().Substring(startIndex: startIndex);
            return _str.ToString().Substring(startIndex: startIndex, length: length);
        }

        /// <summary>
        ///     将对象的字符串表示形式插入到此实例中的指定字符位置。
        /// </summary>
        /// <param name="index"> 此实例中开始插入的位置。 </param>
        /// <param name="value"> 要插入的对象或 null。 </param>
        public StrPlus Insert<T>(int index, T value)
        {
            _str.Insert(index: index, value: value);
            return this;
        }

        /// <summary>
        ///     重载+运算符，实现字符串相加的方式
        /// </summary>
        /// <param name="a"> 字符串 </param>
        /// <param name="b"> 本体 </param>
        public static StrPlus operator +(string a, StrPlus b) => b.Insert(index: 0, value: a);

        /// <summary>
        ///     重载+运算符，实现字符串相加的方式
        /// </summary>
        /// <param name="a"> 本体 </param>
        /// <param name="b"> 字符串 </param>
        public static StrPlus operator +(StrPlus b, string a)
        {
            b.Append(Text: a);
            return b;
        }

        /// <summary>
        ///     返回一个新字符串，其中当前实例中出现的所有指定字符串都替换为另一个指定的字符串。
        /// </summary>
        /// <param name="oldValue"> 要被替换的字符串。 </param>
        /// <param name="newValue"> 要替换出现的所有 oldValue 的字符串。 </param>
        public StrPlus Replace(string oldValue, string newValue)
        {
            _str = _str.Replace(oldValue: oldValue, newValue: newValue);
            return this;
        }

        /// <summary>
        ///     分隔字符串
        /// </summary>
        /// <param name="splitString"> 分隔符号 </param>
        /// <returns> </returns>
        public string[] Split(string splitString = ",")
        {
            return _str.ToString().Split(separator: new string[1] { splitString }, options: StringSplitOptions.None);
        }

        /// <summary>
        ///     报告指定字符串在此实例中的第一个匹配项的索引。
        /// </summary>
        public int IndexOf(string value) => _str.ToString().IndexOf(value: value);

        /// <summary>
        ///     报告指定字符串在此实例中的第一个匹配项的索引。
        /// </summary>
        public int IndexOf(char value) => _str.ToString().IndexOf(value: value);
    }
}