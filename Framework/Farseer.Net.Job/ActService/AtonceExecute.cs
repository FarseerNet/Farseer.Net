using System;
using System.Collections.Generic;
using System.Text;
using FS.DI;
using FS.Extends;
using FS.Job.Entity;

namespace FS.Job.ActService
{
    public class AtonceExecute : IExecute
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
                preItem.SubMenuList.Add(new MenuItem(preItem, job.Index, job.JobName)
                {
                    Act = () => IocManager.Instance.Resolve<IJob>(job.IocName).Start(),
                    SubMenuList = new List<MenuItem>()
                });
            }
        }

        /// <summary>
        /// 读取输入命令
        /// </summary>
        /// <param name="lstMeu"></param>
        /// <returns></returns>
        public void OutputReadLine()
        {
            while (true)
            {
                Console.Write("请选择操作（-x是命令行参数）：");
                var cmd = Console.ReadLine().ConvertType(-99999);

                var meu = JobFinder.JobList.Find(o => o.Index == cmd);
                if (meu != null)
                {
                    var job = IocManager.Instance.Resolve<IJob>(meu.IocName);
                    job.Init();
                    job.Start();
                    return;
                }

                Console.Write("非法输入，请重新输入,");
            }
        }
    }
}