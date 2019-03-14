// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2016-12-26 16:16
// ********************************************
namespace FS.Cache.Redis.Configuration
{
    /// <summary>
    /// Redis配置
    /// </summary>
    public class RedisItemConfig
    {
        /// <summary> 集群名称 </summary>
        public string Name = ".";

        /// <summary> 集群IP:Port地址 </summary>
        public string Server = "";

        /// <summary> 命令类型（默认集群模式） </summary>
        public EumCommandType CommandType = 0;

        /// <summary> 密码 </summary>
        public string Password = "";

        /// <summary> 哨兵模式下需要填写 </summary>
        public string ServiceName = "";

        public string TieBreaker = "";
    }
}