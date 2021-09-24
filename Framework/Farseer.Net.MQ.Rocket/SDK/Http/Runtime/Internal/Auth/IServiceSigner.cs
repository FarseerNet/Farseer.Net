namespace FS.MQ.Rocket.SDK.Http.Runtime.Internal.Auth
{
    public interface IServiceSigner
    {
        void Sign(IRequest request, string accessKeyId, string secretAccessKey, string stsToken);
    }
}