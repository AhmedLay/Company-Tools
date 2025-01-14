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
        Console.WriteLine("Message1");
        var result = await _httpClient.PostAsJsonAsync(BackendRoutes.HOLIDAYSFILES, uploadedFile);
        Console.WriteLine("Message2");
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
            Console.WriteLine($"Error: Failed to fetch file records (from GetFromJsonAsync): {ex.Message}");
            return ImmutableList<FileRecord>.Empty;
        }


    }

    public async Task<List<Holiday>> GetHolidaysAsync()
    {
        var holidayview = await _httpClient.GetFromJsonAsync<List<Holiday>>(BackendRoutes.HOLIDAYS);
        return holidayview ?? [];
    }
}
