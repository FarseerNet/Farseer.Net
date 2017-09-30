using MongoDB.Driver;

namespace Farseer.Net.MongoDB
{
    public interface IMongoManager
    {
        MongoClient Client { get; }
    }
}