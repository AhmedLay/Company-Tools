using CTBX.EmployeesImport.Shared;
using Microsoft.AspNetCore.Components.Forms;
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

    public async Task SaveFileToFolder(IBrowserFile file, string folderPath)
    {
        // Überprüfen, ob der Ordner existiert, ansonsten erstellen
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Zielpfad zusammenstellen
        var filePath = Path.Combine(folderPath, file.Name);

        // Dateiinhalt lesen und speichern
        using var stream = file.OpenReadStream();
        using var fileStream = File.Create(filePath); // Datei erstellen im Zielordner
        await stream.CopyToAsync(fileStream); // Inhalt in die Datei kopieren
        stream.Close();
    }


}

public static class BackendRoutes
{
    public const string FILEUPLOAD = "api/ctbx/fileupload";
}
