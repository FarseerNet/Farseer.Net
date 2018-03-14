using Castle.MicroKernel.Registration;
using System.Web.Mvc;
using FS.DI;

namespace Farseer.Net.Web.Mvc
{
    /// <summary>
    ///     Registers all MVC Controllers derived from <see cref="Controller" />.
    /// </summary>
    public class ControllerConventionalRegistrar : IConventionalDependencyRegistrar
    {
        /// <inheritdoc />
        public void RegisterAssembly(IConventionalRegistrationContext context)
        {
            context.IocManager.Container.Register(Classes.FromAssembly(context.Assembly).BasedOn<Controller>().LifestyleTransient());
        }
    }
}