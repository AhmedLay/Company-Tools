using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

public class DataContext
{
    protected readonly IConfiguration Configuration;

    public DataContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(Configuration.GetConnectionString("ctbx-events-db"));
    }
    public async Task Init()
    {
        // Initialize the users table
        await _initUsers();
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

        using var connection = CreateConnection();
        await connection.ExecuteAsync(sql);
        Console.WriteLine("The table 'EmployeeFile' has been successfully created or already exists.");
        Console.WriteLine($"Using Connection String: {Configuration.GetConnectionString("ctbx-events-db")}");

    }
}
