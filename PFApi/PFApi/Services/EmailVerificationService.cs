using PortfolioApi.Models;
using PortfolioApi.Repositories;
using System.Security.Cryptography;

namespace PortfolioApi.Services
{
    public class EmailVerificationService
    {
        private readonly UserRepository _userRepository;
        private readonly EmailService _emailService;

        public EmailVerificationService(UserRepository userRepository, EmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }

        public async Task SendVerificationEmailAsync(User user)
        {
            var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
            

            string body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f4f4f4;'>
                    <div style='background-color: #ffffff; padding: 30px; border-radius: 10px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
                        <h2 style='color: #333333; margin-top: 0; text-align: center;'>Verification Code</h2>
                        <p style='color: #666666; font-size: 16px; line-height: 1.5;'>
                            Your verification code is:
                        </p>
                        <div style='background-color: #f8f9fa; border: 2px dashed #007bff; border-radius: 8px; padding: 20px; text-align: center; margin: 20px 0;'>
                            <span style='font-size: 32px; font-weight: bold; color: #007bff; letter-spacing: 5px;'>{code}</span>
                        </div>
                        <p style='color: #666666; font-size: 14px; line-height: 1.5;'>
                            This code will expire in <strong style='color: #dc3545;'>5 minutes</strong>.
                        </p>
                        <p style='color: #999999; font-size: 12px; margin-top: 30px; padding-top: 20px; border-top: 1px solid #e0e0e0;'>
                            If you didn't request this code, please ignore this email.
                        </p>
                    </div>
                </div>
            ";

            await _emailService.SendEmailAsync(user.Email, "Verification Code", body);

            var expiry = DateTime.UtcNow.AddMinutes(5);

            user.VerificationCode = code;
            user.VerificationCodeExpiryTime = expiry;
            await _userRepository.UpdateAsync(user);
        }
    }
}
