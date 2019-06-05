using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            //MeuList.AddRange(menuItem.SubMenuList);

            MeuList.Add(new MenuItem(null, 2, "JOB 加入队列")
            {
                Act = null,
                SubMenuList = new List<MenuItem>()
            });
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
                Act = null,
                SubMenuList = new List<MenuItem>()
            });
            MeuList.Add(new MenuItem(null, 6, "显示计划列表")
            {
                Act = null,
                SubMenuList = new List<MenuItem>()
            });
            MeuList.Add(new MenuItem(null, 0, "退出程序")
            {
                Act = null,
                SubMenuList = new List<MenuItem>()
            });
            return;
        }

        /// <summary>
        /// 显示菜单
        /// </summary>
        public MenuItem ShowMenu(MenuItem preItem)
        {
            var lstMeu = preItem == null ? MeuList : preItem.SubMenuList;
            var sp = new StringBuilder();
            sp.AppendLine(new string('*', 50));
            foreach (var menuItem in lstMeu)
            {
                sp.AppendLine($"{menuItem.Index} {menuItem.MenuName}");
            }

            sp.AppendLine(new string('*', 50));
            Utils.WriteLine(sp.ToString(), ConsoleColor.Yellow);

            return OutputReadLine(lstMeu);
        }

        /// <summary>
        /// 读取输入命令
        /// </summary>
        /// <param name="lstMeu"></param>
        /// <returns></returns>
        private MenuItem OutputReadLine(List<MenuItem> lstMeu)
        {
            while (true)
            {
                Console.Write("请选择操作（-x是命令行参数）：");
                var cmd = Console.ReadLine().ConvertType(-99999);

                var meu = lstMeu.Find(o => o.Index == cmd);
                if (meu != null)
                {
                    if (meu.Act != null)
                    {
                        if (meu.SubMenuList.Count==0) Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] 开始执行：");
                        var startNew = Stopwatch.StartNew();
                        try
                        {
                            meu.Act();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        startNew.Stop();
                        if (meu.SubMenuList.Count==0) Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] 执行完成，耗时：{startNew.ElapsedMilliseconds:N1}ms");
                        
                    }
                    
                    return meu;
                }

                Console.Write("非法输入，请重新输入,");
            }
        }
    }
}