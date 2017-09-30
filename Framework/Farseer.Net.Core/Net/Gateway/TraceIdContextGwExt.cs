using Farseer.Net.Context;

namespace Farseer.Net.Core.Net.Gateway
{
    public static class TraceIdContextGwExt
    {
        public static void InitTraceIdContext(this GatewayHeaderVO gatewayHeader)
        {
            if (TraceIdContext.Current == null)
            {
                TraceIdContext.Current = new TraceIdContext(gatewayHeader.RootId, gatewayHeader.RootId,
                    gatewayHeader.ParentId, gatewayHeader.ChildId);
            }
        }
    }
}
