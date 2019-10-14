namespace FS.MQ.RocketMQ.SDK.Http.Runtime.Internal.Transform
{
    public interface IMarshaller<T, R>
    {
        T Marshall(R input);
    }
}
