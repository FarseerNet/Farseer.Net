using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FS.Utils.Component
{
    /// <summary>
    ///     队列执行
    /// </summary>
    public class QueueTask
    {
        /// <summary>
        ///     队列
        /// </summary>
        private static readonly List<Action> lstQue = new List<Action>();

        /// <summary>
        ///     是否工作
        /// </summary>
        private static bool IsWork = true;

        static QueueTask()
        {
            Init();
        }

        /// <summary>
        ///     初始化队列并执行
        /// </summary>
        private static void Init()
        {
            var task = Task.Factory.StartNew(action: () =>
            {
                while (true)
                {
                    if (!IsWork || lstQue.Count == 0)
                    {
                        Thread.Sleep(millisecondsTimeout: 1000);
                        continue;
                    }

                    lstQue[index: 0]();
                    lstQue.RemoveAt(index: 0);
                    Thread.Sleep(millisecondsTimeout: 1000);
                }
            });
        }

        /// <summary>
        ///     添加任务
        /// </summary>
        /// <param name="act"> </param>
        public static void Add(Action act)
        {
            lstQue.Add(item: act);
        }

        /// <summary>
        ///     添加任务
        /// </summary>
        public static void Add(int index, Action act)
        {
            lstQue.Insert(index: index, item: act);
        }

        /// <summary>
        ///     添加任务
        /// </summary>
        public static void Add<T>(Action<T> act, T obj)
        {
            Action a = () =>
            {
                act(obj: obj);
            };
            lstQue.Add(item: a);
        }

        /// <summary>
        ///     添加任务
        /// </summary>
        public static void Add<T>(int index, Action<T> act, T obj)
        {
            Action a = () =>
            {
                act(obj: obj);
            };
            lstQue.Insert(index: index, item: a);
        }

        /// <summary>
        ///     移除的对象
        /// </summary>
        /// <param name="item"> </param>
        public static bool Remove(Action item) => lstQue.Remove(item: item);

        /// <summary>
        ///     移除与指定的谓词所定义的条件相匹配的所有元素
        /// </summary>
        /// <param name="match"> </param>
        public static int RemoveAll(Predicate<Action> match) => lstQue.RemoveAll(match: match);

        /// <summary>
        ///     移除指定索引处的元素。
        /// </summary>
        /// <param name="index"> </param>
        public static void RemoveAt(int index)
        {
            lstQue.RemoveAt(index: index);
        }

        /// <summary>
        ///     移除一定范围的元素。
        /// </summary>
        /// <param name="index"> </param>
        /// <param name="count"> </param>
        public static void RemoveRange(int index, int count)
        {
            lstQue.RemoveRange(index: index, count: count);
        }

        /// <summary>
        ///     清除任务
        /// </summary>
        public static void Clear()
        {
            lstQue.Clear();
        }

        /// <summary>
        ///     停止执行
        /// </summary>
        public static void Stop()
        {
            IsWork = false;
        }

        /// <summary>
        ///     开始执行
        /// </summary>
        public static void Start()
        {
            IsWork = true;
        }
    }
}