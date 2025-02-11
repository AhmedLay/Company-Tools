using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;


namespace CTBX.ReportSickLeave.Backend;

public class ReportSickLeaveDbSeeder : IHostedService
{
    private readonly ILogger<ReportSickLeaveDbSeeder> _logger;
    private readonly IConfiguration _configuration;

    public ReportSickLeaveDbSeeder(ILogger<ReportSickLeaveDbSeeder> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var adminConnectionString = _configuration.GetConnectionString("ctbx-events-db")!.GuardAgainstNullOrEmpty("adminConnectionString");
        using var connection = new NpgsqlConnection(adminConnectionString);
        await connection.OpenAsync(cancellationToken);

        await CreateSickLeaveTable(connection, cancellationToken);
        _logger.LogInformation("Table Created!");
    }
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private static async Task CreateSickLeaveTable(NpgsqlConnection connection, CancellationToken cancellationToken)
    {
        string createDbQuery = DbScripts.SickReportScript;
        using var command = new NpgsqlCommand(createDbQuery, connection);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

}

public static class DbScripts
{
    public static string SickReportScript => """
    CREATE TABLE IF NOT EXISTS ctbx.sickreportrequests (
        Id SERIAL PRIMARY KEY,
        Employeeid INTEGER NOT NULL,
        From TIMESTAMP NOT NULL,
        Status TEXT,
        Until TIMESTAMP NOT NULL,
        Comment TEXT,
        Reportedat TIMESTAMP NOT NULL,
        Approvedat TIMESTAMP NULL,
        Rejectedat TIMESTAMP NULL
    );
    """;

}
