using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FS.DI;
using FS.Job.Entity;

namespace FS.Job.ActService
{
    public class LazyExecute
    {
        public static Stack<JobEntity> List = new Stack<JobEntity>();

        public void Run()
        {
        }

        private void Add(JobEntity item)
        {
            List.Push(item);
        }

        public static void Init()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (List.Count == 0)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    var job = List.Pop();
                    var menuItem = new MenuItem(null, -1, job.JobName, true)
                    {
                        Act = () => IocManager.Instance.Resolve<IJob>(job.IocName).Start()
                    };
                    CmdInput.ExecuteMenu(menuItem, false);
                }
            });
        }

        public static void Show()
        {
            var lst = List.ToList();
            Console.Write($"共");
            Utils.Write(lst.Count.ToString(), ConsoleColor.Red);
            Console.WriteLine($"项Job待执行");
            foreach (var msg in lst)
            {
                Utils.WriteLine($"{msg.JobName} ", ConsoleColor.Green);
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
                var item = new MenuItem(preItem, job.Index, job.JobName)
                {
                    SubMenuList = new List<MenuItem>()
                };
                item.Act = () => Add(job);
                preItem.SubMenuList.Add(item);
            }
        }
    }
}