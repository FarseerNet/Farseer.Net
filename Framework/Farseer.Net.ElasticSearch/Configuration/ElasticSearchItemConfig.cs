﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Farseer.Net.ElasticSearch.Configuration
{
    /// <summary>
    /// ES配置项类
    /// </summary>
    public class ElasticSearchItemConfig
    {
        /// <summary> 集群名称 </summary>
        public string Name = ".";

        /// <summary> 集群地址,多个地址用逗号隔开 </summary>
        public string Server = "";
    }
}
