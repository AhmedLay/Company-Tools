using System.Collections.Immutable;
using CTBX.CommonUtils;
using CTBX.ImportHolidays.Shared;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NCommandBus.Core.Abstractions;
using Npgsql;
using YAEP.Utils;

namespace CTBX.ImportHolidays.Backend;

public record UploadHolidayFile(string FileName, byte[] Content);
public record GetAllFileRecords();
public record PersistHolidaysFromFile(string FileName, byte[] Content);
public record GetHolidaysData();

public class ImportService : CommandBusBase
{
    private readonly HolidayImporterOptions _options;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly FluentValidator _validator;

    public ImportService(
        IOptions<HolidayImporterOptions> options,
        IDateTimeProvider dateTimeProvider,
        IConfiguration configuration,
        FluentValidator validator)
    {
        _options = options.Value.GuardAgainstNull(nameof(options));
        _dateTimeProvider = dateTimeProvider.GuardAgainstNull(nameof(dateTimeProvider));
        _validator = validator;

        On<UploadHolidayFile, OperationResult>(HandleUpload);
        On<GetAllFileRecords, OperationResult<IImmutableList<FileRecord>>>(HandleGetAllFileRecords);
        On<PersistHolidaysFromFile, OperationResult>(HandleHolidaysPersistence);
        On<GetHolidaysData, OperationResult<IImmutableList<Holiday>>>(HandleGetHolidaysData);
    }

    private async ValueTask<OperationResult> HandleUpload(UploadHolidayFile command, CancellationToken cancellationToken)
    {
        var fileData = new FileData
        {
            FileName = command.FileName,
            FileContent = command.Content
        };

        var validationResult = await _validator.ValidateAsync(fileData, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
            return OperationResult.Failure($"Validation failed: {string.Join(", ", errors)}");
        }

        if (!Directory.Exists(_options.UploadDirectory))
        {
            Directory.CreateDirectory(_options.UploadDirectory);
        }

        var filePath = Path.Combine(_options.UploadDirectory, command.FileName);

        try
        {
            await File.WriteAllBytesAsync(filePath, command.Content).AsValueTask();
            await PersistToDb(new FileRecord
            {
                FilePath = filePath,
                FileName = command.FileName,
                UploadDate = _dateTimeProvider.UtcNow
            });

            return OperationResult.Success($"File [{command.FileName}] uploaded to [{_options.UploadDirectory}]");
        }
        catch (Exception ex)
        {
            return OperationResult.Failure($"Failed to upload file: {ex.Message}");
        }
    }

    private async Task PersistToDb(FileRecord fileRecord)
    {
        var insertQuery = "INSERT INTO public.holidayimports (FileName, FilePath, UploadDate) VALUES (@FileName, @FilePath, @UploadDate)";

        using var connection = new NpgsqlConnection(_options.ConnectionString);
        connection.Open();

        await using var command = new NpgsqlCommand(insertQuery, connection);

        command.Parameters.AddWithValue("@FileName", fileRecord.FileName);
        command.Parameters.AddWithValue("@FilePath", fileRecord.FilePath);
        command.Parameters.AddWithValue("@UploadDate", fileRecord.UploadDate);

        await command.ExecuteNonQueryAsync();
    }

    private async ValueTask<OperationResult<IImmutableList<FileRecord>>> HandleGetAllFileRecords(GetAllFileRecords command, CancellationToken cancellationToken)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_options.ConnectionString);
            const string selectQuery = "SELECT id, filename, filePath, uploaddate FROM public.holidayimports";

            var result = await connection.QueryAsync<FileRecord>(selectQuery);
            var fileRecords = result?.ToImmutableList() ?? ImmutableList<FileRecord>.Empty;

            return OperationResult.Success("Successfully retrieved file records.", (IImmutableList<FileRecord>)fileRecords);
        }
        catch (Exception ex)
        {
            return (OperationResult<IImmutableList<FileRecord>>)OperationResult.Failure($"Failed to retrieve file records: {ex.Message}");
        }
    }

    private async ValueTask<OperationResult> HandleHolidaysPersistence(PersistHolidaysFromFile command, CancellationToken cancellationToken)
    {
        var filePath = Path.Combine(_options.UploadDirectory, command.FileName);

        if (!File.Exists(filePath))
        {
            return OperationResult.Failure($"File not found: {filePath}");
        }

        try
        {
            var holidays = new List<Holiday>();
            var lines = await File.ReadAllLinesAsync(filePath, cancellationToken);

            foreach (var line in lines)
            {
                var parts = line.Split(';');
                if (parts.Length < 5)
                {
                    return OperationResult.Failure("Invalid file format.");
                }

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

            File.Delete(filePath);
            return OperationResult.Success($"Holidays Imported Successfully from File [{command.FileName}]");
        }
        catch (Exception ex)
        {
            return OperationResult.Failure($"Failed to persist holidays: {ex.Message}");
        }
    }

    private async Task PersistHolidaysToDb(Holiday holiday)
    {
        var insertQuery = @"
        INSERT INTO public.holidays (Country, State, HolidayName, HolidayDate, IsGlobal) 
        VALUES (@Country, @State, @HolidayName, @HolidayDate, @IsGlobal)";

        using var connection = new NpgsqlConnection(_options.ConnectionString);
        connection.Open();

        await using var command = new NpgsqlCommand(insertQuery, connection);

        command.Parameters.AddWithValue("@Country", holiday.Country);
        command.Parameters.AddWithValue("@State", holiday.State);
        command.Parameters.AddWithValue("@HolidayName", holiday.HolidayName);
        command.Parameters.AddWithValue("@HolidayDate", holiday.HolidayDate);
        command.Parameters.AddWithValue("@IsGlobal", holiday.IsGlobal);

        await command.ExecuteNonQueryAsync();
    }

    private async ValueTask<OperationResult<IImmutableList<Holiday>>> HandleGetHolidaysData(GetHolidaysData query, CancellationToken cancellationToken)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_options.ConnectionString);
            const string selectQuery = "SELECT Id, Country, State, HolidayName, HolidayDate, IsGlobal FROM public.Holidays";
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
