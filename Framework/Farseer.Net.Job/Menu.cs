using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Text;
using FS.Extends;
using FS.Job.ActService;

namespace FS.Job
{
    /// <summary>
    /// 添加菜单
    /// </summary>
    internal class Menu
    {
        public static List<MenuItem> MeuList = new List<MenuItem>();

        /// <summary>
        /// 生成菜单
        /// </summary>
        /// <returns></returns>
        internal void CreateMenu()
        {
            MeuList.Clear();

            MeuList.Add(new MenuItem(null, 1, "JOB 单项执行").SetAct(meu => new SingleExecute().Run(meu)));
            MeuList.Add(new MenuItem(null, 2, "JOB 顺序执行").SetAct(meu => new MultipleExecute().Run(meu)));
            MeuList.Add(new MenuItem(null, 3, "JOB 异步执行").SetAct(meu => new AsyncExecute().Run(meu)));
            MeuList.Add(new MenuItem(null, 4, "显示历史列表").SetAct(meu => HistoryExecuteRecord.Show()));
            MeuList.Add(new MenuItem(null, 0, "退出程序").SetAct(meu => Environment.Exit(0)));
        }

        /// <summary>
        /// 显示菜单
        /// </summary>
        public List<MenuItem> ShowMenu(MenuItem preItem)
        {
            var lstMeu = preItem == null ? MeuList : preItem.SubMenuList;
            var sp = new StringBuilder();
            sp.AppendLine(new string('*', 50));
            foreach (var menuItem in lstMeu)
            {
                sp.AppendLine($"{menuItem.Index} {menuItem.MenuName}");
            }

            if (preItem != null)
            {
                sp.AppendLine();
                sp.AppendLine($"-1 返回上一层");
            }

            sp.AppendLine(new string('*', 50));
            Utils.WriteLine(sp.ToString(), ConsoleColor.Yellow);
            return lstMeu;
        }
    }
}