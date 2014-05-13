using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace SocialApp.Models
{
    public class EmailSender : IEmailSender
    {
        private const string Server = "musicnet555@gmail.com";

        public void SendActivationCode(string email, string activationCode)
        {
            MailMessage mail = new MailMessage();
            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress(Server);
            mail.To.Add(email);
            mail.Subject = "Activation code for Collaborative music app";
            string activationLink = string.Format("{0}://{1}/Account/Activate?code={2}",
                HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority, activationCode);
            mail.Body = string.Format("Follow this link to activate your account: {0}", activationLink);
            smtpServer.Port = 587;
            smtpServer.Credentials = new NetworkCredential(Server, "sunnyshores");
            smtpServer.EnableSsl = true;
            smtpServer.Send(mail);
        }
    }
}