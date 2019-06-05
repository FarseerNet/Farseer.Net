using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FS.DI;

namespace FS.Job.ActService
{
    public class LazyExecute
    {
        public static Stack<MenuItem> List = new Stack<MenuItem>();

        public void Run()
        {
        }

        private void Add(MenuItem item)
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

                    var menuItem = List.Pop();
                    CmdInput.ExecuteMenu(menuItem, true);
                }
            });
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
                item.Act = () => Add(item);
                preItem.SubMenuList.Add(item);
            }
        }
    }
}