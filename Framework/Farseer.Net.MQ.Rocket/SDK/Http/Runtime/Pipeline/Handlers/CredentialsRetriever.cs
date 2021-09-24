namespace FS.MQ.Rocket.SDK.Http.Runtime.Pipeline.Handlers
{
    public class CredentialsRetriever : GenericHandler
    {
        public CredentialsRetriever(ServiceCredentials credentials)
        {
            Credentials = credentials;
        }

        protected ServiceCredentials Credentials { get; }

        protected override void PreInvoke(IExecutionContext executionContext)
        {
            ImmutableCredentials ic = null;
            if (Credentials != null && Credentials is BasicServiceCredentials)
            {
                ic = Credentials.GetCredentials();
            }

            executionContext.RequestContext.ImmutableCredentials = ic;
        }
    }
}