using System.Net;
using System.Net.Http.Headers;

namespace FS.Context
{
    public static class HttpWebRequestCatExt
    {
        public static void AddTraceInfoToHeader(this HttpWebRequest httpWebRequest)
        {
            httpWebRequest.Headers.Add("_catRootMessageId", TraceIdContext.Current?.RootId ?? "");
            httpWebRequest.Headers.Add("_catParentMessageId", TraceIdContext.Current?.ParentId ?? "");
            httpWebRequest.Headers.Add("_catChildMessageId", TraceIdContext.Current?.ChildId ?? "");
        }

        public static void AddTraceInfoToHeader(this HttpContentHeaders httpWebRequest)
        {
            httpWebRequest.Add("_catRootMessageId", TraceIdContext.Current?.RootId ?? "");
            httpWebRequest.Add("_catParentMessageId", TraceIdContext.Current?.ParentId ?? "");
            httpWebRequest.Add("_catChildMessageId", TraceIdContext.Current?.ChildId ?? "");
        }
    }

}
