using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FS.Data.Infrastructure;
using FS.DI;

namespace FS.Data.Data
{
    /// <summary>
    ///     数据库操作
    /// </summary>
    public sealed class DbExecutor : IDisposable
    {
        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="commandTimeout">数据库执行时间，单位秒</param>
        /// <param name="tranLevel">
        ///     未提交读（read uncommitted）: 当事务A更新某条数据时，不容许其他事务来更新该数据，但可以读取。
        ///     提交读（read committed）:    当事务A更新某条数据时，不容许其他事务进行任何操作包括读取，但事务A读取时，其他事务可以进行读取、更新。
        ///     重复读（repeatable read）: 当事务A更新数据时，不容许其他事务进行任何操作，但当事务A进行读取时，其他事务只能读取，不能更新。
        ///     序列化（serializable）：     最严格的隔离级别，事务必须依次进行。
        /// </param>
        public DbExecutor(string connectionString, eumDbType dbType = eumDbType.SqlServer, int commandTimeout = 30, IsolationLevel tranLevel = IsolationLevel.Unspecified)
        {
            _connectionString = connectionString;
            _commandTimeout = commandTimeout;
            DataType = dbType;
            OpenTran(tranLevel);
        }

        /// <summary>
        ///     数据库执行时间，单位秒
        /// </summary>
        private readonly int _commandTimeout;

        /// <summary>
        ///     连接字符串
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        ///     数据类型
        /// </summary>
        public readonly eumDbType DataType;

        /// <summary>
        ///     数据提供者
        /// </summary>
        private DbProviderFactory _factory;

        /// <summary>
        ///     是否开启事务
        /// </summary>
        internal bool IsTransaction { get; private set; }
        /// <summary>
        /// 事务级别
        /// </summary>
        internal IsolationLevel TranLevel { get; private set; }
        private DbCommand _comm;
        /// <summary>
        ///     开启事务。
        /// </summary>
        /// <param name="tranLevel">事务方式</param>
        public void OpenTran(IsolationLevel tranLevel)
        {
            TranLevel = tranLevel;
            IsTransaction = tranLevel != IsolationLevel.Unspecified;
        }

        /// <summary>
        ///     关闭事务。
        /// </summary>
        public void CloseTran()
        {
            if (IsTransaction)
            {
                //_comm.Transaction.Rollback();
                _comm?.Transaction?.Dispose();
            }
            IsTransaction = false;
        }

        /// <summary>
        ///     打开数据库连接
        /// </summary>
        private void Open()
        {
            if (_comm == null || _comm.Connection == null)
            {
                _factory = AbsDbProvider.CreateInstance(DataType).DbProviderFactory;
                _comm = _factory.CreateCommand();
                // ReSharper disable once PossibleNullReferenceException
                _comm.Connection = _factory.CreateConnection();
                _comm.Connection.ConnectionString = _connectionString;
                _comm.CommandTimeout = _commandTimeout;
            }

            if (_comm.Connection.State == ConnectionState.Closed)
            {
                _comm.Parameters.Clear();
                _comm.Connection.Open();

                // 是否开启事务
                if (IsTransaction) { _comm.Transaction = _comm.Connection.BeginTransaction(TranLevel); }
            }
        }

        /// <summary>
        ///     打开数据库连接
        /// </summary>
        private async Task OpenAsync()
        {
            if (_comm == null || _comm.Connection == null)
            {
                _factory = AbsDbProvider.CreateInstance(DataType).DbProviderFactory;
                _comm = _factory.CreateCommand();
                // ReSharper disable once PossibleNullReferenceException
                _comm.Connection = _factory.CreateConnection();
                _comm.Connection.ConnectionString = _connectionString;
                _comm.CommandTimeout = _commandTimeout;
            }

            if (_comm.Connection.State == ConnectionState.Closed)
            {
                _comm.Parameters.Clear();
                await _comm.Connection.OpenAsync().ConfigureAwait(false);

                // 是否开启事务
                if (IsTransaction) { _comm.Transaction = _comm.Connection.BeginTransaction(TranLevel); }
            }

        }

