using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;

namespace MicroCredit
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var cancellationTokenSource = new
            CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                cancellationTokenSource.Cancel();
            };

            var host = CreateHostBuilder(args).Build();
            var lifetime = host.Services
            .GetRequiredService<IHostApplicationLifetime>();
            lifetime.ApplicationStopping.Register(OnShutdown);
            host.RunAsync(cancellationTokenSource.Token)
            .GetAwaiter().GetResult();
        }

        private static void OnShutdown()
        {
            Console.WriteLine("Application is shutting down...");
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls(
                        "http://localhost:5000",
                     "https://localhost:5001");
                });
    }
}
