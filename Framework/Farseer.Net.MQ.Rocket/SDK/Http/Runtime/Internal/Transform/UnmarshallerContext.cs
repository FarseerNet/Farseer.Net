using System;
using System.IO;

namespace FS.MQ.Rocket.SDK.Http.Runtime.Internal.Transform
{
    public abstract class UnmarshallerContext : IDisposable
    {
        private   bool             disposed;
        protected IWebResponseData WebResponseData { get; set; }

        public Stream ResponseStream { get; set; }

        public IWebResponseData ResponseData => WebResponseData;

        #region Dispose Pattern Implementation

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (ResponseStream != null)
                    {
                        ResponseStream.Dispose();
                        ResponseStream = null;
                    }
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(obj: this);
        }

        #endregion
    }

    public class XmlUnmarshallerContext : UnmarshallerContext
    {
        #region Constructors

        public XmlUnmarshallerContext(Stream responseStream, IWebResponseData responseData)
        {
            ResponseStream  = responseStream;
            WebResponseData = responseData;
        }

        #endregion
    }
}