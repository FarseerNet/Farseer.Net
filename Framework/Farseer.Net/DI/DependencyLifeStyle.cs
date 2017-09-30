namespace Farseer.Net.DI
{
    /// <summary>
    ///     类型生命周期
    /// </summary>
    public enum DependencyLifeStyle
    {
        /// <summary>
        ///     单例
        /// </summary>
        Singleton,

        /// <summary>
        ///     每个请求一个对象
        /// </summary>
        PerRequest,

        /// <summary>
        ///     临时对象
        /// </summary>
        Transient
    }
}