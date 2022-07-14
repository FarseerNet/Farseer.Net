using System;
using FS.Data.Client;
using FS.Data.Map;

namespace FS.Data.Internal
{
    /// <summary>
    ///     队列，每一次的查询将生成一个新的实例
    /// </summary>
    internal class Query : IDisposable
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

        internal Query(InternalContext context, SetDataMap map)
        {
            ID      = Guid.NewGuid();
            Context = context;
            Map     = map;
        }

        /// <summary>
        ///     当前队列的ID
        /// </summary>
        internal Guid ID { get; set; }

        /// <summary>
        ///     数据库上下文
        /// </summary>
        private InternalContext Context { get; set; }

        internal ExpressionBuilder ExpBuilder => _expBuilder ??= new ExpressionBuilder(map: Map);

        internal AbsSqlBuilder SqlBuilder  => _sqlBuilder ??= Context.DbProvider.CreateSqlBuilder(expBuilder: ExpBuilder, Map);
        internal ProcBuilder   ProcBuilder => _procBuilder ??= new ProcBuilder(dbProvider: Context.DbProvider, setMap: Map);

        /// <summary>
        ///     复制条件
        /// </summary>
        internal void Copy(Guid id, ExpressionBuilder expBuilder)
        {
            ID          = id;
            _expBuilder = expBuilder;
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
                if (_sqlBuilder != null)
                {
                    _sqlBuilder.Dispose();
                    _sqlBuilder = null;
                }
                if (_procBuilder != null)
                {
                    _procBuilder.Dispose();
                    _procBuilder = null;
                }
                if (_expBuilder != null)
                {
                    _expBuilder.Dispose();
                    _expBuilder = null;
                }
                Context = null;
                Map     = null;
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