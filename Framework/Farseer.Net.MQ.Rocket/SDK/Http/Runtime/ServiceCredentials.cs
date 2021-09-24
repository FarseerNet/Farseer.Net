using System;

namespace FS.MQ.Rocket.SDK.Http.Runtime
{
    public class ImmutableCredentials
    {
        #region Public methods

        public ImmutableCredentials Copy()
        {
            var credentials = new ImmutableCredentials
            {
                AccessKey     = AccessKey,
                SecretKey     = SecretKey,
                SecurityToken = SecurityToken
            };
            return credentials;
        }

        #endregion

        #region Properties

        public string AccessKey { get; private set; }

        public string SecretKey { get; private set; }

        public string SecurityToken { get; private set; }

        #endregion


        #region Constructors

        public ImmutableCredentials(string accessKeyId, string secretAccessKey, string stsToken)
        {
            if (string.IsNullOrEmpty(value: accessKeyId)) throw new ArgumentNullException(paramName: "accessKeyId");
            if (string.IsNullOrEmpty(value: secretAccessKey)) throw new ArgumentNullException(paramName: "secretAccessKey");
            if (string.IsNullOrEmpty(value: stsToken))
                SecurityToken = null;
            else
                SecurityToken = stsToken;


            AccessKey = accessKeyId;
            SecretKey = secretAccessKey;
        }

        private ImmutableCredentials()
        {
        }

        #endregion
    }

    public abstract class ServiceCredentials
    {
        public abstract ImmutableCredentials GetCredentials();
    }

    public class BasicServiceCredentials : ServiceCredentials
    {
        #region Private members

        private readonly ImmutableCredentials _credentials;

        #endregion


        #region Constructors

        public BasicServiceCredentials(string accessKey, string secretKey, string stsToken)
        {
            if (!string.IsNullOrEmpty(value: accessKey)) _credentials = new ImmutableCredentials(accessKeyId: accessKey, secretAccessKey: secretKey, stsToken: stsToken);
        }

        #endregion


        #region Abstract class overrides

        public override ImmutableCredentials GetCredentials()
        {
            if (_credentials == null) return null;

            return _credentials.Copy();
        }

        #endregion
    }
}