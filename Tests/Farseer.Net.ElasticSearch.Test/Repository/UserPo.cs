using Nest;

namespace Farseer.Net.ElasticSearch.Test.Repository
{
    [ElasticsearchType(IdProperty = "Id")]
    public class UserPo
    {
        [Keyword]
        public string Id { get; set; }
        [Keyword]
        public string UserName { get; set; }
        public int? Age { get; set; }
        [Text]
        public string Desc { get; set; }
        [Date]
        public long? CreateAt { get; set; }
    }
}