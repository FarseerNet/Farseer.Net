using System;
using System.Collections.Generic;
using System.Linq;
using FS.DI;
using FS.Job.Entity;
using FS.Reflection;
using FS.Utils.Common;

namespace FS.Job
{
    /// <summary>
    /// 查找继承IJob接口的实现类
    /// </summary>
    public class JobFinder
    {
        internal static List<JobEntity> JobList = new List<JobEntity>();

        /// <summary>
        /// 注册实现继承IJob接口的实现类
        /// </summary>
        public void RegisterJob()
        {
            var types = IocManager.Instance.Resolve<IAssemblyFinder>().GetType<IJob>();
            JobList.Clear();
            for (int i = 0; i < types.Length; i++)
            {
                var name = $"job_{types[i].FullName}";
                IocManager.Instance.Register(types[i], name, DependencyLifeStyle.Transient);
                var job = IocManager.Instance.Resolve<IJob>(name);
                if (!job.Setting.Enable) continue;
                JobList.Add(new JobEntity() {Index = job.Setting.Index, JobName = job.Setting.Name, JobType = types[i], IocName = name});
                JobList = JobList.OrderBy(o => o.Index).ToList();
            }
        }

        public void ShowJob()
        {
            var jobs = IocManager.Instance.ResolveAll<IJob>();
            foreach (var job in jobs)
            {
                Console.WriteLine($"{job.Setting.Name}");
            }
        }
    }
}