﻿namespace FS.ElasticSearch.Configuration
{
    /// <summary>
    /// ES配置项类
    /// </summary>
    public class ElasticSearchItemConfig
    {
        /// <summary> 集群名称 </summary>
        public string Name { get; set; }

        /// <summary> 集群地址,多个地址用逗号隔开 </summary>
        public string Server { get; set; }

        /// <summary> 集群用户名 </summary>
        public string Username { get; set; }

        /// <summary> 集群密码 </summary>
        public string Password { get; set; }
    }
}
