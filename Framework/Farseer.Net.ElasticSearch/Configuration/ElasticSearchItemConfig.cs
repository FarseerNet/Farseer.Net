namespace FS.ElasticSearch.Configuration
{
    /// <summary>
    ///     ES配置项类
    /// </summary>
    public class ElasticSearchItemConfig
    {
        /// <summary> 集群名称 </summary>
        public string Name { get; set; }

        /// <summary> 集群地址,多个地址用逗号隔开 </summary>
        public string Server { get; set; }

        /// <summary> 集群用户名 </summary>
        public string Username { get; set; }

        /// <summary> 集群密码 </summary>
        public string Password { get; set; }

        /// <summary> 副本数量（默认1） </summary>
        public int ReplicasCount { get; set; }

        /// <summary> 分片数量（默认3） </summary>
        public int ShardsCount { get; set; }

        /// <summary> 刷新间隔（默认1秒） </summary>
        public int RefreshInterval { get; set; }

        /// <summary> 索引日期格式 </summary>
        public string IndexFormat { get; set; }
    }
}