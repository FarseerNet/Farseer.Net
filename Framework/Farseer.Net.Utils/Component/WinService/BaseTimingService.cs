// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2016-07-13 16:14
// ********************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Farseer.Net.Extends;
using Farseer.Net.Utils.Common;
using FS.Extends;
using FS.Utils.Common;
using FS.Utils.Component;

namespace FS.Utils.Component.WinService
{
    /// <summary>服务程序基类（带定时、间隔控制的任务）</summary>
    public abstract class BaseTimingService<TService> : BaseService<TService> where TService : BaseService<TService>, new()
    {
        /// <summary>
        ///     任务执行列表
        /// </summary>
        private readonly List<TaskCycle> _taskList = new List<TaskCycle>();
        /// <summary>初始化</summary>
        protected BaseTimingService(string serviceName, string displayName, string description) : base(serviceName, displayName, description) { }
        /// <summary>
        /// 执行状态
        /// </summary>
        private readonly Dictionary<int, bool> _executeStatus = new Dictionary<int, bool>();

        /// <summary>
        ///     添加定时任务
        /// </summary>
        /// <param name="day">指定时间</param>
        /// <param name="hour">指定时间</param>
        /// <param name="minute">指定时间</param>
        /// <param name="second">指定时间</param>
        /// <param name="callbacks">执行的方法</param>
        public void AddTiming(int day, int hour, int minute, int second, params TaskEntity[] callbacks)
        {
            var cycle = new TaskCycle { TimeType = EumTimeType.Timing, Tasks = callbacks, Day = day, Hour = hour, Minute = minute, Second = second };
            foreach (var taskEntity in callbacks) { taskEntity.TaskCycle = cycle; }
            _taskList.Add(cycle);
        }
        /// <summary>
        ///     添加定时任务
        /// </summary>
        /// <param name="week">星期</param>
        /// <param name="hour">指定时间</param>
        /// <param name="minute">指定时间</param>
        /// <param name="second">指定时间</param>
        /// <param name="callbacks">执行的方法</param>
        public void AddTiming(DayOfWeek week, int hour, int minute, int second, params TaskEntity[] callbacks)
        {
            var cycle = new TaskCycle { TimeType = EumTimeType.Timing, Tasks = callbacks, Week = week, Hour = hour, Minute = minute, Second = second };
            foreach (var taskEntity in callbacks) { taskEntity.TaskCycle = cycle; }
            _taskList.Add(cycle);
        }
        /// <summary>
        ///     添加间隔任务
        /// </summary>
        /// <param name="callbacks">执行的方法</param>
        /// <param name="day">间隔时间</param>
        /// <param name="hour">间隔时间</param>
        /// <param name="minute">间隔时间</param>
        /// <param name="second">间隔时间</param>
        /// <param name="millisecond">间隔时间</param>
        public void AddInterval(int day, int hour, int minute, int second, int millisecond, params TaskEntity[] callbacks)
        {
            var cycle = new TaskCycle { TimeType = EumTimeType.Interval, Tasks = callbacks, Day = day, Hour = hour, Minute = minute, Second = second, Millisecond = millisecond };
            foreach (var taskEntity in callbacks) { taskEntity.TaskCycle = cycle; }
            _taskList.Add(cycle);
        }

        /// <summary>
        /// 通过索引，找出对应的任务
        /// </summary>
        private TaskEntity FindTaskByIndex(int taskIndex)
        {
            var index = 0;
            return _taskList.SelectMany(task => task.Tasks.Where(taskEntity => index++ == taskIndex)).FirstOrDefault();
        }

        /// <inheritdoc />
        protected override void ReceiveSetp()
        {
            if (_taskList.Count == 0) { return; }
            Console.WriteLine("请输入要调试的任务索引：");
            ReceiveShowTaskList();
            var k = Console.ReadLine().ConvertType(-1);
            Console.WriteLine();
            StartWork(FindTaskByIndex(k));
        }

        /// <inheritdoc />
        protected override void ReceiveRun()
        {
            Console.WriteLine("正在循环调试……");
            _taskList.ForEach(tasks => StartWork(tasks.Tasks));
            Console.WriteLine("循环调试完成！");//  循环调试
        }

        /// <inheritdoc />
        protected override void ReceiveShowTaskList()
        {
            var sum = _taskList.Sum(o => o.Tasks.Length);
            var index = 0;
            foreach (var task in _taskList)
            {
                Consoles.WriteLine($"{ConvertShow(task)}", ConsoleColor.Yellow);

                foreach (var taskEntity in task.Tasks)
                {
                    Console.Write($"\t{(index++).PadLeft(sum.ToString().Length)}：");
                    Consoles.WriteLine(taskEntity.Caption, ConsoleColor.Green);
                }
                Console.WriteLine();
            }
        }

        /// <summary> 初始化（添加任务） </summary>
        protected abstract void Init();

        /// <summary>服务主函数</summary>
        public new static void ServerMain()
        {
            // 初始化
            (Instance as BaseTimingService<TService>)?.Init();
            BaseService<TService>.ServerMain();
        }

