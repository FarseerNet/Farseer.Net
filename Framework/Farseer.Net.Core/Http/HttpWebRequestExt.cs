using System.Net;
using System.Net.Http.Headers;
using FS.Core.LinkTrack;

namespace FS.Core.Http
{
    public static class HttpWebRequestCatExt
    {
        public static void AddTraceInfoToHeader(this HttpWebRequest httpWebRequest)
        {
            httpWebRequest.Headers.Add(name: "FsContextId", value: FsLinkTrack.Current.Get().ContextId);
            httpWebRequest.Headers.Add(name: "FsAppId", value: FsLinkTrack.Current.Get().AppId);
        }

        public static void AddTraceInfoToHeader(this HttpContentHeaders httpWebRequest)
        {
            httpWebRequest.Add(name: "FsContextId", value: FsLinkTrack.Current.Get().ContextId);
            httpWebRequest.Add(name: "FsAppId", value: FsLinkTrack.Current.Get().AppId);
        }
    }
}