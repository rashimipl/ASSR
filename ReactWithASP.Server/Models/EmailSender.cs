using Microsoft.Extensions.Configuration;
using ReactWithASP.Server;
using ReactWithASP.Server.Models;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var smtpSettings = _configuration.GetSection("Smtp").Get<SmtpSettings>();

        var client = new SmtpClient(smtpSettings.Host, int.Parse(smtpSettings.Port))
        {
            Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password),
            EnableSsl = bool.Parse(smtpSettings.EnableSsl)
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(smtpSettings.FromEmail),
            Subject = subject,
            Body = message,
            IsBodyHtml = true
        };

        mailMessage.To.Add(email);

        try
        {
            await client.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {

            throw;
        }
    }
}

