using CTBX.EmployeesImport.Shared;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace CTBX.EmployeesImport.Backend
{
    public class FileImportService : IFileImportHandler
    {
        private readonly string? _connectionString;

        public FileImportService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ctbx-common-db")!;
        }
        public async Task<IEnumerable<FileRecord>> GetPendingFiles()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<FileRecord>
                ("SELECT * FROM public.fileimports WHERE FileStatus = @FileStatus",
                new { FileStatus = "Pending" });
        }
        public async Task UpdateFileStatus(int id,string status)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(
                "UPDATE public.fileimports SET FileStatus = @FileStatus WHERE Id = @Id",
                new { FileStatus = status, Id = id });
        }
        public async Task DeleteFileFromFolder(string filepath)
        {
                await Task.Run(() => File.Delete(filepath));
        }
        public async Task<List<Employee>> ConvertFileToEmployees(string filepath)
        {
            var employees = new List<Employee>();

            var lines = await File.ReadAllLinesAsync(filepath);
                foreach (var line in lines)
                {
                var split = line.Split(';');
                if (split.Length == 6)
                {
                    employees.Add(new Employee
                    {
                       Email = split[0],
                       Surname = split[1],
                       Name = split[2],
                       EmployeeID = int.Parse(split[3]),
                       AnualVacationDays = int.Parse(split[4]),
                       RemainingVacationDays = int.Parse(split[5])
                    });
                }
                }
            return employees;
        }
        public async Task ImportEmployeeFromFile(string filepath)
        {
            var employees = await ConvertFileToEmployees(filepath);

            using var connection = new NpgsqlConnection(_connectionString);
            var query = @"INSERT INTO public.Employees (Surname, Name, Email, EmployeeID, AnualVacationDays, RemainingVacationDays)
                  VALUES (@Surname, @Name, @Email, @EmployeeID, @AnualVacationDays, @RemainingVacationDays)
                         ON CONFLICT (Email) DO NOTHING";

            await connection.ExecuteAsync(query, employees);
        }
    }
}
