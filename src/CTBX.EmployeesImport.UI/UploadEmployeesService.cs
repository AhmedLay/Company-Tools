using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Json;
using CTBX.EmployeesImport.Shared;
namespace CTBX.EmployeesImport.UI
{
    public class UploadEmployeesService
    {
        private readonly HttpClient _httpClient;
        public UploadEmployeesService(HttpClient httpClient)
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

            var result = await _httpClient.PostAsJsonAsync(BackendRoutes.FILEUPLOAD, uploadedFile);
            result.EnsureSuccessStatusCode();
        }
        //public async Task<bool> ValidateFileContent(IBrowserFile file)
        //{
        //    try
        //    {
        //        using var stream = file.OpenReadStream();
        //        using var reader = new StreamReader(stream);

        //        var lines = await reader.ReadToEndAsync();

        //        // Split lines
        //        var dataRows = lines.Split(Environment.NewLine)
        //                             .Where(line => !string.IsNullOrWhiteSpace(line))
        //                             .ToList();
        //        foreach (var line in dataRows)
        //        {

        //            var columns = line.Split(';');

        //            if (columns.Length != 6)
        //            {
        //                return false;
        //            }
        //        }

        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

    }
}




