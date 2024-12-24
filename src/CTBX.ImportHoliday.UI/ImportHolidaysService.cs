using CTBX.EmployeesImport.Shared;
using CTBX.ImportHolidays.Shared;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Immutable;
using System.Net.Http.Json;

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

        var result = await _httpClient.PostAsJsonAsync(BackendRoutes.HOLIDAYSFILES, uploadedFile);
        result.EnsureSuccessStatusCode();
    }

    public async Task<ImmutableList<FileRecord>> GetFileRecordsAsync()
    {
        var fileRecords = await _httpClient.GetFromJsonAsync<ImmutableList<FileRecord>>(BackendRoutes.HOLIDAYSFILES);
        return fileRecords ?? [];
    }

    public async Task<ImmutableList<Holiday>> GetHolidaysAsync()
    {
        var employeeview = await _httpClient.GetFromJsonAsync<ImmutableList<Holiday>>(BackendRoutes.HOLIDAYS);
        return employeeview ?? [];
    }
}
