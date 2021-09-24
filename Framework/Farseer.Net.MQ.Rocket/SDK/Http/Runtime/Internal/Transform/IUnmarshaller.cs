namespace FS.MQ.Rocket.SDK.Http.Runtime.Internal.Transform
{
    public interface IUnmarshaller<T, R>
    {
        T Unmarshall(R input);
    }
}