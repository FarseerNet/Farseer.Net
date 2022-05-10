using System.Net;
using System.Net.Http.Headers;
using FS.Core.LinkTrack;

namespace FS.Core.Http
{
    public static class HttpWebRequestCatExt
    {
        public static void AddTraceInfoToHeader(this HttpWebRequest httpWebRequest)
        {
            var linkTrackContext = FsLinkTrack.Current.Get();
            if (linkTrackContext == null) return;
            
            httpWebRequest.Headers.Add(name: "FsContextId", value: linkTrackContext.ContextId);
            httpWebRequest.Headers.Add(name: "FsAppName", value: linkTrackContext.AppName);
        }

        public static void AddTraceInfoToHeader(this HttpContentHeaders httpWebRequest)
        {
            var linkTrackContext = FsLinkTrack.Current.Get();
            if (linkTrackContext == null) return;
            
            httpWebRequest.Add(name: "FsContextId", value: linkTrackContext.ContextId);
            httpWebRequest.Add(name: "FsAppName", value: linkTrackContext.AppName);
        }
    }
}