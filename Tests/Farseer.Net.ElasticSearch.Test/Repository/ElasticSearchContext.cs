using FS.ElasticSearch;

namespace Farseer.Net.ElasticSearch.Test.Repository
{
    /// <summary>
    ///     测试ES上下文
    /// </summary>
    public class ElasticSearchContext : EsContext<ElasticSearchContext>
    {
        public ElasticSearchContext() : base(configName: "es")
        {
        }

        /// <summary>
        ///     用户索引
        /// </summary>
        public IndexSet<UserPo> User { get; set; }

        protected override void CreateModelInit()
        {
            User.SetName(indexName: $"UnitTest_User", shardsCount: 2, replicasCount: 0, 1, "User");
        }
    }
}