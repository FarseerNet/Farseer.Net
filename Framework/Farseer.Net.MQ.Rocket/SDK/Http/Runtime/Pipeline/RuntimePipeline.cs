using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace FS.MQ.Rocket.SDK.Http.Runtime.Pipeline
{
    public class RuntimePipeline : IDisposable
    {
        #region Properties

        public IPipelineHandler Handler { get; private set; }

        #endregion

        #region Private members

        private bool _disposed;

        // The top-most handler in the pipeline.

        #endregion

        #region Constructors

        public RuntimePipeline(IPipelineHandler handler)
        {
            if (handler == null) throw new ArgumentNullException(paramName: "handler");

            Handler = handler;
        }

        public RuntimePipeline(IList<IPipelineHandler> handlers)
        {
            if (handlers == null || handlers.Count == 0) throw new ArgumentNullException(paramName: "handlers");

            foreach (var handler in handlers) AddHandler(handler: handler);
        }

        #endregion

        #region Invoke methods

        public IResponseContext InvokeSync(IExecutionContext executionContext)
        {
            ThrowIfDisposed();

            Handler.InvokeSync(executionContext: executionContext);
            return executionContext.ResponseContext;
        }

        public IAsyncResult InvokeAsync(IAsyncExecutionContext executionContext)
        {
            ThrowIfDisposed();

            return Handler.InvokeAsync(executionContext: executionContext);
        }

        #endregion

        #region Handler methods

        public void AddHandler(IPipelineHandler handler)
        {
            if (handler == null) throw new ArgumentNullException(paramName: "handler");

            ThrowIfDisposed();

            var innerMostHandler = GetInnermostHandler(handler: handler);

            if (Handler != null)
            {
                innerMostHandler.InnerHandler = Handler;
                Handler.OuterHandler          = innerMostHandler;
            }

            Handler = handler;

            SetHandlerProperties(handler: handler);
        }

        public void AddHandlerAfter<T>(IPipelineHandler handler)
            where T : IPipelineHandler
        {
            if (handler == null) throw new ArgumentNullException(paramName: "handler");

            ThrowIfDisposed();

            var type    = typeof(T);
            var current = Handler;
            while (current != null)
            {
                if (current.GetType() == type)
                {
                    InsertHandler(handler: handler, current: current);
                    SetHandlerProperties(handler: handler);
                    return;
                }

                current = current.InnerHandler;
            }

            throw new InvalidOperationException(
                                                message: string.Format(provider: CultureInfo.InvariantCulture, format: "Cannot find a handler of type {0}", arg0: type.Name));
        }

        public void AddHandlerBefore<T>(IPipelineHandler handler)
            where T : IPipelineHandler
        {
            if (handler == null) throw new ArgumentNullException(paramName: "handler");

            ThrowIfDisposed();

            var type = typeof(T);
            if (Handler.GetType() == type)
            {
                // Add the handler to the top of the pipeline
                AddHandler(handler: handler);
                SetHandlerProperties(handler: handler);
                return;
            }

            var current = Handler;
            while (current != null)
            {
                if (current.InnerHandler           != null &&
                    current.InnerHandler.GetType() == type)
                {
                    InsertHandler(handler: handler, current: current);
                    SetHandlerProperties(handler: handler);
                    return;
                }

                current = current.InnerHandler;
            }

            throw new InvalidOperationException(
                                                message: string.Format(provider: CultureInfo.InvariantCulture, format: "Cannot find a handler of type {0}", arg0: type.Name));
        }

        public void RemoveHandler<T>()
        {
            ThrowIfDisposed();

            var type = typeof(T);

            IPipelineHandler previous = null;
            var              current  = Handler;

            while (current != null)
            {
                if (current.GetType() == type)
                {
                    // Cannot remove the handler if it's the only one in the pipeline
                    if (current == Handler && Handler.InnerHandler == null)
                    {
                        throw new InvalidOperationException(
                                                            message: "The pipeline contains a single handler, cannot remove the only handler in the pipeline.");
                    }

                    // current is the top, point top to current's inner handler
                    if (current == Handler) Handler = current.InnerHandler;


                    // Wireup outer handler to current's inner handler
                    if (current.OuterHandler != null) current.OuterHandler.InnerHandler = current.InnerHandler;

                    // Wireup inner handler to current's outer handler
                    if (current.InnerHandler != null) current.InnerHandler.OuterHandler = current.OuterHandler;

                    // Cleanup current
                    current.InnerHandler = null;
                    current.OuterHandler = null;

                    return;
                }

                previous = current;
                current  = current.InnerHandler;
            }

            throw new InvalidOperationException(
                                                message: string.Format(provider: CultureInfo.InvariantCulture, format: "Cannot find a handler of type {0}", arg0: type.Name));
        }

        public void ReplaceHandler<T>(IPipelineHandler handler)
            where T : IPipelineHandler
        {
            if (handler == null) throw new ArgumentNullException(paramName: "handler");

            ThrowIfDisposed();

            var              type     = typeof(T);
            IPipelineHandler previous = null;
            var              current  = Handler;
            while (current != null)
            {
                if (current.GetType() == type)
                {
                    // Replace current with handler.
                    handler.InnerHandler = current.InnerHandler;
                    handler.OuterHandler = current.OuterHandler;
                    if (previous != null)
                    {
                        // Wireup previous handler
                        previous.InnerHandler = handler;
                    }
                    else
                    {
                        // Current is the top, replace it.
                        Handler = handler;
                    }

                    if (current.InnerHandler != null)
                    {
                        // Wireup next handler
                        current.InnerHandler.OuterHandler = handler;
                    }

                    // Cleanup current
                    current.InnerHandler = null;
                    current.OuterHandler = null;

                    SetHandlerProperties(handler: handler);
                    return;
                }

                previous = current;
                current  = current.InnerHandler;
            }

            throw new InvalidOperationException(
                                                message: string.Format(provider: CultureInfo.InvariantCulture, format: "Cannot find a handler of type {0}", arg0: type.Name));
        }

        private static void InsertHandler(IPipelineHandler handler, IPipelineHandler current)
        {
            var next = current.InnerHandler;
            current.InnerHandler = handler;
            handler.OuterHandler = current;

            if (next != null)
            {
                var innerMostHandler = GetInnermostHandler(handler: handler);
                innerMostHandler.InnerHandler = next;
                next.OuterHandler             = innerMostHandler;
            }
        }

        private static IPipelineHandler GetInnermostHandler(IPipelineHandler handler)
        {
            Debug.Assert(condition: handler != null);

            var current                                  = handler;
            while (current.InnerHandler != null) current = current.InnerHandler;
            return current;
        }

        private void SetHandlerProperties(IPipelineHandler handler)
        {
            ThrowIfDisposed();
        }

        public List<IPipelineHandler> Handlers => EnumerateHandlers().ToList();

        public IEnumerable<IPipelineHandler> EnumerateHandlers()
        {
            var handler = Handler;
            while (handler != null)
            {
                yield return handler;
                handler = handler.InnerHandler;
            }
        }

        #endregion

        #region Dispose methods

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(obj: this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                var handler = Handler;
                while (handler != null)
                {
                    var innerHandler = handler.InnerHandler;
                    var disposable   = handler as IDisposable;
                    if (disposable != null) disposable.Dispose();
                    handler = innerHandler;
                }

                _disposed = true;
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(objectName: GetType().FullName);
        }

        #endregion
    }
}