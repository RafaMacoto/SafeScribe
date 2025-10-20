using SafeScribe.Models;
using SafeScribe.DTOs;

namespace SafeScribe.Services
{
    public interface ITokenService
    {
        System.Threading.Tasks.Task<User> RegisterAsync(UserRegisterDto dto);
        System.Threading.Tasks.Task<string?> LoginAndGenerateTokenAsync(LoginRequestDto dto);
    }
}
