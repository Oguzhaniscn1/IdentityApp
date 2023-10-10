namespace IdentityApp.Models
{
    public interface IEmailSender
    {
        Task SendEmailSender(string email,string subject,string message);
    }
}