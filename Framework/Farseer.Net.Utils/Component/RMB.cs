using System;
using FS.Extends;

namespace FS.Utils.Component
{
    /// <summary>
    ///     人民币
    /// </summary>
    public class RMB
    {
        /// <summary>
        ///     将数字，转换成大写金额
        /// </summary>
        /// <param name="price"> </param>
        public static string ConvertChinaPrice(decimal price)
        {
            var chinaPrice     = new[] { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
            var chinaPriceUnit = new[] { "分", "角", "元", "拾", "佰", "仟", "万" };
            // 转换成 *.?? 格式
            var strPrice = price.ToString(format: "f2");
            var arrPrice = strPrice.Split('.');
            //分
            var result = arrPrice[1].ToCharArray()[1] + chinaPriceUnit[0];
            //角
            result = arrPrice[1].ToCharArray()[0] + chinaPriceUnit[1] + result;

            var arrLeftPrice = arrPrice[0].ToList(defValue: 0, splitString: string.Empty);
            arrLeftPrice.Reverse();

            for (var i = 0; i < arrLeftPrice.Count && i < 5; i++) result = chinaPrice[arrLeftPrice[index: i]] + chinaPriceUnit[i + 2] + result;
            return result;
        }

        /// <summary>
        ///     转换数字金额主函数（包括小数）
        /// </summary>
        /// <param name="str"> 数字字符串;转换成中文大写后的字符串或者出错信息提示字符串 </param>
        public static string ConvertSum(string str)
        {
            if (!IsPositveDecimal(str: str)) return "输入的不是正数字！";
            if (double.Parse(s: str) > 999999999999.99) return "数字太大，无法换算，请输入一万亿元以下的金额";
            var ch = new char[1];
            ch[0] = '.';                 //小数点
            string[] splitstr = null;    //定义按小数点分割后的字符串数组
            splitstr = str.Split(ch[0]); //按小数点分割字符串
            if (splitstr.Length == 1)    //只有整数部分
                return ConvertData(str: str) + "元整";
            var rstr = ConvertData(str: splitstr[0]) + "元";
            rstr += ConvertXiaoShu(str: splitstr[1]); //转换小数部分
            return rstr;
        }

        /// <summary>
        ///     判断是否是正数字字符串
        /// </summary>
        /// <param name="str"> 判断字符串;如果是数字，返回true，否则返回false </param>
        private static bool IsPositveDecimal(string str)
        {
            decimal d;
            try
            {
                d = decimal.Parse(s: str);
            }
            catch (Exception)
            {
                return false;
            }

            return d > 0;
        }

        /// <summary>
        ///     转换数字（整数）
        /// </summary>
        /// <param name="str"> 需要转换的整数数字字符串;转换成中文大写后的字符串 </param>
        private static string ConvertData(string str)
        {
            var rstr   = "";
            var strlen = str.Length;
            if (strlen <= 4) //数字长度小于四位
                rstr = ConvertDigit(str: str);
            else
            {
                var tmpstr = "";
                if (strlen <= 8) //数字长度大于四位，小于八位
                {
                    tmpstr = str.Substring(startIndex: strlen - 4, length: 4); //先截取最后四位数字
                    rstr   = ConvertDigit(str: tmpstr);                        //转换最后四位数字
                    tmpstr = str.Substring(startIndex: 0, length: strlen - 4); //截取其余数字
                    //将两次转换的数字加上万后相连接
                    rstr = string.Concat(str0: ConvertDigit(str: tmpstr) + "万", str1: rstr);
                    rstr = rstr.Replace(oldValue: "零零", newValue: "零");
                }
                else if (strlen <= 12) //数字长度大于八位，小于十二位
                {
                    tmpstr = str.Substring(startIndex: strlen - 4, length: 4);              //先截取最后四位数字
                    rstr   = ConvertDigit(str: tmpstr);                                     //转换最后四位数字
                    tmpstr = str.Substring(startIndex: strlen              - 8, length: 4); //再截取四位数字
                    rstr   = string.Concat(str0: ConvertDigit(str: tmpstr) + "万", str1: rstr);
                    tmpstr = str.Substring(startIndex: 0, length: strlen   - 8);
                    rstr   = string.Concat(str0: ConvertDigit(str: tmpstr) + "亿", str1: rstr);
                    rstr   = rstr.Replace(oldValue: "零亿", newValue: "亿");
                    rstr   = rstr.Replace(oldValue: "零万", newValue: "零");
                    rstr   = rstr.Replace(oldValue: "零零", newValue: "零");
                    rstr   = rstr.Replace(oldValue: "零零", newValue: "零");
                }
            }

            strlen = rstr.Length;
            if (strlen >= 2)
            {
                switch (rstr.Substring(startIndex: strlen - 2, length: 2))
                {
                    case "佰零":
                        rstr = rstr.Substring(startIndex: 0, length: strlen - 2) + "佰";
                        break;
                    case "仟零":
                        rstr = rstr.Substring(startIndex: 0, length: strlen - 2) + "仟";
                        break;
                    case "万零":
                        rstr = rstr.Substring(startIndex: 0, length: strlen - 2) + "万";
                        break;
                    case "亿零":
                        rstr = rstr.Substring(startIndex: 0, length: strlen - 2) + "亿";
                        break;
                }
            }

            return rstr;
        }

        /// <summary>
        ///     转换数字（小数部分）
        /// </summary>
        /// <param name="str"> 需要转换的小数部分数字字符串;转换成中文大写后的字符串 </param>
        private static string ConvertXiaoShu(string str)
        {
            var    strlen = str.Length;
            string rstr;
            if (strlen == 1)
            {
                rstr = ConvertChinese(str: str) + "角";
                return rstr;
            }

            var tmpstr = str.Substring(startIndex: 0, length: 1);
            rstr   =  ConvertChinese(str: tmpstr) + "角";
            tmpstr =  str.Substring(startIndex: 1, length: 1);
            rstr   += ConvertChinese(str: tmpstr) + "分";
            //rstr = rstr.Replace("零分", "");
            //rstr = rstr.Replace("零角", "");
            return rstr;
        }

        /// <summary>
        ///     转换数字
        /// </summary>
        /// <param name="str"> 转换的字符串（四位以内） </param>
        private static string ConvertDigit(string str)
        {
            var strlen = str.Length;
            var rstr   = "";
            switch (strlen)
            {
                case 1:
                    rstr = ConvertChinese(str: str);
                    break;
                case 2:
                    rstr = Convert2Digit(str: str);
                    break;
                case 3:
                    rstr = Convert3Digit(str: str);
                    break;
                case 4:
                    rstr = Convert4Digit(str: str);
                    break;
            }

            rstr   = rstr.Replace(oldValue: "拾零", newValue: "拾");
            strlen = rstr.Length;
            return rstr;
        }

        /// <summary>
        ///     转换四位数字
        /// </summary>
        /// <param name="str"> </param>
        private static string Convert4Digit(string str)
        {
            var str1    = str.Substring(startIndex: 0, length: 1);
            var str2    = str.Substring(startIndex: 1, length: 1);
            var str3    = str.Substring(startIndex: 2, length: 1);
            var str4    = str.Substring(startIndex: 3, length: 1);
            var rstring = "";
            rstring += ConvertChinese(str: str1) + "仟";
            rstring += ConvertChinese(str: str2) + "佰";
            rstring += ConvertChinese(str: str3) + "拾";
            rstring += ConvertChinese(str: str4);
            rstring =  rstring.Replace(oldValue: "零仟", newValue: "零");
            rstring =  rstring.Replace(oldValue: "零佰", newValue: "零");
            rstring =  rstring.Replace(oldValue: "零拾", newValue: "零");
            rstring =  rstring.Replace(oldValue: "零零", newValue: "零");
            rstring =  rstring.Replace(oldValue: "零零", newValue: "零");
            rstring =  rstring.Replace(oldValue: "零零", newValue: "零");
            return rstring;
        }

        /// <summary>
        ///     转换三位数字
        /// </summary>
        /// <param name="str"> </param>
        private static string Convert3Digit(string str)
        {
            var str1    = str.Substring(startIndex: 0, length: 1);
            var str2    = str.Substring(startIndex: 1, length: 1);
            var str3    = str.Substring(startIndex: 2, length: 1);
            var rstring = "";
            rstring += ConvertChinese(str: str1) + "佰";
            rstring += ConvertChinese(str: str2) + "拾";
            rstring += ConvertChinese(str: str3);
            rstring =  rstring.Replace(oldValue: "零佰", newValue: "零");
            rstring =  rstring.Replace(oldValue: "零拾", newValue: "零");
            rstring =  rstring.Replace(oldValue: "零零", newValue: "零");
            rstring =  rstring.Replace(oldValue: "零零", newValue: "零");
            return rstring;
        }

        /// <summary>
        ///     转换二位数字
        /// </summary>
        /// <param name="str"> </param>
        private static string Convert2Digit(string str)
        {
            var str1    = str.Substring(startIndex: 0, length: 1);
            var str2    = str.Substring(startIndex: 1, length: 1);
            var rstring = "";
            rstring += ConvertChinese(str: str1) + "拾";
            rstring += ConvertChinese(str: str2);
            rstring =  rstring.Replace(oldValue: "零拾", newValue: "零");
            rstring =  rstring.Replace(oldValue: "零零", newValue: "零");
            return rstring;
        }

        /// <summary>
        ///     将一位数字转换成中文大写数字
        /// </summary>
        /// <param name="str"> </param>
        private static string ConvertChinese(string str)
        {
            //"零壹贰叁肆伍陆柒捌玖拾佰仟万亿圆整角分"
            var cstr = "";
            switch (str)
            {
                case "0":
                    cstr = "零";
                    break;
                case "1":
                    cstr = "壹";
                    break;
                case "2":
                    cstr = "贰";
                    break;
                case "3":
                    cstr = "叁";
                    break;
                case "4":
                    cstr = "肆";
                    break;
                case "5":
                    cstr = "伍";
                    break;
                case "6":
                    cstr = "陆";
                    break;
                case "7":
                    cstr = "柒";
                    break;
                case "8":
                    cstr = "捌";
                    break;
                case "9":
                    cstr = "玖";
                    break;
            }

            return cstr;
        }
    }
}