        /// <summary>
        ///     关闭数据库连接
        /// </summary>
        public void Close(bool dispose)
        {
            if (_comm == null) return;
            _comm.Parameters.Clear();
            if ((dispose || _comm.Transaction == null) && _comm.Connection != null && _comm.Connection?.State != ConnectionState.Closed)
            {
                _comm.Connection?.Close();
                _comm.Connection?.Dispose();
                _comm.Dispose();
                _comm = null;
            }
        }

        /// <summary>
        ///     提交事务
        ///     如果未开启事务则会引发异常
        /// </summary>
        public void Commit()
        {
            if (_comm.Transaction == null) { throw new Exception("未开启事务"); }
            _comm.Transaction.Commit();
        }

        /// <summary>
        ///     回滚事务
        ///     如果未开启事务则会引发异常
        /// </summary>
        public void Rollback()
        {
            if (_comm?.Transaction == null) { throw new Exception("未开启事务"); }
            _comm.Transaction.Rollback();
        }

        /// <summary>
        ///     返回第一行第一列数据
        /// </summary>
        /// <param name="cmdType">执行方式</param>
        /// <param name="cmdText">SQL或者存储过程名称</param>
        /// <param name="parameters">参数</param>
        [SuppressMessage("Microsoft.Security", "CA2100:检查 SQL 查询是否存在安全漏洞")]
        public object ExecuteScalar(CommandType cmdType, string cmdText, params DbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(cmdText)) { return null; }
            try
            {
                Open();
                _comm.CommandType = cmdType;
                _comm.CommandText = cmdText;
                if (parameters != null && parameters.Length > 0) { _comm.Parameters.AddRange(parameters); }

                return _comm.ExecuteScalar();
            }
            catch (Exception exp) { Close(true); IocManager.Instance.Logger.Error(exp.Message, exp); throw exp; }
            finally { Close(false); }
        }

