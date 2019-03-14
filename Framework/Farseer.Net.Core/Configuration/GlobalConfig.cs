// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2017-04-07 15:59
// ********************************************

using FS.Configuration;

namespace FS.Core.Configuration
{
    /// <summary>
    /// 全局配置
    /// </summary>
    public class GlobalConfig : IFarseerConfig
    {
        /// <summary> 应用名称 </summary>
        public string AppName { get; set; }

        /// <summary> Api网关环境 </summary>
        public EumApiGatewayType ApiGatewayType { get; set; }
    }
}