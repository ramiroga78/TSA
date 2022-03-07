using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System;
using System.Threading.Tasks;

namespace TSA.Utilities
{
    public class Email
    {
        public class ServerSettings
        {
            public string Host { get; set; }
            public int Port { get; set; }
            public string AuthenticationEmail { get; set; }
            public string AuthenticationPassword { get; set; }
        }
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public ServerSettings Settings { get; set; }

        public async Task<Response> SendEmailAsync(Email email)
        {
            Response response = new Response();

            try
            {
                var mail = new MimeMessage();

                mail.Sender = MailboxAddress.Parse(email.From);
                mail.To.Add(MailboxAddress.Parse(email.To));
                mail.Subject = email.Subject;

                var builder = new BodyBuilder();

                builder.HtmlBody = email.Body;

                mail.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();

                smtp.Connect(email.Settings.Host, email.Settings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(email.Settings.AuthenticationEmail, email.Settings.AuthenticationPassword);

                await smtp.SendAsync(mail);

                smtp.Disconnect(true);

                response.Result = "OK";
                response.Message = "Email " + email.Subject + " de " + email.From + " para " + email.To + " enviado correctamente el " + DateTime.Now;
            }
            catch (System.Exception ex)
            {
                response.Result = "ERROR";
                response.Message = ex.Message;
            }

            return response;
        }
    }
}