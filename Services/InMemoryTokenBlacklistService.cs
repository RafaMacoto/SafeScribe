using System.Collections.Concurrent;

namespace SafeScribe.Services
{
    public class InMemoryTokenBlacklistService : ITokenBlacklistService
    {
        // jti -> expiry
        private readonly ConcurrentDictionary<string, DateTime> _blacklist = new();

        public System.Threading.Tasks.Task AddToBlacklistAsync(string jti, DateTime expiry)
        {
            _blacklist[jti] = expiry;
            return System.Threading.Tasks.Task.CompletedTask;
        }

        public System.Threading.Tasks.Task<bool> IsBlacklistedAsync(string jti)
        {
            if (_blacklist.TryGetValue(jti, out var expiry))
            {
                if (expiry < DateTime.UtcNow)
                {
                    _blacklist.TryRemove(jti, out _);
                    return System.Threading.Tasks.Task.FromResult(false);
                }
                return System.Threading.Tasks.Task.FromResult(true);
            }
            return System.Threading.Tasks.Task.FromResult(false);
        }
    }
}
