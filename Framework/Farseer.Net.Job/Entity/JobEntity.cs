using System;

namespace FS.Job.Entity
{
    public class JobEntity
    {
        public int Index { get; set; }
        public string JobName { get; set; }
        public Type JobType { get; set; }
        public string IocName { get; set; }
    }
}