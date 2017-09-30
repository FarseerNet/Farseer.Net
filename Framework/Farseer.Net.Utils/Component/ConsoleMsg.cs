using System;
using System.Threading;

namespace Farseer.Net.Utils.Component
{
    /// <summary>
    ///     用于Console的输出
    /// </summary>
    public class ConsoleMsg : IDisposable
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public readonly bool IsSuccess = true;
        private readonly Timer _timer;
        private readonly ConsoleColor _errorColor;

        /// <summary>
        ///     用于Console的输出
        /// </summary>
        public ConsoleMsg(int millisecond = 500, ConsoleColor errorColor = ConsoleColor.Red)
        {
            _timer = TimingTasks.Interval(o => { Console.Write("."); }, millisecond);
            _errorColor = errorColor;
        }

        /// <summary>
        /// 开始
        /// </summary>
        /// <param name="log"></param>
        /// <param name="errorColor"></param>
        /// <param name="isAddTime"></param>
        /// <param name="millisecond"></param>
        /// <param name="modify"></param>
        /// <returns></returns>
        public static ConsoleMsg Start(string log, ConsoleColor errorColor = ConsoleColor.Red, bool isAddTime = true, int millisecond = 500, string modify = "正在{0}")
        {
            var sp = new StrPlus();
            if (isAddTime) { sp.Append($"{DateTime.Now.ToString("HH:mm:ss")}\t"); }
            sp.Append(string.Format(modify + " ", log));
            Console.Write(sp);
            return new ConsoleMsg(millisecond, errorColor);
        }

        /// <summary>
        /// 摧毁
        /// </summary>
        public void Dispose()
        {
            _timer.Dispose();
            if (IsSuccess) { Console.WriteLine("成功！"); }
            else
            {
                Console.ForegroundColor = _errorColor;
                Console.WriteLine("失败！");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
    }
}
