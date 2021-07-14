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

        [Number(NumberType.Long)]
        public override long EndTs { get; set; }
        
        [Keyword]
        public override string Domain { get; set; }

        [Keyword]
        public override string Path { get; set; }
        
        [Keyword]
        public override string Method { get; set; }

        [Object]
        public override Dictionary<string, string> Headers { get; set; }

        [Keyword]
        public override string ContentType { get; set; }

        [Text]
        public override string RequestBody { get; set; }

        [Text]
        public override string ResponseBody { get; set; }

        [Keyword]
        public override string RequestIp { get; set; }

        [Object]
        public override List<LinkTrackDetail> List { get; set; }
    }
}