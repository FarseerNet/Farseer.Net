using Nest;

namespace FS.EC.Dal
{
    /// <summary>
    /// 主机环境相关的数据
    /// </summary>
    public class HostPO
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
        /// 硬盘使用率 DiskUse / DiskMax（单位MB)
        /// </summary>
        [Number(type: NumberType.ScaledFloat, ScalingFactor = 100)]
        public decimal DiskRate { get; set; }

        /// <summary>
        /// 硬盘使用实际值（单位MB)
        /// </summary>
        [Number(type: NumberType.ScaledFloat, ScalingFactor = 100)]
        public decimal DiskUse { get; set; }

        /// <summary>
        /// 硬盘总容量（单位MB)
        /// </summary>
        [Number(type: NumberType.ScaledFloat, ScalingFactor = 100)]
        public decimal DiskMax { get; set; }

        /// <summary>
        /// 进程数量
        /// </summary>
        [Number(NumberType.Integer)]
        public int ProcessCount { get; set; }

        /// <summary>
        /// TCP CLOSED数量
        /// </summary>
        [Number(NumberType.Integer)]
        public int TcpClosedCount { get; set; }

        /// <summary>
        /// TCP LISTEN数量
        /// </summary>
        [Number(NumberType.Integer)]
        public int TcpListenCount { get; set; }

        /// <summary>
        /// TCP SYN_RCVD数量
        /// </summary>
        [Number(NumberType.Integer)]
        public int TcpSynRcvdCount { get; set; }

        /// <summary>
        /// TCP SYN_SENT数量
        /// </summary>
        [Number(NumberType.Integer)]
        public int TcpSynSentCount { get; set; }

        /// <summary>
        /// TCP ESTABLISHED数量
        /// </summary>
        [Number(NumberType.Integer)]
        public int TcpEstablishedCount { get; set; }

        /// <summary>
        /// TCP FIN_WAIT_1数量
        /// </summary>
        [Number(NumberType.Integer)]
        public int TcpFinWait1Count { get; set; }

        /// <summary>
        /// TCP FIN_WAIT_2数量
        /// </summary>
        [Number(NumberType.Integer)]
        public int TcpFinWait2Count { get; set; }

        /// <summary>
        /// TCP TIME_WAIT数量
        /// </summary>
        [Number(NumberType.Integer)]
        public int TcpTimeWaitCount { get; set; }

        /// <summary>
        /// TCP CLOSING数量
        /// </summary>
        [Number(NumberType.Integer)]
        public int TcpClosingCount { get; set; }

        /// <summary>
        /// TCP CLOSE_WAIT数量
        /// </summary>
        [Number(NumberType.Integer)]
        public int TcpCloseWaitCount { get; set; }

        /// <summary>
        /// TCP LAST_ACK数量
        /// </summary>
        [Number(NumberType.Integer)]
        public int TcpLastAckCount { get; set; }
        
        /// <summary>
        /// 采集时间（只记录到分钟，秒后面用0表示）
        /// </summary>
        [Number(NumberType.Long)]
        public long CreateAt { get; set; }

    }
}