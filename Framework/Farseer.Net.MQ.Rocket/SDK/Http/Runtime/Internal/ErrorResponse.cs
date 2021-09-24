namespace FS.MQ.Rocket.SDK.Http.Runtime.Internal
{
    public class ErrorResponse
    {
        public string Code { get; set; }

        public string Message { get; set; }

        public string RequestId { get; set; }

        public string HostId { get; set; }
    }
}