using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using FS.Core.LinkTrack;
using FS.Data.Client;
using FS.Data.Data;
using FS.Data.Infrastructure;

namespace FS.Data.Internal
{
    /// <summary> 将SQL发送到数据库（代理类、记录SQL、执行时间） </summary>
    internal sealed class ExecuteSqlMonitorProxy : IExecuteSql
    {
        private readonly IExecuteSql   _dbExecutor;
        private readonly AbsDbProvider _dbProvider;

        /// <summary>
        ///     将SQL发送到数据库（代理类、记录SQL、执行时间）
        /// </summary>
        /// <param name="db"> 数据库执行者 </param>
        /// <param name="absDbProvider"> </param>
        internal ExecuteSqlMonitorProxy(IExecuteSql db, AbsDbProvider dbProvider)
        {
            _dbExecutor = db;
            _dbProvider = dbProvider;
        }

        /// <summary>
        ///     本次执行的SQL
        /// </summary>
        public ISqlParam SqlParam { get; set; }

        public DbExecutor DataBase => _dbExecutor.DataBase;

        public int Execute(string callMethod, ISqlParam sqlParam)
        {
            SqlParam = sqlParam;
            return SpeedTest(callMethod: callMethod, dbName: sqlParam.DbName, tableName: sqlParam.TableName, cmdType: CommandType.Text, sql: sqlParam.Sql.ToString(), param: sqlParam.Param, func: () => _dbExecutor.Execute(callMethod: callMethod, sqlParam: sqlParam));
        }

        public Task<int> ExecuteAsync(string callMethod, ISqlParam sqlParam)
        {
            SqlParam = sqlParam;
            return SpeedTestAsync(callMethod: callMethod, dbName: sqlParam.DbName, tableName: sqlParam.TableName, cmdType: CommandType.Text, sql: sqlParam.Sql.ToString(), param: sqlParam.Param, func: () => _dbExecutor.ExecuteAsync(callMethod: callMethod, sqlParam: sqlParam));
        }

        public int Execute<TEntity>(string callMethod, ProcBuilder procBuilder, TEntity entity) where TEntity : class, new()
        {
            SqlParam = new SqlParam(procParam: procBuilder);
            return SpeedTest(callMethod: callMethod, dbName: procBuilder.DbName, tableName: procBuilder.ProcName, cmdType: CommandType.StoredProcedure, sql: null, param: procBuilder.Param, func: () => _dbExecutor.Execute(callMethod: callMethod, procBuilder: procBuilder, entity: entity));
        }

        public Task<int> ExecuteAsync<TEntity>(string callMethod, ProcBuilder procBuilder, TEntity entity) where TEntity : class, new()
        {
            SqlParam = new SqlParam(procParam: procBuilder);
            return SpeedTestAsync(callMethod: callMethod, dbName: procBuilder.DbName, tableName: procBuilder.ProcName, cmdType: CommandType.StoredProcedure, sql: null, param: procBuilder.Param, func: () => _dbExecutor.ExecuteAsync(callMethod: callMethod, procBuilder: procBuilder, entity: entity));
        }

        public DataTable ToTable(string callMethod, ISqlParam sqlParam)
        {
            SqlParam = sqlParam;
            return SpeedTest(callMethod: callMethod, dbName: sqlParam.DbName, tableName: sqlParam.TableName, cmdType: CommandType.Text, sql: sqlParam.Sql.ToString(), param: sqlParam.Param, func: () => _dbExecutor.ToTable(callMethod: callMethod, sqlParam: sqlParam));
        }

        public Task<DataTable> ToTableAsync(string callMethod, ISqlParam sqlParam)
        {
            SqlParam = sqlParam;
            return SpeedTestAsync(callMethod: callMethod, dbName: sqlParam.DbName, tableName: sqlParam.TableName, cmdType: CommandType.Text, sql: sqlParam.Sql.ToString(), param: sqlParam.Param, func: () => _dbExecutor.ToTableAsync(callMethod: callMethod, sqlParam: sqlParam));
        }

