namespace FS.Core.LinkTrack
{
    public enum EumCallType
    {
        HttpClient    = 0,
        GrpcClient    = 1,
        Database      = 2,
        Redis         = 3,
        Mq            = 4,
        Elasticsearch = 5,
        Custom        = 6,
    }
}