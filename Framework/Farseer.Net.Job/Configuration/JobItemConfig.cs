namespace FS.Job.Configuration
{
    public class JobItemConfig
    {
        /// <summary>
        /// 启动Grpc服务，用于调度平台通讯
        /// </summary>
        public int GrpcServicePort { get; set; } = 9573;
        
        /// <summary>
        /// 调度平台的服务地址
        /// </summary>
        public string Server { get; set; }
        
        /// <summary>
        /// 连接到FSS平台的心跳时间
        /// </summary>
        public int ConnectFssServerTime { get; set; } = 5000;
    }
}