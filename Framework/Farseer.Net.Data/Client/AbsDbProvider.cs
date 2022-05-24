using System.Data.Common;
using FS.Data.Client.ClickHouse;
using FS.Data.Client.MySql;
using FS.Data.Client.PostgreSql;
using FS.Data.Client.SqLite;
using FS.Data.Client.SqlServer;
using FS.Data.Infrastructure;
using FS.Data.Internal;
using FS.Data.Map;
#if !CORE
using FS.Data.Client.OleDb;
using FS.Data.Client.Oracle;
#endif

namespace FS.Data.Client
{
    /// <summary>
    ///     数据库提供者（不同数据库的特性）
    /// </summary>
    public abstract class AbsDbProvider
    {
        /// <summary>
        ///     是否支持事务操作
        /// </summary>
        public abstract bool IsSupportTransaction { get; }

        /// <summary>
        ///     创建提供程序对数据源类的实现的实例
        /// </summary>
        public abstract DbProviderFactory DbProviderFactory { get; }

        /// <summary>
        ///     创建提供程序对数据源类的实现的实例
        /// </summary>
        public abstract AbsFunctionProvider FunctionProvider { get; }

        /// <summary>
        ///     参数化支持
        /// </summary>
        public abstract AbsDbParam DbParam { get; }

        /// <summary>
        ///     数据库连接字符串
        /// </summary>
        public abstract AbsConnectionString ConnectionString { get; }

        /// <summary>
        ///     创建字段保护符
        /// </summary>
        /// <param name="fieldName"> 字符名称 </param>
        public virtual string KeywordAegis(string fieldName) => $"[{fieldName}]"; //if (Regex.IsMatch(fieldName, "[\\(\\)\\,\\[\\]\\+\\= ]+")) { return fieldName; }

        /// <summary>
        ///     返回数据库类型名称
        /// </summary>
        /// <param name="dbType"> 数据库类型 </param>
        /// <param name="dataVer"> 数据库版本 </param>
        public static AbsDbProvider CreateInstance(eumDbType dbType, string dataVer = null)
        {
            switch (dbType)
            {
#if !CORE
                case eumDbType.OleDb: return new OleDbProvider();
                case eumDbType.Oracle: return new OracleProvider();
#endif
                case eumDbType.MySql:      return new MySqlProvider();
                case eumDbType.ClickHouse: return new ClickHouseProvider();
                case eumDbType.SQLite:     return new SqLiteProvider();
                case eumDbType.PostgreSql: return new PostgreSqlProvider();
            }

            return new SqlServerProvider();
        }

        /// <summary>
        ///     创建SQL查询
        /// </summary>
        /// <param name="expBuilder"> 表达式持久化 </param>
        /// <param name="setMap">实体类结构映射 </param>
        internal abstract AbsSqlBuilder CreateSqlBuilder(ExpressionBuilder expBuilder, SetDataMap setMap);
    }
}