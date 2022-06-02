using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Collections.Pooled;
using FS.Data.Abstract;
using FS.Data.Internal;
using FS.DI;
using FS.Modules;
using FS.Reflection;
using Microsoft.Extensions.Logging;
using Activator = FS.Reflection.Activator;

namespace FS.Data
{
    /// <summary>
    ///     Db模块
    /// </summary>
    public class DataModule : FarseerModule
    {
        private readonly ITypeFinder _typeFinder;

        public DataModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        /// <summary>
        ///     初始化之前
        /// </summary>
        public override void PreInitialize()
        {
        }
        /// <summary>
        ///     初始化
        /// </summary>
        public override void Initialize()
        {
            IocManager.Container.Install(new DataInstaller());
            IocManager.RegisterAssemblyByConvention(assembly: Assembly.GetExecutingAssembly(), config: new ConventionalRegistrationConfig { InstallInstallers = false });
        }

        public override void PostInitialize()
        {
            using var lstPoType = new PooledList<Type>();
            // 找到Context
            using var lstContext = _typeFinder.Find(o => !o.IsGenericType && o.IsClass && o.BaseType != null && o.BaseType.BaseType != null && o.BaseType.BaseType == typeof(DbContext));

            using var lstLog = new PooledList<string>();
            foreach (var context in lstContext)
            {
                // 需要做实例化，才能初始化上下文
                Activator.CreateInstance(context);
                // 找到Set属性
                var set       = context.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                var lstPOType = set.Select(o => o.PropertyType).Where(o => o.GetInterfaces().Any(i => i == typeof(IDbSet))).Select(o => o.GenericTypeArguments[0]);
                lstPoType.AddRange(lstPOType);
            }

            var sw = Stopwatch.StartNew();
            DynamicCompilationEntity.Instance.BuildEntities(lstPoType);
            IocManager.Logger<FarseerApplication>().LogInformation($"编译{lstPoType.Count}个PO类，共耗时{sw.ElapsedMilliseconds:n} ms");
        }
    }
}