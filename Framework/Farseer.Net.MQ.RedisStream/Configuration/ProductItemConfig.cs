namespace FS.MQ.RedisStream.Configuration
{
    public class ProductItemConfig
    {
        /// <summary> 队列名称 </summary>
        public string QueueName { get; set; }

        /// <summary> 队列最大长度 </summary>
        public int MaxLength { get; set; }
    }
}