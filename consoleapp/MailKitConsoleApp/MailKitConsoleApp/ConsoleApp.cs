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
            Console.WriteLine("Send email to?");
            string to = Console.ReadLine();

            Console.WriteLine("Send email from?");
            string from = Console.ReadLine();

            Console.WriteLine("How many?");
            int numberOfEmails = int.Parse(Console.ReadLine());

            for (int i = 0; i < numberOfEmails; i++)
            {
                Console.WriteLine($"Run {i + 1} of {numberOfEmails}");
                _emailService.SendEmail(from, to);
                Console.WriteLine();
            }

            Console.WriteLine("Emails sent");
            Console.ReadLine();
        }
    }
}
