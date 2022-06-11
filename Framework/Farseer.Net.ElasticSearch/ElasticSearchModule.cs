using System.Reflection;
using Collections.Pooled;
using FS.Cache;
using FS.DI;
using FS.Modules;
using FS.Reflection;

namespace FS.ElasticSearch
{
    public class ElasticSearchModule : FarseerModule
    {
        private readonly ITypeFinder _typeFinder;

        public ElasticSearchModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        /// <summary>
        ///     初始化
        /// </summary>
        public override void Initialize()
        {
            // 耗时：220ms，优化后：74ms
            IocManager.Container.Install(new ElasticSearchInstaller(iocResolver: IocManager));
            IocManager.RegisterAssemblyByConvention(assembly: Assembly.GetExecutingAssembly(), config: new ConventionalRegistrationConfig { InstallInstallers = false });
        }
        
        public override void PostInitialize()
        {
            // 找到Context
            using var lstContext = _typeFinder.Find(o => !o.IsGenericType && o.IsClass && o.BaseType != null && o.BaseType.BaseType != null && o.BaseType.BaseType == typeof(EsContext));

            using var lstLog = new PooledList<string>();
            foreach (var context in lstContext)
            {
                // 需要做实例化，才能初始化上下文
                InstanceCacheManger.Cache(context);
            }
        }
    }
}