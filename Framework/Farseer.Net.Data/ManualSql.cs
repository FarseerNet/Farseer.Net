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
        ///     数据库上下文
        /// </summary>
        private readonly InternalContext _context;

        /// <summary>
        ///     手动编写SQL
        /// </summary>
        /// <param name="context"> 上下文 </param>
        internal ManualSql(InternalContext context)
        {
            _context = context;
        }

        /// <summary>
        ///     返回查询的值
        /// </summary>
        /// <param name="sql"> SQL语句 </param>
        /// <param name="parameters"> 参数 </param>
        /// <param name="t"> 失败时返回的值 </param>
        public T GetValue<T>(string sql, T t = default, params DbParameter[] parameters) => _context.QueueManger.Commit(map: null, act: queue => _context.Executeor.GetValue(callMethod: "ManualSql.GetValue", sqlParam: new SqlParam(sql: sql, parameters: parameters), defValue: t), joinSoftDeleteCondition: false);

        /// <summary>
        ///     返回查询的值
        /// </summary>
        /// <param name="sql"> SQL语句 </param>
        /// <param name="parameters"> 参数 </param>
        /// <param name="t"> 失败时返回的值 </param>
        public Task<T> GetValueAsync<T>(string sql, T t = default, params DbParameter[] parameters) => _context.QueueManger.CommitAsync(map: null, act: queue => _context.Executeor.GetValueAsync(callMethod: "ManualSql.GetValueAsync", sqlParam: new SqlParam(sql: sql, parameters: parameters), defValue: t), joinSoftDeleteCondition: false);

        /// <summary>
        ///     返回单条记录
        /// </summary>
        /// <param name="sql"> SQL语句 </param>
        /// <param name="parameters"> 参数 </param>
        public TEntity ToEntity<TEntity>(string sql, params DbParameter[] parameters) where TEntity : class, new() => _context.QueueManger.Commit(map: null, act: queue => _context.Executeor.ToEntity<TEntity>(callMethod: "ManualSql.ToEntity", sqlParam: new SqlParam(sql: sql, parameters: parameters)), joinSoftDeleteCondition: false);

        /// <summary>
        ///     返回单条记录
        /// </summary>
        /// <param name="sql"> SQL语句 </param>
        /// <param name="parameters"> 参数 </param>
        public Task<TEntity> ToEntityAsync<TEntity>(string sql, params DbParameter[] parameters) where TEntity : class, new() => _context.QueueManger.CommitAsync(map: null, act: queue => _context.Executeor.ToEntityAsync<TEntity>(callMethod: "ManualSql.ToEntityAsync", sqlParam: new SqlParam(sql: sql, parameters: parameters)), joinSoftDeleteCondition: false);

        /// <summary>
        ///     返回DataTable
        /// </summary>
        /// <param name="sql"> SQL语句 </param>
        /// <param name="parameters"> 参数 </param>
        public DataTable ToTable(string sql, params DbParameter[] parameters) => _context.QueueManger.Commit(map: null, act: queue => _context.Executeor.ToTable(callMethod: "ManualSql.ToTable", sqlParam: new SqlParam(sql: sql, parameters: parameters)), joinSoftDeleteCondition: false);

        /// <summary>
        ///     返回DataTable异步
        /// </summary>
        /// <param name="sql"> SQL语句 </param>
        /// <param name="parameters"> 参数 </param>
        public Task<DataTable> ToTableAsync(string sql, params DbParameter[] parameters) => _context.QueueManger.CommitAsync(map: null, act: queue => _context.Executeor.ToTableAsync(callMethod: "ManualSql.ToTableAsync", sqlParam: new SqlParam(sql: sql, parameters: parameters)), joinSoftDeleteCondition: false);

        /// <summary>
        ///     返回多条记录
        /// </summary>
        /// <param name="sql"> SQL语句 </param>
        /// <param name="parameters"> 参数 </param>
        public List<TEntity> ToList<TEntity>(string sql, params DbParameter[] parameters) where TEntity : class, new() => _context.QueueManger.Commit(map: null, act: queue => _context.Executeor.ToList<TEntity>(callMethod: "ManualSql.ToList", sqlParam: new SqlParam(sql: sql, parameters: parameters)), joinSoftDeleteCondition: false);

        /// <summary>
        ///     返回多条记录
        /// </summary>
        /// <param name="sql"> SQL语句 </param>
        /// <param name="parameters"> 参数 </param>
        public Task<List<TEntity>> ToListAsync<TEntity>(string sql, params DbParameter[] parameters) where TEntity : class, new() => _context.QueueManger.CommitAsync(map: null, act: queue => _context.Executeor.ToListAsync<TEntity>(callMethod: "ManualSql.ToListAsync", sqlParam: new SqlParam(sql: sql, parameters: parameters)), joinSoftDeleteCondition: false);

        /// <summary>
        ///     返回影响行数
        /// </summary>
        /// <param name="sql"> SQL语句 </param>
        /// <param name="parameters"> 参数 </param>
        public int Execute(string sql, params DbParameter[] parameters) => _context.QueueManger.Commit(map: null, act: queue => _context.Executeor.Execute(callMethod: "ManualSql.Execute", sqlParam: new SqlParam(sql: sql, parameters: parameters)), joinSoftDeleteCondition: false);

        /// <summary>
        ///     返回影响行数
        /// </summary>
        /// <param name="sql"> SQL语句 </param>
        /// <param name="parameters"> 参数 </param>
        public Task<int> ExecuteAsync(string sql, params DbParameter[] parameters) => _context.QueueManger.CommitAsync(map: null, act: queue => _context.Executeor.ExecuteAsync(callMethod: "ManualSql.ExecuteAsync", sqlParam: new SqlParam(sql: sql, parameters: parameters)), joinSoftDeleteCondition: false);
    }
}