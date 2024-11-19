using IDP.Common;
using Microsoft.Extensions.Options;
using Polly;

namespace IDP.Data;

public class DbSchemaInitializer : IHostedService
{
    private readonly ResiliencePipeline _resilience;
    private readonly ILoggerFactory? _loggerFactory;
    private readonly DbInitOptions _options;

    public DbSchemaInitializer([FromKeyedServices(CommonConstants.ResiliencePipeline)] ResiliencePipeline resilience, ILoggerFactory loggerFactory, IOptions<DbInitOptions> options)
    {        
        _resilience = resilience.GuardAgainstNull(nameof(resilience));
        _loggerFactory = loggerFactory.GuardAgainstNull(nameof(loggerFactory));
        _options = options.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var schema = new QuartzSchema();
        await _resilience.ExecuteAsync(async token => await schema.CreateSchema(_options.QuartzDbConnectionString, _loggerFactory!.CreateLogger<QuartzSchema>(), cancellationToken));

    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
