using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

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

        /// <summary>
        /// 获取IP
        /// </summary>
        /// <returns></returns>
        public static UnicastIPAddressInformation[] GetIps()
        {
            return NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(network => network.OperationalStatus == OperationalStatus.Up)
                .Select(network => network.GetIPProperties())
                .OrderByDescending(properties => properties.GatewayAddresses.Count)
                .SelectMany(properties => properties.UnicastAddresses)
                .Where(address => !IPAddress.IsLoopback(address.Address) && address.Address.AddressFamily == AddressFamily.InterNetwork)
                .ToArray();
        }
    }
}
