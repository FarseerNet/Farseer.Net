namespace FS.Core.Configuration
{
    /// <summary>
    /// Api网关环境
    /// </summary>
    public enum EumApiGatewayType
    {
        /// <summary>
        /// 生产环境
        /// </summary>
        External,
        /// <summary>
        /// 开发环境
        /// </summary>
        Dev,
        /// <summary>
        /// 测试环境
        /// </summary> 
        Test,
        /// <summary>
        /// 预发布环境
        /// </summary>
        Stg,
        /// <summary>
        /// 内部环境（内部域名到生产环境）
        /// </summary>
        Inside
    }
}