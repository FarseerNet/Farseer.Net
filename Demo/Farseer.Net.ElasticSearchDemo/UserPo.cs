using Nest;

namespace Farseer.Net.ElasticSearchDemo
{
    [ElasticsearchType(IdProperty = "Id")]
    public class UserPo
    {
        [Keyword]
        public string Id { get; set; }
        [Keyword]
        public string UserName { get; set; }
        
        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }
        
        /// <summary>
        /// 自我介绍
        /// </summary>
        [Text]
        public string Desc { get; set; }
    }
}