        public DataTable ToTable<TEntity>(string callMethod, ProcBuilder procBuilder, TEntity entity) where TEntity : class, new()
        {
            SqlParam = new SqlParam(procParam: procBuilder);
            return SpeedTest(callMethod: callMethod, dbName: procBuilder.DbName, tableName: procBuilder.ProcName, cmdType: CommandType.StoredProcedure, sql: null, param: procBuilder.Param, func: () => _dbExecutor.ToTable(callMethod: callMethod, procBuilder: procBuilder, entity: entity));
        }

        public Task<DataTable> ToTableAsync<TEntity>(string callMethod, ProcBuilder procBuilder, TEntity entity)
            where TEntity : class, new()
        {
            SqlParam = new SqlParam(procParam: procBuilder);
            return SpeedTestAsync(callMethod: callMethod, dbName: procBuilder.DbName, tableName: procBuilder.ProcName, cmdType: CommandType.StoredProcedure, sql: null, param: procBuilder.Param, func: () => _dbExecutor.ToTableAsync(callMethod: callMethod, procBuilder: procBuilder, entity: entity));
        }

        public List<TEntity> ToList<TEntity>(string callMethod, ISqlParam sqlParam) where TEntity : class, new()
        {
            SqlParam = sqlParam;
            return SpeedTest(callMethod: callMethod, dbName: sqlParam.DbName, tableName: sqlParam.TableName, cmdType: CommandType.Text, sql: sqlParam.Sql.ToString(), param: sqlParam.Param, func: () => _dbExecutor.ToList<TEntity>(callMethod: callMethod, sqlParam: sqlParam));
        }

        public Task<List<TEntity>> ToListAsync<TEntity>(string callMethod, ISqlParam sqlParam) where TEntity : class, new()
        {
            SqlParam = sqlParam;
            return SpeedTestAsync(callMethod: callMethod, dbName: sqlParam.DbName, tableName: sqlParam.TableName, cmdType: CommandType.Text, sql: sqlParam.Sql.ToString(), param: sqlParam.Param, func: () => _dbExecutor.ToListAsync<TEntity>(callMethod: callMethod, sqlParam: sqlParam));
        }

        public List<TEntity> ToList<TEntity>(string callMethod, ProcBuilder procBuilder, TEntity entity) where TEntity : class, new()
        {
            SqlParam = new SqlParam(procParam: procBuilder);
            return SpeedTest(callMethod: callMethod, dbName: procBuilder.DbName, tableName: procBuilder.ProcName, cmdType: CommandType.StoredProcedure, sql: null, param: procBuilder.Param, func: () => _dbExecutor.ToList(callMethod: callMethod, procBuilder: procBuilder, entity: entity));
        }

        public Task<List<TEntity>> ToListAsync<TEntity>(string callMethod, ProcBuilder procBuilder, TEntity entity)
            where TEntity : class, new()
        {
            SqlParam = new SqlParam(procParam: procBuilder);
            return SpeedTestAsync(callMethod: callMethod, dbName: procBuilder.DbName, tableName: procBuilder.ProcName, cmdType: CommandType.StoredProcedure, sql: null, param: procBuilder.Param, func: () => _dbExecutor.ToListAsync(callMethod: callMethod, procBuilder: procBuilder, entity: entity));
        }

        TEntity IExecuteSql.ToEntity<TEntity>(string callMethod, ISqlParam sqlParam)
        {
            SqlParam = sqlParam;
            return SpeedTest(callMethod: callMethod, dbName: sqlParam.DbName, tableName: sqlParam.TableName, cmdType: CommandType.Text, sql: sqlParam.Sql.ToString(), param: sqlParam.Param, func: () => _dbExecutor.ToEntity<TEntity>(callMethod: callMethod, sqlParam: sqlParam));
        }

        public Task<TEntity> ToEntityAsync<TEntity>(string callMethod, ISqlParam sqlParam) where TEntity : class, new()
        {
            SqlParam = sqlParam;
            return SpeedTestAsync(callMethod: callMethod, dbName: sqlParam.DbName, tableName: sqlParam.TableName, cmdType: CommandType.Text, sql: sqlParam.Sql.ToString(), param: sqlParam.Param, func: () => _dbExecutor.ToEntityAsync<TEntity>(callMethod: callMethod, sqlParam: sqlParam));
        }

