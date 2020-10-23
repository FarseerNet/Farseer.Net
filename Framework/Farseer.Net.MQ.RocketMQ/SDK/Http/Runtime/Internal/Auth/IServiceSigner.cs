namespace FS.MQ.RocketMQ.SDK.Http.Runtime.Internal.Auth
{
    public partial interface IServiceSigner
    {
         void Sign(IRequest request, string accessKeyId, string secretAccessKey, string stsToken);
    }
}
