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
    }
}