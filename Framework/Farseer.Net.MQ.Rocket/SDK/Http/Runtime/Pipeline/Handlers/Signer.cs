namespace FS.MQ.Rocket.SDK.Http.Runtime.Pipeline.Handlers
{
    public class Signer : GenericHandler
    {
        protected override void PreInvoke(IExecutionContext executionContext)
        {
            if (ShouldSign(requestContext: executionContext.RequestContext))
            {
                SignRequest(requestContext: executionContext.RequestContext);
                executionContext.RequestContext.IsSigned = true;
            }
        }

        private static bool ShouldSign(IRequestContext requestContext) => !requestContext.IsSigned;

        internal static void SignRequest(IRequestContext requestContext)
        {
            var immutableCredentials = requestContext.ImmutableCredentials;

            if (immutableCredentials == null) return;

            requestContext.Signer.Sign(request: requestContext.Request, accessKeyId: immutableCredentials.AccessKey, secretAccessKey: immutableCredentials.SecretKey, stsToken: immutableCredentials.SecurityToken);
        }
    }
}