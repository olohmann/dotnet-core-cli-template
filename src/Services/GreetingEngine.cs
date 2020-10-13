using System;
using System.Threading;
using System.Threading.Tasks;
using DotNetCliTemplate.Options;
using Microsoft.Extensions.Options;

namespace DotNetCliTemplate.Services
{
    public class GreetingEngine
    {
        private readonly IOptions<GreetingEngineOptions> _options;

        public GreetingEngine(IOptions<GreetingEngineOptions> options)
        {
            _options = options;
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            for (var i = 0; i < _options.Value.MessageCount && !cancellationToken.IsCancellationRequested; i++)
            {
                Console.WriteLine(_options.Value.Message);
                await Task.Delay(500, cancellationToken);
            }
        }
    }
}