using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using FS.DI;
using FS.Extends;
using FS.Job.Entity;

namespace FS.Job.ActService
{
    internal class MultipleExecute : IExecute
    {
        public void Run(MenuItem meu)
        {
            foreach (var job in JobFinder.JobList)
            {
                var resolve = IocManager.Instance.Resolve<IJob>(job.IocName);
                resolve.Init();
                resolve.Start(CancellationToken.None);
                resolve.Stop();
            }
        }

        /// <summary>
        /// 显示菜单
        /// </summary>
        public void CreateMenu(MenuItem preItem)
        {
        }
    }
}