        public TEntity ToEntity<TEntity>(string callMethod, ProcBuilder procBuilder, TEntity entity) where TEntity : class, new()
        {
            SqlParam = new SqlParam(procParam: procBuilder);
            return SpeedTest(callMethod: callMethod, dbName: procBuilder.DbName, tableName: procBuilder.ProcName, cmdType: CommandType.StoredProcedure, sql: null, param: procBuilder.Param, func: () => _dbExecutor.ToEntity(callMethod: callMethod, procBuilder: procBuilder, entity: entity));
        }

        public Task<TEntity> ToEntityAsync<TEntity>(string callMethod, ProcBuilder procBuilder, TEntity entity)
            where TEntity : class, new()
        {
            SqlParam = new SqlParam(procParam: procBuilder);
            return SpeedTestAsync(callMethod: callMethod, dbName: procBuilder.DbName, tableName: procBuilder.ProcName, cmdType: CommandType.StoredProcedure, sql: null, param: procBuilder.Param, func: () => _dbExecutor.ToEntityAsync(callMethod: callMethod, procBuilder: procBuilder, entity: entity));
        }

        public T GetValue<T>(string callMethod, ISqlParam sqlParam, T defValue = default)
        {
            SqlParam = sqlParam;
            return SpeedTest(callMethod: callMethod, dbName: sqlParam.DbName, tableName: sqlParam.TableName, cmdType: CommandType.Text, sql: sqlParam.Sql.ToString(), param: sqlParam.Param, func: () => _dbExecutor.GetValue(callMethod: callMethod, sqlParam: sqlParam, defValue: defValue));
        }

        public Task<T> GetValueAsync<T>(string callMethod, ISqlParam sqlParam, T defValue = default)
        {
            SqlParam = sqlParam;
            return SpeedTestAsync(callMethod: callMethod, dbName: sqlParam.DbName, tableName: sqlParam.TableName, cmdType: CommandType.Text, sql: sqlParam.Sql.ToString(), param: sqlParam.Param, func: () => _dbExecutor.GetValueAsync(callMethod: callMethod, sqlParam: sqlParam, defValue: defValue));
        }

        public T GetValue<TEntity, T>(string callMethod, ProcBuilder procBuilder, TEntity entity, T defValue = default)
            where TEntity : class, new()
        {
            SqlParam = new SqlParam(procParam: procBuilder);
            return SpeedTest(callMethod: callMethod, dbName: procBuilder.DbName, tableName: procBuilder.ProcName, cmdType: CommandType.StoredProcedure, sql: null, param: procBuilder.Param, func: () => _dbExecutor.GetValue(callMethod: callMethod, procBuilder: procBuilder, entity: entity, defValue: defValue));
        }

        public Task<T> GetValueAsync<TEntity, T>(string callMethod, ProcBuilder procBuilder, TEntity entity, T defValue = default) where TEntity : class, new()
        {
            SqlParam = new SqlParam(procParam: procBuilder);
            return SpeedTestAsync(callMethod: callMethod, dbName: procBuilder.DbName, tableName: procBuilder.ProcName, cmdType: CommandType.StoredProcedure, sql: null, param: procBuilder.Param, func: () => _dbExecutor.GetValueAsync(callMethod: callMethod, procBuilder: procBuilder, entity: entity, defValue: defValue));
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
                SqlParam     = _dbProvider.IsSupportParam ? param.ToDictionary(keySelector: o => o.ParameterName, elementSelector: o => o.Value.ToString()) : new Dictionary<string, string>()
            };

            using (FsLinkTrack.TrackDatabase(method: callMethod, dbLinkTrackDetail: dbLinkTrackDetail))
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
                SqlParam     = _dbProvider.IsSupportParam ? param.ToDictionary(keySelector: o => o.ParameterName, elementSelector: o => o.Value.ToString()) : new Dictionary<string, string>()
            };

            using (FsLinkTrack.TrackDatabase(method: callMethod, dbLinkTrackDetail: dbLinkTrackDetail))
            {
                return await func();
            }
        }
    }
}