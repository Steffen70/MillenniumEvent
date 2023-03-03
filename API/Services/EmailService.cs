using System;
using API.Helpers;
using API.Logic;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace API.Services
{
    public class EmailService
    {
        private readonly EmailConfiguration _emailConfig;
        public EmailService(IOptions<ApiSettings> apiSettings)
        {
            _emailConfig = apiSettings.Value.EmailConfiguration;
        }
        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.Username, _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = message.Content;

            return emailMessage;
        }

        private void Send(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfig.Username, _emailConfig.Password);

                client.Send(mailMessage);
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
    }
}
