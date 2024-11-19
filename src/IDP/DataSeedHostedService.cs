using System.Threading;

public class DataSeedHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public DataSeedHostedService(IServiceProvider serviceProvider)
        => _serviceProvider = serviceProvider;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        await DataSeedHelper.SeedDemoData(scope.ServiceProvider, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
