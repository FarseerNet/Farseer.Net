using System;
using System.Collections.Generic;
using FS.ElasticSearch;
using FS.ElasticSearch.Map;
using FS.LinkTrack.Dal;

namespace FS.LinkTrack
{
    public class LinkTrackEsContext : EsContext<LinkTrackEsContext>
    {
        public LinkTrackEsContext() : base(configName: "LinkTrack")
        {
        }

        public IndexSet<LinkTrackContextPO> LinkTrackContext { get; set; }
        public IndexSet<SlowQueryPO>        SlowQuery        { get; set; }
        public IndexSet<ExceptionDetailPO>  ExceptionDetail  { get; set; }

        protected override void CreateModelInit()
        {
            LinkTrackContext.SetName(indexName: $"link_track_{DateTime.Now:yyyy_MM}", shardsCount: 1, replicasCount: 0, 1, "link_track");
            SlowQuery.SetName(indexName: $"slow_query_{DateTime.Now:yyyy_MM}", shardsCount: 1, replicasCount: 0, 1, "slow_query");
            ExceptionDetail.SetName(indexName: $"exception_track_{DateTime.Now:yyyy_MM}", shardsCount: 1, replicasCount: 0, 1, "exception_track");
        }
    }
}