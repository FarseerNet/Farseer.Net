using FS.Extends;

namespace FS.Job
{
    public class JobManager
    {
        public void Run()
        {
            var meu = new Menu();
            MenuItem preItem = null;
            while (true)
            {
                // 显示菜单，并拿到输入的命令
                preItem = meu.ShowMenu(preItem);
                if (preItem.SubMenuList.Count == 0) preItem = preItem.PreItem;
            }
        }
    }
}