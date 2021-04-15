using System;
using System.Collections.Generic;
using FS.ElasticSearch;
using FS.ElasticSearch.Map;

namespace Farseer.Net.ElasticSearch.Console
{
    /// <summary>
    /// 测试ES上下文
    /// </summary>
    public class TestContext : EsContext<TestContext>
    {
        public TestContext() : base("default")
        {
        }

        protected override void CreateModelInit(Dictionary<string, SetDataMap> map)
        {
            map["User"].SetName($"User_{DateTime.Now:yyyy_MM_dd}", 2, 0, "User");
        }

        /// <summary>
        /// 用户索引
        /// </summary>
        public IndexSet<UserPo> User { get; set; }
    }
}