        /// <summary>
        ///     返回第一行第一列数据
        /// </summary>
        /// <param name="cmdType">执行方式</param>
        /// <param name="cmdText">SQL或者存储过程名称</param>
        /// <param name="parameters">参数</param>
        [SuppressMessage("Microsoft.Security", "CA2100:检查 SQL 查询是否存在安全漏洞")]
        public async Task<object> ExecuteScalarAsync(CommandType cmdType, string cmdText, params DbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(cmdText)) { return null; }
            try
            {
                await OpenAsync();
                _comm.CommandType = cmdType;
                _comm.CommandText = cmdText;
                if (parameters != null && parameters.Length > 0) { _comm.Parameters.AddRange(parameters); }

                return await _comm.ExecuteScalarAsync();
            }
            catch (Exception exp) { Close(true); IocManager.Instance.Logger.Error(exp.Message, exp); throw exp; }
            finally { Close(false); }
        }

        /// <summary>
        ///     返回受影响的行数
        /// </summary>
        /// <param name="cmdType">执行方式</param>
        /// <param name="cmdText">SQL或者存储过程名称</param>
        /// <param name="parameters">参数</param>
        [SuppressMessage("Microsoft.Security", "CA2100:检查 SQL 查询是否存在安全漏洞")]
        public int ExecuteNonQuery(CommandType cmdType, string cmdText, params DbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(cmdText)) { return 0; }
            try
            {
                Open();
                _comm.CommandType = cmdType;
                _comm.CommandText = cmdText;
                if (parameters != null && parameters.Length > 0) { _comm.Parameters.AddRange(parameters); }

                return _comm.ExecuteNonQuery();
            }
            catch (Exception exp) { Close(true); IocManager.Instance.Logger.Error(exp.Message, exp); throw exp; }
            finally { Close(false); }
        }

        /// <summary>
        ///     返回受影响的行数
        /// </summary>
        /// <param name="cmdType">执行方式</param>
        /// <param name="cmdText">SQL或者存储过程名称</param>
        /// <param name="parameters">参数</param>
        [SuppressMessage("Microsoft.Security", "CA2100:检查 SQL 查询是否存在安全漏洞")]
        public async Task<int> ExecuteNonQueryAsync(CommandType cmdType, string cmdText, params DbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(cmdText)) { return 0; }
            try
            {
                await OpenAsync();
                _comm.CommandType = cmdType;
                _comm.CommandText = cmdText;
                if (parameters != null && parameters.Length > 0) { _comm.Parameters.AddRange(parameters); }

                return await _comm.ExecuteNonQueryAsync();
            }
            catch (Exception exp) { Close(true); IocManager.Instance.Logger.Error(exp.Message, exp); throw exp; }
            finally { Close(false); }
        }

        /// <summary>
        ///     返回数据(IDataReader)
        /// </summary>
        /// <param name="cmdType">执行方式</param>
        /// <param name="cmdText">SQL或者存储过程名称</param>
        /// <param name="parameters">参数</param>
        [SuppressMessage("Microsoft.Security", "CA2100:检查 SQL 查询是否存在安全漏洞")]
        public DbDataReader GetReader(CommandType cmdType, string cmdText, params DbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(cmdText)) { return null; }
            try
            {
                Open();
                _comm.CommandType = cmdType;
                _comm.CommandText = cmdText;
                if (parameters != null && parameters.Length > 0) { _comm.Parameters.AddRange(parameters); }

                return IsTransaction ? _comm.ExecuteReader() : _comm.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception exp) { Close(true); IocManager.Instance.Logger.Error(exp.Message, exp); throw exp; }
        }

        /// <summary>
        ///     返回数据(IDataReader)
        /// </summary>
        /// <param name="cmdType">执行方式</param>
        /// <param name="cmdText">SQL或者存储过程名称</param>
        /// <param name="parameters">参数</param>
        [SuppressMessage("Microsoft.Security", "CA2100:检查 SQL 查询是否存在安全漏洞")]
        public async Task<DbDataReader> GetReaderAsync(CommandType cmdType, string cmdText, params DbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(cmdText)) { return null; }
            try
            {
                await OpenAsync();

                _comm.CommandType = cmdType;
                _comm.CommandText = cmdText;
                if (parameters != null && parameters.Length > 0) { _comm.Parameters.AddRange(parameters); }

                return await (IsTransaction ? _comm.ExecuteReaderAsync().ConfigureAwait(false) : _comm.ExecuteReaderAsync(CommandBehavior.CloseConnection).ConfigureAwait(false));
            }
            catch (Exception exp) { Close(true); IocManager.Instance.Logger.Error(exp.Message, exp); throw exp; }
        }

        /// <summary>
        ///     返回数据(DataSet)
        /// </summary>
        /// <param name="cmdType">执行方式</param>
        /// <param name="cmdText">SQL或者存储过程名称</param>
        /// <param name="parameters">参数</param>
        [SuppressMessage("Microsoft.Security", "CA2100:检查 SQL 查询是否存在安全漏洞")]
        public DataSet GetDataSet(CommandType cmdType, string cmdText, params DbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(cmdText)) { return new DataSet(); }
            try
            {
                Open();
                _comm.CommandType = cmdType;
                _comm.CommandText = cmdText;
                if (parameters != null && parameters.Length > 0) { _comm.Parameters.AddRange(parameters); }
                var ada = _factory.CreateDataAdapter();
                ada.SelectCommand = _comm;
                var ds = new DataSet();
                ada.Fill(ds);
                return ds;
            }
            catch (Exception exp) { Close(true); IocManager.Instance.Logger.Error(exp.Message, exp); throw exp; }
            finally { Close(false); }
        }

        /// <summary>
        ///     返回数据(DataSet)
        /// </summary>
        /// <param name="cmdType">执行方式</param>
        /// <param name="cmdText">SQL或者存储过程名称</param>
        /// <param name="parameters">参数</param>
        [SuppressMessage("Microsoft.Security", "CA2100:检查 SQL 查询是否存在安全漏洞")]
        public async Task<DataSet> GetDataSetAsync(CommandType cmdType, string cmdText, params DbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(cmdText)) { return new DataSet(); }
            try
            {
                await OpenAsync();
                _comm.CommandType = cmdType;
                _comm.CommandText = cmdText;
                if (parameters != null && parameters.Length > 0) { _comm.Parameters.AddRange(parameters); }
                var dataAdapter = _factory.CreateDataAdapter();
                // ReSharper disable once PossibleNullReferenceException
                dataAdapter.SelectCommand = _comm;
                var ds = new DataSet();
                dataAdapter.Fill(ds);
                return ds;
            }
            catch (Exception exp) { Close(true); IocManager.Instance.Logger.Error(exp.Message, exp); throw exp; }
            finally { Close(false); }
        }

        /// <summary>
        ///     返回数据(DataTable)
        /// </summary>
        /// <param name="cmdType">执行方式</param>
        /// <param name="cmdText">SQL或者存储过程名称</param>
        /// <param name="parameters">参数</param>
        public DataTable GetDataTable(CommandType cmdType, string cmdText, params DbParameter[] parameters)
        {
            var ds = GetDataSet(cmdType, cmdText, parameters);
            return ds == null || ds.Tables.Count == 0 ? new DataTable() : ds.Tables[0];
        }

        /// <summary>
        ///     返回数据(DataTable)
        /// </summary>
        /// <param name="cmdType">执行方式</param>
        /// <param name="cmdText">SQL或者存储过程名称</param>
        /// <param name="parameters">参数</param>
        public async Task<DataTable> GetDataTableAsync(CommandType cmdType, string cmdText, params DbParameter[] parameters)
        {
            var ds = await GetDataSetAsync(cmdType, cmdText, parameters);
            return ds == null || ds.Tables.Count == 0 ? new DataTable() : ds.Tables[0];
        }

        /// <summary>
        ///     指量操作数据（仅支付Sql Server)
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dt">数据</param>
        public void ExecuteSqlBulkCopy(string tableName, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) { return; }

            try
            {
                Open();
                using (var bulkCopy = new SqlBulkCopy((SqlConnection)_comm.Connection, SqlBulkCopyOptions.Default, (SqlTransaction)_comm.Transaction))
                {
                    bulkCopy.DestinationTableName = tableName;
                    bulkCopy.BatchSize = dt.Rows.Count;
                    bulkCopy.BulkCopyTimeout = 3600;
                    bulkCopy.WriteToServer(dt);
                }
            }
            catch (Exception exp) { Close(true); IocManager.Instance.Logger.Error(exp.Message, exp); throw exp; }
            finally { Close(false); }
        }

        /// <summary>
        ///     指量操作数据（仅支付Sql Server)
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dt">数据</param>
        public async Task ExecuteSqlBulkCopyAsync(string tableName, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) { return; }

            try
            {
                await OpenAsync();
                using (var bulkCopy = new SqlBulkCopy((SqlConnection)_comm.Connection, SqlBulkCopyOptions.Default, (SqlTransaction)_comm.Transaction))
                {
                    bulkCopy.DestinationTableName = tableName;
                    bulkCopy.BatchSize = dt.Rows.Count;
                    bulkCopy.BulkCopyTimeout = 3600;
                    await bulkCopy.WriteToServerAsync(dt);
                }
            }
            catch (Exception exp) { Close(true); IocManager.Instance.Logger.Error(exp.Message, exp); throw exp; }
            finally { Close(false); }
        }

        private void Dispose(bool disposing)
        {
            //释放托管资源
            if (disposing) { Close(true); }
        }

        /// <summary>
        ///     注销
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}