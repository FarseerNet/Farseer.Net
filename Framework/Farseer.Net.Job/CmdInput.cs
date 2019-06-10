using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using FS.Extends;
using FS.Job.ActService;

namespace FS.Job
{
    public class CmdInput
    {
        /// <summary>
        /// 读取输入命令
        /// </summary>
        /// <param name="lstMeu"></param>
        /// <returns></returns>
        public MenuItem OutputReadLine(List<MenuItem> lstMeu)
        {
            while (true)
            {
                Console.Write("请选择操作：");
                var readLine = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(readLine)) return lstMeu[0].PreItem;
                var cmd = readLine.ConvertType(-99999);

                // -1 返回上一层
                if (cmd == -1)
                {
                    if (lstMeu[0].PreItem != null) return lstMeu[0].PreItem.PreItem;

                    Console.Write("当前已是最上层，");
                    continue;
                }

                var meu = lstMeu.Find(o => o.Index == cmd);
                if (meu != null) return ExecuteMenu(meu);

                Console.Write("非法输入，请重新输入,");
            }
        }

        /// <summary>
        /// 执行目录
        /// </summary>
        /// <param name="meu"></param>
        /// <param name="isOutputLog">是否输出打印信息</param>
        /// <returns></returns>
        public static MenuItem ExecuteMenu(MenuItem meu, bool isOutputLog = true)
        {
            if (meu.Act != null)
            {
                if (meu.SaveRecord && isOutputLog) Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] 开始执行：");
                var startNew = Stopwatch.StartNew();
                try
                {
                    meu.Act(meu);
                    if (meu.SaveRecord)
                        HistoryExecuteRecord.Success(meu.MenuName, startNew.ElapsedMilliseconds);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    if (meu.SaveRecord)
                        HistoryExecuteRecord.Error(meu.MenuName, e.Message);
                }

                startNew.Stop();
                if (meu.SaveRecord && isOutputLog)
                    Console.WriteLine(
                        $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] 执行完成，耗时：{startNew.ElapsedMilliseconds:N1}ms");
            }

            return meu;
        }
    }
}