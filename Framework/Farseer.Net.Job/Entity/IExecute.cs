using System.Collections.Generic;

namespace FS.Job.Entity
{
    public interface IExecute
    {
        /// <summary>
        /// 运行
        /// </summary>
        void Run();

        /// <summary>
        /// 显示菜单
        /// </summary>
        void CreateMenu(MenuItem preItem);

        /// <summary>
        /// 读取输入命令
        /// </summary>
        /// <param name="lstMeu"></param>
        /// <returns></returns>
        void OutputReadLine();
    }
}