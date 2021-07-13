using System.Collections.Generic;
using FS.Core.LinkTrack;
using Nest;

namespace FS.LinkTrack
{
    [ElasticsearchType(IdProperty = "Id")]
    public class LinkTrackContextPO : LinkTrackContext
    {
        [Keyword]
        public string Id { get; set; }

        [Keyword]
        public override string ContextId { get; set; }

        [Keyword]
        public override string AppId { get; set; }

        [Number(NumberType.Long)]
        public override long StartTs { get; set; }

        [Object]
        public override List<LinkTrackDetail> List { get; set; }
    }
}