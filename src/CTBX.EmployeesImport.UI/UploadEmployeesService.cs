using CTBX.EmployeesImport.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
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
        try
        {
            // Maximale Dateigröße: 10MB
            using var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);

            // Erstellen eines Puffers für die Dateidaten
            var fileContent = new byte[file.Size];

            // Erstellen eines Puffers, der während des Lesevorgangs verwendet wird
            var buffer = new byte[8192]; // 8 KB Puffergröße
            int bytesRead;
            int totalBytesRead = 0;

            // Lesen der Datei in Blöcken
            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                // Die gelesenen Bytes in das fileContent-Array kopieren
                Array.Copy(buffer, 0, fileContent, totalBytesRead, bytesRead);
                totalBytesRead += bytesRead;
            }

            // Überprüfen, ob die Datei korrekt gelesen wurde
            Console.WriteLine($"FileContent Length: {fileContent.Length}");

            // Erstellen des FileData-Objekts
            var uploadedFile = new FileData
            {
                FileName = file.Name,
                FileContent = fileContent
            };

            // Den Inhalt an den Server senden
            await _httpClient.PostAsJsonAsync(BackendRoutes.FILEUPLOAD, uploadedFile);

        }
        catch (Exception ex)
        {
            // Fehlerbehandlung
            Console.WriteLine($"Error uploading file: {ex.Message}");
            throw;
        }
    }




}





