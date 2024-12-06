using System.Data;
using Dapper;
using Npgsql;
using Microsoft.Extensions.Configuration;

public class DataContext
{
    protected readonly IConfiguration Configuration;

    public DataContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IDbConnection CreateConnection()
    {
        //return new NpgsqlConnection(Configuration.GetConnectionString("FileUploadDB"));
        return new NpgsqlConnection("Server=localhost;Port=5432;Database=employee-db;User Id=admin;Password=admin;");
    }

    public async Task ExecuteAsync(string sql, object? parameters = null)
    {
        using var connection = CreateConnection();
        await connection.ExecuteAsync(sql, parameters);
    }

    public async Task Init()
    {
        // create database tables if they don't exist
        using var connection = CreateConnection();
        await _initUsers();

        async Task _initUsers()
        {
            var sql = """
                CREATE TABLE IF NOT EXISTS 
                Users (
                    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    FileName TEXT,
                    FilePath TEXT,
                    FileStatus TEXT,
                );
            """;
            await connection.ExecuteAsync(sql);
        }
    }
}
