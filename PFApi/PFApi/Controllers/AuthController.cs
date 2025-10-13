using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PortfolioApi.Models;
using PortfolioApi.Repositories;
using PortfolioApi.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PortfolioApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UserRepository _userRepository;
        private readonly EmailVerificationService _verificationService;
        public AuthController(IConfiguration config, UserRepository userRepository, EmailVerificationService verificationService)
        {
            _config = config;
            _userRepository = userRepository;
            _verificationService = verificationService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequest request)
        {
            try
            {

                // Get the user entity using the login request
                var user = await _userRepository.GetByLoginRequestAsync(request);

                // Check if the user exists
                if (user == null || !PasswordHelper.VerifyPassword(request.Password, user.PasswordHash))
                {
                    return Unauthorized(new { message = "Invalid credentials" });
                }

                // Checking if user is active
                if (user.Active != 1)
                {
                    var result = await _verificationService.SendVerificationEmailAsync(user);

                    return Ok(new
                    {
                        status = "email_verification_required",
                        message = "Verification email sent",
                        email = result.Email,
                        expires = result.ExpiryTime
                    });
                }

                // Get JWT settings from configuration
                var jwtSection = _config.GetSection("JWTUser");
                var secret = jwtSection["Secret"];
                var issuer = jwtSection["Issuer"];
                var tokenValidMinutes = int.Parse(jwtSection["TokenValidMinutes"]);
                var refreshTokenValidMinutes = int.Parse(jwtSection["RefreshTokenValidMinutes"]);

                // Defining claims
                var claims = new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, request.Email),
                new Claim("role", "User"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

                // Keys and credentials
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                // Token generation
                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: issuer,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(tokenValidMinutes),
                    signingCredentials: creds
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                // Refresh Token generation
                var randomBytes = new byte[64];
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(randomBytes);
                var refreshToken = Convert.ToBase64String(randomBytes);

                // Save tokens in the database
                await _userRepository.SaveTokensAsync(
                    user,
                    tokenString,
                    DateTime.UtcNow.AddMinutes(tokenValidMinutes),
                    refreshToken,
                    DateTime.UtcNow.AddMinutes(refreshTokenValidMinutes)
                );

                // Returning user data with tokens
                return Ok(new
                {
                    userId = user.Id,
                    email = user.Email,
                    username = user.Username,
                    token = tokenString,
                    refreshToken = refreshToken,
                    expires = DateTime.UtcNow.AddMinutes(tokenValidMinutes)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
            }
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthRequest request)
        {

            // Get the user entity using the login request
            var sameEmailUser = await _userRepository.GetByEmailAsync(request.Email);
            var sameUsernameUser = await _userRepository.GetByUsernameAsync(request.UserName);


            if (sameEmailUser != null && sameUsernameUser != null)
            {
                return Conflict(new { message = "Email and Username already in use" });
            }

            if (sameEmailUser != null)
            {
                return Conflict(new { message = "Email already in use" });
            }

            if (sameUsernameUser != null)
            {
                return Conflict(new { message = "Username already in use" });
            }

            User newUser = new User
            {
                Email = request.Email,
                Username = request.UserName
            };

            // Hash the password
            var userId = await _userRepository.InsertUserAsync(request);

            if (userId == null)
            {
                return StatusCode(500, new { message = "Error creating user" });
            }

            newUser.Id = (long)userId;

            // Servizio per invio email di verifica

            return Ok(newUser);
        }


        [HttpPost("sendVerificationEmail")]
        public async Task<IActionResult> sendVerificationEmail([FromBody] User user)
        {

            if (string.IsNullOrWhiteSpace(user.Email))
                return BadRequest("Email non valida.");

            await _verificationService.SendVerificationEmailAsync(user);

            return Ok();

        }

    }
}
