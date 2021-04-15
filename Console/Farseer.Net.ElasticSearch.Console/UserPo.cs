using Nest;

namespace Farseer.Net.ElasticSearch.Console
{
    [ElasticsearchType(IdProperty = "UserName")]
    public class UserPo
    {
        [Keyword]
        public string UserName { get; set; }
    }
}