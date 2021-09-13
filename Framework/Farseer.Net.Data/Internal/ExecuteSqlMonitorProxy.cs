using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FS.Core;
using FS.Core.Entity;
using FS.Core.LinkTrack;
using FS.Data.Client;
using FS.Data.Data;
using FS.Data.Infrastructure;
using FS.DI;
using FS.Extends;
using FS.Utils.Common;

namespace FS.Data.Internal
{
    /// <summary> 将SQL发送到数据库（代理类、记录SQL、执行时间） </summary>
    internal sealed class ExecuteSqlMonitorProxy : IExecuteSql
    {
        private readonly IExecuteSql   _dbExecutor;
        private readonly AbsDbProvider _dbProvider;

        /// <summary>
        /// 本次执行的SQL 
        /// </summary>
        public ISqlParam SqlParam { get; set; }

        /// <summary>
        ///     将SQL发送到数据库（代理类、记录SQL、执行时间）
        /// </summary>
        /// <param name="db">数据库执行者</param>
        /// <param name="absDbProvider"> </param>
        internal ExecuteSqlMonitorProxy(IExecuteSql db, AbsDbProvider dbProvider)
        {
            _dbExecutor = db;
            _dbProvider = dbProvider;
        }

        public DbExecutor DataBase => _dbExecutor.DataBase;

        public int Execute(string callMethod, ISqlParam sqlParam)
        {
            SqlParam = sqlParam;
            return SpeedTest(callMethod, sqlParam.DbName, sqlParam.TableName, CommandType.Text, sqlParam.Sql.ToString(), sqlParam.Param, () => _dbExecutor.Execute(callMethod, sqlParam));
        }

        public Task<int> ExecuteAsync(string callMethod, ISqlParam sqlParam)
        {
            SqlParam = sqlParam;
            return SpeedTestAsync(callMethod, sqlParam.DbName, sqlParam.TableName, CommandType.Text, sqlParam.Sql.ToString(), sqlParam.Param, () => _dbExecutor.ExecuteAsync(callMethod, sqlParam));
        }

        public int Execute<TEntity>(string callMethod, ProcBuilder procBuilder, TEntity entity) where TEntity : class, new()
        {
            SqlParam = new SqlParam(procBuilder);
            return SpeedTest(callMethod, procBuilder.DbName, procBuilder.ProcName, CommandType.StoredProcedure, null, procBuilder.Param, () => _dbExecutor.Execute(callMethod, procBuilder, entity));
        }

        public Task<int> ExecuteAsync<TEntity>(string callMethod, ProcBuilder procBuilder, TEntity entity) where TEntity : class, new()
        {
            SqlParam = new SqlParam(procBuilder);
            return SpeedTestAsync(callMethod, procBuilder.DbName, procBuilder.ProcName, CommandType.StoredProcedure, null, procBuilder.Param, () => _dbExecutor.ExecuteAsync(callMethod, procBuilder, entity));
        }

        public DataTable ToTable(string callMethod, ISqlParam sqlParam)
        {
            SqlParam = sqlParam;
            return SpeedTest(callMethod, sqlParam.DbName, sqlParam.TableName, CommandType.Text, sqlParam.Sql.ToString(), sqlParam.Param, () => _dbExecutor.ToTable(callMethod, sqlParam));
        }

        public Task<DataTable> ToTableAsync(string callMethod, ISqlParam sqlParam)
        {
            SqlParam = sqlParam;
            return SpeedTestAsync(callMethod, sqlParam.DbName, sqlParam.TableName, CommandType.Text, sqlParam.Sql.ToString(), sqlParam.Param, () => _dbExecutor.ToTableAsync(callMethod, sqlParam));
        }

        public DataTable ToTable<TEntity>(string callMethod, ProcBuilder procBuilder, TEntity entity) where TEntity : class, new()
        {
            SqlParam = new SqlParam(procBuilder);
            return SpeedTest(callMethod, procBuilder.DbName, procBuilder.ProcName, CommandType.StoredProcedure, null, procBuilder.Param, () => _dbExecutor.ToTable(callMethod, procBuilder, entity));
        }

        public Task<DataTable> ToTableAsync<TEntity>(string callMethod, ProcBuilder procBuilder, TEntity entity)
            where TEntity : class, new()
        {
            SqlParam = new SqlParam(procBuilder);
            return SpeedTestAsync(callMethod, procBuilder.DbName, procBuilder.ProcName, CommandType.StoredProcedure, null, procBuilder.Param, () => _dbExecutor.ToTableAsync(callMethod, procBuilder, entity));
        }

        public List<TEntity> ToList<TEntity>(string callMethod, ISqlParam sqlParam) where TEntity : class, new()
        {
            SqlParam = sqlParam;
            return SpeedTest(callMethod, sqlParam.DbName, sqlParam.TableName, CommandType.Text, sqlParam.Sql.ToString(), sqlParam.Param, () => _dbExecutor.ToList<TEntity>(callMethod, sqlParam));
        }

        public Task<List<TEntity>> ToListAsync<TEntity>(string callMethod, ISqlParam sqlParam) where TEntity : class, new()
        {
            SqlParam = sqlParam;
            return SpeedTestAsync(callMethod, sqlParam.DbName, sqlParam.TableName, CommandType.Text, sqlParam.Sql.ToString(), sqlParam.Param, () => _dbExecutor.ToListAsync<TEntity>(callMethod, sqlParam));
        }

        public List<TEntity> ToList<TEntity>(string callMethod, ProcBuilder procBuilder, TEntity entity) where TEntity : class, new()
        {
            SqlParam = new SqlParam(procBuilder);
            return SpeedTest(callMethod, procBuilder.DbName, procBuilder.ProcName, CommandType.StoredProcedure, null, procBuilder.Param, () => _dbExecutor.ToList(callMethod, procBuilder, entity));
        }

