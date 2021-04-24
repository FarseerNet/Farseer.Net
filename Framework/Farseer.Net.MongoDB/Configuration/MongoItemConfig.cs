namespace FS.MongoDB.Configuration
{
    /// <summary>
    /// Mongo配置项类
    /// </summary>
    public class MongoItemConfig
    {
        /// <summary> 集群名称 </summary>
        public string Name { get; set; }

        /// <summary> 集群地址,多个地址用逗号隔开 </summary>
        public string Server { get; set; }
    }

}
