using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;

namespace FS.DI;

/// <summary>
///     约定注册器
///     注册实现ITransientDependency、ISingletonDependency、IPerRequestDependency的类型
/// </summary>
public class BasicConventionalRegistrarInstaller : IConventionalDependencyRegistrar
{
    /// <summary>
    ///     注册程序集
    /// </summary>
    /// <param name="context"> </param>
    public void RegisterAssembly(IConventionalRegistrationContext context)
    {
        //注册临时对象
        context.IocManager.Container.Register(Classes.FromAssembly(assembly: context.Assembly).IncludeNonPublicTypes().BasedOn<ITransientDependency>().WithService.Self().WithService.DefaultInterfaces().LifestyleTransient());

        // 注册单例对象
        context.IocManager.Container.Register(Classes.FromAssembly(assembly: context.Assembly).IncludeNonPublicTypes().BasedOn<ISingletonDependency>().WithService.Self().WithService.DefaultInterfaces().LifestyleSingleton());

        // 注册拦截器
        context.IocManager.Container.Register(Classes.FromAssembly(assembly: context.Assembly).IncludeNonPublicTypes().BasedOn<IInterceptor>().WithService.Self().LifestyleTransient());
    }
}