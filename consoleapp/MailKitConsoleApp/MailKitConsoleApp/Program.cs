using System;
using EmailService;
using EmailService.Implementation;
using EmailService.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MailKitConsoleApp
{
    class Program
    {
        private static ServiceProvider _serviceProvider;

        static void Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .AddEnvironmentVariables()
                .Build();

            RegisterServices(configuration);

            IServiceScope scope = _serviceProvider.CreateScope();
            scope.ServiceProvider.GetRequiredService<ConsoleApp>().Run();

            DisposeServices();
        }

        private static void RegisterServices(IConfiguration configuration)
        {
            var services = new ServiceCollection();

            services.AddLogging(conf => conf.AddConsole());

            services.AddSingleton<IEmailService, SmtpEmailService>();
            services.AddSingleton<ConsoleApp>();

            services.Configure<SmtpOptions>(opt =>
            {
                configuration.GetSection("AppSettings").GetSection(SmtpOptions.SECTION).Bind(opt);
            });

            _serviceProvider = services.BuildServiceProvider(true);
        }

        private static void DisposeServices()
        {
            if (_serviceProvider == null)
            {
                return;
            }
            if (_serviceProvider is IDisposable)
            {
                ((IDisposable)_serviceProvider).Dispose();
            }
        }
    }
}
