using System;
using System.Threading;

namespace FS.MQ.Rocket.SDK.Http.Runtime.Pipeline
{
    public class RuntimeAsyncResult : IAsyncResult, IDisposable
    {
        private          bool             _callbackInvoked;
        private          bool             _disposed;
        private readonly object           _lockObj;
        private          ManualResetEvent _waitHandle;

        public RuntimeAsyncResult(AsyncCallback asyncCallback, object asyncState)
        {
            _lockObj         = new object();
            _callbackInvoked = false;

            AsyncState             = asyncState;
            IsCompleted            = false;
            AsyncCallback          = asyncCallback;
            CompletedSynchronously = false;
        }

        public AsyncCallback AsyncCallback { get; }

        public Exception Exception { get; set; }

        public WebServiceResponse Response { get; set; }

        public object AsyncState { get; }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                if (_waitHandle != null) return _waitHandle;

                lock (_lockObj)
                {
                    if (_waitHandle == null) _waitHandle = new ManualResetEvent(initialState: IsCompleted);
                }

                return _waitHandle;
            }
        }

        public bool CompletedSynchronously { get; }

        public bool IsCompleted { get; private set; }

        internal void SignalWaitHandle()
        {
            IsCompleted = true;
            if (_waitHandle != null) _waitHandle.Set();
        }

        internal void HandleException(Exception exception)
        {
            Exception = exception;
            InvokeCallback();
        }

        internal void InvokeCallback()
        {
            SignalWaitHandle();
            if (!_callbackInvoked && AsyncCallback != null)
            {
                _callbackInvoked = true;
                AsyncCallback(ar: this);
            }
        }

        #region Dispose Pattern Implementation

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing && _waitHandle != null)
                {
#if WIN_RT
                    _waitHandle.Dispose();
#else
                    _waitHandle.Close();
#endif
                    _waitHandle = null;
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(obj: this);
        }

        #endregion
    }
}