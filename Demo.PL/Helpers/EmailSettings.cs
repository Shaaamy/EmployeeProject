using Demo.DAL.Models;
using System.Net;
using System.Net.Mail;

namespace Demo.PL.Helpers
{
	public static class EmailSettings
	{
		public static void SendEmail(Email email)
		{
			var Client = new SmtpClient("smtp.gmail.com", 587);
			Client.EnableSsl = true;
			Client.Credentials = new NetworkCredential("amrshamy91@gmail.com", "qauzwupgxurndmjy");
			Client.Send("amrshamy91@gmail.com",email.To,email.Subject,email.Body);

		}
	}
}
