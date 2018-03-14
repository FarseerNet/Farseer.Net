namespace FS.Configuration.Startup
{
    /// <summary>
    /// 模块配置接口
    /// </summary>
    public interface IModuleConfigurations
    {
        /// <summary>
        /// 系统全局配置接口
        /// </summary>
        IFarseerStartupConfiguration Configuration { get; }
    }
}