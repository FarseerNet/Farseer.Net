using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FS.DI;
using FS.Job.Entity;

namespace FS.Job.ActService
{
    /// <summary>
    /// 异步执行
    /// </summary>
    public class AsyncExecute
    {
        public static Dictionary<Type, TaskStatus> List = new Dictionary<Type, TaskStatus>();

        public void Run(MenuItem meu)
        {
            if (List.Count == 0)
            {
                foreach (var job in JobFinder.JobList)
                {
                    List[job.JobType] = new TaskStatus();
                }
            }

            meu.SubMenuList.Clear();
            CreateMenu(meu);
        }

        private Task CreateTask(JobEntity job)
        {
            List[job.JobType].TokenSource = new CancellationTokenSource();
            List[job.JobType].Task = new Task(() =>
            {
                using (var co = new ConsoleOutput())
                {
                    co.Execute(job.JobName, () =>
                    {
                        var resolve = IocManager.Instance.Resolve<IJob>(job.IocName);
                        resolve.Init();
                        resolve.Start(List[job.JobType].TokenSource.Token);
                        resolve.Stop();
                    },false,true);
                }
            }, List[job.JobType].TokenSource.Token);
            return List[job.JobType].Task;
        }

        private void RunOrStop(JobEntity job)
        {
            switch (List[job.JobType].Task?.Status)
            {
                case System.Threading.Tasks.TaskStatus.Running:
                    List[job.JobType].TokenSource.Cancel();
                    List[job.JobType].Task.Wait();
                    break;
                default:
                    CreateTask(job).Start();
                    break;
            }
        }

        /// <summary>
        /// 显示菜单
        /// </summary>
        public void CreateMenu(MenuItem preItem)
        {
            preItem.SubMenuList = new List<MenuItem>();
            foreach (var job in JobFinder.JobList)
            {
                var status = List[job.JobType].Task?.Status == System.Threading.Tasks.TaskStatus.Running ||
                             List[job.JobType].Task?.Status == System.Threading.Tasks.TaskStatus.WaitingToRun;
                preItem.SubMenuList.Add(
                    new MenuItem(preItem, job.Index, $"{job.JobName} ({(status ? "运行中" : " - ")})").SetAct(meu =>
                    {
                        RunOrStop(job);
                    }));
            }
        }
    }

    public class TaskStatus
    {
        public Task Task { get; set; }
        public CancellationTokenSource TokenSource { get; set; } = new CancellationTokenSource();
    }
}