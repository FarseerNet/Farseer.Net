using System;

namespace FS.ElasticSearch.Queue
{
    /// <summary>
    /// 队列中针对ES的Extra数据
    /// </summary>
    public class QueueDataEs
    {
        public String IndexName { get; set; }
        public String TypeName { get; set; }

    }
}
