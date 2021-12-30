using System;
using System.Collections.Generic;
using FS.Core.Mapping.Attribute;

namespace Farseer.Net.DataDemo
{
    /// <summary>
    /// 任务组记录
    /// </summary>
    public class TaskGroupPO
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Field(Name = "id", IsPrimaryKey = true, IsDbGenerated = true)]
        public int? Id { get; set; }

        /// <summary>
        /// 任务组标题
        /// </summary>
        [Field(Name = "caption")]
        public string Caption { get; set; }

        /// <summary>
        /// 实现Job的特性名称（客户端识别哪个实现类）
        /// </summary>
        [Field(Name = "job_name")]
        public string JobName { get; set; }

        /// <summary>
        /// 传给客户端的参数，按逗号分隔
        /// </summary>
        [Field(Name = "data", StorageType = EumStorageType.Json)]
        public Dictionary<string, string> Data { get; set; }

    }
}