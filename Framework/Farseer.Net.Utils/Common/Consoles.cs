// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2016-07-21 12:04
// ********************************************

using System;

namespace FS.Utils.Common
{
    /// <summary>
    /// 控制台
    /// </summary>
    public class Consoles
    {
        /// <summary>
        /// 将指定的字符串值（后跟当前行终止符）写入标准输出流。
        /// </summary>
        public static void WriteLine(string value, ConsoleColor color)
        {
            WriteLine(() => Console.WriteLine(value), color);
        }
        /// <summary>
        /// 将指定的字符串值（后跟当前行终止符）写入标准输出流。
        /// </summary>
        public static void WriteLine(int value, ConsoleColor color)
        {
            WriteLine(() => Console.WriteLine(value), color);
        }
        /// <summary>
        /// 将指定的字符串值（后跟当前行终止符）写入标准输出流。
        /// </summary>
        public static void WriteLine(long value, ConsoleColor color)
        {
            WriteLine(() => Console.WriteLine(value), color);
        }

        /// <summary>
        /// 将指定的字符串值写入标准输出流。
        /// </summary>
        public static void Write(string value, ConsoleColor color)
        {
            WriteLine(() => Console.Write(value), color);
        }
        /// <summary>
        /// 将指定的字符串值写入标准输出流。
        /// </summary>
        public static void Write(int value, ConsoleColor color)
        {
            WriteLine(() => Console.Write(value), color);
        }

        /// <summary>
        /// 将指定的字符串值写入标准输出流。
        /// </summary>
        public static void Write(long value, ConsoleColor color)
        {
            WriteLine(() => Console.Write(value), color);
        }

        private static void WriteLine(Action output, ConsoleColor color)
        {
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            output();
            Console.ForegroundColor = consoleColor;
        }
    }
}