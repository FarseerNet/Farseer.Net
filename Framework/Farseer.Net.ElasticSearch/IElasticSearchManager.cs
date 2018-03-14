using Nest;

namespace FS.ElasticSearch
{
    public interface IElasticSearchManager
    {
       ElasticClient Client { get; }
    }
}