namespace EmailService
{
    public interface IEmailService
    {
        bool SendEmail(string from, string to);
    }
}
