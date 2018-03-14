// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2016-08-10 19:58
// ********************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FS.Utils.Component
{
    /// <summary>
    /// 将集合进行分组后并行
    /// </summary>
    public static class ParallelGroup
    {
        /// <summary>
        /// 将lst按splitCount数量进行并行分组后，再并行foreach
        /// </summary>
        /// <param name="lst">集合</param>
        /// <param name="splitCount">分组数量</param>
        /// <param name="maxDegree">最大并行线程</param>
        /// <param name="act">每个并行分组内进行ForEach执行的委托</param>
        public static void ForEach<T>(List<T> lst, int splitCount, int maxDegree, Action<T> act)
        {
            if (lst == null || lst.Count == 0) { return; }
            // 数量过大，需要进行分组
            var groupLength = (int)Math.Ceiling(((decimal)lst.Count / splitCount));

            // 并行分组
            Parallel.For(0, groupLength, new ParallelOptions { MaxDegreeOfParallelism = maxDegree }, i =>
            {
                var lstGroup = lst.Skip(splitCount * i).Take(splitCount).ToList();
                // 并行分组内的list
                Parallel.ForEach(lstGroup, new ParallelOptions { MaxDegreeOfParallelism = maxDegree }, act);
            });
        }

        /// <summary>
        /// 将lst按splitCount数量进行并行分组
        /// </summary>
        /// <param name="lst">集合</param>
        /// <param name="splitCount">分组数量</param>
        /// <param name="maxDegree">最大并行线程</param>
        /// <param name="act">每个并行分组内进行ForEach执行的委托</param>
        public static void Run<T>(List<T> lst, int splitCount, int maxDegree, Action<List<T>> act)
        {
            if (lst == null || lst.Count == 0) { return; }
            // 数量过大，需要进行分组
            var groupLength = (int)Math.Ceiling(((decimal)lst.Count / splitCount));

            // 并行分组
            Parallel.For(0, groupLength, new ParallelOptions { MaxDegreeOfParallelism = maxDegree }, i =>
            {
                var lstGroup = lst.Skip(splitCount * i).Take(splitCount).ToList();
                // 执行委托
                act(lstGroup);
            });
        }

        /// <summary>
        /// 将lst按splitCount数量进行并行分组
        /// </summary>
        /// <param name="lst">集合</param>
        /// <param name="splitCount">分组数量</param>
        /// <param name="maxDegree">最大并行线程</param>
        /// <param name="act">每个并行分组内进行ForEach执行的委托</param>
        public static void Run<T>(List<T> lst, int splitCount, int maxDegree, Action<List<T>, ParallelLoopState> act)
        {
            if (lst == null || lst.Count == 0) { return; }
            // 数量过大，需要进行分组
            var groupLength = (int)Math.Ceiling(((decimal)lst.Count / splitCount));

            // 并行分组
            Parallel.For(0, groupLength, new ParallelOptions { MaxDegreeOfParallelism = maxDegree }, (i, loopState) =>
            {
                var lstGroup = lst.Skip(splitCount * i).Take(splitCount).ToList();
                // 执行委托
                act(lstGroup, loopState);
            });
        }
    }
}