using System.Collections.Concurrent;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace FS.Extends
{
    public static class QueueExtend
    {
        /// <summary>
        /// 读取全部内容到集合
        /// </summary>
        public static List<T> DequeueAll<T>(this Queue<T> queue)
        {
            var lst = new List<T>();
            while (queue.Count > 0)
            {
                lst.Add(queue.Dequeue());
            }
            return lst;
        }

        /// <summary>
        /// 读取全部内容到集合
        /// </summary>
        public static List<T> DequeueAll<T>(this Queue<T> queue, int maxItemCount)
        {
            var lst       = new List<T>();
            var indexItem = 0;
            while (queue.Count > 0 && indexItem < maxItemCount)
            {
                lst.Add(queue.Dequeue());
                indexItem++;
            }
            return lst;
        }
        /// <summary>
        /// 读取全部内容到集合
        /// </summary>
        public static List<T> DequeueAll<T>(this ConcurrentQueue<T> queue)
        {
            var lst = new List<T>();
            while (queue.TryDequeue(out var t))
            {
                lst.Add(t);
            }
            return lst;
        }

        /// <summary>
        /// 读取全部内容到集合
        /// </summary>
        public static List<T> DequeueAll<T>(this ConcurrentQueue<T> queue, int maxItemCount)
        {
            var lst       = new List<T>();
            var indexItem = 0;
            while (indexItem < maxItemCount && queue.TryDequeue(out var t))
            {
                lst.Add(t);
                indexItem++;
            }
            return lst;
        }
    }
}