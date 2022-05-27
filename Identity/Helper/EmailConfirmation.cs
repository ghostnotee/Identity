using System.Net.Mail;

namespace Identity.Helper;

public static class EmailConfirmation
{
    public static void SendEmail(string link, string email)
    {
        MailMessage mail = new();
        SmtpClient smtpClient = new SmtpClient("smtp.mailtrap.io");

        mail.From = new MailAddress("selbilgen@gmail.com");
        mail.To.Add(email);

        mail.Subject = $"www.mysite.com. Email doğrulama";
        mail.Body = "<h2>Email doğrulama için lütfen aşağıdaki linke tıklayınız.</h2><hr/>";
        mail.Body += $"<a href='{link}'>Email doğrulama linki</a>";
        mail.IsBodyHtml = true;
        smtpClient.Port = 587;
        smtpClient.Credentials = new System.Net.NetworkCredential("6d4786debd1b24", "19f3164c9e9e64");

        smtpClient.Send(mail);
    }
}