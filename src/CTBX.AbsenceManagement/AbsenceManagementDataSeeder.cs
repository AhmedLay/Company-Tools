using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

public class AbsenceManagementDataSeeder : IHostedService
{
    private readonly ILogger<AbsenceManagementDataSeeder> _logger;
    private readonly IConfiguration _configuration;
    public AbsenceManagementDataSeeder(ILogger<AbsenceManagementDataSeeder> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var adminConnectionString = _configuration.GetConnectionString("ctbx-events-db")!.GuardAgainstNullOrEmpty("adminConnectionString");
        using var connection = new NpgsqlConnection(adminConnectionString);
        await connection.OpenAsync(cancellationToken);

        await CreateEmployeesFileTable(connection, cancellationToken);
        await CreateEmployeesTable(connection, cancellationToken);
        _logger.LogInformation("Table created");

    }
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
    private static async Task CreateEmployeesFileTable(NpgsqlConnection connection, CancellationToken cancellationToken)
    {
        string createDbQuery = DbScripts.VacationScheduleScript;
        using var command = new NpgsqlCommand(createDbQuery, connection);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
    private static async Task CreateEmployeesTable(NpgsqlConnection connection, CancellationToken cancellationToken)
    {
        string createDbQuery = DbScripts.VacationScheduleScript;
        using var command = new NpgsqlCommand(createDbQuery, connection);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
public static class DbScripts
{
    public static string VacationScheduleScript => """
    CREATE TABLE IF NOT EXISTS ctbx.vacationrequests (
        Id SERIAL PRIMARY KEY,
        Employeeid INTEGER NOT NULL,
        "From" TIMESTAMP NOT NULL,
        "To" TIMESTAMP NOT NULL,
        Comment TEXT,
        Scheduledat TIMESTAMP NOT NULL
    );
    """;

}

