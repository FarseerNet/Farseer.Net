using System.Data;
using System.Threading.Tasks;
using Collections.Pooled;
using FS.Data.Abstract;
using FS.Data.Attribute;
using FS.Extends;
using FS.Utils.Common;

namespace FS.Data.Internal
{
    /// <summary> 将SQL发送到数据库 </summary>
    internal sealed class ExecuteSql : IExecuteSql
    {
        /// <summary>
        ///     数据库上下文
        /// </summary>
        private readonly InternalContext _contextProvider;

        /// <summary>
        ///     将SQL发送到数据库
        /// </summary>
        /// <param name="contextProvider"> 数据库上下文 </param>
        internal ExecuteSql(InternalContext contextProvider)
        {
            _contextProvider = contextProvider;
        }

        /// <summary>
        ///     本次执行的SQL
        /// </summary>
        public ISqlParam SqlParam { get; private set; }

        /// <summary>
        ///     返回影响行数
        /// </summary>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="sqlParam"> SQL语句与参数 </param>
        [LinkTrackSqlParam]
        public int Execute(string callMethod, ISqlParam sqlParam)
        {
            SqlParam = sqlParam;
            var param = sqlParam.Param?.ToArray();
            return sqlParam.Sql.Length < 1 ? 0 : _contextProvider.DbExecutor.ExecuteNonQuery(cmdType: CommandType.Text, cmdText: sqlParam.Sql.ToString(), parameters: param);
        }

        /// <summary>
        ///     返回影响行数
        /// </summary>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="sqlParam"> SQL语句与参数 </param>
        [LinkTrackSqlParam]
        public async Task<int> ExecuteAsync(string callMethod, ISqlParam sqlParam)
        {
            SqlParam = sqlParam;
            var param = sqlParam.Param?.ToArray();
            return sqlParam.Sql.Length < 1 ? 0 : await _contextProvider.DbExecutor.ExecuteNonQueryAsync(cmdType: CommandType.Text, cmdText: sqlParam.Sql.ToString(), parameters: param);
        }

        /// <summary>
        ///     返回影响行数
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="procBuilder"> SQL语句与参数 </param>
        /// <param name="entity"> 实体类 </param>
        [LinkTrackProcParam]
        public int Execute<TEntity>(string callMethod, IProcParam procBuilder, TEntity entity) where TEntity : class, new()
        {
            // 生成SQL 输入、输出参数化
            var sqlParam = procBuilder.InitParam(entity: entity);
            SqlParam = new SqlParam(procParam: sqlParam);

            var param = sqlParam.Param?.ToArray();
            var value = _contextProvider.DbExecutor.ExecuteNonQuery(cmdType: CommandType.StoredProcedure, cmdText: sqlParam.ProcName, parameters: param);
            procBuilder.SetParamToEntity(entity: entity);
            return value;
        }

        /// <summary>
        ///     返回影响行数
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="procBuilder"> SQL语句与参数 </param>
        /// <param name="entity"> 实体类 </param>
        [LinkTrackProcParam]
        public async Task<int> ExecuteAsync<TEntity>(string callMethod, IProcParam procBuilder, TEntity entity) where TEntity : class, new()
        {
            // 生成SQL 输入、输出参数化
            var sqlParam = procBuilder.InitParam(entity: entity);
            SqlParam = new SqlParam(procParam: sqlParam);

            var param = sqlParam.Param?.ToArray();
            var value = await _contextProvider.DbExecutor.ExecuteNonQueryAsync(cmdType: CommandType.StoredProcedure, cmdText: sqlParam.ProcName, parameters: param);
            procBuilder.SetParamToEntity(entity: entity);
            return value;
        }

        /// <summary>
        ///     返回DataTable
        /// </summary>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="sqlParam"> SQL语句与参数 </param>
        [LinkTrackSqlParam]
        public DataTable ToTable(string callMethod, ISqlParam sqlParam)
        {
            SqlParam = sqlParam;
            var param = sqlParam.Param?.ToArray();
            return _contextProvider.DbExecutor.GetDataTable(cmdType: CommandType.Text, cmdText: sqlParam.Sql.ToString(), parameters: param);
        }

