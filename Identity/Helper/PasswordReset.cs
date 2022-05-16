using System.Net.Mail;

namespace Identity.Helper;

public static class PasswordReset
{
    public static void PasswordResetSendEmail(string link)
    {
        MailMessage mail = new();
        SmtpClient smtpClient = new SmtpClient("smtp.mailtrap.io");

        mail.From = new MailAddress("selbilgen@gmail.com");
        mail.To.Add("selbilgen@gmail.com");

        mail.Subject = $"www.mysite.com::Şifre sıfırlama";
        mail.Body = "<h2>Şifrenizi yenilemek için lütfen aşağıdaki linke tıklayınız.</h2><hr/>";
        mail.Body += $"<a href='{link}'>şifre yenileme linki</a>";
        mail.IsBodyHtml = true;
        smtpClient.Port = 587;
        smtpClient.Credentials = new System.Net.NetworkCredential("6d4786debd1b24", "19f3164c9e9e64");

        smtpClient.Send(mail);
    }
}