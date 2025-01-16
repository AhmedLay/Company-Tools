using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;


namespace CTBX.SkillManagment.Backend;

public class SkillsManagmentDataSeeder : IHostedService
{
    private readonly ILogger<SkillsManagmentDataSeeder> _logger;
    private readonly IConfiguration _configuration;
    public SkillsManagmentDataSeeder(ILogger<SkillsManagmentDataSeeder> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var adminConnectionString = _configuration.GetConnectionString("ctbx-common-db")!.GuardAgainstNullOrEmpty("adminConnectionString");
        try
        {
            using var connection = new NpgsqlConnection(adminConnectionString);
            await connection.OpenAsync(cancellationToken);

            await CreateSkillsTable(connection, cancellationToken);
            _logger.LogInformation("Table created");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the database.");
        }
    }
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
 
    private static async Task CreateSkillsTable(NpgsqlConnection connection, CancellationToken cancellationToken)
    {
        string createDbQuery = DbScripts.CreateEmployeesDbIfNotExists;
        using var command = new NpgsqlCommand(createDbQuery, connection);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}

public static class DbScripts
{
    public static string CreateEmployeesDbIfNotExists => """
            CREATE TABLE IF NOT EXISTS public.Skills (
                Id SERIAL PRIMARY KEY,
                Name TEXT,
                Description TEXT,
                Type TEXT                
            );
        """;
}
