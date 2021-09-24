using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FS.DI;
using FS.Reflection;

namespace FS.Job
{
    public class JobInstaller : IWindsorInstaller
    {
        public static readonly Dictionary<string, Type> JobImpList = new();

        /// <summary>
        ///     依赖获取接口
        /// </summary>
        private readonly IIocResolver _iocResolver;

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="iocResolver"> </param>
        public JobInstaller(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        /// <summary>
        ///     初始化IOC
        /// </summary>
        /// <param name="container"> </param>
        /// <param name="store"> </param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // 业务job
            var types = container.Resolve<IAssemblyFinder>().GetType<IFssJob>();
            foreach (var jobType in types)
            {
                var fssJobAttribute = jobType.GetCustomAttribute<FssJobAttribute>();
                if (fssJobAttribute == null || !fssJobAttribute.Enable) return;

                // 把找到的JOB实现，存到字典中，用于向服务端注册时，告知当前客户端能处理的JOB列表
                JobImpList[key: fssJobAttribute.Name] = jobType;
                container.Register(Component.For<IFssJob>().ImplementedBy(type: jobType).Named(name: $"fss_job_{fssJobAttribute.Name}").LifestyleTransient());
            }
        }
    }
}