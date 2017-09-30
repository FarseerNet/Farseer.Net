using System.Linq;
using System.Text.RegularExpressions;

namespace Farseer.Net.Utils.Common
{
    /// <summary>
    ///     判断类型
    /// </summary>
    public abstract class IsType
    {
        /// <summary>
        ///     检测是否符合email格式
        /// </summary>
        /// <param name="strEmail">要判断的email字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsValidEmail(string strEmail)
        {
            return Regex.IsMatch(strEmail, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        /// <summary>
        ///     是否为ip
        /// </summary>
        public static bool IsIP(string ip)
        {
            //return Regex.IsMatch(ip,  @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){2}((2[0-4]\d|25[0-5]|[01]?\d\d?|\*)\.)(2[0-4]\d|25[0-5]|[01]?\d\d?|\*)$");
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        /// <summary>
        ///     判断给定的字符串数组(strNumber)中的数据是不是都为数值型
        /// </summary>
        /// <param name="strDouble">要确认的字符串数组</param>
        /// <returns>是则返加true 不是则返回 false</returns>
        public static bool IsDoubleArray(string[] strDouble)
        {
            return strDouble != null && (strDouble.Length >= 1 && strDouble.All(id => IsNumber(id)));
        }

        /// <summary>
        ///     判断给定的字符串数组(strNumber)中的数据是不是都为数值型
        /// </summary>
        /// <param name="strInt">要确认的字符串数组</param>
        /// <returns>是则返加true 不是则返回 false</returns>
        public static bool IsIntArray(string[] strInt)
        {
            return strInt != null && strInt.Length >= 1 && strInt.All(id => IsInt(id));
        }

        /// <summary>
        ///     判断是否为base64字符串
        /// </summary>
        public static bool IsBase64String(string str)
        {
            //A-Z, a-z, 0-9, +, /, =
            return Regex.IsMatch(str, @"[A-Za-z0-9\+\/\=]");
        }

        /// <summary>
        ///     验证带小数点的数字
        /// </summary>
        /// <returns></returns>
        public static bool IsNumber(string number, string tag = "")
        {
            return Regex.IsMatch(Regex.Replace(number, tag, "", RegexOptions.IgnoreCase), @"[1-9]+[\d.][0-9]");
        }

        /// <summary>
        ///     是否为数字
        /// </summary>
        /// <param name="number">0-9</param>
        /// <returns></returns>
        public static bool IsDigital(string number)
        {
            return Regex.IsMatch(number, @"[\d]+");
        }

        /// <summary>
        ///     是否为整型
        /// </summary>
        /// <param name="number">第一位，不能为0，不带小数点</param>
        /// <returns></returns>
        public static bool IsInt(string number)
        {
            return Regex.IsMatch(number, @"[1-9]+[0-9]*");
        }

        /// <summary>
        ///     是否为字母
        /// </summary>
        /// <param name="number">a-zA-Z</param>
        /// <returns></returns>
        public static bool IsLetter(string number)
        {
            return Regex.IsMatch(number, @"[a-zA-Z]+");
        }

        /// <summary>
        ///     验证座机号码
        /// </summary>
        /// <param name="tel"></param>
        /// <returns></returns>
        public static bool IsTel(string tel)
        {
            return Regex.IsMatch(tel, @"1(3|5|8)\d{9}");
        }

        /// <summary>
        ///     验证手机号码
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static bool IsMobile(string mobile)
        {
            if (string.IsNullOrEmpty(mobile)) { return false; }
            return new Regex(@"^(1(([34578][0-9])|(47)|[8][01236789]))\d{8}$").IsMatch(mobile.Trim());
        }

        /// <summary>
        ///     验证电话号码(包括手机、座机)
        /// </summary>
        public static bool IsPhone(string tel)
        {
            //匹配格式：
            //11位手机号码
            //3-4位区号，7-8位直播号码，1－4位分机号
            //如：12345678901、1234-12345678-1234
            return Regex.IsMatch(tel, @"((\d{11})|^((\d{7,8})|(\d{4}|\d{3})-(\d{7,8})|(\d{4}|\d{3})-(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1})|(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1}))$)");
        }

        /// <summary>
        ///     验证邮编号码
        /// </summary>
        /// <param name="zip"></param>
        /// <returns></returns>
        public static bool IsZIP(string zip)
        {
            return Regex.IsMatch(zip, @"[0-9]{6}");
        }

        /// <summary>
        ///     登陆账号
        /// </summary>
        /// <param name="name">必须是英文开头的：英文、数字、-、_</param>
        public static bool IsLoginName(string name)
        {
            return Regex.IsMatch(name, @"^[a-z]+[a-z0-9-_]*$");
        }
    }
}