using System;
using FS.Data.Client;
using FS.Data.Map;

namespace FS.Data.Internal
{
    /// <summary>
    ///     队列，每一次的查询将生成一个新的实例
    /// </summary>
    internal class Queue : IDisposable
    {
        /// <summary>
        ///     表达式持久化
        /// </summary>
        private ExpressionBuilder _expBuilder;

        /// <summary>
        ///     存储过程生成器
        /// </summary>
        private ProcBuilder _procBuilder;

        /// <summary>
        ///     SQL生成器
        /// </summary>
        private AbsSqlBuilder _sqlBuilder;

        /// <summary>
        ///     保存字段映射的信息
        /// </summary>
        internal SetDataMap Map;

        internal Queue(InternalContext context, SetDataMap map)
        {
            ID      = Guid.NewGuid();
            Context = context;
            Map     = map;
        }

        /// <summary>
        ///     当前队列的ID
        /// </summary>
        private Guid ID { get; set; }

        /// <summary>
        ///     数据库上下文
        /// </summary>
        private InternalContext Context { get; set; }

        internal ExpressionBuilder ExpBuilder  => _expBuilder ??= new ExpressionBuilder(map: Map);
        internal AbsSqlBuilder     SqlBuilder  => _sqlBuilder ??= Context.DbProvider.CreateSqlBuilder(expBuilder: ExpBuilder, dbName: Map.DbName, tableName: Map.TableName);
        internal ProcBuilder       ProcBuilder => _procBuilder ??= new ProcBuilder(dbProvider: Context.DbProvider, setMap: Map);

        /// <summary>
        ///     复制条件
        /// </summary>
        /// <param name="queue"> 队列 </param>
        internal void Copy(Queue queue)
        {
            ID          = queue.ID;
            Context     = queue.Context;
            _expBuilder = queue.ExpBuilder;
            queue.Dispose();
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
                Context      = null;
                Map          = null;
                _expBuilder  = null;
                _sqlBuilder  = null;
                _procBuilder = null;
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