using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

public class DataSeeder
{
    readonly ILogger<DataSeeder> _logger;
    public DataSeeder(ILogger<DataSeeder> logger)
    {
        _logger = logger;
    }

    public async Task CreateSchema(NpgsqlDataSource dataSource, CancellationToken cancellationToken = default)
    {
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);

        var schemaExistsSql = @"
            SELECT EXISTS (
                SELECT 1
                FROM pg_catalog.pg_namespace
                WHERE nspname = 'employee'
            );
        ";

        var schemaExistsCmd = new NpgsqlCommand(schemaExistsSql, connection);

        var result = await schemaExistsCmd.ExecuteScalarAsync();

        var schemaExists = (bool?)result ?? false;

        if (schemaExists)
        {
            return;
        }

        var transaction = await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            var assembly = typeof(DataSeeder).Assembly;
            var names = assembly.GetManifestResourceNames().Where(x => x.EndsWith(".sql")).OrderBy(x => x);

            foreach (var name in names)
            {
                await using var stream = assembly.GetManifestResourceStream(name);
                using var reader = new StreamReader(stream!);

                var script = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
                var cmdScript = script;//.Replace("__schema__", "todos");

                await using var cmd = new NpgsqlCommand(cmdScript, connection, transaction);

                await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);


            _logger.LogError(e, "Seed data failed.");
            throw;
        }

        await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
    }
}

public class DbInitService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public DbInitService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<DataSeeder>();
            var dataSeeder = new DataSeeder(logger);
            var dataSource = scope.ServiceProvider.GetRequiredService<NpgsqlDataSource>();
            await dataSeeder.CreateSchema(dataSource, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
