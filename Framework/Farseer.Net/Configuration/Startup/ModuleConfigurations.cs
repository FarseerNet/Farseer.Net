namespace FS.Configuration.Startup
{
    /// <summary>
    ///     模块配置
    /// </summary>
    internal class ModuleConfigurations : IModuleConfigurations
    {
        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="abpConfiguration"> </param>
        public ModuleConfigurations(IFarseerStartupConfiguration abpConfiguration)
        {
            Configuration = abpConfiguration;
        }

        /// <summary>
        ///     系统启动配置
        /// </summary>
        public IFarseerStartupConfiguration Configuration { get; }
    }
}