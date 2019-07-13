using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using FS.DI;
using FS.Extends;
using FS.Job.Entity;

namespace FS.Job.ActService
{
    internal class SingleExecute : IExecute
    {
        public void Run(MenuItem meu)
        {
            if (meu.SubMenuList.Count == 0) CreateMenu(meu);
        }

        /// <summary>
        /// 显示菜单
        /// </summary>
        public void CreateMenu(MenuItem preItem)
        {
            preItem.SubMenuList = new List<MenuItem>();
            foreach (var job in JobFinder.JobList)
            {
                preItem.SubMenuList.Add(new MenuItem(preItem, job.Index, job.JobName, true).SetAct(meu =>
                {
                    using (var co = new ConsoleOutput())
                    {
                        co.Execute(job.JobName, () =>
                        {
                            var resolve = IocManager.Instance.Resolve<IJob>(job.IocName);
                            resolve.Init();
                            resolve.Start(CancellationToken.None);
                            resolve.Stop();
                        }, true, true);
                    }
                }));
            }
        }
    }
}