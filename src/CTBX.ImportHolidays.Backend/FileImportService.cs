
using System.ComponentModel.DataAnnotations;
using CTBX.EmployeesImport.Shared;
using CTBX.ImportHolidays.Shared;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace CTBX.ImportHolidays.Backend
{
    public class FileImportService : IFileImportHandler
    {
        private readonly string? _connectionString;

        public FileImportService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ctbx-common")!;
        }
        public async Task<IEnumerable<FileRecord>> GetPendingFiles()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<FileRecord>
                ("SELECT * FROM public.fileimport WHere FileStatus = @FileStatus",
                new { FileStatus = "Pending" });
        }
        public async Task UpdateFileStatus(int id, string status)
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
        public async Task<List<Holiday>> ConvertFileToHolidays(string filepath)
        {
            var holidays = new List<Holiday>();

            var lines = await File.ReadAllLinesAsync(filepath);
            foreach (var line in lines)
            {
                var split = line.Split(';');
                if (split.Length == 6)
                {
                    holidays.Add(new Holiday
                    {
                        Country = split[0],
                        State = split[1],
                        HolidayName = split[2],
                        HolidayDate = DateOnly.Parse(split[3]),
                        IsGlobal = bool.Parse(split[4])
                    });
                }
            }
            return holidays;
        }
        public async Task ImportHolidayFromFile(string filepath)
        {
            var holidays = await ConvertFileToHolidays(filepath);

            using var connection = new NpgsqlConnection(_connectionString);
            var query = @"INSERT INTO public.Holidays (Country, State, Date, HolidayName, IsGlobal)
                VALUES (@Country, @State, @Date, @HolidayName, @IsGlobal)
                        ON CONFLICT (Date, Country, State) DO NOTHING";

            await connection.ExecuteAsync(query, holidays);

        }
    }
}
