using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace FS.Utils.Common
{
    /// <summary>
    ///     IP地址工具类
    /// </summary>
    public static class IpHelper
    {
        private static string[] _localIpList;

        /// <summary>
        ///     获取当前节点IP
        /// </summary>
        public static string GetIp => _localIpList.FirstOrDefault();
        
        /// <summary>
        ///     获取当前节点IP
        /// </summary>
        public static string[] GetIpList => _localIpList ??= GetIps().Select(selector: o => o.Address.MapToIPv4().ToString()).ToArray();

        // /// <summary>
        // /// 获取网络IP
        // /// </summary>
        // public static string GetIP()
        // {
        //     var ip = "127.0.0.1";
        //     var strHostName = Dns.GetHostName();
        //     var ipHost = Dns.GetHostEntry(strHostName);
        //     foreach (var item in ipHost.AddressList) { if (item.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) { ip = item.ToString(); } }
        //     return ip;
        // }

        /// <summary>
        ///     获取IP
        /// </summary>
        /// <returns> </returns>
        public static UnicastIPAddressInformation[] GetIps()
        {
            return NetworkInterface
                   .GetAllNetworkInterfaces()
                   .Where(predicate: network => network.OperationalStatus == OperationalStatus.Up)
                   .Select(selector: network => network.GetIPProperties())
                   .OrderByDescending(keySelector: properties => properties.GatewayAddresses.Count)
                   .SelectMany(selector: properties => properties.UnicastAddresses)
                   .Where(predicate: address => !IPAddress.IsLoopback(address: address.Address) && address.Address.AddressFamily == AddressFamily.InterNetwork)
                   .ToArray();
        }
    }
}