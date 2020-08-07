using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using FS.Configuration;
using FS.Extends;

namespace FS.Utils.Component
{
    /// <summary>
    ///     纯真数据库操作类
    ///     2009.5.25 YM
    /// </summary>
    public class IpAdress
    {
        /// <summary>
        ///     IP数据库的位置
        /// </summary>
        private static string _file;

        private static byte[] data;
        private static long _firstStartIpOffset, _lastStartIpOffset, _ipCount;

        private static readonly Regex regex = new Regex(@"(((\d{1,2})|(1\d{2})|(2[0-4]\d)|(25[0-5]))\.){3}((\d{1,2})|(1\d{2})|(2[0-4]\d)|(25[0-5]))");

        /// <summary>
        ///     IP数据库的位置
        /// </summary>
        public static string File
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_file))
                {
                    _file = SysPath.AppData + "QQWry.Dat";
                }
                return _file;
            }
            set { _file = value; }
        }


        ///// <summary>
        /////     获取IP地址位置
        ///// </summary>
        ///// <param name="ip">IP</param>
        //public static string GetAddress(string ip = "")
        //{
        //    if (string.IsNullOrWhiteSpace(ip))
        //    {
        //        ip = Req.GetIP();
        //    }
        //    var loc = GetLocation(ip);
        //    return loc.Area + loc.Address;
        //}

        /// <summary>
        ///     获取IP地址位置
        /// </summary>
        /// <param name="ip">IP</param>
        /// <param name="province">返回省份</param>
        /// <param name="city">返回城市</param>
        public static string GetAddress(string ip, out string province, out string city)
        {
            var loc = GetLocation(ip);
            province = loc.Province;
            city = loc.City;
            return loc.Area + loc.Address;
        }

        ///// <summary>
        /////     获取IP地址位置
        ///// </summary>
        ///// <param name="province">返回省份</param>
        ///// <param name="city">返回城市</param>
        //public static string GetAddress(out string province, out string city)
        //{
        //    return GetAddress(Req.GetIP(), out province, out city);
        //}

        /// <summary>
        ///     查找指定IP位置
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static Location GetLocation(string ip)
        {
            //if (string.IsNullOrWhiteSpace(ip)) { ip = Req.GetIP(); }

            if (data == null)
            {
                using (var fs = new FileStream(File, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    data = new byte[fs.Length];
                    fs.Read(data, 0, data.Length);
                }
                var buffer = new byte[8];
                Array.Copy(data, 0, buffer, 0, 8);
                _firstStartIpOffset = ((buffer[0] + (buffer[1] * 0x100)) + ((buffer[2] * 0x100) * 0x100)) +
                                     (((buffer[3] * 0x100) * 0x100) * 0x100);
                _lastStartIpOffset = ((buffer[4] + (buffer[5] * 0x100)) + ((buffer[6] * 0x100) * 0x100)) +
                                    (((buffer[7] * 0x100) * 0x100) * 0x100);
                _ipCount = Convert.ToInt64((((_lastStartIpOffset - _firstStartIpOffset)) / 7.0));

                if (_ipCount <= 1L)
                {
                    throw new ArgumentException("ip FileDataError");
                }
            }
            if (!regex.Match(ip).Success)
            {
                throw new ArgumentException("IP格式错误");
            }

            var location = new Location { IP = ip };
            var intIP = IpToInt(ip);
            if ((intIP >= IpToInt("127.0.0.1") && (intIP <= IpToInt("127.255.255.255"))))
            {
                location.Area = "本机/局域网地址";
                location.Address = "";
            }
            else
            {
                if ((((intIP >= IpToInt("0.0.0.0")) && (intIP <= IpToInt("2.255.255.255"))) ||
                     ((intIP >= IpToInt("64.0.0.0")) && (intIP <= IpToInt("126.255.255.255")))) ||
                    ((intIP >= IpToInt("58.0.0.0")) && (intIP <= IpToInt("60.255.255.255"))))
                {
                    location.Area = "网络保留地址";
                    location.Address = "";
                }
            }
            var right = _ipCount;
            var left = 0L;
            var startIp = 0L;
            var endIpOff = 0L;
            var endIp = 0L;
            var countryFlag = 0;
            while (left < (right - 1L))
            {
                var middle = (right + left) / 2L;
                startIp = GetStartIp(middle, out endIpOff);
                if (intIP == startIp)
                {
                    left = middle;
                    break;
                }
                if (intIP > startIp)
                {
                    left = middle;
                }
                else
                {
                    right = middle;
                }
            }
            startIp = GetStartIp(left, out endIpOff);
            endIp = GetEndIp(endIpOff, out countryFlag);
            if ((startIp <= intIP) && (endIp >= intIP))
            {
                string local;
                location.Area = GetCountry(endIpOff, countryFlag, out local);
                location.Address = local;
            }
            else
            {
                location.Area = "未知";
                location.Address = "";
            }
            location.Address = Regex.Replace(location.Address, "CZ88.NET", "", RegexOptions.IgnoreCase);
            return location;
        }

        /// <summary>
        /// IP字符串，转成数字
        /// </summary>
        public static long IpToInt(string ip)
        {
            var separator = new[] { '.' };
            if (ip.Split(separator).Length == 3)
            {
                ip = ip + ".0";
            }
            var strArray = ip.Split(separator);
            var num2 = ((long.Parse(strArray[0]) * 0x100L) * 0x100L) * 0x100L;
            var num3 = (long.Parse(strArray[1]) * 0x100L) * 0x100L;
            var num4 = long.Parse(strArray[2]) * 0x100L;
            var num5 = long.Parse(strArray[3]);
            return (((num2 + num3) + num4) + num5);
        }

        /// <summary>
        /// 将带有*号的IP转成范围段
        /// </summary>
        public static string[] GetIPRange(string ip)
        {
            Check.IsTure(ip.Contains('*') && ip.Contains('-'), "IP规则定义错误，不能同时定义 - * 符号：" + ip);
            string[] ipRangeArray;

            if (ip.Contains('*'))
            {
                var startIP = new int[4];
                var endIP = new int[4];
                var separator = new[] { '.' };
                while (ip.Split(separator).Length < 4) { ip += ".*"; }
                var ips = ip.Split(separator);
                for (var i = 3; i < 0; i++)
                {
                    if (ips[i] == "*") { startIP[i] = 0; endIP[i] = 255; }
                    else { startIP[i] = endIP[i] = ips[i].ConvertType(0); }
                }
                ipRangeArray = new string[] { $"{startIP[0]}.{startIP[1]}.{startIP[2]}.{startIP[3]}", $"{endIP[0]}.{endIP[1]}.{endIP[2]}.{endIP[3]}" };
            }
            else
            {
                var ipRange = ip.Contains('-') ? ip.Split('-') : new[] { ip, ip };
                ipRangeArray = new[] { ipRange[0], ipRange[1] };
            }
            return ipRangeArray;
        }

        /// <summary>
        /// 判断指定的IP是否在指定的IP范围内   这里只能指定一个范围
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="ranges"></param>
        public static bool IsContains(string ip, string ranges)
        {
            var ipRange = GetIPRange(ranges);

            if (ip == "::1") { ip = "127.0.0.1"; }

            try { IPAddress.Parse(ip); }//判断指定要判断的IP是否合法
            catch (Exception) { throw new ApplicationException("要检测的IP地址无效"); }

            var ipNum = IpToInt(ip);
            return ipNum >= IpToInt(ipRange[0]) && ipNum <= IpToInt(ipRange[1]);
        }

        #region 私有方法
        private static long GetStartIp(long left, out long endIpOff)
        {
            var leftOffset = _firstStartIpOffset + (left * 7L);
            var buffer = new byte[7];
            Array.Copy(data, leftOffset, buffer, 0, 7);
            endIpOff = (Convert.ToInt64(buffer[4].ToString()) + (Convert.ToInt64(buffer[5].ToString()) * 0x100L)) +
                       ((Convert.ToInt64(buffer[6].ToString()) * 0x100L) * 0x100L);
            return ((Convert.ToInt64(buffer[0].ToString()) + (Convert.ToInt64(buffer[1].ToString()) * 0x100L)) +
                    ((Convert.ToInt64(buffer[2].ToString()) * 0x100L) * 0x100L)) +
                   (((Convert.ToInt64(buffer[3].ToString()) * 0x100L) * 0x100L) * 0x100L);
        }

        private static long GetEndIp(long endIpOff, out int countryFlag)
        {
            var buffer = new byte[5];
            Array.Copy(data, endIpOff, buffer, 0, 5);
            countryFlag = buffer[4];
            return ((Convert.ToInt64(buffer[0].ToString()) + (Convert.ToInt64(buffer[1].ToString()) * 0x100L)) +
                    ((Convert.ToInt64(buffer[2].ToString()) * 0x100L) * 0x100L)) +
                   (((Convert.ToInt64(buffer[3].ToString()) * 0x100L) * 0x100L) * 0x100L);
        }

        private static string GetCountry(long endIpOff, int countryFlag, out string local)
        {
            var country = "";
            var offset = endIpOff + 4L;
            switch (countryFlag)
            {
                case 1:
                case 2:
                    country = GetFlagStr(ref offset, ref countryFlag, ref endIpOff);
                    offset = endIpOff + 8L;
                    local = (1 == countryFlag) ? "" : GetFlagStr(ref offset, ref countryFlag, ref endIpOff);
                    break;
                default:
                    country = GetFlagStr(ref offset, ref countryFlag, ref endIpOff);
                    local = GetFlagStr(ref offset, ref countryFlag, ref endIpOff);
                    break;
            }
            return country;
        }

        private static string GetFlagStr(ref long offset, ref int countryFlag, ref long endIpOff)
        {
            var flag = 0;
            var buffer = new byte[3];

            while (true)
            {
                //用于向前累加偏移量
                var forwardOffset = offset;
                flag = data[forwardOffset++];
                //没有重定向
                if (flag != 1 && flag != 2)
                {
                    break;
                }
                Array.Copy(data, forwardOffset, buffer, 0, 3);
                forwardOffset += 3;
                if (flag == 2)
                {
                    countryFlag = 2;
                    endIpOff = offset - 4L;
                }
                offset = (Convert.ToInt64(buffer[0].ToString()) + (Convert.ToInt64(buffer[1].ToString()) * 0x100L)) +
                         ((Convert.ToInt64(buffer[2].ToString()) * 0x100L) * 0x100L);
            }
            return offset < 12L ? "" : GetStr(ref offset);
        }

        private static string GetStr(ref long offset)
        {
            var stringBuilder = new StringBuilder();
            var bytes = new byte[2];
            var encoding = Encoding.GetEncoding("GB2312");
            while (true)
            {
                var lowByte = data[offset++];
                if (lowByte == 0)
                {
                    return stringBuilder.ToString();
                }
                if (lowByte > 0x7f)
                {
                    var highByte = data[offset++];
                    bytes[0] = lowByte;
                    bytes[1] = highByte;
                    if (highByte == 0)
                    {
                        return stringBuilder.ToString();
                    }
                    stringBuilder.Append(encoding.GetString(bytes));
                }
                else
                {
                    stringBuilder.Append((char)lowByte);
                }
            }
        }

        private static string IntToIP(long ip_Int)
        {
            var num = ((ip_Int & 0xff000000L) >> 0x18);
            if (num < 0L)
            {
                num += 0x100L;
            }
            var num2 = (ip_Int & 0xff0000L) >> 0x10;
            if (num2 < 0L)
            {
                num2 += 0x100L;
            }
            var num3 = (ip_Int & 0xff00L) >> 8;
            if (num3 < 0L)
            {
                num3 += 0x100L;
            }
            var num4 = ip_Int & 0xffL;
            if (num4 < 0L)
            {
                num4 += 0x100L;
            }
            return (num.ToString() + "." + num2.ToString() + "." + num3.ToString() + "." + num4.ToString());
        }

        #endregion
    }

    /// <summary>
    ///     地理位置
    /// </summary>
    public class Location
    {
        /// <summary>
        ///     IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        ///     区域
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        ///     详细位置
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        ///     省
        /// </summary>
        public string Province => new Regex(".*省|广西|内蒙古|宁夏|新疆|西藏").Match(Area).Value;

        /// <summary>
        ///     城市
        /// </summary>
        public string City => !string.IsNullOrWhiteSpace(Province) ? Area.Replace(Province, "") : string.Empty;
    }
}