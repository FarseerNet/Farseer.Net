using Nest;

namespace Farseer.Net.ElasticSearchDemo
{
    [ElasticsearchType(IdProperty = "UserName")]
    public class UserPo
    {
        [Keyword]
        public string UserName { get; set; }
    }
}