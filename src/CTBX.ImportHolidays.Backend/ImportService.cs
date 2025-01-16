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


public record UploadHolidayFile(string FileName, byte[] Content);
public record GetAllFileRecords();
public record PersistHolidaysFromFile(string FileName, byte[] Content);
public record GetHolidaysData();


public class ImportService : CommandBusBase
{
    private readonly HolidayImporterOptions _options;
    private readonly IDateTimeProvider _dateTimeProvider;
    

    public ImportService(IOptions<HolidayImporterOptions> options,
                            IDateTimeProvider dateTimeProvider, IConfiguration configuration)
    {
        _options = options.Value.GuardAgainstNull(nameof(options));

        On<UploadHolidayFile, OperationResult>(HandleUpload);
        On<GetAllFileRecords, OperationResult<IImmutableList<FileRecord>>>(HandleGetAllFileRecords);

        On<PersistHolidaysFromFile, OperationResult>(HandleHolidaysPersistence);
        On<GetHolidaysData, OperationResult<IImmutableList<Holiday>>>(HandleGetHolidaysData);

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

    private async ValueTask<OperationResult<IImmutableList<FileRecord>>> HandleGetAllFileRecords(GetAllFileRecords command, CancellationToken cancellationToken)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_options.ConnectionString);
            const string selectQuery = "SELECT id, filename, filepath, filestatus, uploaddate FROM public.holidayimports";

            var result = await connection.QueryAsync<FileRecord>(selectQuery);
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




