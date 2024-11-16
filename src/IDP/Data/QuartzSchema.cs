using Npgsql;
using System.Reflection;

namespace IDP.Data;

public class QuartzSchema
{
    static readonly Assembly Assembly = typeof(QuartzSchema).Assembly;

    public async Task CreateSchema(string connectionString, ILogger<QuartzSchema> log, CancellationToken cancellationToken = default)
    {
        log.LogInformation("Creating schema");

        var dataSource = new NpgsqlDataSourceBuilder(connectionString).Build();

        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken).ConfigureAwait(false);

        var schemaCreatedSql = @$"SELECT EXISTS (
                                 SELECT FROM pg_catalog.pg_class c
                                 JOIN   pg_catalog.pg_namespace n
                                 ON     n.oid = c.relnamespace                                 
                                 AND    c.relname = 'organizations'
                                 AND    c.relkind = 'r'    -- only tables
                               );";

        var schemaExistsCmd = new NpgsqlCommand(schemaCreatedSql, connection);

        var result = await schemaExistsCmd.ExecuteScalarAsync();

        var schemaExists = (bool?)result ?? false;

        if (schemaExists)
        {
            log?.LogDebug("The schema already exists");
            return;
        }

        var transaction = await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            var names =Assembly
                        .GetManifestResourceNames()
                        .Where(x => x.EndsWith(".sql"))
                        .OrderBy(x => x);

            foreach (var name in names)
            {
                log?.LogInformation("Executing {Script}", name);
                await using var stream = Assembly.GetManifestResourceStream(name);
                using var reader = new StreamReader(stream!);

                var script = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
                //var cmdScript = script.Replace("__schema__", schema);

                await using var cmd = new NpgsqlCommand(script, connection, transaction);

                await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception e)
        {
            log?.LogCritical(e, "Unable to initialize the database schema");
            await transaction.RollbackAsync(cancellationToken);

            throw;
        }

        await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        log?.LogInformation("Database schema initialized");
    }
}
