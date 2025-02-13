namespace hio_dotnet.Demos.WSSessionsServer
{
    public class JWTAuthorizedData
    {
        public string Token { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    }
}
