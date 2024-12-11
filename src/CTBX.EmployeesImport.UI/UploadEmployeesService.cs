using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Json;
using CTBX.EmployeesImport.Shared;
using System.Text;
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
            // maximum size 1 MB
            using var stream = file.OpenReadStream(maxAllowedSize: 1024 * 1024);

            // Read file content as a stream
            var reader = new StreamReader(stream);
            var lines = await reader.ReadToEndAsync();

            // Split lines
            var dataRows = lines.Split(Environment.NewLine)
                                 .Where(line => !string.IsNullOrWhiteSpace(line))
                                 .ToList();
            foreach (var line in dataRows)
            {

                var columns = line.Split(';');

                if (columns.Length != 6)
                {
                    throw new Exception("The file content is not valid. Make sure that your file has excatly 6 columns");
                }
            }
            var uploadedFile = new FileData
            {
                FileName = file.Name,
                FileContent = Encoding.UTF8.GetBytes(lines),
                UploadTime = DateTimeOffset.UtcNow
            };
            
            var result = await _httpClient.PostAsJsonAsync(BackendRoutes.FILEUPLOAD, uploadedFile);
            result.EnsureSuccessStatusCode();
        }
    }
}




