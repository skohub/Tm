using Serilog;

namespace Api.Service
{
    public static class LoggingExtensions
    {
        public static void AddLogging(this WebApplicationBuilder builder)
        {
            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithCorrelationId()
                .CreateLogger();
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);
            builder.Services.AddHttpContextAccessor();
        }
    }
}