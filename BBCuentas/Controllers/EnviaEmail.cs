using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;

namespace BBCuentas.Controllers
{
    public class EnviaEmail
    {
        private string correo = WebConfigurationManager.AppSettings["Correo"].ToString();
        private string smtp = WebConfigurationManager.AppSettings["HostSMTP"].ToString();
        private int port = Convert.ToInt32(WebConfigurationManager.AppSettings["PtoSMTP"].ToString());
        private string MailValPassword = WebConfigurationManager.AppSettings["MailValPassword"].ToString();

        public bool EnviaEmailC(string to,string ruta)
        {
            bool response;

            SmtpClient SmtpServer = new SmtpClient(smtp);
            var mail = new MailMessage();
            mail.From = new MailAddress(correo);
            mail.To.Add(to);
            mail.Subject = "Correo confirmación Conauto";
            mail.IsBodyHtml = true;
            mail.Attachments.Add(new Attachment(ruta));
            string htmlBody;
            htmlBody = "Estado de Cuenta";
            mail.Body = htmlBody;
            SmtpServer.Port = port;
            SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Credentials = new System.Net.NetworkCredential(correo, MailValPassword);
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);
            response = true;

            return response;
        }

        

        public bool SendEmail(string to, string Body, string subject)
        {
            bool response = true;
             SmtpClient SmtpServer = new SmtpClient(smtp);
            var mail = new MailMessage();
            mail.From = new MailAddress(correo);
            mail.To.Add(to);
            mail.Subject = subject;
            mail.IsBodyHtml = true;
            mail.Body = Body;
            SmtpServer.Port = port;
            SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Credentials = new System.Net.NetworkCredential(correo, MailValPassword);
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);
            return response;
        }
    }
}