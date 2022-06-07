using System;
using Collections.Pooled;
using FS.Core.LinkTrack;
using Nest;

namespace FS.LinkTrack.Dal;

public class LinkTrackContextPO : LinkTrackContext
{
    /// <summary>
    ///     应用Id
    /// </summary>
    public long AppId { get; set; }

    /// <summary>
    ///     应用Ip
    /// </summary>
    [Keyword]
    public string AppIp { get; set; }
    /// <summary>long
    ///     应用名称
    /// </summary>
    [Keyword]
    public string AppName { get; set; }

    [Keyword]
    public override string ContextId { get; set; }

    [Keyword]
    public override string ParentAppName { get; set; }

    [Keyword]
    public override string Domain { get; set; }

    [Text]
    public override string Path { get; set; }

    [Keyword]
    public override string Method { get; set; }

    [Flattened]
    public override PooledDictionary<string, string> Headers { get; set; }

    [Keyword]
    public override string ContentType { get; set; }

    [Text]
    public override string RequestBody { get; set; }

    [Text]
    public override string ResponseBody { get; set; }

    [Keyword]
    public override string RequestIp { get; set; }

    [Object()]
    public override PooledList<LinkTrackDetail> List { get; set; }

    [Keyword]
    public override string StatusCode { get; set; }
}