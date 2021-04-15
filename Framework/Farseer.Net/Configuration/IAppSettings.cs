namespace FS.Configuration
{
    /// <summary>
    /// 用于支持.net core appsetting.json文件的反序化
    /// </summary>
    public interface IAppSettings
    {
        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="config">配置的对象实体类</param>
        /// <typeparam name="TConfig">用于反序列化的实体对象</typeparam>
        TConfig Get<TConfig>(TConfig config);
    }
}