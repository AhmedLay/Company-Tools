using System.Runtime.CompilerServices;
using CTBX.CommonUtils;
using CTBX.EmployeesImport.Shared;
using CTBX.ImportHolidays.Shared;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NCommandBus.Core.Abstractions;
using Npgsql;
using YAEP.Utils;
using static MudBlazor.CategoryTypes;

namespace CTBX.ImportHolidays.Backend;

public class HolidayImporterOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public string UploadDirectory { get; set; } = "Uploads";
}

public record UploadHolidayFile(string FileName, byte[] Content);
public record PersistHolidaysFromFile(string FileName, byte[] Content);

public class HolidaysImporter : CommandBusBase
{
    private readonly HolidayImporterOptions _options;
    private readonly IDateTimeProvider _dateTimeProvider;
    

    public HolidaysImporter(IOptions<HolidayImporterOptions> options,
                            IDateTimeProvider dateTimeProvider, IConfiguration configuration)
    {
        _options = options.Value.GuardAgainstNull(nameof(options));

        On<UploadHolidayFile, OperationResult>(HandleUpload);
        On<PersistHolidaysFromFile, OperationResult>(HandleHolidaysPersistence);

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