        #region 服务控制
        /// <inheritdoc />
        protected override void Start(string[] args)
        {
            //加入到时间定时器
            _taskList.ForEach(task =>
            {
                switch (task.TimeType)
                {
                    case EumTimeType.Timing:
                        if (task.Week == null) { TimingTasks.Timing(t => StartWork(task.Tasks), task.Day, task.Hour, task.Minute, task.Second); }
                        else { TimingTasks.Timing(t => StartWork(task.Tasks), task.Week.GetValueOrDefault(), task.Hour, task.Minute, task.Second); }
                        break;
                    case EumTimeType.Interval:
                        {
                            TimingTasks.Interval(t => StartWork(task.Tasks), task.Day, task.Hour, task.Minute, task.Second, task.Millisecond); break;
                        }
                }
            });
        }
        /// <summary>
        /// 开始执行
        /// </summary>
        /// <param name="lstTaskEntities"></param>
        private void StartWork(TaskEntity[] lstTaskEntities)
        {
            // 锁住执行，直到完成
            foreach (var lstTaskEntity in lstTaskEntities)
            {
                try { StartWork(lstTaskEntity); }
                catch (Exception ex)
                {
                    //LogManger.Log.Error(ex);
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        /// <summary>开始工作</summary>
        /// <param name="task">任务实体</param>
        private void StartWork(TaskEntity task)
        {
            if (task == null) { return; }
            var hashCode = task.GetHashCode();
            if (!_executeStatus.ContainsKey(hashCode)) { _executeStatus[hashCode] = false; }

            if (_executeStatus[hashCode])
            {
                Console.Write("上一次执行");
                Consoles.Write(task.Caption, ConsoleColor.Green);
                Console.WriteLine("未完成，跳过");
                return;
            }

            _executeStatus[hashCode] = true;
            Console.Write($"开始执行******");

            Consoles.Write(task.Caption, ConsoleColor.Green);
            Console.Write($"******，周期：");
            Consoles.Write(ConvertShow(task.TaskCycle), ConsoleColor.Yellow);
            using (var speed = new SpeedTestMultiple().Begin())
            {
                try { task.Callback(task.Param); }
                catch (Exception err)
                {
                    //LogManger.Log.Error(err);
                    Console.WriteLine(err.ToString());
                }
                _executeStatus[hashCode] = false;
                Console.Write($"，耗时");
                Consoles.Write(speed.Timer.ElapsedMilliseconds.ToString("n0"), ConsoleColor.Red);
                Console.WriteLine($"毫秒");
            }
        }

        private string ConvertShow(TaskCycle task)
        {
            var str = new StringBuilder();
            if (task.TimeType == EumTimeType.Timing)
            {
                str.Append(task.Day > 0 ? $"每月{task.Day}号" : $"每天");
                str.Append($"{task.Hour}时{task.Minute}分{task.Second}秒");
            }
            else
            {
                str.Append($"每隔");
                if (task.Day > 0) { str.Append($"{task.Day}天"); }
                if (task.Hour > 0) { str.Append($"{task.Hour}时"); }
                if (task.Minute > 0) { str.Append($"{task.Minute}分"); }
                if (task.Second > 0) { str.Append($"{task.Second}秒"); }
                if (task.Millisecond > 0) { str.Append($"{task.Millisecond}毫秒"); }
            }

            return str + "执行";
        }

        /// <summary>服务停止事件</summary>
        protected override void OnStop()
        {
            _taskList.Clear();
            TimingTasks.Clear();
        }
        #endregion
    }

    /// <summary>
    /// 任务周期
    /// </summary>
    public class TaskCycle
    {
        /// <summary> 任务类型 </summary>
        public EumTimeType TimeType { get; set; }
        /// <summary> 指定一周中的某天 </summary>
        public DayOfWeek? Week { get; set; }
        /// <summary> 任务委托 </summary>
        public TaskEntity[] Tasks { get; set; }
        /// <summary> 指定几号执行 </summary>
        public int Day { get; set; }
        /// <summary> 指定几时执行 </summary>
        public int Hour { get; set; }
        /// <summary> 指定几分执行 </summary>
        public int Minute { get; set; }
        /// <summary> 指定几秒执行 </summary>
        public int Second { get; set; }
        /// <summary> 指定几毫秒执行 </summary>
        public int Millisecond { get; set; }
    }

    /// <summary>
    /// 任务委托
    /// </summary>
    public class TaskEntity
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        /// <param name="caption">任务名称</param>
        /// <param name="callback">任务委托</param>
        /// <param name="param"></param>
        public TaskEntity(string caption, Action<dynamic> callback, dynamic param = null)
        {
            Caption = caption;
            Callback = callback;
            Param = param;
        }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string Caption { get; set; }
        /// <summary>
        /// 任务委托
        /// </summary>
        public Action<dynamic> Callback { get; set; }
        /// <summary>
        /// 所属任务的周期
        /// </summary>
        public TaskCycle TaskCycle { get; set; }
        /// <summary>
        /// 传递参数
        /// </summary>
        public dynamic Param { get; set; }
    }

    /// <summary>
    ///     任务类型
    /// </summary>
    public enum EumTimeType
    {
        /// <summary>
        ///     定时
        /// </summary>
        Timing,
        /// <summary>
        ///     间隔
        /// </summary>
        Interval
    }
}