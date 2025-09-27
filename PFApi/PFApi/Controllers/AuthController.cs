using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PortfolioApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PortfolioApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // 1. Validazione fake (per ora hardcoded)
            if (request.Email != "test@test.com" || request.Password != "1234")
                return Unauthorized(new { message = "Invalid credentials" });

            // 2. Recupero configurazione JWTUser
            var jwtSection = _config.GetSection("JWTUser");
            var secret = jwtSection["Secret"];
            var issuer = jwtSection["Issuer"];
            var tokenValidMinutes = int.Parse(jwtSection["TokenValidMinutes"]);

            // 3. Creazione claims
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, request.Email),
                new Claim("role", "User"), // puoi gestire ruoli
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // 4. Chiave + credenziali
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 5. Creazione token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: issuer,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(tokenValidMinutes),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // 6. Ritorno token
            return Ok(new
            {
                token = tokenString,
                expires = DateTime.UtcNow.AddMinutes(tokenValidMinutes)
            });
        }
    }
}
