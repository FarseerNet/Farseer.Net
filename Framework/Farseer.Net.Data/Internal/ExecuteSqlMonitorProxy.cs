using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;
using FS.Core;
using FS.Data.Data;
using FS.Data.Infrastructure;
using FS.DI;

namespace FS.Data.Internal
{
    /// <summary> 将SQL发送到数据库（代理类、记录SQL、执行时间） </summary>
    internal sealed class ExecuteSqlMonitorProxy : IExecuteSql
    {
        private readonly IExecuteSql _dbExecutor;
        /// <summary>SQL执行监控 </summary>
        private readonly ISqlMonitor[] _sqlMonitors;

        /// <summary>
        ///     将SQL发送到数据库（代理类、记录SQL、执行时间）
        /// </summary>
        /// <param name="db">数据库执行者</param>
        internal ExecuteSqlMonitorProxy(IExecuteSql db)
        {
            _dbExecutor = db;
            _sqlMonitors = IocManager.Instance.ResolveAll<ISqlMonitor>();
        }

        public DbExecutor DataBase => _dbExecutor.DataBase;

        public int Execute(ISqlParam sqlParam)
        {
            return SpeedTest("Execute", sqlParam.Name, CommandType.Text, sqlParam.Sql.ToString(), sqlParam.Param, () => _dbExecutor.Execute(sqlParam));
        }

        public Task<int> ExecuteAsync(ISqlParam sqlParam)
        {
            return SpeedTest("ExecuteAsync", sqlParam.Name, CommandType.Text, sqlParam.Sql.ToString(), sqlParam.Param, async () => await _dbExecutor.ExecuteAsync(sqlParam));
        }

        public int Execute<TEntity>(ProcBuilder procBuilder, TEntity entity) where TEntity : class, new()
        {
            return SpeedTest("Execute", procBuilder.Name, CommandType.StoredProcedure, null, procBuilder.Param, () => _dbExecutor.Execute(procBuilder, entity));
        }

        public Task<int> ExecuteAsync<TEntity>(ProcBuilder procBuilder, TEntity entity) where TEntity : class, new()
        {
            return SpeedTest("ExecuteAsync", procBuilder.Name, CommandType.StoredProcedure, null, procBuilder.Param, async () => await _dbExecutor.ExecuteAsync(procBuilder, entity));
        }

        public DataTable ToTable(ISqlParam sqlParam)
        {
            return SpeedTest("ToTable", sqlParam.Name, CommandType.Text, sqlParam.Sql.ToString(), sqlParam.Param, () => _dbExecutor.ToTable(sqlParam));
        }

        public Task<DataTable> ToTableAsync(ISqlParam sqlParam)
        {
            return SpeedTest("ToTableAsync", sqlParam.Name, CommandType.Text, sqlParam.Sql.ToString(), sqlParam.Param, async () => await _dbExecutor.ToTableAsync(sqlParam));
        }

        public DataTable ToTable<TEntity>(ProcBuilder procBuilder, TEntity entity) where TEntity : class, new()
        {
            return SpeedTest("ToTable", procBuilder.Name, CommandType.StoredProcedure, null, procBuilder.Param, () => _dbExecutor.ToTable(procBuilder, entity));
        }

        public Task<DataTable> ToTableAsync<TEntity>(ProcBuilder procBuilder, TEntity entity)
            where TEntity : class, new()
        {
            return SpeedTest("ToTableAsync", procBuilder.Name, CommandType.StoredProcedure, null, procBuilder.Param, async () => await _dbExecutor.ToTableAsync(procBuilder, entity));
        }

        public List<TEntity> ToList<TEntity>(ISqlParam sqlParam) where TEntity : class, new()
        {
            return SpeedTest("ToList", sqlParam.Name, CommandType.Text, sqlParam.Sql.ToString(), sqlParam.Param, () => _dbExecutor.ToList<TEntity>(sqlParam));
        }

        public Task<List<TEntity>> ToListAsync<TEntity>(ISqlParam sqlParam) where TEntity : class, new()
        {
            return SpeedTest("ToListAsync", sqlParam.Name, CommandType.Text, sqlParam.Sql.ToString(), sqlParam.Param, async () => await _dbExecutor.ToListAsync<TEntity>(sqlParam));
        }

