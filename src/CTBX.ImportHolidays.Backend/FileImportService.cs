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

public class HolidaysImporter : CommandBusBase
{
    private readonly HolidayImporterOptions _options;
    private readonly IDateTimeProvider _dateTimeProvider;

    public HolidaysImporter(IOptions<HolidayImporterOptions> options,
                            IDateTimeProvider dateTimeProvider)
    {
        _options = options.Value.GuardAgainstNull(nameof(options));

        On<UploadHolidayFile, OperationResult>(HandleUpload);
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

    private Task PersistToDb(FileRecord fileRecord)
    {
        var insertQuery = "INSERT INTO holidayimports (FileName, FilePath, FileStatus,UploadDate) VALUES (@FileName, @FilePath, @FileStatus,@UploadDate)";

        using var connection = new NpgsqlConnection(_options.ConnectionString);
        return connection.ExecuteAsync(insertQuery, fileRecord);
    }
}



public record UpdateFileStatus(int id, string status);
public record DeleteFile(string FilePath);
public record ConvertFileToHolidaysCommand(string FilePath);
public record ImportHolidayFromFileCommand(string FilePath);


public class FileImportService : CommandBusBase /*, IFileImportHandler*/
{
    private readonly string? _connectionString;

    public FileImportService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("ctbx-common")!;
        On<UpdateFileStatus, OperationResult>(HandleUpdateFileStatus);
        On<DeleteFile, OperationResult>(HandleDeleteFile);
        On<ConvertFileToHolidaysCommand, OperationResult<List<Holiday>>>(HandleConvertFileToHolidays);

        On<ImportHolidayFromFileCommand, OperationResult>(HandleImportHolidayFromFile);


    }
    public async Task<IEnumerable<FileRecord>> GetPendingFiles()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryAsync<FileRecord>
            ("SELECT * FROM public.fileimport WHere FileStatus = @FileStatus",
            new { FileStatus = "Pending" });
    }



    private async ValueTask<OperationResult> HandleUpdateFileStatus(UpdateFileStatus command, CancellationToken cancellationToken)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync(
                "UPDATE public.fileimports SET FileStatus = @FileStatus WHERE Id = @Id",
                new { FileStatus = command.status, Id = command.id });

            return OperationResult.Success($"File status updated to {command.status} for File ID {command.id}");
        }
        catch (Exception ex)
        {
            return OperationResult.Failure($"Failed to update file status: {ex.Message}");
        }
    }



    //public async Task UpdateFileStatus(int id, string status)
    //{
    //    using var connection = new NpgsqlConnection(_connectionString);
    //    await connection.ExecuteAsync(
    //        "UPDATE public.fileimports SET FileStatus = @FileStatus WHERE Id = @Id",
    //        new { FileStatus = status, Id = id });
    //}


    private async ValueTask<OperationResult> HandleDeleteFile(DeleteFile command, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(command.FilePath) || !File.Exists(command.FilePath))
            {
                return OperationResult.Failure($"Invalid or non-existent file path: {command.FilePath}");
            }

            await Task.Run(() => File.Delete(command.FilePath));
            return OperationResult.Success($"File [{command.FilePath}] deleted successfully.");

        }
        catch (Exception ex)
        {
            return OperationResult.Failure($"Failed to delete file: {ex.Message}");
        }
    }

    //public async Task DeleteFileFromFolder(string filepath)
    //{
    //    await Task.Run(() => File.Delete(filepath));
    //}


    private async ValueTask<OperationResult<List<Holiday>>> HandleConvertFileToHolidays(ConvertFileToHolidaysCommand command, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(command.FilePath) || !File.Exists(command.FilePath))
            {
                return OperationResult<List<Holiday>>.Failure($"Invalid or non-existent file path: {command.FilePath}");
            }

            var holidays = new List<Holiday>();
            var lines = await File.ReadAllLinesAsync(command.FilePath);

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
            return OperationResult<List<Holiday>>.Success(holidays);
        }
        catch (Exception ex)
        {
            return OperationResult<List<Holiday>>.Failure($"Failed to convert file to holidays: {ex.Message}");
        }
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



    private async ValueTask<OperationResult> HandleImportHolidayFromFile(ImportHolidayFromFileCommand command, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(command.FilePath) || !File.Exists(command.FilePath))
            {
                return OperationResult.Failure($"Invalid or non-existent file path: {command.FilePath}");
            }

            var holidays = await ConvertFileToHolidays(command.FilePath);

            using var connection = new NpgsqlConnection(_connectionString);
            var query = @"INSERT INTO public.Holidays (Country, State, Date, HolidayName, IsGlobal)
                      VALUES (@Country, @State, @Date, @HolidayName, @IsGlobal)
                      ON CONFLICT (Date, Country, State) DO NOTHING";

            await connection.ExecuteAsync(query, holidays);

            return OperationResult.Success($"Holidays from file [{command.FilePath}] have been successfully imported.");
        }
        catch (Exception ex)
        {
            return OperationResult.Failure($"Failed to import holidays from file: {ex.Message}");
        }
    }




    //public async Task ImportHolidayFromFile(string filepath)
    //{
    //    var holidays = await ConvertFileToHolidays(filepath);

    //    using var connection = new NpgsqlConnection(_connectionString);
    //    var query = @"INSERT INTO public.Holidays (Country, State, Date, HolidayName, IsGlobal)
    //            VALUES (@Country, @State, @Date, @HolidayName, @IsGlobal)
    //                    ON CONFLICT (Date, Country, State) DO NOTHING";

    //    await connection.ExecuteAsync(query, holidays);

    //}
}
