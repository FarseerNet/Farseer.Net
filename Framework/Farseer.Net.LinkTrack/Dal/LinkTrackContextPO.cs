using System.Collections.Generic;
using FS.Core.LinkTrack;
using Mapster;
using Nest;

namespace FS.LinkTrack.Dal
{
    public class LinkTrackContextPO : LinkTrackContext
    {
        [Keyword]
        public override string ContextId { get; set; }

        [Keyword]
        public override string AppName { get; set; }

        [Keyword]
        public override string AppIp { get; set; }

        [Keyword]
        public override string ParentAppName { get; set; }

        [Keyword]
        public override string Domain { get; set; }

        [Text]
        public override string Path { get; set; }

        [Keyword]
        public override string Method { get; set; }

        [Flattened]
        public override Dictionary<string, string> Headers { get; set; }

        [Keyword]
        public override string ContentType { get; set; }

        [Text]
        public override string RequestBody { get; set; }

        [Text]
        public override string ResponseBody { get; set; }

        [Keyword]
        public override string RequestIp { get; set; }

        [Object()]
        public override List<LinkTrackDetail> List { get; set; }

        [Keyword]
        public override string StatusCode { get; set; }
    }
}