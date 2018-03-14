using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace FS.Utils.Common
{
    /// <summary>
    /// 性能测试
    /// </summary>
    public class SpeedTest
    {
        /// <summary>
        ///     初始化进程
        /// </summary>
        public static void Initialize()
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
        }

        /// <summary>
        ///     计算运行方法耗时情况
        /// </summary>
        /// <param name="name">本次计算的名称</param>
        /// <param name="iteration">计算次数</param>
        /// <param name="action">要计算的方法</param>
        public static void ConsoleTime(string name, int iteration, Action action)
        {
            if (String.IsNullOrWhiteSpace(name)) { return; }

            // 设置控制台前景色
            var currentForeColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(name);

            // 强制进去垃圾回收
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            var gcCounts = new int[GC.MaxGeneration + 1];
            for (var i = 0; i <= GC.MaxGeneration; i++) { gcCounts[i] = GC.CollectionCount(i); }

            // 开始计时
            var watch = new Stopwatch();
            watch.Start();
            var cycleCount = GetCycleCount();
            for (var i = 0; i < iteration; i++) action();
            var cpuCycles = GetCycleCount() - cycleCount;
            watch.Stop();

            // 输出计时结果
            Console.ForegroundColor = currentForeColor;
            Console.WriteLine("\tTime Elapsed:\t" + watch.ElapsedMilliseconds.ToString("N0") + "ms");
            Console.WriteLine("\tCPU Cycles:\t" + cpuCycles.ToString("N0"));

            // 输出垃圾回收结果
            for (var i = 0; i <= GC.MaxGeneration; i++)
            {
                var count = GC.CollectionCount(i) - gcCounts[i];
                Console.WriteLine("\tGen " + i + ": \t\t" + count);
            }

            Console.WriteLine();
        }

        /// <summary>
        /// WEB中测试
        /// </summary>
        /// <param name="key"></param>
        /// <param name="count"></param>
        /// <param name="act"></param>
        /// <param name="outPut"></param>
        /// <returns></returns>
        public static string WebTime(string key, int count, Action act, bool outPut = true)
        {
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);

            var watch = new Stopwatch();
            watch.Start();
            for (var i = 0; i < count; i++) act();
            watch.Stop();
            var result = $"{key}: {watch.ElapsedMilliseconds}ms";
            if (outPut)
            {
#if IsWeb
                System.Web.HttpContext.Current.Response.Write(result + "<br />"); 
#endif
            }
            return result;
        }

        private static ulong GetCycleCount()
        {
            ulong cycleCount = 0;
            QueryThreadCycleTime(GetCurrentThread(), ref cycleCount);
            return cycleCount;
        }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool QueryThreadCycleTime(IntPtr threadHandle, ref ulong cycleTime);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentThread();
    }
}