using System;
using System.Collections.Generic;
using System.Threading;

namespace FS.Utils.Component
{
    /// <summary>
    ///     定义任务
    ///     1：间隔：每隔（秒分时天星期月 ），执行ITasks.Execute()
    ///     2：指定：每当（秒分时天星期月 ），执行ITasks.Execute()
    /// </summary>
    public static class TimingTasks
    {
        private static readonly List<Timer> lstTasks;

        static TimingTasks()
        {
            lstTasks = new List<Timer>();
        }

        /// <summary>
        ///     清除所有正在执行的操作
        /// </summary>
        public static void Clear()
        {
            lstTasks.Clear();
        }


        /// <summary>
        ///     间隔执行
        /// </summary>
        /// <param name="millisecond">间隔时间</param>
        /// <param name="callback">执行的方法</param>
        public static Timer Interval(Action<object> callback, int millisecond)
        {
            return Interval(callback, 0, 0, 0, 0, millisecond);
        }
        /// <summary>
        ///     间隔执行
        /// </summary>
        /// <param name="second">间隔时间</param>
        /// <param name="millisecond">间隔时间</param>
        /// <param name="callback">执行的方法</param>
        public static Timer Interval(Action<object> callback, int second, int millisecond)
        {
            return Interval(callback, 0, 0, 0, second, millisecond);
        }
        /// <summary>
        ///     间隔执行
        /// </summary>
        /// <param name="minute">间隔时间</param>
        /// <param name="second">间隔时间</param>
        /// <param name="millisecond">间隔时间</param>
        /// <param name="callback">执行的方法</param>
        public static Timer Interval(Action<object> callback, int minute, int second, int millisecond)
        {
            return Interval(callback, 0, 0, minute, second, millisecond);
        }
        /// <summary>
        ///     间隔执行
        /// </summary>
        /// <param name="hour">间隔时间</param>
        /// <param name="minute">间隔时间</param>
        /// <param name="second">间隔时间</param>
        /// <param name="millisecond">间隔时间</param>
        /// <param name="callback">执行的方法</param>
        public static Timer Interval(Action<object> callback, int hour, int minute, int second, int millisecond)
        {
            return Interval(callback, 0, hour, minute, second, millisecond);
        }
        /// <summary>
        ///     间隔执行
        /// </summary>
        /// <param name="day">间隔时间</param>
        /// <param name="hour">间隔时间</param>
        /// <param name="minute">间隔时间</param>
        /// <param name="second">间隔时间</param>
        /// <param name="millisecond">间隔时间</param>
        /// <param name="callback">执行的方法</param>
        public static Timer Interval(Action<object> callback, int day, int hour, int minute, int second, int millisecond)
        {
            var period = 0;
            if (day > 0) { period += 1000 * 60 * 60 * 24 * day; }
            if (hour > 0) { period += 1000 * 60 * 60 * hour; }
            if (minute > 0) { period += 1000 * 60 * minute; }
            if (second > 0) { period += 1000 * second; }
            if (millisecond > 0) { period += millisecond; }

            var timer = new Timer(new TimerCallback(callback), null, period, period);
            lstTasks.Add(timer);
            return timer;
        }



        /// <summary>
        ///     定时执行
        /// </summary>
        /// <param name="day">指定时间</param>
        /// <param name="hour">指定时间</param>
        /// <param name="minute">指定时间</param>
        /// <param name="second">指定时间</param>
        /// <param name="callback">执行的方法</param>
        public static Timer Timing(Action<object> callback, int day, int hour, int minute, int second)
        {
            var period = 1;
            if (day > 0) { period = 1000*60*60*24; }
            if (hour > -1) { period = 1000*60*60; }
            if (minute > -1) { period = 1000*60; }
            if (second > -1) { period = 1000; }

            var timer = new Timer(o =>
            {
                var dt = DateTime.Now;
                if (day > 0 && dt.Day != day) { return; }
                if (hour > -1 && dt.Hour != hour) { return; }
                if (minute > -1 && dt.Minute != minute) { return; }
                if (second > -1 && dt.Second != second) { return; }

                callback(o);
            }, null, 0, period);
            lstTasks.Add(timer);
            return timer;
        }
        /// <summary>
        ///     定时执行
        /// </summary>
        /// <param name="week">星期</param>
        /// <param name="hour">指定时间</param>
        /// <param name="minute">指定时间</param>
        /// <param name="second">指定时间</param>
        /// <param name="callback">执行的方法</param>
        public static Timer Timing(Action<object> callback, DayOfWeek week, int hour, int minute, int second)
        {
            var period = 1000*60*60*24;
            if (hour > -1) { period = 1000*60*60; }
            if (minute > -1) { period = 1000*60; }
            if (second > -1) { period = 1000; }

            var timer = new Timer(o =>
            {
                var dt = DateTime.Now;
                if (dt.DayOfWeek != week) { return; }
                if (hour > -1 && dt.Hour != hour) { return; }
                if (minute > -1 && dt.Minute != minute) { return; }
                if (second > -1 && dt.Second != second) { return; }

                callback(o);
            }, null, 0, period);
            lstTasks.Add(timer);

            return timer;
        }
        /// <summary>
        ///     定时执行
        /// </summary>
        /// <param name="second">指定时间</param>
        /// <param name="callback">执行的方法</param>
        public static Timer Timing(Action<object> callback, int second)
        {
            return Timing(callback, -1, -1, -1, second);
        }
        /// <summary>
        ///     定时执行
        /// </summary>
        /// <param name="minute">指定时间</param>
        /// <param name="second">指定时间</param>
        /// <param name="callback">执行的方法</param>
        public static Timer Timing(Action<object> callback, int minute, int second)
        {
            return Timing(callback, -1, -1, minute, second);
        }
        /// <summary>
        ///     定时执行
        /// </summary>
        /// <param name="hour">指定时间</param>
        /// <param name="minute">指定时间</param>
        /// <param name="second">指定时间</param>
        /// <param name="callback">执行的方法</param>
        public static Timer Timing(Action<object> callback, int hour, int minute, int second)
        {
            return Timing(callback, -1, hour, minute, second);
        }
    }

    #region 调用Demo

    //static void Main(string[] args)
    //    {
    //        TimingTasks.Interval(
    //            o =>
    //            {
    //                Console.WriteLine(string.Format("当前时间：{0} 我每5秒执行。", DateTime.Now.ToString("HH:mm:ss")));
    //            }, 5, 0);

    //        TimingTasks.Interval(
    //            o =>
    //            {
    //                Console.WriteLine(string.Format("当前时间：{0} ", DateTime.Now.ToString("HH:mm:ss")));
    //            }, 500);

    //        TimingTasks.Timing(
    //            o =>
    //            {
    //                Console.WriteLine(string.Format("当前时间：{0} 我只在每天的 9点35分11秒 的时候执行。", DateTime.Now.ToString("HH:mm:ss")));
    //            }, 9,35,11,0);


    //        // 窗口不关闭
    //        while (true) { Thread.Sleep(1); }
    //    }

    #endregion
}