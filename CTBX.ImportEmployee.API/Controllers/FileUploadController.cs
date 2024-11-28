using CTBX.EmployeesImport.API.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CTBX.EmployeesImport.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        [HttpPost]
        [Route("save-file-to-physicallocation")]
        public async Task<IActionResult> SaveToPhysicalLocation([FromBody] SaveFile saveFile)
        {
            foreach(var file in saveFile.Files)
            {
                string fileExtenstion = file.FileType.ToLower().Contains("txt") ? "csv" : "txt";
                string filename = $@"D:\MyTest\{Guid.NewGuid()}.{fileExtenstion}";
                using( var fileStream = System.IO.File.Create(filename))
                {
                    await fileStream.WriteAsync(file.ImagesBytes);
                }
            }
            return Ok();
        }
    }
}
