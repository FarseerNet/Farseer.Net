using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Farseer.Net.Configuration.Format;

namespace Farseer.Net.Configuration
{
    /// <summary>
    ///     系统核心组件注册类
    /// </summary>
    public class ConfigurationInstaller : IWindsorInstaller
    {
        /// <summary>
        ///     注册
        /// </summary>
        /// <param name="container">容器</param>
        /// <param name="store"></param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // 注册核心组件到依赖注入容器中，包括配置。
            container.Register(
            Component.For<IConfigFormat, JsonConfigFormat>().ImplementedBy<JsonConfigFormat>().LifestyleSingleton(),
            Component.For<IConfigResolver, LocalConfigResolver>().ImplementedBy<LocalConfigResolver>().LifestyleSingleton().Named("LocalConfigResolver"));
        }
    }
}