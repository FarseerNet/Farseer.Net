using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Farseer.Net.Data.Map;

namespace Farseer.Net.Data.Internal
{
    /// <summary>
    ///     队列管理模块
    /// </summary>
    internal class QueueManger : IDisposable
    {
        /// <summary>
        ///     队列管理模块
        /// </summary>
        /// <param name="provider">数据库上下文</param>
        internal QueueManger(InternalContext provider)
        {
            ContextProvider = provider;
        }

        /// <summary>
        ///     当前所有持久化列表
        /// </summary>
        //private readonly List<Queue> _groupQueueList = new List<Queue>();

        /// <summary>
        ///     数据库上下文
        /// </summary>
        private InternalContext ContextProvider { get; }

        /// <summary>
        ///     当前组查询队列（支持批量提交SQL）
        /// </summary>
        private Queue _queue;

        /// <summary>
        ///     获取当前队列（不存在，则创建）
        /// </summary>
        /// <param name="map">字段映射</param>
        internal Queue CreateQueue(SetDataMap map)
        {
            return _queue ?? (_queue = new Queue(ContextProvider, map));
        }
/*
        /// <summary>
        ///     延迟执行数据库交互，并提交到队列
        /// </summary>
        /// <param name="act">要延迟操作的委托</param>
        /// <param name="map">字段映射</param>
        /// <param name="joinSoftDeleteCondition">是否加入逻辑删除数据过滤</param>
        internal int CommitLazy(SetDataMap map, Func<Queue, int> act, bool joinSoftDeleteCondition)
        {
            if (ContextProvider.IsUnitOfWork || ContextProvider.Executeor.DataBase.IsTransaction) { return Commit(map, act, joinSoftDeleteCondition); }
            try
            {
                var queue = CreateQueue(map);
                if (joinSoftDeleteCondition) { queue.ExpBuilder.DeleteSortCondition(); }
                queue.LazyAct = act;
                _groupQueueList.Add(_queue);
                return 0;
            }
            finally { Clear(); }
        }

        /// <summary>
        ///     延迟执行数据库交互，并提交到队列
        /// </summary>
        /// <param name="act">要延迟操作的委托</param>
        /// <param name="map">字段映射</param>
        /// <param name="joinSoftDeleteCondition">是否加入逻辑删除数据过滤</param>
        internal async Task<int> CommitLazyAsync(SetDataMap map, Func<Queue, int> act, bool joinSoftDeleteCondition)
        {
            if (ContextProvider.IsUnitOfWork || ContextProvider.Executeor.DataBase.IsTransaction) return await CommitAsync(map, act, joinSoftDeleteCondition);
            try
            {
                var queue = CreateQueue(map);
                if (joinSoftDeleteCondition) { queue.ExpBuilder.DeleteSortCondition(); }
                queue.LazyAct = act;
                _groupQueueList.Add(_queue);
                return 0;
            }
            finally { Clear(); }
        }
        */

        /// <summary>
        ///     立即执行
        /// </summary>
        /// <param name="act">要延迟操作的委托</param>
        /// <param name="map">字段映射</param>
        /// <param name="joinSoftDeleteCondition">是否加入逻辑删除数据过滤</param>
        internal TReturn Commit<TReturn>(SetDataMap map, Func<Queue, TReturn> act, bool joinSoftDeleteCondition)
        {
            try
            {
                // 立即删除时，先提交队列中的数据
                //ContextProvider.QueueManger.CommitAll();
                var queue = CreateQueue(map);
                if (joinSoftDeleteCondition) { queue.ExpBuilder.DeleteSortCondition(); }
                return act(queue);
            }
            finally
            {
                Clear();
                if (ContextProvider.IsUnitOfWork) { ContextProvider.Executeor.DataBase.Close(true); }
            }
        }

        /// <summary>
        ///     立即执行
        /// </summary>
        /// <param name="act">要延迟操作的委托</param>
        /// <param name="map">字段映射</param>
        /// <param name="joinSoftDeleteCondition">是否加入逻辑删除数据过滤</param>
        internal async Task<TReturn> CommitAsync<TReturn>(SetDataMap map, Func<Queue, Task<TReturn>> act, bool joinSoftDeleteCondition)
        {
            try
            {
                var queue = CreateQueue(map);
                if (joinSoftDeleteCondition) { queue.ExpBuilder.DeleteSortCondition(); }
                return await act(queue);
            }
            finally
            {
                Clear();
                if (ContextProvider.IsUnitOfWork) { ContextProvider.Executeor.DataBase.Close(true); }
            }
        }

        ///// <summary>
        /////     立即执行
        ///// </summary>
        ///// <param name="act">要延迟操作的委托</param>
        ///// <param name="map">字段映射</param>
        ///// <param name="joinSoftDeleteCondition">是否加入逻辑删除数据过滤</param>
        //internal async Task<TReturn> CommitAsync<TReturn>(SetDataMap map, Func<Queue, Task<TReturn>> act, bool joinSoftDeleteCondition)
        //{
        //    try
        //    {
        //        // 立即删除时，先提交队列中的数据
        //        //var commitAllAsync = ContextProvider.QueueManger.CommitAllAsync();
        //        var queue = CreateQueue(map);
        //        if (joinSoftDeleteCondition) { queue.ExpBuilder.DeleteSortCondition(); }

        //        //await commitAllAsync;
        //        return await act(queue);
        //    }
        //    finally
        //    {
        //        Clear();
        //        if (ContextProvider.IsUnitOfWork) { ContextProvider.Executeor.DataBase.Close(true); }
        //    }
        //}

        ///// <summary>
        /////     提交所有Queue
        ///// </summary>
        //internal int CommitAll()
        //{
        //    // 单元模式下，不需要执行当前方法
        //    if (ContextProvider.IsUnitOfWork) { return 0; }

        //    var result = 0;
        //    try
        //    {
        //        foreach (var queue in _groupQueueList)
        //        {
        //            // 查看是否延迟执行
        //            if (queue.LazyAct != null) { result += queue.LazyAct(queue); }
        //            // 清除队列
        //            queue.Dispose();
        //        }
        //    }
        //    finally
        //    {
        //        // 清除队列
        //        _groupQueueList.Clear();
        //    }
        //    return result;
        //}

        ///// <summary>
        /////     提交所有Queue
        ///// </summary>
        //internal Task<int> CommitAllAsync() => Task.Run(() => CommitAll());

        ///// <summary>
        /////     清除所有Queue
        ///// </summary>
        //internal void ClearAll()
        //{
        //    // 单元模式下，不需要执行当前方法
        //    if (ContextProvider.IsUnitOfWork) { return; }
        //    try
        //    {
        //        // 清除队列
        //        _groupQueueList.ForEach(o => o.Dispose());
        //    }
        //    finally
        //    {
        //        // 清除队列
        //        _groupQueueList.Clear();
        //    }
        //}

        /// <summary>
        ///     清除当前队列
        /// </summary>
        private void Clear()
        {
            _queue = null;
        }

        #region 释放

        /// <summary>
        ///     释放资源
        /// </summary>
        /// <param name="disposing">是否释放托管资源</param>
        private void Dispose(bool disposing)
        {
            //释放托管资源
            if (disposing)
            {
                //_groupQueueList.Clear();
            }
        }

        /// <summary>
        ///     释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}