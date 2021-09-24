using System;
using System.Collections.Generic;
using FS.ElasticSearch;
using FS.ElasticSearch.Map;

namespace FS.LinkTrack
{
    public class LinkTrackEsContext : EsContext<LinkTrackEsContext>
    {
        public LinkTrackEsContext() : base(configName: "LinkTrack")
        {
        }

        public IndexSet<LinkTrackContextPO> LinkTrackContext { get; set; }

        protected override void CreateModelInit(Dictionary<string, SetDataMap> map)
        {
            map[key: "LinkTrackContext"].SetName(indexName: $"link_track_{DateTime.Now:yyyy_MM}", shardsCount: 1, replicasCount: 0, "link_track");
        }
    }
}