        public Task<List<TEntity>> ToListAsync<TEntity>(string callMethod, ProcBuilder procBuilder, TEntity entity)
            where TEntity : class, new()
        {
            SqlParam = new SqlParam(procBuilder);
            return SpeedTestAsync(callMethod, procBuilder.DbName, procBuilder.ProcName, CommandType.StoredProcedure, null, procBuilder.Param, () => _dbExecutor.ToListAsync(callMethod, procBuilder, entity));
        }

        TEntity IExecuteSql.ToEntity<TEntity>(string callMethod, ISqlParam sqlParam)
        {
            SqlParam = sqlParam;
            return SpeedTest(callMethod, sqlParam.DbName, sqlParam.TableName, CommandType.Text, sqlParam.Sql.ToString(), sqlParam.Param, () => _dbExecutor.ToEntity<TEntity>(callMethod, sqlParam));
        }

        public Task<TEntity> ToEntityAsync<TEntity>(string callMethod, ISqlParam sqlParam) where TEntity : class, new()
        {
            SqlParam = sqlParam;
            return SpeedTestAsync(callMethod, sqlParam.DbName, sqlParam.TableName, CommandType.Text, sqlParam.Sql.ToString(), sqlParam.Param, () => _dbExecutor.ToEntityAsync<TEntity>(callMethod, sqlParam));
        }

        public TEntity ToEntity<TEntity>(string callMethod, ProcBuilder procBuilder, TEntity entity) where TEntity : class, new()
        {
            SqlParam = new SqlParam(procBuilder);
            return SpeedTest(callMethod, procBuilder.DbName, procBuilder.ProcName, CommandType.StoredProcedure, null, procBuilder.Param, () => _dbExecutor.ToEntity(callMethod, procBuilder, entity));
        }

        public Task<TEntity> ToEntityAsync<TEntity>(string callMethod, ProcBuilder procBuilder, TEntity entity)
            where TEntity : class, new()
        {
            SqlParam = new SqlParam(procBuilder);
            return SpeedTestAsync(callMethod, procBuilder.DbName, procBuilder.ProcName, CommandType.StoredProcedure, null, procBuilder.Param, () => _dbExecutor.ToEntityAsync(callMethod, procBuilder, entity));
        }

        public T GetValue<T>(string callMethod, ISqlParam sqlParam, T defValue = default(T))
        {
            SqlParam = sqlParam;
            return SpeedTest(callMethod, sqlParam.DbName, sqlParam.TableName, CommandType.Text, sqlParam.Sql.ToString(), sqlParam.Param, () => _dbExecutor.GetValue(callMethod, sqlParam, defValue));
        }

        public Task<T> GetValueAsync<T>(string callMethod, ISqlParam sqlParam, T defValue = default(T))
        {
            SqlParam = sqlParam;
            return SpeedTestAsync(callMethod, sqlParam.DbName, sqlParam.TableName, CommandType.Text, sqlParam.Sql.ToString(), sqlParam.Param, () => _dbExecutor.GetValueAsync(callMethod, sqlParam, defValue));
        }

        public T GetValue<TEntity, T>(string callMethod, ProcBuilder procBuilder, TEntity entity, T defValue = default(T))
            where TEntity : class, new()
        {
            SqlParam = new SqlParam(procBuilder);
            return SpeedTest(callMethod, procBuilder.DbName, procBuilder.ProcName, CommandType.StoredProcedure, null, procBuilder.Param, () => _dbExecutor.GetValue(callMethod, procBuilder, entity, defValue));
        }

        public Task<T> GetValueAsync<TEntity, T>(string callMethod, ProcBuilder procBuilder, TEntity entity, T defValue = default(T)) where TEntity : class, new()
        {
            SqlParam = new SqlParam(procBuilder);
            return SpeedTestAsync(callMethod, procBuilder.DbName, procBuilder.ProcName, CommandType.StoredProcedure, null, procBuilder.Param, () => _dbExecutor.GetValueAsync(callMethod, procBuilder, entity, defValue));
        }

        /// <summary>
        ///     计算执行时间
        /// </summary>
        private TReturn SpeedTest<TReturn>(string callMethod, string dbName, string tableName, CommandType cmdType, string sql, IEnumerable<DbParameter> param, Func<TReturn> func)
        {
            // 调用链上下文
            var dbLinkTrackDetail = new DbLinkTrackDetail
            {
                DataBaseName = dbName,
                TableName    = tableName,
                CommandType  = cmdType,
                Sql          = sql,
                SqlParam     = _dbProvider.IsSupportParam ? param.ToDictionary(o => o.ParameterName, o => o.Value.ToString()) : new()
            };

            using (FsLinkTrack.TrackDatabase(callMethod, dbLinkTrackDetail))
            {
                return func();
            }
        }

        /// <summary>
        ///     计算执行时间
        /// </summary>
        private async Task<TReturn> SpeedTestAsync<TReturn>(string callMethod, string dbName, string tableName, CommandType cmdType, string sql, IEnumerable<DbParameter> param, Func<Task<TReturn>> func)
        {
            // 调用链上下文
            var dbLinkTrackDetail = new DbLinkTrackDetail
            {
                DataBaseName = dbName,
                TableName    = tableName,
                CommandType  = cmdType,
                Sql          = sql,
                SqlParam     = _dbProvider.IsSupportParam ? param.ToDictionary(o => o.ParameterName, o => o.Value.ToString()) : new()
            };

            using (FsLinkTrack.TrackDatabase(callMethod, dbLinkTrackDetail))
            {
                return await func();
            }
        }
    }
}