        /// <summary>
        ///     返回DataTable
        /// </summary>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="sqlParam"> SQL语句与参数 </param>
        [LinkTrackSqlParam]
        public async Task<DataTable> ToTableAsync(string callMethod, ISqlParam sqlParam)
        {
            SqlParam = sqlParam;
            var param = sqlParam.Param?.ToArray();
            return await _contextProvider.DbExecutor.GetDataTableAsync(cmdType: CommandType.Text, cmdText: sqlParam.Sql.ToString(), parameters: param);
        }

        /// <summary>
        ///     返回影响行数
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="procBuilder"> SQL语句与参数 </param>
        /// <param name="entity"> 实体类 </param>
        [LinkTrackProcParam]
        public DataTable ToTable<TEntity>(string callMethod, IProcParam procBuilder, TEntity entity) where TEntity : class, new()
        {
            // 生成SQL 输入、输出参数化
            var sqlParam = procBuilder.InitParam(entity: entity);
            SqlParam = new SqlParam(procParam: sqlParam);

            var param = sqlParam.Param?.ToArray();
            var value = _contextProvider.DbExecutor.GetDataTable(cmdType: CommandType.StoredProcedure, cmdText: sqlParam.ProcName, parameters: param);
            procBuilder.SetParamToEntity(entity: entity);
            return value;
        }

        /// <summary>
        ///     返回影响行数
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="procBuilder"> SQL语句与参数 </param>
        /// <param name="entity"> 实体类 </param>
        [LinkTrackProcParam]
        public async Task<DataTable> ToTableAsync<TEntity>(string callMethod, IProcParam procBuilder, TEntity entity) where TEntity : class, new()
        {
            // 生成SQL 输入、输出参数化
            var sqlParam = procBuilder.InitParam(entity: entity);
            SqlParam = new SqlParam(procParam: sqlParam);

            var param = sqlParam.Param?.ToArray();
            var value = await _contextProvider.DbExecutor.GetDataTableAsync(cmdType: CommandType.StoredProcedure, cmdText: sqlParam.ProcName, parameters: param);
            procBuilder.SetParamToEntity(entity: entity);
            return value;
        }

        /// <summary>
        ///     返回返回泛型集合
        /// </summary>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="sqlParam"> SQL语句与参数 </param>
        [LinkTrackSqlParam]
        public PooledList<TEntity> ToList<TEntity>(string callMethod, ISqlParam sqlParam) where TEntity : class, new()
        {
            SqlParam = sqlParam;
            var param = sqlParam.Param?.ToArray();
            return _contextProvider.DbExecutor.GetReader(cmdType: CommandType.Text, cmdText: sqlParam.Sql.ToString(), parameters: param).ToList<TEntity>();
        }

        /// <summary>
        ///     返回返回泛型集合
        /// </summary>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="sqlParam"> SQL语句与参数 </param>
        [LinkTrackSqlParam]
        public Task<PooledList<TEntity>> ToListAsync<TEntity>(string callMethod, ISqlParam sqlParam) where TEntity : class, new()
        {
            SqlParam = sqlParam;
            var param = sqlParam.Param?.ToArray();
            return _contextProvider.DbExecutor.GetReaderAsync(cmdType: CommandType.Text, cmdText: sqlParam.Sql.ToString(), parameters: param).ToListAsync<TEntity>();
        }

        [LinkTrackProcParam]
        public PooledList<TEntity> ToList<TEntity>(string callMethod, IProcParam procBuilder, TEntity entity) where TEntity : class, new()
        {
            // 生成SQL 输入、输出参数化
            var sqlParam = procBuilder.InitParam(entity: entity);
            SqlParam = new SqlParam(procParam: sqlParam);

            var param = sqlParam.Param?.ToArray();
            var value = _contextProvider.DbExecutor.GetReader(cmdType: CommandType.StoredProcedure, cmdText: sqlParam.ProcName, parameters: param).ToList<TEntity>();
            procBuilder.SetParamToEntity(entity: entity);
            return value;
        }

