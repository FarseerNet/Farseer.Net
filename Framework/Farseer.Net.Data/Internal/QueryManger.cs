﻿using System;
using System.Threading.Tasks;
using FS.Data.Map;

namespace FS.Data.Internal
{
    /// <summary>
    ///     查询管理，用于提交、创建每个QueryInstance。
    /// </summary>
    internal class QueryManger : IDisposable
    {
        /// <summary>
        ///     当前组查询队列（支持批量提交SQL）
        /// </summary>
        private Query _query;

        /// <summary>
        ///     队列管理模块
        /// </summary>
        /// <param name="provider"> 数据库上下文 </param>
        internal QueryManger(InternalContext provider)
        {
            ContextProvider = provider;
        }

        /// <summary>
        ///     数据库上下文
        /// </summary>
        private InternalContext ContextProvider { get; }

        /// <summary>
        ///     获取当前队列（不存在，则创建）
        /// </summary>
        /// <param name="map"> 字段映射 </param>
        internal Query CreateQueue(SetDataMap map) => _query ??= new Query(context: ContextProvider, map: map);

        /// <summary>
        ///     立即执行
        /// </summary>
        /// <param name="act"> 要延迟操作的委托 </param>
        /// <param name="map"> 字段映射 </param>
        /// <param name="joinSoftDeleteCondition"> 是否加入逻辑删除数据过滤 </param>
        internal TReturn Commit<TReturn>(SetDataMap map, Func<Query, TReturn> act, bool joinSoftDeleteCondition)
        {
            try
            {
                // 立即删除时，先提交队列中的数据
                var queue = CreateQueue(map: map);
                if (joinSoftDeleteCondition) queue.ExpBuilder.DeleteSortCondition();

                return act(arg: queue);
            }
            finally
            {
                Clear();
                if (!ContextProvider.DbExecutor.IsTransaction) ContextProvider.DbExecutor.Close(dispose: true);
            }
        }

        /// <summary>
        ///     立即执行
        /// </summary>
        /// <param name="act"> 要延迟操作的委托 </param>
        /// <param name="map"> 字段映射 </param>
        /// <param name="joinSoftDeleteCondition"> 是否加入逻辑删除数据过滤 </param>
        internal async Task<TReturn> CommitAsync<TReturn>(SetDataMap map, Func<Query, Task<TReturn>> act, bool joinSoftDeleteCondition)
        {
            try
            {
                var queue = CreateQueue(map: map);
                if (joinSoftDeleteCondition) queue.ExpBuilder.DeleteSortCondition();

                return await act(arg: queue);
            }
            finally
            {
                Clear();
                if (!ContextProvider.DbExecutor.IsTransaction) ContextProvider.DbExecutor.Close(dispose: true);
            }
        }

        /// <summary>
        ///     清除当前队列
        /// </summary>
        private void Clear()
        {
            _query.Dispose();
            _query = null;
        }

        #region 释放

        /// <summary>
        ///     释放资源
        /// </summary>
        /// <param name="disposing"> 是否释放托管资源 </param>
        private void Dispose(bool disposing)
        {
            //释放托管资源
            if (disposing)
            {
                if (_query != null) Clear();
            }
        }

        /// <summary>
        ///     释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(obj: this);
        }

        #endregion
    }
}