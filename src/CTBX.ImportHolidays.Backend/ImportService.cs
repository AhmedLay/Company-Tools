using System.Collections.Immutable;
using CTBX.CommonUtils;
using CTBX.EmployeesImport.Shared;
using CTBX.ImportHolidays.Shared;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NCommandBus.Core.Abstractions;
using Npgsql;
using YAEP.Utils;


namespace CTBX.ImportHolidays.Backend;

public class HolidayImporterOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public string UploadDirectory { get; set; } = "Uploads";
}

public record UploadHolidayFile(string FileName, byte[] Content);
public record GetAllFileRecordsQuery();
public record PersistHolidaysFromFile(string FileName, byte[] Content);
public record GetHolidaysDataQuery();

public class HolidaysImporter : CommandBusBase
{
    private readonly HolidayImporterOptions _options;
    private readonly IDateTimeProvider _dateTimeProvider;
    

    public HolidaysImporter(IOptions<HolidayImporterOptions> options,
                            IDateTimeProvider dateTimeProvider, IConfiguration configuration)
    {
        _options = options.Value.GuardAgainstNull(nameof(options));

        On<UploadHolidayFile, OperationResult>(HandleUpload);
        On<GetAllFileRecordsQuery, OperationResult<IImmutableList<FileRecord>>>(HandleGetAllFileRecords);

        On<PersistHolidaysFromFile, OperationResult>(HandleHolidaysPersistence);
        On<GetHolidaysDataQuery, OperationResult<IImmutableList<Holiday>>>(HandleGetHolidaysData);

        _dateTimeProvider = dateTimeProvider.GuardAgainstNull(nameof(dateTimeProvider));
    }

    private async ValueTask<OperationResult> HandleUpload(UploadHolidayFile command, CancellationToken cancellationToken)
    {

        if (command.FileName.IsNullOrEmpty())
            return OperationResult.Failure($"Invalid Filename : [{command.FileName}]");
        if (command.Content.Length == 0)
            return OperationResult.Failure("The uploaded file should not be empty.");

        if (!Directory.Exists(_options.UploadDirectory))
        {
            Directory.CreateDirectory(_options.UploadDirectory);
        }


        var filePath = Path.Combine(_options.UploadDirectory, command.FileName);

        await File.WriteAllBytesAsync(filePath, command.Content)
                  .AsValueTask();

        await PersistToDb(new FileRecord
        {
            FilePath = filePath,
            FileName = command.FileName,
            FileStatus = "Pending",
            UploadDate = _dateTimeProvider.UtcNow
        });



        return
        OperationResult.Success($"File [{command.FileName}] uploaded to [{_options.UploadDirectory}]");
    }



    private async Task PersistToDb(FileRecord fileRecord)
    {
        var insertQuery = "INSERT INTO public.holidayimports (FileName, FilePath, FileStatus,UploadDate) VALUES (@FileName, @FilePath, @FileStatus,@UploadDate)";
       
        using var connection = new NpgsqlConnection(_options.ConnectionString);
        connection.Open();

        await using var command = new NpgsqlCommand(insertQuery, connection);

        command.Parameters.AddWithValue("@FileName", fileRecord.FileName);
        command.Parameters.AddWithValue("@FilePath", fileRecord.FilePath);
        command.Parameters.AddWithValue("@FileStatus", fileRecord.FileStatus);
        command.Parameters.AddWithValue("@UploadDate", fileRecord.UploadDate);

        var result = await command.ExecuteNonQueryAsync();

    }

    //Just added noww
    private async ValueTask<OperationResult<IImmutableList<FileRecord>>> HandleGetAllFileRecords(GetAllFileRecordsQuery command, CancellationToken cancellationToken)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_options.ConnectionString);
            const string selectQuery = "SELECT id, filename, filepath, filestatus, uploaddate FROM public.holidayimports";

            var result = await connection.QueryAsync<FileRecord>(selectQuery);
            //var filerecords = result.ToList();
            var fileRecords = result?.ToImmutableList() ?? ImmutableList<FileRecord>.Empty;

            return OperationResult.Success("Successfully retrieved file records.", (IImmutableList<FileRecord>)fileRecords);
        }
        catch (Exception ex)
        {
            return (OperationResult<IImmutableList<FileRecord>>)OperationResult.Failure($"Failed to retrieve file records: {ex.Message}");

        }
    }







    // Handlling for persisting holidays to db out of file (using this ATM (working !!!!)
    public async ValueTask<OperationResult> HandleHolidaysPersistence(PersistHolidaysFromFile command, CancellationToken cancellationToken)
    {
        
        if (command.Content.Length == 0)
            return OperationResult.Failure("The file is empty.");

        var holidays = new List<Holiday>();
        var filepath = Path.Combine(_options.UploadDirectory, command.FileName);
        
        var lines = await File.ReadAllLinesAsync(filepath, cancellationToken);

        foreach (var line in lines)
        {
            var parts = line.Split(';');
            if (parts.Length < 5)
                return OperationResult.Failure("Invalid file format.");

            holidays.Add(new Holiday
            {
                Country = parts[0].Trim(),
                State = parts[1].Trim(),
                HolidayName = parts[2].Trim(),
                HolidayDate = DateTime.Parse(parts[3].Trim()), 
                IsGlobal = bool.Parse(parts[4].Trim())
            });
        }

        foreach (var holiday in holidays)
        {
            await PersistHolidaysToDb(holiday);
        }
        //Delete file after everything was extracted
        await Task.Run(() => File.Delete(command.FileName));

        return
            OperationResult.Success($"Holidays Imported Successfully from File [{command.FileName}]");
        
    }

    private async Task PersistHolidaysToDb(Holiday holidays)
    {
        var insertQuery = @"
        INSERT INTO public.holidays (Country, State, HolidayName, HolidayDate, IsGlobal) 
        VALUES (@Country, @State, @HolidayName, @HolidayDate, @IsGlobal)";
        using var connection = new NpgsqlConnection(_options.ConnectionString);
        connection.Open();

        await using var command = new NpgsqlCommand(insertQuery, connection);

        command.Parameters.AddWithValue("@Country", holidays.Country);
        command.Parameters.AddWithValue("@State", holidays.State);
        command.Parameters.AddWithValue("@HolidayName", holidays.HolidayName);
        command.Parameters.AddWithValue("@HolidayDate", holidays.HolidayDate);
        command.Parameters.AddWithValue("@IsGlobal", holidays.IsGlobal);


        var result = await command.ExecuteNonQueryAsync();


    }

    private async ValueTask<OperationResult<IImmutableList<Holiday>>> HandleGetHolidaysData(GetHolidaysDataQuery query, CancellationToken cancellationToken)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_options.ConnectionString);
            const string selectQuery = "SELECT Country, State, HolidayName, HolidayDate, IsGlobal FROM public.Holidays";
            var result = await connection.QueryAsync<Holiday>(selectQuery);

            var holidays = result?.ToImmutableList() ?? ImmutableList<Holiday>.Empty;

            return OperationResult.Success("Successfully retrieved Holidays", (IImmutableList<Holiday>)holidays);
        }
        catch (Exception ex)
        {
            return (OperationResult<IImmutableList<Holiday>>)OperationResult.Failure($"Failed to retrieve holiday data: {ex.Message}");
        }
    }
}








