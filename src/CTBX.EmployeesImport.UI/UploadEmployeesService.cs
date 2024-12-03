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
        // FOLDER PART
        //check if the path exists
        if (!Directory.Exists(folderPath))
        { // if it doenst it gets created 
            Directory.CreateDirectory(folderPath);
        }
        //sets the path and the file 
        var filePath = Path.Combine(folderPath, file.Name);

        // open a stream
        using var stream = file.OpenReadStream();
        using var fileStream = File.Create(filePath); // creates a file in the folder
        await stream.CopyToAsync(fileStream); // copy the data from the file 
        stream.Close(); // close the stream

        //DATABASE PART


    }


}

public static class BackendRoutes
{
    public const string FILEUPLOAD = "api/ctbx/fileupload";
}
