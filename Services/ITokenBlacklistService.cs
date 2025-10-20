namespace SafeScribe.Services
{
    public interface ITokenBlacklistService
    {
        System.Threading.Tasks.Task AddToBlacklistAsync(string jti, System.DateTime expiry);
        System.Threading.Tasks.Task<bool> IsBlacklistedAsync(string jti);
    }
}
