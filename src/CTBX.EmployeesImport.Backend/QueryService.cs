using Dapper;
using Npgsql;
using Microsoft.Extensions.Configuration;
using CTBX.EmployeesImport.Shared;

namespace CTBX.EmployeesImport.Backend
{
    public class QueryService
    {
        private readonly string _connectionString;

        public QueryService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ctbx-common-db")!;
        }

        public async Task InsertFileRecordAsync(FileRecord fileRecord)
        {
            var insertQuery = "INSERT INTO fileimports (FileName, FilePath, FileStatus) VALUES (@FileName, @FilePath, @FileStatus)";

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(insertQuery, fileRecord);
        }

    }
}

