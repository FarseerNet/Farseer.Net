using System;
using FS.ElasticSearch;

namespace Farseer.Net.ElasticSearchDemo
{
    /// <summary>
    ///     测试ES上下文
    /// </summary>
    public class TestContext : EsContext<TestContext>
    {
        public TestContext() : base(configName: "default")
        {
        }

        /// <summary>
        ///     用户索引
        /// </summary>
        public IndexSet<UserPo> User { get; set; }

        protected override void CreateModelInit()
        {
            User.SetName(indexName: $"User_{DateTime.Now:yyyy_MM_dd}", shardsCount: 2, replicasCount: 0, 1, "User");
        }
    }
}