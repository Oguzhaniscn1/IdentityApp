
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;

namespace IdentityApp.Models
{

    public class SmtpEmailSender : IEmailSender
    {
        private string? _host;
        private int _port;
        private bool _enebleSSL;
        private string? _userName;
        private string? _password;
        public SmtpEmailSender(string? host,int port,bool enableSSL,string? username,string? password)
        {
            _host=host;
            _port=port;
            _enebleSSL=enableSSL;
            _userName=username;
            _password=password;
            
        }


        public Task SendEmailSender(string email, string subject, string message)
        {
            var client=new SmtpClient(_host,_port)
            {
                Credentials=new NetworkCredential(_userName,_password),
                EnableSsl=_enebleSSL
            };

            return client.SendMailAsync(new MailMessage(_userName??"",email,subject,message) {IsBodyHtml=true});



        }
    }
}