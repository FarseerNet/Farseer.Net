using System;
using System.Net;
using FS.MQ.Rocket.SDK.Http.Util;

namespace FS.MQ.Rocket.SDK.Http.Runtime
{
    public abstract class ClientConfig
    {
        private static readonly TimeSpan InfiniteTimeout = TimeSpan.FromMilliseconds(value: -1);
        private static readonly TimeSpan MaxTimeout      = TimeSpan.FromMilliseconds(value: int.MaxValue);

        private int? _connectionLimit = 200;
        private int? _maxIdleTime;

        private TimeSpan? _readWriteTimeout;
        private Uri       _regionEndpoint;
        private TimeSpan? _timeout;

        #region Constructor

        public ClientConfig()
        {
            Initialize();
        }

        #endregion

        /// <summary>
        ///     Gets Service Version.
        /// </summary>
        public abstract string ServiceVersion { get; }

        /// <summary>
        ///     Gets Service Name.
        /// </summary>
        public abstract string ServiceName { get; }

        /// <summary>
        ///     Gets of the UserAgent property.
        /// </summary>
        public string UserAgent { get; } = AliyunSDKUtils.SDKUserAgent;

        /// <summary>
        ///     Gets and sets the RegionEndpoint property.
        /// </summary>
        public Uri RegionEndpoint
        {
            get
            {
                if (_regionEndpoint == null) throw new ArgumentException(message: "Endpoint must be specified.");
                return _regionEndpoint;
            }
            set => _regionEndpoint = value;
        }

        /// <summary>
        ///     Gets and sets of the MaxErrorRetry property.
        /// </summary>
        public int MaxErrorRetry { get; set; } = 3;

        /// <summary>
        ///     Gets and Sets the BufferSize property.
        ///     The BufferSize controls the buffer used to read in from input streams and write
        ///     out to the request.
        /// </summary>
        public int BufferSize { get; set; } = AliyunSDKUtils.DefaultBufferSize;

        /// <summary>
        ///     This flag controls if .NET HTTP infrastructure should follow redirection responses.
        /// </summary>
        internal bool AllowAutoRedirect { get; set; } = false;

        /// <summary>
        ///     Flag on whether to completely disable logging for this client or not.
        /// </summary>
        internal bool DisableLogging { get; set; } = true;

        /// <summary>
        ///     Credentials to use with a proxy.
        /// </summary>
        public ICredentials ProxyCredentials { get; set; }

        /// <summary>
        ///     Gets and sets of the ProxyHost property.
        /// </summary>
        public string ProxyHost { get; set; }


        /// <summary>
        ///     Gets and sets of the ProxyPort property.
        /// </summary>
        public int ProxyPort { get; set; } = -1;

        /// <summary>
        ///     Gets and sets the max idle time set on the ServicePoint for the WebRequest.
        /// </summary>
        public int MaxIdleTime
        {
            get => AliyunSDKUtils.GetMaxIdleTime(clientConfigValue: _maxIdleTime);
            set => _maxIdleTime = value;
        }

        /// <summary>
        ///     Gets and sets the connection limit set on the ServicePoint for the WebRequest.
        /// </summary>
        public int ConnectionLimit
        {
            get => AliyunSDKUtils.GetConnectionLimit(clientConfigValue: _connectionLimit);
            set => _connectionLimit = value;
        }

        /// <summary>
        ///     Overrides the default read-write timeout value.
        /// </summary>
        public TimeSpan? ReadWriteTimeout
        {
            get => _readWriteTimeout;
            set
            {
                ValidateTimeout(timeout: value);
                _readWriteTimeout = value;
            }
        }

        /// <summary>
        ///     Overrides the default request timeout value.
        /// </summary>
        public TimeSpan? Timeout
        {
            get => _timeout;
            set
            {
                ValidateTimeout(timeout: value);
                _timeout = value;
            }
        }

        protected virtual void Initialize()
        {
        }

        internal static void ValidateTimeout(TimeSpan? timeout)
        {
            if (!timeout.HasValue) throw new ArgumentNullException(paramName: "timeout");

            if (timeout != InfiniteTimeout && (timeout <= TimeSpan.Zero || timeout > MaxTimeout)) throw new ArgumentOutOfRangeException(paramName: "timeout");
        }

        /// <summary>
        ///     Returns the request timeout value if its value is set,
        ///     else returns client timeout value.
        /// </summary>
        internal static TimeSpan? GetTimeoutValue(TimeSpan? clientTimeout, TimeSpan? requestTimeout) => requestTimeout.HasValue ? requestTimeout
                                                                                                        : clientTimeout.HasValue ? clientTimeout : null;
    }
}