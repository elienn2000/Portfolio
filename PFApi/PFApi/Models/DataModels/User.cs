using Dapper.Contrib.Extensions;

namespace PortfolioApi.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public long Id { get; set; }

        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        // Access token
        public string? AccessToken { get; set; }
        public DateTime? AccessTokenExpiryTime { get; set; }

        // Refresh token
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }

}
