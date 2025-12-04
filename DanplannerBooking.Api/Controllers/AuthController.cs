using DanplannerBooking.Application.Dtos.User;  // DTO’er til login/refresh svar og request
using DanplannerBooking.Application.Interfaces; // Interface til UserRepository
using DanplannerBooking.Domain.Entities;        // Domain User-entity
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;           // Bruges til at signere JWT tokens
using System.IdentityModel.Tokens.Jwt;         // JWT token generator
using System.Security.Claims;                  // Claims (navn, email, rolle osv.)
using System.Security.Cryptography;            // Bruges til at generere secure refresh tokens
using System.Text;
using DanplannerBooking.Domain.Security;

namespace DanplannerBooking.Api.Controllers
{
    [ApiController]                             // Gør at model binding, validation osv. fungerer automatisk
    [Route("api/[controller]")]                 // Endpoint = /api/auth
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
            // Henter alle brugere (I et rigtigt system ville dette være et "GetByEmail")
            var users = await _userRepository.GetAllAsync();
            var user = users.FirstOrDefault(u => u.Email == request.Email);

            if (user == null)
                return Unauthorized("Invalid email or password");

            // Tjek password – understøtter både gamle klartekst og nye hashes
            var passwordOk = PasswordHasher.VerifyPassword(request.Password, user.Password);

            if (!passwordOk)
                return Unauthorized("Invalid email or password");

            // Genererer access token (JWT)
            var jwt = GenerateJwtToken(user);

            // Genererer et secure refresh token
            var refresh = GenerateRefreshToken();

            // Gemmer refresh token på brugeren
            user.RefreshToken = refresh.Token;
            user.RefreshTokenExpiryTime = refresh.Expires;

            await _userRepository.UpdateAsync(user.Id, user);

            // Returnerer både JWT + refresh token
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
            // Udtrækker ClaimsPrincipal selvom tokenen er udløbet
            var principal = GetPrincipalFromExpiredToken(request.Token);

            // Henter userId claimet fra token
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
            if (user == null)
                return Unauthorized();

            // Tjekker om refresh token matcher det som ligger i databasen
            // OG at refresh token stadig er gyldig
            if (user.RefreshToken != request.RefreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Unauthorized("Invalid refresh token");
            }

            // Genererer nyt access token + nyt refresh token
            var jwt = GenerateJwtToken(user);
            var refresh = GenerateRefreshToken();

            // Opdaterer refresh token
            user.RefreshToken = refresh.Token;
            user.RefreshTokenExpiryTime = refresh.Expires;

            await _userRepository.UpdateAsync(user.Id, user);

            // Returnerer nye tokens
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
            // Henter værdier fra appsettings.json
            var jwtSection = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]));

            // Signeringsmetode til token
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Token udløber efter X minutter
            var expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSection["ExpireMinutes"]));

            // Claims der kommer med i tokenet (identitet + autorisation)
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role) 
            };

            // Selve JWT token objektet
            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            // Returnerer token string + expiration
            return (new JwtSecurityTokenHandler().WriteToken(token), expires);
        }

        // -------------------------
        // Refresh Token Creation
        // -------------------------
        // Genererer et krypografisk secure refresh token (32 random bytes)
        private (string Token, DateTime Expires) GenerateRefreshToken()
        {
            var bytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);

            return (Convert.ToBase64String(bytes), DateTime.UtcNow.AddHours(10));
        }

        // Udtrækker claims fra en EXPIRED token (bruges i refresh-flow)
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var jwtSection = _config.GetSection("Jwt");

            // Validerer token men IGNORERER lifetime (den må godt være udløbet)
            var validationParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSection["Issuer"],

                ValidateAudience = true,
                ValidAudience = jwtSection["Audience"],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"])),

                ValidateLifetime = false // Denne er vigtig! Ellers kan vi ikke læse expired tokens
            };

            return new JwtSecurityTokenHandler()
                .ValidateToken(token, validationParams, out _);
        }
    }
}
