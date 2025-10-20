using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SafeScribe.Data;
using SafeScribe.DTOs;
using SafeScribe.Models;

namespace SafeScribe.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _db;
        private readonly TimeSpan _expiry;

        public TokenService(IConfiguration config, ApplicationDbContext db)
        {
            _config = config;
            _db = db;
            var minutes = _config.GetValue<int>("Jwt:ExpiresMinutes");
            _expiry = TimeSpan.FromMinutes(minutes > 0 ? minutes : 60);
        }

        public async System.Threading.Tasks.Task<User> RegisterAsync(UserRegisterDto dto)
        {
            var exists = await _db.Users.AnyAsync(u => u.Username == dto.Username);
            if (exists) throw new InvalidOperationException("Username já existe.");

            // Hash com BCrypt (gera salt automaticamente)
            var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = hash,
                Role = dto.Role
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async System.Threading.Tasks.Task<string?> LoginAndGenerateTokenAsync(LoginRequestDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null) return null;

            // Verifica hash
            var valid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!valid) return null;

            // Claims essenciais: sub (UserId), role, jti (unique id)
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, System.Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.Add(_expiry);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
