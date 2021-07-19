using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using FS.Data.Internal;

namespace FS.Data
{
    /// <summary>
    ///     手动编写SQL
    /// </summary>
    public class ManualSql
    {
        /// <summary>
        ///     手动编写SQL
        /// </summary>
        /// <param name="context">上下文</param>
        internal ManualSql(InternalContext context)
        {
            _context = context;
        }

        /// <summary>
        ///     数据库上下文
        /// </summary>
        private readonly InternalContext _context;

        /// <summary>
        ///     返回查询的值
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="t">失败时返回的值</param>
        public T GetValue<T>(string sql, T t = default(T), params DbParameter[] parameters) => _context.QueueManger.Commit(null, (queue) => _context.Executeor.GetValue($"ManualSql.GetValue", new SqlParam(sql, parameters), t), false);

        /// <summary>
        ///     返回查询的值
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="t">失败时返回的值</param>
        public Task<T> GetValueAsync<T>(string sql, T t = default(T), params DbParameter[] parameters) => _context.QueueManger.CommitAsync(null, (queue) => _context.Executeor.GetValueAsync($"ManualSql.GetValueAsync", new SqlParam(sql, parameters), t), false);

        /// <summary>
        ///     返回单条记录
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        public TEntity ToEntity<TEntity>(string sql, params DbParameter[] parameters) where TEntity : class, new() => _context.QueueManger.Commit(null, (queue) => _context.Executeor.ToEntity<TEntity>($"ManualSql.ToEntity", new SqlParam(sql, parameters)), false);

        /// <summary>
        ///     返回单条记录
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        public Task<TEntity> ToEntityAsync<TEntity>(string sql, params DbParameter[] parameters) where TEntity : class, new() => _context.QueueManger.CommitAsync(null, (queue) => _context.Executeor.ToEntityAsync<TEntity>($"ManualSql.ToEntityAsync", new SqlParam(sql, parameters)), false);

        /// <summary>
        ///     返回DataTable
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        public DataTable ToTable(string sql, params DbParameter[] parameters) => _context.QueueManger.Commit(null, (queue) => _context.Executeor.ToTable($"ManualSql.ToTable", new SqlParam(sql, parameters)), false);

        /// <summary>
        ///     返回DataTable异步
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        public Task<DataTable> ToTableAsync(string sql, params DbParameter[] parameters) => _context.QueueManger.CommitAsync(null, (queue) => _context.Executeor.ToTableAsync($"ManualSql.ToTableAsync", new SqlParam(sql, parameters)), false);

        /// <summary>
        ///     返回多条记录
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        public List<TEntity> ToList<TEntity>(string sql, params DbParameter[] parameters) where TEntity : class, new() => _context.QueueManger.Commit(null, (queue) => _context.Executeor.ToList<TEntity>($"ManualSql.ToList", new SqlParam(sql, parameters)), false);

        /// <summary>
        ///     返回多条记录
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        public Task<List<TEntity>> ToListAsync<TEntity>(string sql, params DbParameter[] parameters) where TEntity : class, new() => _context.QueueManger.CommitAsync(null, (queue) => _context.Executeor.ToListAsync<TEntity>($"ManualSql.ToListAsync", new SqlParam(sql, parameters)), false);

        /// <summary>
        ///     返回影响行数
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        public int Execute(string sql, params DbParameter[] parameters) => _context.QueueManger.Commit(null, (queue) => _context.Executeor.Execute($"ManualSql.Execute", new SqlParam(sql, parameters)), false);

        /// <summary>
        ///     返回影响行数
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        public Task<int> ExecuteAsync(string sql, params DbParameter[] parameters) => _context.QueueManger.CommitAsync(null, (queue) => _context.Executeor.ExecuteAsync($"ManualSql.ExecuteAsync", new SqlParam(sql, parameters)), false);
    }
}