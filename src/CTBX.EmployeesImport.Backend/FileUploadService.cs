using Dapper;
using Npgsql;
using Microsoft.Extensions.Configuration;
using CTBX.EmployeesImport.Shared;
using System.Data;

namespace CTBX.EmployeesImport.Backend
{
    public class FileUploadService : IFileUploadHandler
    {
        private readonly string _connectionString;

        public FileUploadService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ctbx-common-db")!;
        }

        public async Task PersistToDb(FileRecord fileRecord)
        {
            var insertQuery = "INSERT INTO fileimports (FileName, FilePath, FileStatus,UploadDate) VALUES (@FileName, @FilePath, @FileStatus,@UploadDate)";

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(insertQuery, fileRecord);
        }

        public async Task<string> SaveFileToFolder(string folderPath, FileData file)
        {
            // Ensure the folder exists
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Set the name of the file and create the final path
            var fileName = file.FileName!.GuardAgainstNullOrEmpty("fileName");
            var filePath = Path.Combine(folderPath, fileName);



            if (file.FileContent == null || file.FileContent.Length == 0)
            {
                throw new ArgumentException("The uploaded file is empty.");
            }
            await File.WriteAllBytesAsync(filePath, file.FileContent);
            return filePath;
        }

        public async Task<List<FileRecord>> GetAllFileRecordsAsync()
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            const string query = "SELECT Id, FileName, FilePath, FileStatus, UploadDate FROM public.fileimports";
            var result = await connection.QueryAsync<FileRecord>(query);
            return result.ToList();
        }

    }
}

