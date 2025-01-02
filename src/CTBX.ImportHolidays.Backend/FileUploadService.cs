using Dapper;
using Npgsql;
using Microsoft.Extensions.Configuration;
using CTBX.ImportHolidays.Shared;
using CTBX.EmployeesImport.Shared;
using NCommandBus.Core.Abstractions;
using YAEP.Utils;

namespace CTBX.ImportHolidays.Backend;

public record PersistFileRecordCommand(FileRecord FileRecord);
public record SaveFileToFolderCommand(string FolderPath, FileData File);
public record GetAllFileRecordsQuery();
public record GetHolidaysDataQuery();

public class FileUploadCommandHandler : CommandBusBase
{
    private readonly string _connectionString;

    public FileUploadCommandHandler(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("ctbx-common-db")!;

     
        On<PersistFileRecordCommand, OperationResult>(HandlePersistFileRecord);
        On<SaveFileToFolderCommand, OperationResult>(HandleSaveFileToFolder);
        On<GetAllFileRecordsQuery, OperationResult<List<FileRecord>>>(HandleGetAllFileRecords); 
        On<GetHolidaysDataQuery, OperationResult>(HandleGetHolidaysData);
    }

    private async ValueTask<OperationResult> HandlePersistFileRecord(PersistFileRecordCommand command, CancellationToken cancellationToken)
    {
        try
        {
            const string insertQuery = "INSERT INTO holidayimports (FileName, FilePath, FileStatus, UploadDate) VALUES (@FileName, @FilePath, @FileStatus, @UploadDate)";

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(insertQuery, command.FileRecord);

            return OperationResult.Success("File record persisted successfully.");
        }
        catch (Exception ex)
        {
            return OperationResult.Failure($"Failed to persist file record: {ex.Message}");
        }
    }

    private async ValueTask<OperationResult> HandleSaveFileToFolder(SaveFileToFolderCommand command, CancellationToken cancellationToken)
    {
        try
        {
            // Ensure the folder exists
            if (!Directory.Exists(command.FolderPath))
            {
                Directory.CreateDirectory(command.FolderPath);
            }

            // Set the file name and create the final path
            var fileName = command.File.FileName.GuardAgainstNullOrEmpty("fileName");
            var filePath = Path.Combine(command.FolderPath, fileName);

            if (command.File.FileContent == null || command.File.FileContent.Length == 0)
            {
                throw new ArgumentException("The uploaded file is empty.");
            }

            await File.WriteAllBytesAsync(filePath, command.File.FileContent, cancellationToken);
            return OperationResult.Success(filePath);
        }
        catch (Exception ex)
        {
            return OperationResult.Failure($"Failed to save file to folder: {ex.Message}");
        }
    }

    private async ValueTask<OperationResult<List<FileRecord>>> HandleGetAllFileRecords(GetAllFileRecordsQuery query, CancellationToken cancellationToken)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            const string selectQuery = "SELECT Id, FileName, FilePath, FileStatus, UploadDate FROM public.fileimports";
            var result = await connection.QueryAsync<FileRecord>(selectQuery);
            var filerecords = result.ToList();

            return OperationResult.Success("Success", filerecords);
        }
        catch (Exception ex)
        {
            return (OperationResult<List<FileRecord>>) OperationResult.Failure($"Failed to retrieve file records: {ex.Message}");
        }
    }

    private async ValueTask<OperationResult> HandleGetHolidaysData(GetHolidaysDataQuery query, CancellationToken cancellationToken)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            const string selectQuery = "SELECT Country, State, HolidayName, HolidayDate, IsGlobal FROM public.Holidays";
            var result = await connection.QueryAsync<Holiday>(selectQuery);
            var filerecords = result.ToList();

            return OperationResult.Success("Success", filerecords);
        }
        catch (Exception ex)
        {
            return OperationResult.Failure($"Failed to retrieve holiday data: {ex.Message}");
        }
    }
}

//public class FileUploadService : IFileUploadHandler
//{
//    private readonly string _connectionString;

//    public FileUploadService(IConfiguration configuration)
//    {
//        _connectionString = configuration.GetConnectionString("ctbx-common-db")!;
//    }

//    public async Task PersistToDb(FileRecord fileRecord)
//    {
//        var insertQuery = "INSERT INTO holidayimports (FileName, FilePath, FileStatus,UploadDate) VALUES (@FileName, @FilePath, @FileStatus,@UploadDate)";

//        await using var connection = new NpgsqlConnection(_connectionString);
//        await connection.ExecuteAsync(insertQuery, fileRecord);
//    }

//    public async Task<string> SaveFileToFolder(string folderPath, FileData file)
//    {
//        // Ensure the folder exists
//        if (!Directory.Exists(folderPath))
//        {
//            Directory.CreateDirectory(folderPath);
//        }

//        // Set the name of the file and create the final path
//        var fileName = file.FileName!.GuardAgainstNullOrEmpty("fileName");
//        var filePath = Path.Combine(folderPath, fileName);



//        if (file.FileContent == null || file.FileContent.Length == 0)
//        {
//            throw new ArgumentException("The uploaded file is empty.");
//        }
//        await File.WriteAllBytesAsync(filePath, file.FileContent);
//        return filePath;
//    }

//    public async Task<List<FileRecord>> GetAllFileRecordsAsync()
//    {
//        await using var connection = new NpgsqlConnection(_connectionString);
//        const string query = "SELECT Id, FileName, FilePath, FileStatus, UploadDate FROM public.fileimports";
//        var result = await connection.QueryAsync<FileRecord>(query);
//        return result.ToList();
//    }

//    public async Task<List<Holiday>> GetHolidaysDataAsync()
//    {
//        await using var connection = new NpgsqlConnection(_connectionString);
//        const string query = "SELECT Country, State, HolidayName, HolidayDate, IsGlobalFROM public.Holidays";
//        var result = await connection.QueryAsync<Holiday>(query);
//        return result.ToList();
//    }
//}






//    public FileImporter(FileImportService fileImportService, ILogger<FileImportService> logger)
//    {
//        _fileImportService = fileImportService;
//        _logger = logger;
//    }
//    public async Task ImportHolidayFromFile()
//    {
//        _logger.LogInformation("Start processing files");
//        var pendingFiles = await _fileImportService.GetPendingFiles();
//        foreach (var file in pendingFiles)
//        {
//            await _fileImportService.UpdateFileStatus(file.Id, "In Progress");
//            _logger.LogDebug("Processing file {fileName}.", file.FileName);

//            try
//            {
//                await _fileImportService.ImportHolidayFromFile(file.FilePath);
//                await _fileImportService.DeleteFileFromFolder(file.FilePath);
//                await _fileImportService.UpdateFileStatus(file.Id, "In Progress");
//                _logger.LogInformation("{fileName} successfully processed.", file.FileName);
//            }
//            catch (Exception ex)
//            {
//                await _fileImportService.UpdateFileStatus(file.Id, "Failed");
//                await _fileImportService.DeleteFileFromFolder(file.FilePath);
//                _logger.LogError(ex, "{fileName} failed.", file.FileName);
//            }

//        }

//    }

//}
