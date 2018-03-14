using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace FS.Utils.Common
{
    /// <summary>
    /// ���ܲ���
    /// </summary>
    public class SpeedTest
    {
        /// <summary>
        ///     ��ʼ������
        /// </summary>
        public static void Initialize()
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
        }

        /// <summary>
        ///     �������з�����ʱ���
        /// </summary>
        /// <param name="name">���μ��������</param>
        /// <param name="iteration">�������</param>
        /// <param name="action">Ҫ����ķ���</param>
        public static void ConsoleTime(string name, int iteration, Action action)
        {
            if (String.IsNullOrWhiteSpace(name)) { return; }

            // ���ÿ���̨ǰ��ɫ
            var currentForeColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(name);

            // ǿ�ƽ�ȥ��������
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            var gcCounts = new int[GC.MaxGeneration + 1];
            for (var i = 0; i <= GC.MaxGeneration; i++) { gcCounts[i] = GC.CollectionCount(i); }

            // ��ʼ��ʱ
            var watch = new Stopwatch();
            watch.Start();
            var cycleCount = GetCycleCount();
            for (var i = 0; i < iteration; i++) action();
            var cpuCycles = GetCycleCount() - cycleCount;
            watch.Stop();

            // �����ʱ���
            Console.ForegroundColor = currentForeColor;
            Console.WriteLine("\tTime Elapsed:\t" + watch.ElapsedMilliseconds.ToString("N0") + "ms");
            Console.WriteLine("\tCPU Cycles:\t" + cpuCycles.ToString("N0"));

            // ����������ս��
            for (var i = 0; i <= GC.MaxGeneration; i++)
            {
                var count = GC.CollectionCount(i) - gcCounts[i];
                Console.WriteLine("\tGen " + i + ": \t\t" + count);
            }

            Console.WriteLine();
        }

        /// <summary>
        /// WEB�в���
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