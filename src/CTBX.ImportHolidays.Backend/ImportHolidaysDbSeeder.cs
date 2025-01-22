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

                await CreateHolidaysTables(connection, cancellationToken);
                _logger.LogInformation("Tables created");
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

        private static async Task CreateHolidaysTables(NpgsqlConnection connection, CancellationToken cancellationToken)
        {
            string createDbQuery1 = DbScripts.CreateHolidaysDbIfNotExists;
            string createDbQuery2 = DbScripts.CreateHolidaysTableIfNotExists;
            using var command1 = new NpgsqlCommand(createDbQuery1, connection);
            using var command2 = new NpgsqlCommand(createDbQuery2, connection);
            await command1.ExecuteNonQueryAsync(cancellationToken);
            await command2.ExecuteNonQueryAsync(cancellationToken);
        }
    }


public static class DbScripts
{
    public static string CreateHolidaysDbIfNotExists => """
            CREATE TABLE IF NOT EXISTS public.holidayimports (
                Id SERIAL PRIMARY KEY,
                FileName TEXT,
                FilePath TEXT,
                Status TEXT,
                UploadDate TIMESTAMP DEFAULT NOW()
                
            );
        """;

    public static string CreateHolidaysTableIfNotExists => """
            CREATE TABLE IF NOT EXISTS public.holidays (
                Id SERIAL PRIMARY KEY,
                Country TEXT,
                State TEXT,
                HolidayName TEXT,
                HolidayDate DATE NOT NULL,
                IsGlobal BOOL NOT NULL  
            );
        """;
}
