using System;
using System.Collections.Generic;
using System.Text;
using FS.DI;
using FS.Extends;
using FS.Job.Entity;

namespace FS.Job.ActService
{
    internal class AtonceExecute : IExecute
    {
        public void Run()
        {
        }

        /// <summary>
        /// 显示菜单
        /// </summary>
        public void CreateMenu(MenuItem preItem)
        {
            preItem.SubMenuList = new List<MenuItem>();
            foreach (var job in JobFinder.JobList)
            {
                preItem.SubMenuList.Add(new MenuItem(preItem, job.Index, job.JobName, true)
                {
                    Act = () =>  IocManager.Instance.Resolve<IJob>(job.IocName).Start(),
                    SubMenuList = new List<MenuItem>()
                });
            }
        }
    }
}