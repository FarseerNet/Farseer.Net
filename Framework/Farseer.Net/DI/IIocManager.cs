using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.Core;
using Castle.Windsor;
using Microsoft.Extensions.Logging;

namespace FS.DI
{
    /// <summary>
    ///     依赖注入管理器，用来执行依赖注入任务
    /// </summary>
    public interface IIocManager : IIocRegistrar, IIocResolver, IDisposable
    {
        /// <summary>
        ///     依赖注入容器
        /// </summary>
        IWindsorContainer Container { get; }

        /// <summary>
        ///     日志接口
        /// </summary>
        ILogger Logger<T>();

        /// <summary>
        ///     是否注册
        /// </summary>
        /// <param name="type"> </param>
        /// <returns> </returns>
        new bool IsRegistered(Type type);

        /// <summary>
        ///     是否注册
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <returns> </returns>
        new bool IsRegistered<T>();

        /// <summary>
        ///     根据约定注册程序集
        /// </summary>
        void RegisterAssemblyByConvention(IEnumerable<Assembly> assemblys);

        /// <summary>
        ///     注册
        /// </summary>
        /// <param name="name"> </param>
        /// <returns> </returns>
        bool IsRegistered(string name);

        /// <summary>
        ///     获取当前业务注册的IOC
        /// </summary>
        /// <returns> </returns>
        List<ComponentModel> GetCustomComponent();
        /// <summary>
        ///     注册
        /// </summary>
        /// <param name="instance">实例 </param>
        /// <param name="lifeStyle">实例生命周期</param>
        void Register<T>(T instance, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton) where T : class;
    }
}