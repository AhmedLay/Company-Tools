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
        
    }
}




