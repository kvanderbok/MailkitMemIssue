using MailKit.Security;

namespace EmailService.Options
{
    public class SmtpOptions
    {
        public const string SECTION = "Smtp";
        /// <summary>
        /// The SMTP server's host name
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// The SMTP server's port number 
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The user name to authenticate to the SMTP server
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The password to authenticate to the SMTP server
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The connection configuration (i.e. SSL, TLS etc)
        /// </summary>
        public SecureSocketOptions SecureSocketOptions { get; set; }
    }
}
