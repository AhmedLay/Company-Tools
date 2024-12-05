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
            // maximum size 10 mb 
            using var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);

            // create a buffer 
            var fileContent = new byte[file.Size];

         
            var buffer = new byte[8192]; // 8 KB Puffergröße
            int bytesRead;
            int totalBytesRead = 0;


            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
            
                Array.Copy(buffer, 0, fileContent, totalBytesRead, bytesRead);
                totalBytesRead += bytesRead;
            }

         
            Console.WriteLine($"FileContent Length: {fileContent.Length}");

          
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





