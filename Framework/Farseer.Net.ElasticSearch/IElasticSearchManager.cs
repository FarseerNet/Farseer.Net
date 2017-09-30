using Nest;

namespace Farseer.Net.ElasticSearch
{
    public interface IElasticSearchManager
    {
       ElasticClient Client { get; }
    }
}