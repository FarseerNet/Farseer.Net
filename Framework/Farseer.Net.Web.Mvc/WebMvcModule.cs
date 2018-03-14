using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using FS.DI;
using FS.Modules;

namespace Farseer.Net.Web.Mvc
{
    /// <summary>
    /// This module is used to build ASP.NET MVC web sites using Abp.
    /// </summary>
    [DependsOn()]
    public class WebMvcModule : FarseerModule
    {
        /// <inheritdoc/>
        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(new ControllerConventionalRegistrar());
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            System.Web.Mvc.ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(IocManager));
        }

        /// <inheritdoc/>
        public override void PostInitialize()
        {
            //var startAt = DateTime.Now;
            var assemblys = System.Web.Compilation.BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToArray();
            var lstRegisterAssemblys = new List<Assembly>();
            foreach (var assembly in assemblys)
            {
                var assTypes = assembly.GetTypes();
                // 包括FarseerModule、IWindsorInstaller的程序集，跳过
                if (assTypes.Any(o => (typeof(FarseerModule).IsAssignableFrom(o)) || typeof(IWindsorInstaller).IsAssignableFrom(o)))
                {
                    if (assTypes.Any(o => typeof(System.Web.Mvc.Controller).IsAssignableFrom(o))) { lstRegisterAssemblys.Add(assembly); }
                    continue;
                }

                // 是否含Dependency接口
                if (!assTypes.Any(o => typeof(IPerRequestDependency).IsAssignableFrom(o) || typeof(ISingletonDependency).IsAssignableFrom(o) || typeof(ITransientDependency).IsAssignableFrom(o))) { continue; }

                // 符合条件的，加入到程序集
                lstRegisterAssemblys.Add(assembly);
            }

            // 注册
            foreach (var assembly in lstRegisterAssemblys) { IocManager.RegisterAssemblyByConvention(assembly); }
            // 重置MVC控制器
            System.Web.Mvc.ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(IocManager));
            //throw new Exception((DateTime.Now - startAt).Milliseconds.ToString());
        }
    }
}