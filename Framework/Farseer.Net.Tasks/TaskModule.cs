using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FS.Core.Abstract.Tasks;
using FS.Core.LinkTrack;
using FS.DI;
using FS.Extends;
using FS.Modules;
using FS.Reflection;
using Microsoft.Extensions.Logging;

namespace FS.Tasks;

public class TaskModule : FarseerModule
{

    private readonly ITypeFinder _typeFinder;
    public TaskModule(ITypeFinder typeFinder)
    {
        _typeFinder = typeFinder;
    }

    /// <summary>
    ///     初始化
    /// </summary>
    public override void Initialize()
    {
        IocManager.Container.Install(new TaskInstaller(typeFinder: _typeFinder));
        IocManager.RegisterAssemblyByConvention(assembly: Assembly.GetExecutingAssembly(), config: new ConventionalRegistrationConfig { InstallInstallers = false });
    }

    public override void PostInitialize()
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly.ManifestModule.Name != "ReSharperTestRunner.dll")
        {
            var tasksAttribute = entryAssembly.EntryPoint.DeclaringType.GetCustomAttribute<TasksAttribute>();
            if (tasksAttribute is not { Enable: true }) return;
        }

        FarseerApplication.AddInitCallback(act: () =>
        {
            // 查找启用了Debug状态的job，立即执行
            foreach (var job in TaskInstaller.JobImpList)
                Task.Factory.StartNew(action: () =>
                {
                    // 是否启动后立即执行，否则，先休眠，再执行
                    if (!job.Value.StartupExecute && job.Value.Interval > 0) Thread.Sleep(timeout: TimeSpan.FromMilliseconds(value: job.Value.Interval));

                    while (true)
                    {
                        var sw          = Stopwatch.StartNew();
                        var taskContext = new TaskContext(jobType: job.Key, sw: sw);
                        try
                        {
                            Task.WaitAll(IocManager.Resolve<IJob>(name: $"task_job_{job.Key.FullName}").Execute(context: taskContext));
                        }
                        catch (Exception e)
                        {
                            IocManager.Logger<TaskModule>().LogError(exception: e, message: e.Message);
                        }

                        // 如果设置了下次执行时间
                        if (taskContext.NextTimespan > 0) job.Value.Interval = taskContext.NextTimespan - DateTime.Now.ToTimestamps();
                        // 间隔执行
                        if (job.Value.Interval > 0) Thread.Sleep(timeout: TimeSpan.FromMilliseconds(value: job.Value.Interval));
                    }
                }, creationOptions: TaskCreationOptions.LongRunning);
        });
    }
}