        [LinkTrackProcParam]
        public async Task<PooledList<TEntity>> ToListAsync<TEntity>(string callMethod, IProcParam procBuilder, TEntity entity) where TEntity : class, new()
        {
            // 生成SQL 输入、输出参数化
            var sqlParam = procBuilder.InitParam(entity: entity);
            SqlParam = new SqlParam(procParam: sqlParam);

            var param = sqlParam.Param?.ToArray();
            var value = (await _contextProvider.DbExecutor.GetReaderAsync(cmdType: CommandType.StoredProcedure, cmdText: sqlParam.ProcName, parameters: param)).ToList<TEntity>();
            procBuilder.SetParamToEntity(entity: entity);
            return value;
        }

        /// <summary>
        ///     返回单条数据
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="sqlParam"> SQL语句与参数 </param>
        [LinkTrackSqlParam]
        public TEntity ToEntity<TEntity>(string callMethod, ISqlParam sqlParam) where TEntity : class, new()
        {
            SqlParam = sqlParam;
            var     param = sqlParam.Param?.ToArray();
            TEntity t;
            using (var reader = _contextProvider.DbExecutor.GetReader(cmdType: CommandType.Text, cmdText: sqlParam.Sql.ToString(), parameters: param))
            {
                t = reader.ToEntity<TEntity>();
            }

            _contextProvider.DbExecutor.Close(dispose: false);
            return t;
        }

        /// <summary>
        ///     返回单条数据
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="sqlParam"> SQL语句与参数 </param>
        [LinkTrackSqlParam]
        public async Task<TEntity> ToEntityAsync<TEntity>(string callMethod, ISqlParam sqlParam) where TEntity : class, new()
        {
            SqlParam = sqlParam;
            var     param = sqlParam.Param?.ToArray();
            TEntity t;
            await using (var reader = await _contextProvider.DbExecutor.GetReaderAsync(cmdType: CommandType.Text, cmdText: sqlParam.Sql.ToString(), parameters: param))
            {
                t = reader.ToEntity<TEntity>();
            }

            _contextProvider.DbExecutor.Close(dispose: false);
            return t;
        }

        /// <summary>
        ///     返回影响行数
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="procBuilder"> SQL语句与参数 </param>
        /// <param name="entity"> 实体类 </param>
        [LinkTrackProcParam]
        public TEntity ToEntity<TEntity>(string callMethod, IProcParam procBuilder, TEntity entity) where TEntity : class, new()
        {
            // 生成SQL 输入、输出参数化
            var sqlParam = procBuilder.InitParam(entity: entity);
            SqlParam = new SqlParam(procParam: sqlParam);

            var     param = sqlParam.Param?.ToArray();
            TEntity t;
            using (var reader = _contextProvider.DbExecutor.GetReader(cmdType: CommandType.StoredProcedure, cmdText: sqlParam.ProcName, parameters: param))
            {
                t = reader.ToEntity<TEntity>();
            }

            _contextProvider.DbExecutor.Close(dispose: false);
            procBuilder.SetParamToEntity(entity: entity);
            return t;
        }

        /// <summary>
        ///     返回影响行数
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="procBuilder"> SQL语句与参数 </param>
        /// <param name="entity"> 实体类 </param>
        [LinkTrackProcParam]
        public async Task<TEntity> ToEntityAsync<TEntity>(string callMethod, IProcParam procBuilder, TEntity entity) where TEntity : class, new()
        {
            // 生成SQL 输入、输出参数化
            var sqlParam = procBuilder.InitParam(entity: entity);
            SqlParam = new SqlParam(procParam: sqlParam);

            var     param = sqlParam.Param?.ToArray();
            TEntity t;
            using (var reader = await _contextProvider.DbExecutor.GetReaderAsync(cmdType: CommandType.StoredProcedure, cmdText: sqlParam.ProcName, parameters: param))
            {
                t = reader.ToEntity<TEntity>();
            }

            _contextProvider.DbExecutor.Close(dispose: false);
            procBuilder.SetParamToEntity(entity: entity);
            return t;
        }

