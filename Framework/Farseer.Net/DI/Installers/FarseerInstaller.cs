using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FS.Modules;
using FS.Reflection;

namespace FS.DI.Installers;

/// <summary>
///     系统核心组件注册类（所有组件最先执行的注册器）
/// </summary>
public class FarseerInstaller : IWindsorInstaller
{
    /// <summary>
    ///     注册
    /// </summary>
    /// <param name="container"> 容器 </param>
    /// <param name="store"> </param>
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
        // 注册核心组件到依赖注入容器中，包括配置。
        container.Register(Component.For<ITypeFinder, TypeFinder>().ImplementedBy<TypeFinder>().LifestyleSingleton());
        container.Register(Component.For<IFarseerModuleManager, FarseerModuleManager>().ImplementedBy<FarseerModuleManager>().LifestyleSingleton());
        container.Register(Component.For<IAssemblyFinder, AssemblyFinder>().ImplementedBy<AssemblyFinder>().LifestyleSingleton());
    }
}