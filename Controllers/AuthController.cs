using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using SafeScribe.DTOs;
using SafeScribe.Services;

namespace SafeScribe.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly ITokenBlacklistService _blacklist;

        public AuthController(ITokenService tokenService, ITokenBlacklistService blacklist)
        {
            _tokenService = tokenService;
            _blacklist = blacklist;
        }

        [HttpPost("registrar")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            try
            {
                var user = await _tokenService.RegisterAsync(dto);
                return CreatedAtAction(null, new { user.Id, user.Username, user.Role });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var token = await _tokenService.LoginAndGenerateTokenAsync(dto);
            if (token == null) return Unauthorized(new { message = "Credenciais inválidas." });
            return Ok(new { token });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var jti = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            if (string.IsNullOrEmpty(jti)) return BadRequest(new { message = "Token não contém jti." });

            var expClaim = User.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;
            DateTime expiry = DateTime.UtcNow.AddMinutes(60);
            if (long.TryParse(expClaim, out var expSeconds))
            {
                expiry = DateTimeOffset.FromUnixTimeSeconds(expSeconds).UtcDateTime;
            }

            await _blacklist.AddToBlacklistAsync(jti, expiry);
            return Ok(new { message = "Logout efetuado. Token invalidado." });
        }
    }
}
