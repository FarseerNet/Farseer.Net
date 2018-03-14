using System.Net;

namespace FS.Utils.Common
{
    /// <summary>
    /// IP地址工具类
    /// </summary>
    public static class IpHelper
    {
        /// <summary>
        /// 获取网络IP
        /// </summary>
        public static string GetIP()
        {
            var ip = "127.0.0.1";
            var strHostName = Dns.GetHostName();
            var ipHost = Dns.GetHostEntry(strHostName);
            foreach (var item in ipHost.AddressList) { if (item.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) { ip = item.ToString(); } }
            return ip;
        }
    }
}
