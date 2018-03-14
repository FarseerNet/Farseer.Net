using System.Collections.Generic;
using System.Linq;

using System.Management;

namespace FS.Utils.Common
{
    /// <summary>
    ///     机器硬件信息
    /// </summary>
    public class Mac
    {
        /// <summary>
        ///     获取CPU码
        /// </summary>
        /// <returns></returns>
        public static List<string> GetCpu()
        {
            var mc = new ManagementClass("Win32_Processor");
            return (from ManagementObject mo in mc.GetInstances() select mo.Properties["ProcessorId"].Value.ToString()).ToList();
        }

        /// <summary>
        ///     获取硬盘码
        /// </summary>
        /// <returns></returns>
        public static List<string> GetHd()
        {
            var mc = new ManagementClass("Win32_DiskDrive");
            return (from ManagementObject mo in mc.GetInstances() select mo.Properties["Model"].Value.ToString()).ToList();
        }

        /// <summary>
        ///     获取网卡码
        /// </summary>
        /// <returns></returns>
        public static List<string> GetMac()
        {
            var lst = new List<string>();
            var mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            foreach (var o in mc.GetInstances())
            {
                var mo = (ManagementObject) o;
                if ((bool) mo["IPEnabled"]) { lst.Add(mo["MacAddress"].ToString()); }
                mo.Dispose();
            }
            return lst;
        }
    }
}