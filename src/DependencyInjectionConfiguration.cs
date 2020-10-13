using System;
using DotNetCliTemplate.Options;
using DotNetCliTemplate.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetCliTemplate
{
    public static class DependencyInjectionConfiguration
    {
        public static IServiceProvider Configure(IConfiguration configuration)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddOptions();
            serviceCollection.Configure<GreetingEngineOptions>(
                configuration.GetSection(GreetingEngineOptions.SectionName));
            serviceCollection.Configure<PrometheusOptions>(configuration.GetSection(PrometheusOptions.SectionName));
            serviceCollection.AddSingleton<GreetingEngine>();

            return serviceCollection.BuildServiceProvider();
        }
    }
}