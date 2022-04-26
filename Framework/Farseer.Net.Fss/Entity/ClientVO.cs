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
        public string ClientIp { get; set; }

        /// <summary>
        ///     客户端名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        ///     客户端能执行的任务
        /// </summary>
        public string[] Jobs { get; set; }
    }
}