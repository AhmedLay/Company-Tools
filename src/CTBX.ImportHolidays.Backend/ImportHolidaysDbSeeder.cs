using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;



namespace CTBX.ImportHolidays.Backend;

    public class ImportHolidaysDbSeeder : IHostedService
    {
        private readonly ILogger<ImportHolidaysDbSeeder> _logger;
        private readonly IConfiguration _configuration;
        public ImportHolidaysDbSeeder(ILogger<ImportHolidaysDbSeeder> logger, IConfiguration configuration)
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

                await CreateHolidaysTable(connection, cancellationToken);
                _logger.LogInformation("Table created");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing the database.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Clean-up if needed
            return Task.CompletedTask;
        }

        private static async Task CreateHolidaysTable(NpgsqlConnection connection, CancellationToken cancellationToken)
        {
            string createDbQuery = DbScripts.CreateHolidaysDbIfNotExists;
            using var command = new NpgsqlCommand(createDbQuery, connection);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }


public static class DbScripts
{
    public static string CreateHolidaysDbIfNotExists => """
            CREATE TABLE IF NOT EXISTS public.holidayimports (
                Id SERIAL PRIMARY KEY,
                FileName TEXT,
                FilePath TEXT,
                FileStatus TEXT,
                UploadDate TIMESTAMP DEFAULT NOW()
                
            );
        """;
}
