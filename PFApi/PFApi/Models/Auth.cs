using System.Globalization;

namespace PortfolioApi.Models
{

    public class EmailVerificationRequest
    {
        public string Email { get; set; } = "";
        public string VerificationCode { get; set; } = "";

        public DateTime ExpiryTime { get; set; } = DateTime.Now;
    }

    public enum VerificationStatus
    {
        UserNotFound,
        CodeExpired,
        CodeInvalid,
        Success
    }

    public class AuthRequest
    {
        public string Email { get; set; } = "";

        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";

    }

    public class MailVerifyRequest
    {
        public string Email { get; set; } = "";

        public DateTime ExpiryTime { get; set; }
    }
}