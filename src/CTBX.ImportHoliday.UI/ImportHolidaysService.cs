using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using CTBX.ImportHolidays.Shared;
using System.Net.Http.Json;
using CTBX.EmployeesImport.Shared;

namespace CTBX.ImportHoliday.UI
{
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

            var result = await _httpClient.PostAsJsonAsync(BackendRoutes.FILEUPLOAD, uploadedFile);
            result.EnsureSuccessStatusCode();
        }

        public async Task<List<FileRecord>> GetFileRecordsAsync()
        {
            var fileRecords = await _httpClient.GetFromJsonAsync<List<FileRecord>>(BackendRoutes.GETFILERECORDS);
            return fileRecords ?? new List<FileRecord>();
        }

        public async Task<List<Holiday>> GetHolidaysAsync()
        {
            var employeeview = await _httpClient.GetFromJsonAsync<List<Holiday>>(BackendRoutes.GETEMPLOYEES);
            return employeeview ?? new List<Holiday>();
        }
    }
}
