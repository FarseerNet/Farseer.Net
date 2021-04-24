using FS.Data.Infrastructure;

namespace FS.Data.Configuration
{
    /// <summary> 数据库连接配置 </summary>
    public class DbItemConfig
    {
        /// <summary> 配置名称 </summary>
        public string Name { get; set; }

        /// <summary> 服务器地址 </summary>
        public string Server { get; set; }

        /// <summary> 数据库帐号 </summary>
        public string UserID { get; set; }

        /// <summary> 数据库密码 </summary>
        public string PassWord { get; set; }

        /// <summary> 端口号 </summary>
        public string Port { get; set; }

        /// <summary> 数据库类型 </summary>
        public eumDbType DataType { get; set; }

        /// <summary> 数据库版本 </summary>
        public string DataVer { get; set; }

        /// <summary> 数据库目录 </summary>
        public string Catalog { get; set; }

        /// <summary> 最小连接池 </summary>
        public int PoolMinSize { get; set; }

        /// <summary> 最大连接池 </summary>
        public int PoolMaxSize { get; set; }

        /// <summary> 数据库连接时间限制，单位秒 </summary>
        public int ConnectTimeout { get; set; }

        /// <summary> 数据库执行时间限制，单位秒 </summary>
        public int CommandTimeout { get; set; }

        /// <summary> 自定义连接字符串 </summary>
        public string Additional { get; set; }
    }
}