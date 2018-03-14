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
        /// <param name="price"></param>
        public static string ConvertChinaPrice(decimal price)
        {
            var chinaPrice = new[] {"零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖"};
            var chinaPriceUnit = new[] {"分", "角", "元", "拾", "佰", "仟", "万"};
            // 转换成 *.?? 格式
            var strPrice = price.ToString("f2");
            var arrPrice = strPrice.Split('.');
            //分
            var result = arrPrice[1].ToCharArray()[1] + chinaPriceUnit[0];
            //角
            result = arrPrice[1].ToCharArray()[0] + chinaPriceUnit[1] + result;

            var arrLeftPrice = arrPrice[0].ToList(0, string.Empty);
            arrLeftPrice.Reverse();

            for (var i = 0; i < arrLeftPrice.Count && i < 5; i++) { result = chinaPrice[arrLeftPrice[i]] + chinaPriceUnit[i + 2] + result; }
            return result;
        }

        /// <summary>
        ///     转换数字金额主函数（包括小数）
        /// </summary>
        /// <param name="str">数字字符串;转换成中文大写后的字符串或者出错信息提示字符串</param>
        public static string ConvertSum(string str)
        {
            if (!IsPositveDecimal(str)) return "输入的不是正数字！";
            if (Double.Parse(str) > 999999999999.99) return "数字太大，无法换算，请输入一万亿元以下的金额";
            var ch = new char[1];
            ch[0] = '.'; //小数点
            string[] splitstr = null; //定义按小数点分割后的字符串数组
            splitstr = str.Split(ch[0]); //按小数点分割字符串
            if (splitstr.Length == 1) //只有整数部分
                return ConvertData(str) + "元整";
            else //有小数部分
            {
                var rstr = ConvertData(splitstr[0]) + "元";
                rstr += ConvertXiaoShu(splitstr[1]); //转换小数部分
                return rstr;
            }
        }

        /// <summary>
        ///     判断是否是正数字字符串
        /// </summary>
        /// <param name="str">判断字符串;如果是数字，返回true，否则返回false</param>
        private static bool IsPositveDecimal(string str)
        {
            Decimal d;
            try { d = Decimal.Parse(str); }
            catch (Exception) {
                return false;
            }
            return d > 0;
        }

        /// <summary>
        ///     转换数字（整数）
        /// </summary>
        /// <param name="str">需要转换的整数数字字符串;转换成中文大写后的字符串</param>
        private static string ConvertData(string str)
        {
            var rstr = "";
            var strlen = str.Length;
            if (strlen <= 4) //数字长度小于四位
            { rstr = ConvertDigit(str); }
            else
            {
                var tmpstr = "";
                if (strlen <= 8) //数字长度大于四位，小于八位
                {
                    tmpstr = str.Substring(strlen - 4, 4); //先截取最后四位数字
                    rstr = ConvertDigit(tmpstr); //转换最后四位数字
                    tmpstr = str.Substring(0, strlen - 4); //截取其余数字
                    //将两次转换的数字加上万后相连接
                    rstr = String.Concat(ConvertDigit(tmpstr) + "万", rstr);
                    rstr = rstr.Replace("零零", "零");
                }
                else if (strlen <= 12) //数字长度大于八位，小于十二位
                {
                    tmpstr = str.Substring(strlen - 4, 4); //先截取最后四位数字
                    rstr = ConvertDigit(tmpstr); //转换最后四位数字
                    tmpstr = str.Substring(strlen - 8, 4); //再截取四位数字
                    rstr = String.Concat(ConvertDigit(tmpstr) + "万", rstr);
                    tmpstr = str.Substring(0, strlen - 8);
                    rstr = String.Concat(ConvertDigit(tmpstr) + "亿", rstr);
                    rstr = rstr.Replace("零亿", "亿");
                    rstr = rstr.Replace("零万", "零");
                    rstr = rstr.Replace("零零", "零");
                    rstr = rstr.Replace("零零", "零");
                }
            }
            strlen = rstr.Length;
            if (strlen >= 2)
            {
                switch (rstr.Substring(strlen - 2, 2))
                {
                    case "佰零":
                        rstr = rstr.Substring(0, strlen - 2) + "佰";
                        break;
                    case "仟零":
                        rstr = rstr.Substring(0, strlen - 2) + "仟";
                        break;
                    case "万零":
                        rstr = rstr.Substring(0, strlen - 2) + "万";
                        break;
                    case "亿零":
                        rstr = rstr.Substring(0, strlen - 2) + "亿";
                        break;
                }
            }
            return rstr;
        }

        /// <summary>
        ///     转换数字（小数部分）
        /// </summary>
        /// <param name="str">需要转换的小数部分数字字符串;转换成中文大写后的字符串</param>
        private static string ConvertXiaoShu(string str)
        {
            var strlen = str.Length;
            string rstr;
            if (strlen == 1)
            {
                rstr = ConvertChinese(str) + "角";
                return rstr;
            }
            else
            {
                var tmpstr = str.Substring(0, 1);
                rstr = ConvertChinese(tmpstr) + "角";
                tmpstr = str.Substring(1, 1);
                rstr += ConvertChinese(tmpstr) + "分";
                //rstr = rstr.Replace("零分", "");
                //rstr = rstr.Replace("零角", "");
                return rstr;
            }
        }

        /// <summary>
        ///     转换数字
        /// </summary>
        /// <param name="str">转换的字符串（四位以内）</param>
        private static string ConvertDigit(string str)
        {
            var strlen = str.Length;
            var rstr = "";
            switch (strlen)
            {
                case 1:
                    rstr = ConvertChinese(str);
                    break;
                case 2:
                    rstr = Convert2Digit(str);
                    break;
                case 3:
                    rstr = Convert3Digit(str);
                    break;
                case 4:
                    rstr = Convert4Digit(str);
                    break;
            }
            rstr = rstr.Replace("拾零", "拾");
            strlen = rstr.Length;
            return rstr;
        }

        /// <summary>
        ///     转换四位数字
        /// </summary>
        /// <param name="str"></param>
        private static string Convert4Digit(string str)
        {
            var str1 = str.Substring(0, 1);
            var str2 = str.Substring(1, 1);
            var str3 = str.Substring(2, 1);
            var str4 = str.Substring(3, 1);
            var rstring = "";
            rstring += ConvertChinese(str1) + "仟";
            rstring += ConvertChinese(str2) + "佰";
            rstring += ConvertChinese(str3) + "拾";
            rstring += ConvertChinese(str4);
            rstring = rstring.Replace("零仟", "零");
            rstring = rstring.Replace("零佰", "零");
            rstring = rstring.Replace("零拾", "零");
            rstring = rstring.Replace("零零", "零");
            rstring = rstring.Replace("零零", "零");
            rstring = rstring.Replace("零零", "零");
            return rstring;
        }

        /// <summary>
        ///     转换三位数字
        /// </summary>
        /// <param name="str"></param>
        private static string Convert3Digit(string str)
        {
            var str1 = str.Substring(0, 1);
            var str2 = str.Substring(1, 1);
            var str3 = str.Substring(2, 1);
            var rstring = "";
            rstring += ConvertChinese(str1) + "佰";
            rstring += ConvertChinese(str2) + "拾";
            rstring += ConvertChinese(str3);
            rstring = rstring.Replace("零佰", "零");
            rstring = rstring.Replace("零拾", "零");
            rstring = rstring.Replace("零零", "零");
            rstring = rstring.Replace("零零", "零");
            return rstring;
        }

        /// <summary>
        ///     转换二位数字
        /// </summary>
        /// <param name="str"></param>
        private static string Convert2Digit(string str)
        {
            var str1 = str.Substring(0, 1);
            var str2 = str.Substring(1, 1);
            var rstring = "";
            rstring += ConvertChinese(str1) + "拾";
            rstring += ConvertChinese(str2);
            rstring = rstring.Replace("零拾", "零");
            rstring = rstring.Replace("零零", "零");
            return rstring;
        }

        /// <summary>
        ///     将一位数字转换成中文大写数字
        /// </summary>
        /// <param name="str"></param>
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
            return (cstr);
        }
    }
}