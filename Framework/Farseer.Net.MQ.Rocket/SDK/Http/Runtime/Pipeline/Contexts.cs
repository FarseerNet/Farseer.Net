﻿using System;
using FS.MQ.Rocket.SDK.Http.Runtime.Internal;
using FS.MQ.Rocket.SDK.Http.Runtime.Internal.Auth;
using FS.MQ.Rocket.SDK.Http.Runtime.Internal.Transform;

namespace FS.MQ.Rocket.SDK.Http.Runtime.Pipeline
{
    public interface IRequestContext
    {
        WebServiceRequest                        OriginalRequest      { get; }
        string                                   RequestName          { get; }
        IMarshaller<IRequest, WebServiceRequest> Marshaller           { get; }
        ResponseUnmarshaller                     Unmarshaller         { get; }
        IServiceSigner                           Signer               { get; }
        ClientConfig                             ClientConfig         { get; }
        ImmutableCredentials                     ImmutableCredentials { get; set; }

        IRequest Request  { get; set; }
        bool     IsSigned { get; set; }
        bool     IsAsync  { get; }
        int      Retries  { get; set; }
    }

    public interface IResponseContext
    {
        WebServiceResponse Response     { get; set; }
        IWebResponseData   HttpResponse { get; set; }
    }

    public interface IAsyncRequestContext : IRequestContext
    {
        AsyncCallback Callback { get; }
        object        State    { get; }
    }

    public interface IAsyncResponseContext : IResponseContext
    {
        RuntimeAsyncResult AsyncResult { get; set; }
    }

    public interface IExecutionContext
    {
        IResponseContext ResponseContext { get; }
        IRequestContext  RequestContext  { get; }
    }

    public interface IAsyncExecutionContext
    {
        IAsyncResponseContext ResponseContext { get; }
        IAsyncRequestContext  RequestContext  { get; }

        object RuntimeState { get; set; }
    }

    public class RequestContext : IRequestContext
    {
        public IRequest                                 Request              { get; set; }
        public IServiceSigner                           Signer               { get; set; }
        public ClientConfig                             ClientConfig         { get; set; }
        public int                                      Retries              { get; set; }
        public bool                                     IsSigned             { get; set; }
        public bool                                     IsAsync              { get; set; }
        public WebServiceRequest                        OriginalRequest      { get; set; }
        public IMarshaller<IRequest, WebServiceRequest> Marshaller           { get; set; }
        public ResponseUnmarshaller                     Unmarshaller         { get; set; }
        public ImmutableCredentials                     ImmutableCredentials { get; set; }

        public string RequestName => OriginalRequest.GetType().Name;
    }

    public class AsyncRequestContext : RequestContext, IAsyncRequestContext
    {
        public AsyncCallback Callback { get; set; }
        public object        State    { get; set; }
    }

    public class ResponseContext : IResponseContext
    {
        public WebServiceResponse Response     { get; set; }
        public IWebResponseData   HttpResponse { get; set; }
    }

    public class AsyncResponseContext : ResponseContext, IAsyncResponseContext
    {
        public RuntimeAsyncResult AsyncResult { get; set; }
    }

    public class ExecutionContext : IExecutionContext
    {
        public ExecutionContext()
        {
            RequestContext  = new RequestContext();
            ResponseContext = new ResponseContext();
        }

        public ExecutionContext(IRequestContext requestContext, IResponseContext responseContext)
        {
            RequestContext  = requestContext;
            ResponseContext = responseContext;
        }

        public IRequestContext  RequestContext  { get; }
        public IResponseContext ResponseContext { get; }

        public static IExecutionContext CreateFromAsyncContext(IAsyncExecutionContext asyncContext) => new ExecutionContext(requestContext: asyncContext.RequestContext,
                                                                                                                            responseContext: asyncContext.ResponseContext);
    }

    public class AsyncExecutionContext : IAsyncExecutionContext
    {
        public AsyncExecutionContext()
        {
            RequestContext  = new AsyncRequestContext();
            ResponseContext = new AsyncResponseContext();
        }

        public AsyncExecutionContext(IAsyncRequestContext requestContext, IAsyncResponseContext responseContext)
        {
            RequestContext  = requestContext;
            ResponseContext = responseContext;
        }

        public IAsyncResponseContext ResponseContext { get; }
        public IAsyncRequestContext  RequestContext  { get; }

        public object RuntimeState { get; set; }
    }
}