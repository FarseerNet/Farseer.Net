using System;

namespace FS.Job.Entity
{
    public class HistoryExecuteEntity
    {
        public DateTime CreateAt { get; set; }
        public string Msg { get; set; }
        public long UseTime { get; set; }
    }
}