using System.Reflection;
using FS.Configuration;
using FS.DI;
using FS.Job.ActService;
using FS.Modules;

namespace FS.Job
{
    public class JobModule : FarseerModule
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
            IocManager.Container.Install(new JobInstaller(IocManager));
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(), new ConventionalRegistrationConfig {InstallInstallers = false});
            new JobFinder().RegisterJob();
            new Menu().CreateMenu();
            LazyExecute.Init();
        }

        public override void PostInitialize()
        {
            new JobManager().Run();
        }
    }
}