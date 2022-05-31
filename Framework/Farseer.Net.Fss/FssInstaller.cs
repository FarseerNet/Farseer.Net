using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Collections.Pooled;
using FS.Core.Abstract.Fss;
using FS.Reflection;

namespace FS.Fss
{
    public class FssInstaller : IWindsorInstaller
    {
        public static readonly PooledDictionary<string, Type> JobImpList = new();

        private readonly ITypeFinder _typeFinder;

        /// <summary>
        ///     构造函数
        /// </summary>
        public FssInstaller(ITypeFinder typeFinder)
        {
            _typeFinder  = typeFinder;
        }

        /// <summary>
        ///     初始化IOC
        /// </summary>
        /// <param name="container"> </param>
        /// <param name="store"> </param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // 业务job
            using var types = _typeFinder.Find<IFssJob>();
            foreach (var jobType in types)
            {
                var fssJobAttribute = jobType.GetCustomAttribute<FssJobAttribute>();
                if (fssJobAttribute == null || !fssJobAttribute.Enable) continue;

                // 把找到的JOB实现，存到字典中，用于向服务端注册时，告知当前客户端能处理的JOB列表
                JobImpList[key: fssJobAttribute.Name] = jobType;
                container.Register(Component.For<IFssJob>().ImplementedBy(type: jobType).Named(name: $"fss_job_{fssJobAttribute.Name}").LifestyleTransient());
            }
        }
    }
}