using MongoDB.Driver;

namespace FS.MongoDB
{
    public interface IMongoManager
    {
        MongoClient Client { get; }
    }
}