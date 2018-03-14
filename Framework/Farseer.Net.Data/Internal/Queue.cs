using System;
using FS.Data.Infrastructure;
using FS.Data.Map;

namespace FS.Data.Internal
{
    /// <summary>
    ///     队列，每一次的查询将生成一个新的实例
    /// </summary>
    internal class Queue : IDisposable
    {
        internal Queue(InternalContext context, SetDataMap map)
        {
            ID = Guid.NewGuid();
            Context = context;
            Map = map;
            CreateAt = DateTime.Now;
        }

        /// <summary>
        ///     当前队列的ID
        /// </summary>
        private Guid ID { get; set; }

        /// <summary>
        ///     数据库上下文
        /// </summary>
        private InternalContext Context { get; set; }

        /// <summary>
        ///     保存字段映射的信息
        /// </summary>
        internal SetDataMap Map;

        /// <summary>
        ///     队列创建时间
        /// </summary>
        internal DateTime CreateAt;

        /// <summary>
        ///     复制条件
        /// </summary>
        /// <param name="queue">队列</param>
        internal void Copy(Queue queue)
        {
            ID = queue.ID;
            Context = queue.Context;
            _expBuilder = queue.ExpBuilder;
            queue.Dispose();
        }

        /// <summary>
        ///     延迟执行的委托（返回影响行数）
        /// </summary>
        internal Func<Queue, int> LazyAct { get; set; }

        #region ExpBuilder表达式持久化

        private ExpressionBuilder _expBuilder;

        /// <summary>
        ///     表达式持久化
        /// </summary>
        internal ExpressionBuilder ExpBuilder => _expBuilder ?? (_expBuilder = new ExpressionBuilder(Map));

        #endregion

        #region SqlBuilderSQL生成器

        private AbsSqlBuilder _sqlBuilder;

        /// <summary>
        ///     SQL生成器
        /// </summary>
        internal AbsSqlBuilder SqlBuilder => _sqlBuilder ?? (_sqlBuilder = Context.DbProvider.CreateSqlBuilder(ExpBuilder, Map.Name));

        #endregion

        #region ProcBuilder存储过程生成器

        private ProcBuilder _procBuilder;

        /// <summary>
        ///     存储过程生成器
        /// </summary>
        internal ProcBuilder ProcBuilder => _procBuilder ?? (_procBuilder = new ProcBuilder(Context.DbProvider, Map, Map.Name));

        #endregion

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
                Context = null;
                Map = null;
                _expBuilder = null;
                _sqlBuilder = null;
                _procBuilder = null;
                LazyAct = null;
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