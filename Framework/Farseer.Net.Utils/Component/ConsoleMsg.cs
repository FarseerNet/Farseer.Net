using System;
using System.Threading;

namespace FS.Utils.Component
{
    /// <summary>
    ///     用于Console的输出
    /// </summary>
    public class ConsoleMsg : IDisposable
    {
        private readonly ConsoleColor _errorColor;
        private readonly Timer        _timer;

        /// <summary>
        ///     是否成功
        /// </summary>
        public readonly bool IsSuccess = true;

        /// <summary>
        ///     用于Console的输出
        /// </summary>
        public ConsoleMsg(int millisecond = 500, ConsoleColor errorColor = ConsoleColor.Red)
        {
            _timer = TimingTasks.Interval(callback: o =>
            {
                Console.Write(value: ".");
            }, millisecond: millisecond);
            _errorColor = errorColor;
        }

        /// <summary>
        ///     摧毁
        /// </summary>
        public void Dispose()
        {
            _timer?.Dispose();
            if (IsSuccess)
                Console.WriteLine(value: "成功！");
            else
            {
                Console.ForegroundColor = _errorColor;
                Console.WriteLine(value: "失败！");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        /// <summary>
        ///     开始
        /// </summary>
        /// <param name="log"> </param>
        /// <param name="errorColor"> </param>
        /// <param name="isAddTime"> </param>
        /// <param name="millisecond"> </param>
        /// <param name="modify"> </param>
        /// <returns> </returns>
        public static ConsoleMsg Start(string log, ConsoleColor errorColor = ConsoleColor.Red, bool isAddTime = true, int millisecond = 500, string modify = "正在{0}")
        {
            var sp = new StrPlus();
            if (isAddTime) sp.Append(Text: $"{DateTime.Now.ToString(format: "HH:mm:ss")}\t");
            sp.Append(Text: string.Format(format: modify + " ", arg0: log));
            Console.Write(value: sp);
            return new ConsoleMsg(millisecond: millisecond, errorColor: errorColor);
        }
    }
}