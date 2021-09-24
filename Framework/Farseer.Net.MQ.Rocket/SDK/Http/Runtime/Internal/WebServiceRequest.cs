using System;
using System.Collections.Generic;

namespace FS.MQ.Rocket.SDK.Http.Runtime.Internal
{
    public abstract class WebServiceRequest
    {
        private TimeSpan? _readWriteTimeoutInternal;

        private TimeSpan? _timeoutInternal;

        public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>(comparer: StringComparer.OrdinalIgnoreCase);

        public IDictionary<string, string> Parameters { get; } = new Dictionary<string, string>(comparer: StringComparer.Ordinal);

        public string ContentType { get; set; }

        public string ContentMD5 { get; set; }

        internal TimeSpan? TimeoutInternal
        {
            get => _timeoutInternal;
            set
            {
                ClientConfig.ValidateTimeout(timeout: value);
                _timeoutInternal = value;
            }
        }

        internal TimeSpan? ReadWriteTimeoutInternal
        {
            get => _readWriteTimeoutInternal;
            set
            {
                ClientConfig.ValidateTimeout(timeout: value);
                _readWriteTimeoutInternal = value;
            }
        }

        internal virtual bool Expect100Continue => true;

        internal virtual bool KeepAlive => true;

        public void AddHeader(string headerName, string value)
        {
            Headers.Add(key: headerName, value: value);
        }

        public void AddParameter(string paramName, string value)
        {
            Parameters.Add(key: paramName, value: value);
        }

        public bool IsSetContentType() => ContentType != null;

        public bool IsSetContentMD5() => ContentMD5 != null;
    }
}