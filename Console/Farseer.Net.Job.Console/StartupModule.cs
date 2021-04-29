using FS.Job;
using FS.Modules;

namespace Farseer.Net.Job.Console
{
    /// <summary>
    /// 启动模块
    /// </summary>
    [DependsOn(typeof(JobModule))] // 依赖Job模块
    public class StartupModule : FarseerModule
    {
        public override void PreInitialize()
        {
        }
    }
}