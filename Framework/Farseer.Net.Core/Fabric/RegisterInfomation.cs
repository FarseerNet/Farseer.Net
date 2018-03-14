// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-03-01 13:10
// ********************************************

using FS.Core.Net.Gateway;

namespace FS.Core.Fabric
{
    /// <summary>
    /// Fabric注册信息
    /// </summary>
    public class RegisterInfomation
    {
        /// <summary>
        /// 服务注册别名
        /// </summary>
        public const string ApplicationServiceName = "fabric:/Farseer.ServiceManager.Ms/WCFRegister";

        /// <summary>
        /// 注册信息
        /// </summary>
        public static RegisterServiceVO Register { get; set; }
    }
}