using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PointOfSale.Utility
{
    public class EmailService
    {
        
        public static void SendEmail(string subject,string to, string message)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("faisalshahab1192@gmail.com"));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = message };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("ydbnwztkmapd57ti@ethereal.email", "fSQ5SxZqhRfK86P3cS");
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
