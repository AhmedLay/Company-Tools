using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Moq;
using Xunit;
using CTBX.EmployeesImport.UI;
using Microsoft.AspNetCore.Components.Forms;
using CTBX.EmployeesImport.Shared;


namespace CTBX.EmployeesImportTests
{
    public class UploadServiceTest
    {
        private readonly Mock<HttpClient> _httpClientMock;
        private readonly UploadEmployeesService _uploadEmployeesService;

        public UploadServiceTest()
        {
            _httpClientMock = new Mock<HttpClient>();

            // Initialize the service
            _uploadEmployeesService = new UploadEmployeesService(_httpClientMock.Object);
        }
    
        [Fact]
        public void TESTMETHODNAME()
        {
  
        }
    }
}
