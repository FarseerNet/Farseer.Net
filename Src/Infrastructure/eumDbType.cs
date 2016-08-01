using System.ComponentModel.DataAnnotations;

namespace FS.Infrastructure
{
    /// <summary> 数据库类型 </summary>
    public enum eumDbType
    {
        /// <summary> SqlServer数据库 </summary>
        [Display(Name = "System.Data.SqlClient")] SqlServer,
        /// <summary> Access数据库 </summary>
        [Display(Name = "System.Data.OleDb")] OleDb,
        /// <summary> MySql数据库 </summary>
        [Display(Name = "MySql.Data.MySqlClient")] MySql,
        /// <summary> SQLite </summary>
        [Display(Name = "System.Data.SQLite")] SQLite,
        /// <summary> Oracle </summary>
        [Display(Name = "System.Data.OracleClient")]Oracle,
        /// <summary> Redis </summary>
        [Display(Name = "FS.NoSql")]Redis,
        /// <summary> PostgreSql </summary>
        [Display(Name = "FS.NoSql")]PostgreSql,
    }
}