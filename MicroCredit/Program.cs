using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace MicroCredit
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            var host = CreateHostBuilder(args).Build();
            var lifetime = host.Services
            .GetRequiredService<IHostApplicationLifetime>();
            lifetime.ApplicationStopping.Register(OnShutdown);
            host.RunAsync(cts.Token)
            .GetAwaiter().GetResult();
        }

        private static void OnShutdown()
        {
            Console.WriteLine("Application is shutting down...");
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Information);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureKestrel((context, serverOptions) =>
                    {
                        serverOptions.Configure(context.Configuration.GetSection("Kestrel"));
                    });
                    webBuilder.UseUrls("http://localhost:5000", "https://localhost:5001");
                });
    }
}