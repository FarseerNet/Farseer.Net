using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using FS.Data.Data;
using FS.Data.Internal;

namespace FS.Data.Infrastructure
{
    /// <summary> 将SQL发送到数据库 </summary>
    internal interface IExecuteSql
    {
        /// <summary>
        ///     数据库操作
        /// </summary>
        DbExecutor DataBase { get; }

        /// <summary>
        ///     返回影响行数
        /// </summary>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="sqlParam"> SQL语句与参数 </param>
        int Execute(string callMethod, ISqlParam sqlParam);

        /// <summary>
        ///     返回影响行数
        /// </summary>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="sqlParam"> SQL语句与参数 </param>
        Task<int> ExecuteAsync(string callMethod, ISqlParam sqlParam);

        /// <summary>
        ///     返回影响行数
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="procBuilder"> SQL语句与参数 </param>
        /// <param name="entity"> 实体类 </param>
        int Execute<TEntity>(string callMethod, ProcBuilder procBuilder, TEntity entity) where TEntity : class, new();

        /// <summary>
        ///     返回影响行数
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="procBuilder"> SQL语句与参数 </param>
        /// <param name="entity"> 实体类 </param>
        Task<int> ExecuteAsync<TEntity>(string callMethod, ProcBuilder procBuilder, TEntity entity) where TEntity : class, new();

        /// <summary>
        ///     返回DataTable
        /// </summary>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="sqlParam"> SQL语句与参数 </param>
        DataTable ToTable(string callMethod, ISqlParam sqlParam);

        /// <summary>
        ///     返回DataTable
        /// </summary>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="sqlParam"> SQL语句与参数 </param>
        Task<DataTable> ToTableAsync(string callMethod, ISqlParam sqlParam);

        /// <summary>
        ///     返回DataTable
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="procBuilder"> SQL语句与参数 </param>
        /// <param name="entity"> 实体类 </param>
        DataTable ToTable<TEntity>(string callMethod, ProcBuilder procBuilder, TEntity entity) where TEntity : class, new();

        /// <summary>
        ///     返回影响行数
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="procBuilder"> SQL语句与参数 </param>
        /// <param name="entity"> 实体类 </param>
        Task<DataTable> ToTableAsync<TEntity>(string callMethod, ProcBuilder procBuilder, TEntity entity) where TEntity : class, new();

        /// <summary>
        ///     返回泛型集合
        /// </summary>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="sqlParam"> SQL语句与参数 </param>
        List<TEntity> ToList<TEntity>(string callMethod, ISqlParam sqlParam) where TEntity : class, new();

        /// <summary>
        ///     返回返回泛型集合
        /// </summary>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="sqlParam"> SQL语句与参数 </param>
        Task<List<TEntity>> ToListAsync<TEntity>(string callMethod, ISqlParam sqlParam) where TEntity : class, new();

        /// <summary>
        ///     返回返回泛型集合
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="procBuilder"> SQL语句与参数 </param>
        /// <param name="entity"> 实体类 </param>
        List<TEntity> ToList<TEntity>(string callMethod, ProcBuilder procBuilder, TEntity entity) where TEntity : class, new();

        /// <summary>
        ///     返回返回泛型集合
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="procBuilder"> SQL语句与参数 </param>
        /// <param name="entity"> 实体类 </param>
        Task<List<TEntity>> ToListAsync<TEntity>(string callMethod, ProcBuilder procBuilder, TEntity entity) where TEntity : class, new();

        /// <summary>
        ///     返回单条数据
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="sqlParam"> SQL语句与参数 </param>
        TEntity ToEntity<TEntity>(string callMethod, ISqlParam sqlParam) where TEntity : class, new();

        /// <summary>
        ///     返回单条数据
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="sqlParam"> SQL语句与参数 </param>
        Task<TEntity> ToEntityAsync<TEntity>(string callMethod, ISqlParam sqlParam) where TEntity : class, new();

        /// <summary>
        ///     返回单条数据
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="procBuilder"> SQL语句与参数 </param>
        /// <param name="entity"> 实体类 </param>
        TEntity ToEntity<TEntity>(string callMethod, ProcBuilder procBuilder, TEntity entity) where TEntity : class, new();

        /// <summary>
        ///     返回影响行数
        /// </summary>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="procBuilder"> SQL语句与参数 </param>
        /// <param name="entity"> 实体类 </param>
        Task<TEntity> ToEntityAsync<TEntity>(string callMethod, ProcBuilder procBuilder, TEntity entity) where TEntity : class, new();

        /// <summary>
        ///     查询单个字段值
        /// </summary>
        /// <typeparam name="T"> 返回值类型 </typeparam>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="sqlParam"> SQL语句与参数 </param>
        /// <param name="defValue"> 默认值 </param>
        T GetValue<T>(string callMethod, ISqlParam sqlParam, T defValue = default);

        /// <summary>
        ///     查询单个字段值
        /// </summary>
        /// <typeparam name="T"> 返回值类型 </typeparam>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="sqlParam"> SQL语句与参数 </param>
        /// <param name="defValue"> 默认值 </param>
        Task<T> GetValueAsync<T>(string callMethod, ISqlParam sqlParam, T defValue = default);

        /// <summary>
        ///     查询单个字段值
        /// </summary>
        /// <typeparam name="T"> 返回值类型 </typeparam>
        /// <typeparam name="TEntity"> 实体类 </typeparam>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="procBuilder"> SQL语句与参数 </param>
        /// <param name="entity"> 实体类 </param>
        /// <param name="defValue"> 默认值 </param>
        T GetValue<TEntity, T>(string callMethod, ProcBuilder procBuilder, TEntity entity, T defValue = default) where TEntity : class, new();

        /// <summary>
        ///     查询单个字段值
        /// </summary>
        /// <typeparam name="T"> 返回值类型 </typeparam>
        /// <typeparam name="TEntity"> 实体类型 </typeparam>
        /// <param name="callMethod"> 上游调用的方法名称 </param>
        /// <param name="procBuilder"> 存储过程生成器 </param>
        /// <param name="entity"> 实体 </param>
        /// <param name="defValue"> 默认值 </param>
        Task<T> GetValueAsync<TEntity, T>(string callMethod, ProcBuilder procBuilder, TEntity entity, T defValue = default) where TEntity : class, new();
    }
}