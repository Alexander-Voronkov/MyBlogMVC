namespace MyBlog.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string from, string to, string subject, string html);
    }
}
