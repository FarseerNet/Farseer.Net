using System;
using System.Collections.Generic;

namespace FS.Job
{
    /// <summary>
    /// 菜鸟
    /// </summary>
    public class MenuItem
    {
        public MenuItem(MenuItem preItem, int index, string menuName)
        {
            Index = index;
            PreItem = preItem;
            MenuName = menuName;
        }

        /// <summary>
        /// 当前菜单分配的ID
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 当前菜单名称
        /// </summary>
        public string MenuName { get; set; }
        /// <summary>
        /// 上一步菜单Item
        /// </summary>
        public MenuItem PreItem { get; set; }
        /// <summary>
        /// 子菜单列表
        /// </summary>
        public List<MenuItem> SubMenuList { get; set; }
        /// <summary>
        /// 当前目录要执行的功能
        /// </summary>
        public Action Act { get; set; }
    }
}