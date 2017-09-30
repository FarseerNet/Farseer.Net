using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;

namespace Farseer.Net.DI
{
	/// <summary>
	///     约定注册器
	///     注册实现ITransientDependency、ISingletonDependency、IPerRequestDependency的类型
	/// </summary>
	public class BasicConventionalRegistrar : IConventionalDependencyRegistrar
	{
		/// <summary>
		///     注册程序集
		/// </summary>
		/// <param name="context"></param>
		public void RegisterAssembly(IConventionalRegistrationContext context)
		{
			//注册临时对象
			context.IocManager.Container.Register(Classes.FromAssembly(context.Assembly).IncludeNonPublicTypes().BasedOn<ITransientDependency>().WithService.Self().WithService.DefaultInterfaces().LifestyleTransient());

			// 注册单例对象
			context.IocManager.Container.Register(Classes.FromAssembly(context.Assembly).IncludeNonPublicTypes().BasedOn<ISingletonDependency>().WithService.Self().WithService.DefaultInterfaces().LifestyleSingleton());
#if !CORE
            // 注册每个请求对象
            context.IocManager.Container.Register(Classes.FromAssembly(context.Assembly).IncludeNonPublicTypes().BasedOn<IPerRequestDependency>().WithService.Self().WithService.DefaultInterfaces().LifestylePerWebRequest());
#endif
			// 注册拦截器
			context.IocManager.Container.Register(Classes.FromAssembly(context.Assembly).IncludeNonPublicTypes().BasedOn<IInterceptor>().WithService.Self().LifestyleTransient());
		}
	}
}