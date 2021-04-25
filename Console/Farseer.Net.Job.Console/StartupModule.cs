using Farseer.Net.Grpc;
using FS.Job;
using FS.Modules;

namespace Farseer.Net.Job.Console
{
    /// <summary>
    /// 启动模块
    /// </summary>
    [DependsOn(typeof(GrpcModule), typeof(JobModule))] // 依赖Grpc模块、Job模块
    public class StartupModule : FarseerModule
    {
        public override void PreInitialize()
        {
        }
    }
}