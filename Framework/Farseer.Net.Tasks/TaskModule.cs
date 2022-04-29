﻿using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FS.DI;
using FS.Extends;
using FS.Modules;
using FS.Reflection;
using FS.Tasks.Entity;
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
        AppContext.SetSwitch(switchName: "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", isEnabled: true);
    }

    public override void PostInitialize()
    {
        var tasksAttribute = Assembly.GetEntryAssembly().EntryPoint.DeclaringType.GetCustomAttribute<TasksAttribute>();
        if (tasksAttribute is not { Enable: true }) return;

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

                        IocManager.Logger<TaskModule>().LogDebug(message: $"【{job.Key.Name}】 开始执行");
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
                        finally
                        {
                            IocManager.Logger<TaskModule>().LogDebug(message: $"【{job.Key.Name}】 耗时 {sw.ElapsedMilliseconds} ms");
                            // 如果设置了下次执行时间
                            if (taskContext.NextTimespan > 0) job.Value.Interval = taskContext.NextTimespan - DateTime.Now.ToTimestamps();

                            // 间隔执行
                            if (job.Value.Interval > 0) Thread.Sleep(timeout: TimeSpan.FromMilliseconds(value: job.Value.Interval));
                        }
                    }
                }, creationOptions: TaskCreationOptions.LongRunning);
        });
    }
}