        public List<TEntity> ToList<TEntity>(ProcBuilder procBuilder, TEntity entity) where TEntity : class, new()
        {
            return SpeedTest("ToList", procBuilder.Name, CommandType.StoredProcedure, null, procBuilder.Param, () => _dbExecutor.ToList(procBuilder, entity));
        }

        public Task<List<TEntity>> ToListAsync<TEntity>(ProcBuilder procBuilder, TEntity entity)
            where TEntity : class, new()
        {
            return SpeedTest("ToListAsync", procBuilder.Name, CommandType.StoredProcedure, null, procBuilder.Param, async () => await _dbExecutor.ToListAsync(procBuilder, entity));
        }

        TEntity IExecuteSql.ToEntity<TEntity>(ISqlParam sqlParam)
        {
            return SpeedTest("ToEntity", sqlParam.Name, CommandType.Text, sqlParam.Sql.ToString(), sqlParam.Param, () => _dbExecutor.ToEntity<TEntity>(sqlParam));
        }

        public Task<TEntity> ToEntityAsync<TEntity>(ISqlParam sqlParam) where TEntity : class, new()
        {
            return SpeedTest("ToEntityAsync", sqlParam.Name, CommandType.Text, sqlParam.Sql.ToString(), sqlParam.Param, async () => await _dbExecutor.ToEntityAsync<TEntity>(sqlParam));
        }

        public TEntity ToEntity<TEntity>(ProcBuilder procBuilder, TEntity entity) where TEntity : class, new()
        {
            return SpeedTest("ToEntity", procBuilder.Name, CommandType.StoredProcedure, null, procBuilder.Param, () => _dbExecutor.ToEntity(procBuilder, entity));
        }

        public Task<TEntity> ToEntityAsync<TEntity>(ProcBuilder procBuilder, TEntity entity)
            where TEntity : class, new()
        {
            return SpeedTest("ToEntityAsync", procBuilder.Name, CommandType.StoredProcedure, null, procBuilder.Param, async () => await _dbExecutor.ToEntityAsync(procBuilder, entity));
        }

        public T GetValue<T>(ISqlParam sqlParam, T defValue = default(T))
        {
            return SpeedTest("GetValue", sqlParam.Name, CommandType.Text, sqlParam.Sql.ToString(), sqlParam.Param, () => _dbExecutor.GetValue(sqlParam, defValue));
        }

        public Task<T> GetValueAsync<T>(ISqlParam sqlParam, T defValue = default(T))
        {
            return SpeedTest("GetValueAsync", sqlParam.Name, CommandType.Text, sqlParam.Sql.ToString(), sqlParam.Param, async () => await _dbExecutor.GetValueAsync(sqlParam, defValue));
        }

        public T GetValue<TEntity, T>(ProcBuilder procBuilder, TEntity entity, T defValue = default(T))
            where TEntity : class, new()
        {
            return SpeedTest("GetValue", procBuilder.Name, CommandType.StoredProcedure, null, procBuilder.Param, () => _dbExecutor.GetValue(procBuilder, entity, defValue));
        }

        public Task<T> GetValueAsync<TEntity, T>(ProcBuilder procBuilder, TEntity entity, T defValue = default(T)) where TEntity : class, new()
        {
            return SpeedTest("GetValueAsync", procBuilder.Name, CommandType.StoredProcedure, null, procBuilder.Param, async () => await _dbExecutor.GetValueAsync(procBuilder, entity, defValue));
        }

        /// <summary>
        ///     计算执行时间
        /// </summary>
        private TReturn SpeedTest<TReturn>(string methodName, string name, CommandType cmdType, string sql, IEnumerable<DbParameter> param, Func<TReturn> func)
        {
            var timer = new Stopwatch();
            try
            {
                // 执行前
                foreach (var sqlMonitor in _sqlMonitors) { sqlMonitor.PreExecute(methodName, name, cmdType, sql, param); }

                timer.Start();
                var val = func();
                timer.Stop();

                // 执行后
                foreach (var sqlMonitor in _sqlMonitors) { sqlMonitor.Executed(methodName, name, cmdType, sql, param, timer.ElapsedMilliseconds, val); }
                return val;
            }
            catch (Exception e)
            {
                foreach (var sqlMonitor in _sqlMonitors) { sqlMonitor.ExecuteException(methodName, name, cmdType, sql, param, timer.ElapsedMilliseconds, e); }
                throw;
            }
        }
    }
}