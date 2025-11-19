using DanplannerBooking.Application.Dtos.User;  // 👈 NYT NAMESPACE
using DanplannerBooking.Application.Interfaces;
using DanplannerBooking.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DanplannerBooking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;

        public AuthController(IUserRepository userRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            _config = config;
        }

        // -------------------------
        //          LOGIN
        // -------------------------
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            var users = await _userRepository.GetAllAsync();
            var user = users.FirstOrDefault(u => u.Email == request.Email);

            if (user == null || user.Password != request.Password)
                return Unauthorized("Invalid email or password");

            var jwt = GenerateJwtToken(user);
            var refresh = GenerateRefreshToken();

            user.RefreshToken = refresh.Token;
            user.RefreshTokenExpiryTime = refresh.Expires;

            await _userRepository.UpdateAsync(user.Id, user);

            return Ok(new LoginResponseDto
            {
                Token = jwt.Token,
                ExpiresAt = jwt.Expires,
                RefreshToken = refresh.Token,
                RefreshTokenExpiresAt = refresh.Expires,
                Name = user.Name,
                Role = user.Role
            });
        }

        // -------------------------
        //      REFRESH TOKEN
        // -------------------------
        [HttpPost("refresh")]
        public async Task<ActionResult<LoginResponseDto>> Refresh([FromBody] RefreshTokenRequestDto request)
        {
            var principal = GetPrincipalFromExpiredToken(request.Token);
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
            if (user == null)
                return Unauthorized();

            if (user.RefreshToken != request.RefreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Unauthorized("Invalid refresh token");
            }

            var jwt = GenerateJwtToken(user);
            var refresh = GenerateRefreshToken();

            user.RefreshToken = refresh.Token;
            user.RefreshTokenExpiryTime = refresh.Expires;

            await _userRepository.UpdateAsync(user.Id, user);

            return Ok(new LoginResponseDto
            {
                Token = jwt.Token,
                ExpiresAt = jwt.Expires,
                RefreshToken = refresh.Token,
                RefreshTokenExpiresAt = refresh.Expires,
                Name = user.Name,
                Role = user.Role
            });
        }

        // -------------------------
        // TOKEN CREATION LOGIC
        // -------------------------
        private (string Token, DateTime Expires) GenerateJwtToken(User user)
        {
            var jwtSection = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSection["ExpireMinutes"]));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), expires);
        }

        private (string Token, DateTime Expires) GenerateRefreshToken()
        {
            var bytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);

            return (Convert.ToBase64String(bytes), DateTime.UtcNow.AddDays(7));
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var jwtSection = _config.GetSection("Jwt");

            var validationParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSection["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSection["Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"])),
                ValidateLifetime = false
            };

            return new JwtSecurityTokenHandler()
                .ValidateToken(token, validationParams, out _);
        }
    }
}
