using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DotNetCliTemplate.Options;
using DotNetCliTemplate.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Prometheus;
using Serilog;

namespace DotNetCliTemplate
{
    public static class Program
    {
        private static readonly Dictionary<string, string> DefaultConfiguration = new Dictionary<string, string>
        {
            {$"{PrometheusOptions.SectionName}:{nameof(PrometheusOptions.Port)}", "9090"},
            {$"{GreetingEngineOptions.SectionName}:{nameof(GreetingEngineOptions.Message)}", "Hello World!"},
            {$"{GreetingEngineOptions.SectionName}:{nameof(GreetingEngineOptions.MessageCount)}", "100"}
        };

        private static IConfiguration Configuration { get; set; }

        public static async Task<int> Main(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder
                .AddInMemoryCollection(DefaultConfiguration)
                .AddJsonFile("appsettings.json", false, false)
                .AddEnvironmentVariables();

            Configuration = configurationBuilder.Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .WriteTo.Console() // Always write to console.
                .CreateLogger();

            // Support CTRL-C
            using var cancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, arguments) =>
            {
                arguments.Cancel = true;

                // ReSharper disable once AccessToDisposedClosure | endless loop follows.
                cancellationTokenSource.Cancel();
            };

            // Configure Simple DI (binds options and turns them injectable).
            var serviceProvider = DependencyInjectionConfiguration.Configure(Configuration);

            try
            {
                var prometheusOptions = serviceProvider.GetRequiredService<IOptions<PrometheusOptions>>();

                // Prometheus Scrap API.
                var server = new MetricServer("localhost", prometheusOptions.Value.Port);
                server.Start();

                var engine = serviceProvider.GetService<GreetingEngine>();
                await engine.Run(cancellationTokenSource.Token);
                return 0;
            }
            catch (TaskCanceledException)
            {
                Log.Warning("Cancelled by user.");
                return 1;
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Fatal error.");
                return 2;
            }
        }
    }
}