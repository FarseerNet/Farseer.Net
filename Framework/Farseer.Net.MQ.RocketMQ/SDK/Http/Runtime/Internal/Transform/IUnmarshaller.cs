namespace FS.MQ.RocketMQ.SDK.Http.Runtime.Internal.Transform
{
    public interface IUnmarshaller<T, R>
    {
        T Unmarshall(R input);
    }
}
