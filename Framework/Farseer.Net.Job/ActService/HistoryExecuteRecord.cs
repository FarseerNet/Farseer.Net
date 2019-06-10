using System;
using System.Collections.Generic;
using System.Linq;
using FS.Job.Entity;

namespace FS.Job.ActService
{
    public class HistoryExecuteRecord
    {
        private static Stack<HistoryExecuteEntity> List = new Stack<HistoryExecuteEntity>();

        public static void Success(string jobName, long ms)
        {
            List.Push(new HistoryExecuteEntity {CreateAt = DateTime.Now, UseTime = ms, JobName = jobName});
        }

        public static void Error(string jobName, string msg)
        {
            List.Push(new HistoryExecuteEntity
                {CreateAt = DateTime.Now, Msg = msg, JobName = jobName, IsError = true});
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
                if (!msg.IsError)
                {
                    Utils.Write($"{msg.CreateAt:yy-MM-dd HH:mm:ss} ", ConsoleColor.Green);
                    Console.Write($"【");
                    Utils.Write($"{msg.JobName} ", ConsoleColor.Red);
                    Console.Write($"】 执行耗时:");
                    Utils.WriteLine($"{msg.UseTime} ms", ConsoleColor.Red);
                }
                else
                {
                    Utils.Write($"{msg.CreateAt:yy-MM-dd HH:mm:ss} ", ConsoleColor.Green);
                    Console.Write($"【");
                    Utils.Write($"{msg.JobName} ", ConsoleColor.Red);
                    Console.Write($"】");
                    Utils.Write($"执行失败", ConsoleColor.Red);
                    Console.Write($"失败原因：");
                    Utils.WriteLine($"{msg.Msg}", ConsoleColor.Red);
                }
            }
        }
    }
}