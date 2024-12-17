using Dapper;
using Npgsql;
using Microsoft.Extensions.Configuration;
using CTBX.ImportHolidays.Shared;

namespace CTBX.ImportHolidays.Backend
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
    }

}
