using Farseer.Net.ElasticSearch.Test.Repository;
using FS;
using FS.ElasticSearch;

namespace Farseer.Net.ElasticSearch.Test;

public class BaseTests
{
    [NUnit.Framework.OneTimeSetUp]
    public void Setup()
    {
        FarseerApplication.Run<ElasticSearchModule>().Initialize();
        
        // 先删除索引
        ElasticSearchContext.Data.User.DropIndex();
    }
}