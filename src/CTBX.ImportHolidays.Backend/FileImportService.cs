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
