using System;
using System.Collections.Generic;
using FS.EC.Dal;
using FS.ElasticSearch;
using FS.ElasticSearch.Map;

namespace FS.EC
{
    public class EnvCollectEsContext : EsContext<EnvCollectEsContext>
    {
        public EnvCollectEsContext() : base(configName: "LinkTrack")
        {
        }

        public IndexSet<HostPO> Host { get; set; }

        protected override void CreateModelInit(Dictionary<string, SetDataMap> map)
        {
            map["Host"].SetName(indexName: $"env_host_{DateTime.Now:yyyy_MM}", shardsCount: 1, replicasCount: 0, 1, "env_host");
            Host.SetName(indexName: $"env_host_{DateTime.Now:yyyy_MM}", shardsCount: 1, replicasCount: 0, 1, "env_host");
        }
    }
}