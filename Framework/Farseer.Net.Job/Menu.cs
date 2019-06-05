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

            var menuItem = new MenuItem(null, 1, "JOB 立即执行") {Act = () => new AtonceExecute().Run(),};
            new AtonceExecute().CreateMenu(menuItem);
            MeuList.Add(menuItem);

            
            menuItem = new MenuItem(null, 2, "JOB 加入队列") {Act = () => new LazyExecute().Run(),};
            new LazyExecute().CreateMenu(menuItem);
            MeuList.Add(menuItem);
            
            MeuList.Add(new MenuItem(null, 3, "JOB 设置计划")
            {
                Act = null,
                SubMenuList = new List<MenuItem>()
            });
            MeuList.Add(new MenuItem(null, 4, "显示执行列表")
            {
                Act = null,
                SubMenuList = new List<MenuItem>()
            });
            MeuList.Add(new MenuItem(null, 5, "显示历史列表")
            {
                Act = HistoryExecuteRecord.Show,
                SubMenuList = new List<MenuItem>()
            });
            
            MeuList.Add(new MenuItem(null, 6, "显示计划列表")
            {
                Act = null,
                SubMenuList = new List<MenuItem>()
            });
            MeuList.Add(new MenuItem(null, 0, "退出程序")
            {
                Act = () => Environment.Exit(0),
                SubMenuList = new List<MenuItem>()
            });
            return;
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