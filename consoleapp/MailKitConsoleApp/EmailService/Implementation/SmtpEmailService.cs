using System;
using System.Diagnostics;
using EmailService.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using NLog;

namespace EmailService.Implementation
{
    public class SmtpEmailService : IEmailService
    {
        private readonly SmtpOptions _smtpConfiguration;
        private readonly ILogger<IEmailService> _logger;

        private Process _proc;

        public SmtpEmailService(
            IOptions<SmtpOptions> smtpConfiguration,
            ILogger<IEmailService> logger
        )
        {
            _smtpConfiguration = smtpConfiguration.Value;
            _logger = logger;
        }
 
        public bool SendEmail(string from, string to)
        {
            _proc = Process.GetCurrentProcess();

            var message = new MimeMessage();
            
            try
            {
                message.From.Add(new MailboxAddress("From", from));
                message.To.Add(new MailboxAddress("To", to));
                message.Subject = "How you doin'?";
            
                message.Body = new TextPart("plain")
                {
                    Text = @"Hey, hello this is a test"
                };
            
                using (var client = new SmtpClient())
                {

                    ExecuteWithMemInfo("Connect", () => client.Connect(_smtpConfiguration.Host, _smtpConfiguration.Port, _smtpConfiguration.SecureSocketOptions));

                    ExecuteWithMemInfo("Authenticate", () => client.Authenticate(_smtpConfiguration.UserName, _smtpConfiguration.Password));

                    ExecuteWithMemInfo("Send", () => client.Send(message));

                    ExecuteWithMemInfo("Disconnect", () => client.Disconnect(true));
                }
            }
            catch (Exception ex)
            {
                LogWithMemUsage($"Exception throw type: {ex}, InnerException{ex.InnerException} ");
            
                return false;
            }
            finally
            {
                DisposeMimeMessage(message);
            
                _proc.Dispose();
            }

            return true;
        }
        
        private static string Pretty(long size)
        {
            string[] sizes = { "B", "KB", "MB" };
            double len = size;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return string.Format("{0:0.0} {1}", len, sizes[order]);
        }

        private void LogWithMemUsage(string msg)
        {
            
            long mem = GC.GetTotalMemory(false);
            _proc.Refresh();
            long privateMemory = _proc.PrivateMemorySize64;
            long workingSet = _proc.WorkingSet64;
            _logger.LogInformation($"Message: {msg}\n\tMemory: {Pretty(mem)}\n\tPrivate Memory: {Pretty(privateMemory)}\n\tWorking Set: {Pretty(workingSet)}");
            LogManager.Flush();
        }

        private void ExecuteWithMemInfo(string methodName, Action action)
        {
            _proc.Refresh();
            long workingSetPre = _proc.WorkingSet64;

            action();

            _proc.Refresh();
            long workingSetPost = _proc.WorkingSet64;

            Console.WriteLine($"Called {methodName} Workingset: {Pretty(workingSetPost)} Increase: {Pretty(workingSetPost - workingSetPre)}");
        }

        private static void DisposeMimeMessage(MimeMessage message)
        {
            foreach (var bodyPart in message.BodyParts)
            {
                if (bodyPart is MessagePart rfc822)
                {
                    DisposeMimeMessage(rfc822.Message);
                }
                else
                {
                    var part = (MimePart)bodyPart;

                    part.Content.Stream.Dispose();
                }
            }
        }
    }
}
