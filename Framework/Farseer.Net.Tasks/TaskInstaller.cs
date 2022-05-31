using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FS.Core.Abstract.Tasks;
using FS.Reflection;

namespace FS.Tasks;

public class TaskInstaller : IWindsorInstaller
{
    public static readonly Dictionary<Type, JobAttribute> JobImpList = new();

    private readonly ITypeFinder _typeFinder;

    /// <summary>
    ///     构造函数
    /// </summary>
    public TaskInstaller(ITypeFinder typeFinder)
    {
        _typeFinder = typeFinder;
    }

    /// <summary>
    ///     初始化IOC
    /// </summary>
    /// <param name="container"> </param>
    /// <param name="store"> </param>
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
        // 业务job
        using var jobTypes = _typeFinder.Find<IJob>();
        foreach (var jobType in jobTypes)
        {
            var jobAtt = jobType.GetCustomAttribute<JobAttribute>();
            if (jobAtt == null || !jobAtt.Enable) continue;

            // 把找到的JOB实现，存到字典中，用于向服务端注册时，告知当前客户端能处理的JOB列表
            JobImpList[key: jobType] = jobAtt;
            container.Register(Component.For<IJob>().ImplementedBy(type: jobType).Named(name: $"task_job_{jobType.FullName}").LifestyleTransient());
        }
    }
}