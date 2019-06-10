using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FS.Job.Entity;

namespace FS.Job
{
    public class HistoryExecuteRecord
    {
        private static Stack<HistoryExecuteEntity> List = new Stack<HistoryExecuteEntity>();

        public static void Add(string msg, long ms)
        {
            List.Push(new HistoryExecuteEntity {CreateAt = DateTime.Now, Msg = msg, UseTime = ms});
        }

        private static List<HistoryExecuteEntity> ToList()
        {
            while (List.Count > 50)
            {
                List.Pop();
            }

            return List.ToList();
        }

        public static void Show()
        {
            var lst = ToList();
            if (lst.Count == 0)
            {
                Console.WriteLine("无执行记录");
                return;
            }

            foreach (var msg in lst)
            {
                Utils.Write($"{msg.CreateAt:yy-MM-dd HH:mm:ss} ", ConsoleColor.Green);
                Utils.Write($"{msg.Msg} 耗时:", ConsoleColor.White);
                Utils.WriteLine($"{msg.UseTime} ms", ConsoleColor.Red);
            }
        }
    }
}