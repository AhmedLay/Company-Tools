using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using Microsoft.Extensions.Configuration;
using CTBX.ImportHolidays.Shared;

namespace CTBX.ImportHolidays.Backend
{
    class EndPointServices
    {
        private readonly string _connectionString;

        public EndpointServices(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ctbx-common-db")!;
        }

        public async Task InsertFileRecordAsync(FileRecord fileRecord)
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

            await File.WriteAllBytesAsync(filePath, file.FileContent);
            return filePath;
        }

        public FileRecord CreateRecord(FileData file, string filepath)
        {
            var name = file.FileName!.GuardAgainstNullOrEmpty("fileName");
            return new FileRecord
            {
                FileName = name,
                FilePath = Path.Combine(filepath, name),
                FileStatus = "pending",
                UploadDate = DateTime.Now
            };
        }
    }
}
