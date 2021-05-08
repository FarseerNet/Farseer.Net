using System;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FS.Configuration.Startup;
using FS.Modules;
using FS.Reflection;

namespace FS.DI.Installers
{
    /// <summary>
    ///     系统核心组件注册类（所有组件最先执行的注册器）
    /// </summary>
    public class FarseerInstaller : IWindsorInstaller
    {
        /// <summary>
        ///     注册
        /// </summary>
        /// <param name="container">容器</param>
        /// <param name="store"></param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // 注册核心组件到依赖注入容器中，包括配置。
            if (!IocManager.Instance.IsRegistered<IModuleConfigurations>()) container.Register(Component.For<IModuleConfigurations, ModuleConfigurations>().ImplementedBy<ModuleConfigurations>().LifestyleSingleton());
            if (!IocManager.Instance.IsRegistered<IFarseerStartupConfiguration>()) container.Register(Component.For<IFarseerStartupConfiguration, FarseerStartupConfiguration>().ImplementedBy<FarseerStartupConfiguration>().LifestyleSingleton());
            if (!IocManager.Instance.IsRegistered<ITypeFinder>()) container.Register(Component.For<ITypeFinder, TypeFinder>().ImplementedBy<TypeFinder>().LifestyleSingleton());
            if (!IocManager.Instance.IsRegistered<IFarseerModuleManager>()) container.Register(Component.For<IFarseerModuleManager, FarseerModuleManager>().ImplementedBy<FarseerModuleManager>().LifestyleSingleton());
            if (!IocManager.Instance.IsRegistered<IAssemblyFinder>()) container.Register(Component.For<IAssemblyFinder, AssemblyFinder>().ImplementedBy<AssemblyFinder>().LifestyleSingleton());
        }
    }
}