using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;

namespace DTHStore.Models
{
    public class EmailService
    {
        private readonly string _fromEmail;
        private readonly string _fromPassword;
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;

        public EmailService()
        {
            _fromEmail = System.Configuration.ConfigurationManager.AppSettings["Email"];
            _fromPassword = System.Configuration.ConfigurationManager.AppSettings["PasswordEmail"];
        }

        public void SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                var smtpClient = new SmtpClient(_smtpServer)
                {
                    Port = _smtpPort,
                    Credentials = new NetworkCredential(_fromEmail, _fromPassword),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_fromEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception("Error sending email: " + ex.Message);
            }
        }
    }
}