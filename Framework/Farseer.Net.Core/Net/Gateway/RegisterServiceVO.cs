// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-02-23 14:29
// ********************************************
namespace FS.Core.Net.Gateway
{
    /// <summary>
    /// 注册服务包
    /// </summary>
    public class RegisterServiceVO
    {
        /// <summary>
        /// 系统名称
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// 系统对应URL
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 应用类型名字
        /// </summary>
        public string ApplicationTypeName { get; set; }

        /// <summary>
        /// 服务类型名
        /// </summary>
        public string ServiceTypeName { get; set; }

        /// <summary>
        /// 服务的唯一标示名称 应用类型名/服务类型名
        /// </summary>
        public string AppServiceTypeName => $"{ApplicationTypeName}/{ServiceTypeName}";

        /// <summary>
        /// 用于效验网关到服务的密钥
        /// </summary>
        public string ServiceKey { get; set; }

        /// <summary>
        /// 服务接口版本
        /// </summary>
        public string ServiceVer { get; set; }

        /// <summary>
        /// 默认入参格式
        /// </summary>
        public string DefaultMediaType { get; set; } = "application/json";

        /// <summary>
        /// 通讯方式
        /// </summary>
        public EumConnectType ConnectType { get; set; } = EumConnectType.Wcf;
        /// <summary>
        /// 节点名称
        /// </summary>
        public string NodeName { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 预热进度(预留字段,下一步灰度预热使用)
        /// </summary>
        public int ProgressStatus { get; set; } 

    }
}