namespace FS.Core.Jwt
{
    public class TokenInfo
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public string Token      { get; set; }
        public int    StatusCode { get; set; }
    }
}