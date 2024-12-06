using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

public class DataContext
{
    protected readonly IConfiguration Configuration;

    public DataContext(IConfiguration configuration)
    {
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration), "Configuration cant be null.");
    }

    public IDbConnection CreateConnection(string? database = null)
    {
        var connectionString = Configuration.GetConnectionString("employee-db");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string should not be 0.");
        }
        if (database != null)
        {
            connectionString = connectionString.Replace("Database=employee-db", $"Database={database}");
        }

        return new NpgsqlConnection(connectionString);
    }

    public async Task Init()
    {
        try
        {
            await _initDatabase();
            await _initUsers();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during initialization: {ex.Message}");
        }
    }

    private async Task _initDatabase()
    {
        var databaseName = "employee-db";

        // checking if it exist or not already
        using var connection = CreateConnection("postgres"); // Connect to postgres
        var dbExistsQuery = $"SELECT 1 FROM pg_database WHERE datname = '{databaseName}'";
        var dbExists = await connection.QueryFirstOrDefaultAsync<int>(dbExistsQuery);

        if (dbExists == 0)
        {
            // Database doesn't exist, create it
            var createDbQuery = $"CREATE DATABASE \"{databaseName}\"";
            await connection.ExecuteAsync(createDbQuery);
            Console.WriteLine($"Database '{databaseName}' created successfully.");
        }
        else
        {
            Console.WriteLine($"Database '{databaseName}' already exists.");
        }
    }

    private async Task _initUsers()
    {
        var sql = """
            CREATE TABLE IF NOT EXISTS public.EmployeeFile (
                Id SERIAL PRIMARY KEY,
                FileName TEXT,
                FilePath TEXT,
                FileStatus TEXT
            );
        """;

        using var connection = CreateConnection("employee-db"); // Now connecting to the correct database
        await connection.ExecuteAsync(sql);
        //test to see if we get into that point
        Console.WriteLine("The table 'EmployeeFile' has been successfully created or already exists.");
        Console.WriteLine($"Using Connection String: {Configuration.GetConnectionString("employee-db")}");
    }
}
