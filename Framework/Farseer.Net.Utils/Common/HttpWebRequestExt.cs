using System.Net;
using FS.Context;

namespace FS.Utils.Common
{
    public static class HttpWebRequestCatExt
    {
        public static void AddHeaderCatInfo(this HttpWebRequest httpWebRequest)
        {
            if (TraceIdContext.Current == null) { return; }
            httpWebRequest.Headers.Add("_catRootMessageId", TraceIdContext.Current.RootId ?? "");
            httpWebRequest.Headers.Add("_catParentMessageId", TraceIdContext.Current.ParentId ?? "");
            httpWebRequest.Headers.Add("_catChildMessageId", TraceIdContext.Current.ChildId ?? "");
        }
    }

}
