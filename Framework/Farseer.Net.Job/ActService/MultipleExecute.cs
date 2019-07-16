using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                using (var co = new ConsoleOutput())
                {
                    co.Execute(job.JobName, () =>
                    {
                        var resolve = IocManager.Instance.Resolve<IJob>(job.IocName);
                        resolve.Init();
                        resolve.Start(CancellationToken.None, false);
                        resolve.Stop();
                    }, true, true);
                }
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