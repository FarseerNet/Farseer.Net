using System;
using System.Diagnostics;
using FS.Job.ActService;

namespace FS.Job
{
    public class ConsoleOutput : IDisposable
    {
        private Stopwatch _startNew;

        public void Execute(string jobName, Action act, bool needOutput, bool needSaveHistory)
        {
            _startNew = Stopwatch.StartNew();
            if (needOutput)
            {
                Console.Write(new string('-', 15) + "【");
                Utils.Write($"{jobName} ", ConsoleColor.Red);
                Console.WriteLine("】"+new string('-', 15));
                
                Utils.Write($"{DateTime.Now:yy-MM-dd HH:mm:ss} ", ConsoleColor.Green);
                Console.WriteLine($"开始执行:");
            }

            try
            {
                act();
                var useTime = _startNew.ElapsedMilliseconds;
                if (needOutput)
                {
                    Utils.Write($"{DateTime.Now:yy-MM-dd HH:mm:ss} ", ConsoleColor.Green);
                    Console.Write($"执行完成，耗时:");
                    Utils.WriteLine($"{useTime:N1} ms", ConsoleColor.Red);
                    
                    
                    Console.Write(new string('-', 15) + "【");
                    Utils.Write($"{jobName} ", ConsoleColor.Red);
                    Console.WriteLine("】"+new string('-', 15));
                }

                if (needSaveHistory) HistoryExecuteRecord.Success(jobName, useTime);
            }
            catch (Exception e)
            {
                Utils.WriteLine(e.ToString(), ConsoleColor.Red);
                if (needSaveHistory) HistoryExecuteRecord.Error(jobName, e.Message);
            }
        }

        public void Dispose()
        {
            _startNew.Stop();
        }
    }
}