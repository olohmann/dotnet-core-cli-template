using DotNetCliTemplate.Services;

namespace DotNetCliTemplate.Options
{
    public class GreetingEngineOptions
    {
        public const string SectionName = nameof(GreetingEngine);

        public string Message { get; set; }
        public int MessageCount { get; set; }
    }
}