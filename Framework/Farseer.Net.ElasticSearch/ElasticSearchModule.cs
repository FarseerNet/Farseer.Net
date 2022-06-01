using FS.Modules;

namespace FS.ElasticSearch
{
    public class ElasticSearchModule : FarseerModule
    {
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
            // 耗时：220ms，优化后：74ms
            IocManager.Container.Install(new ElasticSearchInstaller(iocResolver: IocManager));
        }
    }
}