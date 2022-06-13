namespace FS.Fss.Entity
{
    public class ClientVO
    {
        /// <summary>
        ///     客户端ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///     客户端IP
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        ///     客户端名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     客户端能执行的任务
        /// </summary>
        public string[] Jobs { get; set; }
    }
}