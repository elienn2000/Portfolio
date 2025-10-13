using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            var host = _config["Smtp:Host"];
            var port = int.Parse(_config["Smtp:Port"] ?? "587");
            var username = _config["Smtp:User"];
            var password = _config["Smtp:Password"];

            var message = new MailMessage();
            message.From = new MailAddress(username, "Portfolio");
            message.To.Add(new MailAddress(to));
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;


            using (var client = new SmtpClient(host, port))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(username, password);
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                await client.SendMailAsync(message);
            }
        }
        catch
        {
            // Implement SIGNAL R?
        }
    }
}