        /// <summary>
        ///     查询单个字段值
        /// </summary>
        /// <typeparam name="T"> 返回值类型 </typeparam>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="sqlParam"> SQL语句与参数 </param>
        /// <param name="defValue"> 默认值 </param>
        [LinkTrackSqlParam]
        public T GetValue<T>(string callMethod, ISqlParam sqlParam, T defValue = default)
        {
            SqlParam = sqlParam;
            var param = sqlParam.Param?.ToArray();
            var value = _contextProvider.DbExecutor.ExecuteScalar(cmdType: CommandType.Text, cmdText: sqlParam.Sql.ToString(), parameters: param);
            return ConvertHelper.ConvertType(sourceValue: value, defValue: defValue);
        }

        /// <summary>
        ///     查询单个字段值
        /// </summary>
        /// <typeparam name="T"> 返回值类型 </typeparam>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="sqlParam"> SQL语句与参数 </param>
        /// <param name="defValue"> 默认值 </param>
        [LinkTrackSqlParam]
        public async Task<T> GetValueAsync<T>(string callMethod, ISqlParam sqlParam, T defValue = default)
        {
            SqlParam = sqlParam;
            var param = sqlParam.Param?.ToArray();
            var value = await _contextProvider.DbExecutor.ExecuteScalarAsync(cmdType: CommandType.Text, cmdText: sqlParam.Sql.ToString(), parameters: param);
            return ConvertHelper.ConvertType(sourceValue: value, defValue: defValue);
        }

        /// <summary>
        ///     查询单个字段值
        /// </summary>
        /// <typeparam name="T"> 返回值类型 </typeparam>
        /// <typeparam name="TEntity"> 实体类型 </typeparam>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="procBuilder"> 存储过程生成器 </param>
        /// <param name="entity"> 实体 </param>
        /// <param name="defValue"> 默认值 </param>
        [LinkTrackProcParam]
        public T GetValue<TEntity, T>(string callMethod, IProcParam procBuilder, TEntity entity, T defValue = default) where TEntity : class, new()
        {
            // 生成SQL 输入、输出参数化
            var sqlParam = procBuilder.InitParam(entity: entity);
            SqlParam = new SqlParam(procParam: sqlParam);

            var param = sqlParam.Param?.ToArray();
            var value = _contextProvider.DbExecutor.ExecuteScalar(cmdType: CommandType.StoredProcedure, cmdText: sqlParam.ProcName, parameters: param);
            procBuilder.SetParamToEntity(entity: entity);
            return ConvertHelper.ConvertType(sourceValue: value, defValue: defValue);
        }

        /// <summary>
        ///     查询单个字段值
        /// </summary>
        /// <typeparam name="T"> 返回值类型 </typeparam>
        /// <typeparam name="TEntity"> 实体类型 </typeparam>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="procBuilder"> 存储过程生成器 </param>
        /// <param name="entity"> 实体 </param>
        /// <param name="defValue"> 默认值 </param>
        [LinkTrackProcParam]
        public async Task<T> GetValueAsync<TEntity, T>(string callMethod, IProcParam procBuilder, TEntity entity, T defValue = default) where TEntity : class, new()
        {
            // 生成SQL 输入、输出参数化
            var sqlParam = procBuilder.InitParam(entity: entity);
            SqlParam = new SqlParam(procParam: sqlParam);

            var param = sqlParam.Param?.ToArray();
            var value = await _contextProvider.DbExecutor.ExecuteScalarAsync(cmdType: CommandType.StoredProcedure, cmdText: sqlParam.ProcName, parameters: param);
            procBuilder.SetParamToEntity(entity: entity);
            return ConvertHelper.ConvertType(sourceValue: value, defValue: defValue);
        }

        public void Dispose()
        {
            SqlParam?.Dispose();
        }
    }
}