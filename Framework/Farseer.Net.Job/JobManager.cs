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
                // 显示菜单
                var lstMeu = meu.ShowMenu(preItem);
                // 拿到输入的命令
                preItem = new CmdInput().OutputReadLine(lstMeu);
                
                if (preItem?.SubMenuList.Count == 0) preItem = preItem.PreItem;
            }
        }
    }
}