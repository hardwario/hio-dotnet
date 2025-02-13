using hio_dotnet.HWDrivers.Server;
using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;

namespace hio_dotnet.Demos.WSSessionsServer
{
    public static class MainDataContext
    {
        public static string Login { get; set; } = "admin";
        public static string Password { get; set; } = "12345678";

        public static ConcurrentDictionary<string, JWTAuthorizedData> ActiveTokens = new();

        public static ConcurrentDictionary<string, WSSession> Sessions { get; set; } = new ConcurrentDictionary<string, WSSession>();
        public static ConcurrentDictionary<string, WSSessionMessage> ProcessedMessage { get; set; } = new ConcurrentDictionary<string, WSSessionMessage>();

        public static JWTAuthorizedData? GetTokenByToken(string token)
        {
            return ActiveTokens.Values.FirstOrDefault(t => t.Token == token && !t.IsExpired);
        }

        public static void AddToken(string username, string token, DateTime expiresAt)
        {
            var jwtData = new JWTAuthorizedData
            {
                Token = token,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt
            };
            ActiveTokens[username] = jwtData;

            CleanupExpiredTokens();
        }

        public static JWTAuthorizedData? GetToken(string username)
        {
            if (ActiveTokens.TryGetValue(username, out var jwtData) && !jwtData.IsExpired)
            {
                return jwtData;
            }
            return null;
        }

        public static void RemoveToken(string username)
        {
            ActiveTokens.TryRemove(username, out _);
        }

        public static void CleanupExpiredTokens()
        {
            foreach (var key in ActiveTokens.Keys)
            {
                if (ActiveTokens[key].IsExpired)
                {
                    ActiveTokens.TryRemove(key, out _);
                }
            }
        }

        public static bool AddSession(WSSession session)
        {
            if (Sessions.ContainsKey(session.Id.ToString()))
            {
                return false;
            }
            if (Sessions.TryAdd(session.Id.ToString(), session))
            {
                CleanupExpiredSessions();
                return true;
            }
            return false;
        }

        public static bool RemoveSession(string sessionId)
        {
            return Sessions.TryRemove(sessionId, out _);
        }

        public static WSSession? GetSession(string sessionId)
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                return session;
            }
            return null;
        }

        public static void CleanupExpiredSessions()
        {
            foreach (var key in Sessions.Keys)
            {
                if (Sessions[key].IsExpired)
                {
                    Sessions.TryRemove(key, out _);
                }
            }
        }
    }
}
