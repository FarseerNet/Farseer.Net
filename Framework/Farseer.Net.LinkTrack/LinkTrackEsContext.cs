using System;
using System.Collections.Generic;
using FS.ElasticSearch;
using FS.ElasticSearch.Map;

namespace FS.LinkTrack
{
    public class LinkTrackEsContext : EsContext<LinkTrackEsContext>
    {
        public LinkTrackEsContext() : base("LinkTrack")
        {
        }

        protected override void CreateModelInit(Dictionary<string, SetDataMap> map)
        {
            map["LinkTrackContext"].SetName($"link_track_{DateTime.Now:yyyy_MM_dd}", 1, 0, "link_track");
        }

        public IndexSet<LinkTrackContextPO> LinkTrackContext { get; set; }
    }
}