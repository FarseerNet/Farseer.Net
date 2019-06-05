using System;
using System.Collections.Generic;
using System.Linq;
using FS.DI;
using FS.Job.Entity;
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
        /// 找继承IJob接口的实现类
        /// </summary>
        public Type[] FindJobType() => AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IJob)))).ToArray();

        /// <summary>
        /// 注册实现继承IJob接口的实现类
        /// </summary>
        public void RegisterJob()
        {
            var types = FindJobType();
            JobList.Clear();
            for (int i = 0; i < types.Length; i++)
            {
                var name = $"job_{types[i].FullName}";
                IocManager.Instance.Register(types[i], name, DependencyLifeStyle.Transient);
                var job = IocManager.Instance.Resolve<IJob>(name);
                JobList.Add(new JobEntity() {Index = i + 1, JobName = job.Name, JobType = types[i], IocName = name});
            }
        }

        public void ShowJob()
        {
            var jobs = IocManager.Instance.ResolveAll<IJob>();
            foreach (var job in jobs)
            {
                Console.WriteLine($"{job.Name}");
            }
        }
    }
}