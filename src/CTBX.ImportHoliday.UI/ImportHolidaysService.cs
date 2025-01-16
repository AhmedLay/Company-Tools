using CTBX.EmployeesImport.Shared;
using CTBX.ImportHolidays.Shared;
using Microsoft.AspNetCore.Components.Forms;
using NCommandBus.Core.Abstractions;
using System;
using System.Collections.Immutable;
using System.Net.Http.Json;
using System.Text.Json;

namespace CTBX.ImportHoliday.UI;

public class ImportHolidaysService
{
    private readonly HttpClient _httpClient;
    public ImportHolidaysService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task UploadFile(IBrowserFile file)
    {

        using var stream = file.OpenReadStream(maxAllowedSize: 10_000_000);
        using var memoryStream = new MemoryStream();

        await stream.CopyToAsync(memoryStream);

        var fileContent = memoryStream.ToArray();

        var uploadedFile = new FileData
        {
            FileName = file.Name,
            FileContent = fileContent,
        };
        Console.WriteLine("Before endpoint Hit !!!!");

        var result = await _httpClient.PostAsJsonAsync(BackendRoutes.HOLIDAYSFILES, uploadedFile);
        Console.WriteLine("after endpoint Hit !!!!");
        result.EnsureSuccessStatusCode();
        
    }


    public async Task<IImmutableList<FileRecord>> GetFileRecordsAsync()
    { 

        try
        {
            var response = await _httpClient.GetFromJsonAsync<IImmutableList<FileRecord>>(BackendRoutes.HOLIDAYSFILES);
   
            return response ?? ImmutableList<FileRecord>.Empty;

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: Failed to fetch file records: {ex.Message}");
            return ImmutableList<FileRecord>.Empty;
        }
    }

    
    public async Task SaveHolidays(IBrowserFile file)
    {

        using var stream = file.OpenReadStream(maxAllowedSize: 10_000_000);
        using var memoryStream = new MemoryStream();

        await stream.CopyToAsync(memoryStream);

        var fileContent = memoryStream.ToArray();

        var uploadedFile = new FileData
        {
            FileName = file.Name,
            FileContent = fileContent,
        };

        var result = await _httpClient.PostAsJsonAsync(BackendRoutes.HOLIDAYS, uploadedFile);
 
        result.EnsureSuccessStatusCode();
    }

    public async Task<IImmutableList<Holiday>> GetHolidaysAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<IImmutableList<Holiday>>(BackendRoutes.HOLIDAYS);
            return response ?? ImmutableList<Holiday>.Empty;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: Failed to fetch Holidays(from GetHolidaysAsync): {ex.Message}");
            return ImmutableList<Holiday>.Empty;
        }
    }
}



