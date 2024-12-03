using CTBX.EmployeesImport.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace CTBX.EmployeesImport.UI;

public class UploadEmployeesService
{

    private readonly HttpClient _httpClient;
    public UploadEmployeesService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task UploadFile(IBrowserFile file)
    {
        using var stream = file.OpenReadStream();
        var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        stream.Close();

        var uploadedFile = new FileData();
        uploadedFile.FileName = file.Name;
        uploadedFile.FileContent = ms.ToArray();

        ms.Close();
        await _httpClient.PostAsJsonAsync(BackendRoutes.FILEUPLOAD, uploadedFile);

    }




}

public static class BackendRoutes
{
    public const string FILEUPLOAD = "api/ctbx/fileupload";
}
