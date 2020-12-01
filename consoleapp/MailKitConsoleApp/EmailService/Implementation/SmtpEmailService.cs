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

            LogWithMemUsage("SendEmail called");
      
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
            
                LogWithMemUsage("Sending Email");
                using (var client = new SmtpClient())
                {
                    LogWithMemUsage("Created SmtpClient");
                    LogWithMemUsage($"Call client.Connect with: " +
                        $"Host: {_smtpConfiguration.Host}, " +
                        $"Port: {_smtpConfiguration.Port}, " +
                        $"SecureSocketOptions: { Enum.GetName(typeof(SecureSocketOptions), _smtpConfiguration.SecureSocketOptions)}");
            
                    client.Connect(_smtpConfiguration.Host, _smtpConfiguration.Port, _smtpConfiguration.SecureSocketOptions);
            
                    LogWithMemUsage("Call client.Authenticate");
                    client.Authenticate(_smtpConfiguration.UserName, _smtpConfiguration.Password);
            
                    LogWithMemUsage("Call client.Send");
                    client.Send(message);
            
                    LogWithMemUsage("Call client.Disconnect");
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                LogWithMemUsage($"Exception throw type: {ex}, InnerException{ex.InnerException} ");
            
                return false;
            }
            finally
            {
                LogWithMemUsage("Calling DisposeMimeMessage");
                DisposeMimeMessage(message);
                LogWithMemUsage("Called DisposeMimeMessage");
            
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
