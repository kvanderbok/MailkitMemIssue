using System;
using EmailService;

namespace MailKitConsoleApp
{
    public class ConsoleApp
    {
        private readonly IEmailService _emailService;

        public ConsoleApp(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public void Run()
        {
            Console.WriteLine("Send email to");
            string to = Console.ReadLine();

            Console.WriteLine("Send email from");
            string from = Console.ReadLine();

            string send;

            do
            {
                Console.WriteLine("SendEmail? Y / Q to quit");
                send = Console.ReadLine();
                if (send.ToLower() == "y")
                {
                    _emailService.SendEmail(from, to);
                }
            }
            while (send.ToLower() != "q");
        }
    }
}
