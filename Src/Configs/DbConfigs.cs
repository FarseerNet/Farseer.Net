using System;
using System.Collections.Generic;
using FS.Infrastructure;

namespace FS.Configs
{
    /// <summary> 数据库连接配置  </summary>
    public abstract class DbConfigs : AbsConfigs<DbConfig>
    {
    }

    /// <summary> 数据库连接配置 </summary>
    [Serializable]
    public class DbConfig
    {
        /// <summary> 数据库连接配置列表 </summary>
        public readonly List<DbInfo> DbList = new List<DbInfo>();
    }

    /// <summary> 数据库连接配置 </summary>
    public class DbInfo
    {
        /// <summary> 服务器地址 </summary>
        public string Server = ".";

        /// <summary> 数据库帐号 </summary>
        public string UserID = "";

        /// <summary> 数据库密码 </summary>
        public string PassWord = "";

        /// <summary> 端口号 </summary>
        public string Port = "";

        /// <summary> 数据库类型 </summary>
        public eumDbType DataType = eumDbType.SqlServer;

        /// <summary> 数据库版本 </summary>
        public string DataVer = "";

        /// <summary> 数据库目录 </summary>
        public string Catalog = "";

        /// <summary> 最小连接池 </summary>
        public int PoolMinSize = 0;

        /// <summary> 最大连接池 </summary>
        public int PoolMaxSize = 0;

        /// <summary> 数据库连接时间限制，单位秒 </summary>
        public int ConnectTimeout = 0;

        /// <summary> 数据库执行时间限制，单位秒 </summary>
        public int CommandTimeout = 0;

        /// <summary> 自定义连接字符串 </summary>
        public string Additional = "";

        /// <summary> 通过索引返回实体 </summary>
        public static implicit operator DbInfo(int index)
        {
            return DbConfigs.ConfigEntity.DbList.Count <= index ? null : DbConfigs.ConfigEntity.DbList[index];
        }
    }
}