/// Not used so far
/// //////////////////////////////////////////////////////////////// ////////////////////////////////////////////////////////////


//public record UpdateFileStatus(int id, string status);
//public record DeleteFile(string FilePath);




//public class FileImportService : CommandBusBase /*, IFileImportHandler*/
//{
//    private readonly string? _connectionString;

//    public FileImportService(IConfiguration configuration)
//    {
//        _connectionString = configuration.GetConnectionString("ctbx-common-db")!;
//        On<UpdateFileStatus, OperationResult>(HandleUpdateFileStatus);
//        On<DeleteFile, OperationResult>(HandleDeleteFile);
//    }
//    public async Task<IEnumerable<FileRecord>> GetPendingFiles()
//    {
//        using var connection = new NpgsqlConnection(_connectionString);
//        return await connection.QueryAsync<FileRecord>
//            ("SELECT * FROM public.holidayimports WHere FileStatus = @FileStatus",
//            new { FileStatus = "Pending" });
//    }



//    private async ValueTask<OperationResult> HandleUpdateFileStatus(UpdateFileStatus command, CancellationToken cancellationToken)
//    {
//        try
//        {
//            using var connection = new NpgsqlConnection(_connectionString);
//            await connection.ExecuteAsync(
//                "UPDATE public.holidayimports SET FileStatus = @FileStatus WHERE Id = @Id",
//                new { FileStatus = command.status, Id = command.id });

//            return OperationResult.Success($"File status updated to {command.status} for File ID {command.id}");
//        }
//        catch (Exception ex)
//        {
//            return OperationResult.Failure($"Failed to update file status: {ex.Message}");
//        }
//    }



//    //public async Task UpdateFileStatus(int id, string status)
//    //{
//    //    using var connection = new NpgsqlConnection(_connectionString);
//    //    await connection.ExecuteAsync(
//    //        "UPDATE public.fileimports SET FileStatus = @FileStatus WHERE Id = @Id",
//    //        new { FileStatus = status, Id = id });
//    //}


//    private async ValueTask<OperationResult> HandleDeleteFile(DeleteFile command, CancellationToken cancellationToken)
//    {
//        try
//        {
//            if (string.IsNullOrWhiteSpace(command.FilePath) || !File.Exists(command.FilePath))
//            {
//                return OperationResult.Failure($"Invalid or non-existent file path: {command.FilePath}");
//            }

//            await Task.Run(() => File.Delete(command.FilePath));
//            return OperationResult.Success($"File [{command.FilePath}] deleted successfully.");

//        }
//        catch (Exception ex)
//        {
//            return OperationResult.Failure($"Failed to delete file: {ex.Message}");
//        }
//    }

//}








//private async ValueTask<OperationResult> HandleSaveFileToFolder(SaveFileToFolderCommand command, CancellationToken cancellationToken)
//{
//    try
//    {
//        // Ensure the folder exists
//        if (!Directory.Exists(command.FolderPath))
//        {
//            Directory.CreateDirectory(command.FolderPath);
//        }

//        // Set the file name and create the final path
//        var fileName = command.File.FileName.GuardAgainstNullOrEmpty("fileName");
//        var filePath = Path.Combine(command.FolderPath, fileName);

//        if (command.File.FileContent == null || command.File.FileContent.Length == 0)
//        {
//            throw new ArgumentException("The uploaded file is empty.");
//        }

//        await File.WriteAllBytesAsync(filePath, command.File.FileContent, cancellationToken);
//        return OperationResult.Success(filePath);
//    }
//    catch (Exception ex)
//    {
//        return OperationResult.Failure($"Failed to save file to folder: {ex.Message}");
//    }
//}





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
