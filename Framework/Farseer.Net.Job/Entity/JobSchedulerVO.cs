using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FS.Job.Entity
{
    public class JobSchedulerVO
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public int TaskId { get; set; }
        
        /// <summary>
        /// 任务组ID
        /// </summary>
        public int TaskGroupId { get; set; }

        /// <summary>
        /// 任务组标题
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// 实现Job的特性名称（客户端识别哪个实现类）
        /// </summary>
        public string JobTypeName { get; set; }
        
        /// <summary>
        /// 对Data的修改将会同步到服务端
        /// </summary>
        public Dictionary<string,string> Data { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartAt { get; set; }
        
        /// <summary>
        /// 客户端
        /// </summary>
        public string ClientHost { get; set; }
        
        /// <summary>
        /// 客户端IP
        /// </summary>
        public string ClientIp { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}