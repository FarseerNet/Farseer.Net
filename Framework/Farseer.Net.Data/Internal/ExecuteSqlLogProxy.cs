using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using FS.Data.Data;
using FS.Data.Infrastructure;
using FS.Data.Log.Default;

namespace FS.Data.Internal
{
    /// <summary> 将SQL发送到数据库（代理类、记录SQL、执行时间） </summary>
    internal sealed class ExecuteSqlLogProxy : IExecuteSql
    {
        /// <summary>
        ///     将SQL发送到数据库（代理类、记录SQL、执行时间）
        /// </summary>
        /// <param name="db">数据库执行者</param>
        internal ExecuteSqlLogProxy(IExecuteSql db)
        {
            _dbExecutor = db;
        }

        private readonly IExecuteSql _dbExecutor;
        public DbExecutor DataBase => _dbExecutor.DataBase;

        /// <summary>
        ///     计算执行时间
        /// </summary>
        private TReturn SpeedTest<TReturn>(ISqlParam sqlParam, Func<TReturn> func)
        {
            var timer = new Stopwatch();
            timer.Start();
            var val = func();
            timer.Stop();
            new SqlRunLog(sqlParam.DbName, sqlParam.TableName, CommandType.Text, sqlParam.Sql.ToString(), sqlParam.Param, timer.ElapsedMilliseconds).Write();
            return val;
        }

        /// <summary>
        ///     计算执行时间
        /// </summary>
        private TReturn SpeedTest<TReturn>(ProcBuilder procBuilder, Func<TReturn> func)
        {
            var timer = new Stopwatch();
            timer.Start();
            var val = func();
            timer.Stop();

            new SqlRunLog(procBuilder.DbName, procBuilder.ProcName, CommandType.StoredProcedure, "", procBuilder.Param, timer.ElapsedMilliseconds).Write();
            return val;
        }

        public int Execute(ISqlParam sqlParam) => SpeedTest(sqlParam, () => _dbExecutor.Execute(sqlParam));
        public Task<int> ExecuteAsync(ISqlParam sqlParam) => SpeedTest(sqlParam, async () => await _dbExecutor.ExecuteAsync(sqlParam));

        public int Execute<TEntity>(ProcBuilder procBuilder, TEntity entity) where TEntity : class, new() => SpeedTest(procBuilder, () => _dbExecutor.Execute(procBuilder, entity));
        public Task<int> ExecuteAsync<TEntity>(ProcBuilder procBuilder, TEntity entity) where TEntity : class, new() => SpeedTest(procBuilder, async () => await _dbExecutor.ExecuteAsync(procBuilder, entity));

        public DataTable ToTable(ISqlParam sqlParam) => SpeedTest(sqlParam, () => _dbExecutor.ToTable(sqlParam));
        public Task<DataTable> ToTableAsync(ISqlParam sqlParam) => SpeedTest(sqlParam, async () => await _dbExecutor.ToTableAsync(sqlParam));

        public DataTable ToTable<TEntity>(ProcBuilder procBuilder, TEntity entity) where TEntity : class, new() => SpeedTest(procBuilder, () => _dbExecutor.ToTable(procBuilder, entity));
        public Task<DataTable> ToTableAsync<TEntity>(ProcBuilder procBuilder, TEntity entity) where TEntity : class, new() => SpeedTest(procBuilder, async () => await _dbExecutor.ToTableAsync(procBuilder, entity));

        public List<TEntity> ToList<TEntity>(ISqlParam sqlParam) where TEntity : class, new() => SpeedTest(sqlParam, () => _dbExecutor.ToList<TEntity>(sqlParam));
        public Task<List<TEntity>> ToListAsync<TEntity>(ISqlParam sqlParam) where TEntity : class, new() => SpeedTest(sqlParam, async () => await _dbExecutor.ToListAsync<TEntity>(sqlParam));

        public List<TEntity> ToList<TEntity>(ProcBuilder procBuilder, TEntity entity) where TEntity : class, new() => SpeedTest(procBuilder, () => _dbExecutor.ToList(procBuilder, entity));
        public Task<List<TEntity>> ToListAsync<TEntity>(ProcBuilder procBuilder, TEntity entity) where TEntity : class, new() => SpeedTest(procBuilder, async () => await _dbExecutor.ToListAsync(procBuilder, entity));

        TEntity IExecuteSql.ToEntity<TEntity>(ISqlParam sqlParam) => SpeedTest(sqlParam, () => _dbExecutor.ToEntity<TEntity>(sqlParam));
        public Task<TEntity> ToEntityAsync<TEntity>(ISqlParam sqlParam) where TEntity : class, new() => SpeedTest(sqlParam, async () => await _dbExecutor.ToEntityAsync<TEntity>(sqlParam));

        public TEntity ToEntity<TEntity>(ProcBuilder procBuilder, TEntity entity) where TEntity : class, new() => SpeedTest(procBuilder, () => _dbExecutor.ToEntity(procBuilder, entity));
        public Task<TEntity> ToEntityAsync<TEntity>(ProcBuilder procBuilder, TEntity entity) where TEntity : class, new() => SpeedTest(procBuilder, async () => await _dbExecutor.ToEntityAsync(procBuilder, entity));

        public T GetValue<T>(ISqlParam sqlParam, T defValue = default(T)) => SpeedTest(sqlParam, () => _dbExecutor.GetValue(sqlParam, defValue));
        public Task<T> GetValueAsync<T>(ISqlParam sqlParam, T defValue = default(T)) => SpeedTest(sqlParam, async () => await _dbExecutor.GetValueAsync(sqlParam, defValue));

        public T GetValue<TEntity, T>(ProcBuilder procBuilder, TEntity entity, T defValue = default(T)) where TEntity : class, new() => SpeedTest(procBuilder, () => _dbExecutor.GetValue(procBuilder, entity, defValue));
        public Task<T> GetValueAsync<TEntity, T>(ProcBuilder procBuilder, TEntity entity, T defValue = default(T)) where TEntity : class, new() => SpeedTest(procBuilder, async () => await _dbExecutor.GetValueAsync(procBuilder, entity, defValue));
    }
}