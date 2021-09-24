using System;
using System.Diagnostics;
using Snowflake.Core;

namespace FS.Core
{
    /// <summary>
    ///     雪花算法ID
    /// </summary>
    public class SnowflakeId
    {
        private static readonly IdWorker worker;

        static SnowflakeId()
        {
            // 以机器名称 + 进程ID 作为数据中心ID
            var dataCenterId                   = HashCode(str: Environment.MachineName);
            if (dataCenterId < 0) dataCenterId *= -1;
            dataCenterId = (dataCenterId + Process.GetCurrentProcess().Id) % 32;

            worker = new IdWorker(workerId: new Random().Next(minValue: 1, maxValue: 31), datacenterId: dataCenterId);
        }

        /// <summary>
        ///     获取唯一ID
        /// </summary>
        public static long GenerateId => worker.NextId();

        /// <summary>
        ///     .net core hashCode
        /// </summary>
        private static int HashCode(string str)
        {
            unchecked
            {
                var hash1 = (5381 << 16) + 5381;
                var hash2 = hash1;

                for (var i = 0; i < str.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[index: i];
                    if (i == str.Length - 1) break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[index: i + 1];
                }

                return hash1 + hash2 * 1566083941;
            }
        }
    }
}