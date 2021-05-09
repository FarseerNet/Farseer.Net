namespace FS.MQ.RedisStream
{
    public class ConsumeContext
    {
        public string[] MessageIds { get; internal set; }
    }
}