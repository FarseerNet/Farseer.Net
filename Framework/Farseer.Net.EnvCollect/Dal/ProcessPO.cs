using Nest;

namespace FS.EC.Dal
{
    /// <summary>
    /// 主机环境相关的数据
    /// </summary>
    public class ProcessPO
    {
        /// <summary>
        /// 主机名称
        /// </summary>
        [Keyword]
        public string Hostname { get; set; }

        /// <summary>
        /// 主机IP
        /// </summary>
        [Keyword]
        public string HostIp { get; set; }
        
        /// <summary>
        /// 应用名称
        /// </summary>
        [Keyword]
        public string AppName { get; set; }

        /// <summary>
        /// CPU使用率
        /// </summary>
        [Number(type: NumberType.ScaledFloat, ScalingFactor = 100)]
        public decimal CpuRate { get; set; }

        /// <summary>
        /// CPU频率
        /// </summary>
        [Number(type: NumberType.ScaledFloat, ScalingFactor = 100)]
        public decimal CpuMax { get; set; }

        /// <summary>
        /// 内存使用率 MemoryRate / MemoryMax（单位MB)
        /// </summary>
        [Number(type: NumberType.ScaledFloat, ScalingFactor = 100)]
        public decimal MemoryRate { get; set; }

        /// <summary>
        /// 内存使用实际值（单位MB)
        /// </summary>
        [Number(type: NumberType.ScaledFloat, ScalingFactor = 100)]
        public decimal MemoryUse { get; set; }

        /// <summary>
        /// 内存总容量（单位MB)
        /// </summary>
        [Number(type: NumberType.ScaledFloat, ScalingFactor = 100)]
        public decimal MemoryMax { get; set; }

        /// <summary>
        /// 线程数量
        /// </summary>
        [Number(NumberType.Integer)]
        public int ThreadCount { get; set; }
        
        /// <summary>
        /// 采集时间（只记录到分钟，秒后面用0表示）
        /// </summary>
        [Number(NumberType.Long)]
        public long CreateAt { get; set; }

    }
}