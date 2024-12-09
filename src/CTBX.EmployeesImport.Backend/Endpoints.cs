using System.Xml.Linq;
using Carter;
using CTBX.EmployeesImport.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
namespace CTBX.EmployeesImport.Backend;

public class Endpoints : CarterModule
{
    private readonly FileUploadService _service;

    public Endpoints(IConfiguration configuration)
    {
        _service = new FileUploadService(configuration);
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        AddUploadEmployeesFilesEndpoint(app);
    }

    public void AddUploadEmployeesFilesEndpoint(IEndpointRouteBuilder app)
    {
        // validate via fluent validation
        app.MapPost(BackendRoutes.FILEUPLOAD, async (FileData file) =>
        {
            Console.WriteLine("POST-Endpoint got reached");
            //TODO handle exception globally 
            //configure the folderpath here 
            var folderPath = @"C:\Users\User\Desktop\TEST FOLDER";
            var filename = file.FileName!.GuardAgainstNullOrEmpty("fileName");
            try
            {
                var fileRecord = new FileRecord
                {
                    FileName = filename,
                    FilePath = Path.Combine(folderPath, filename),
                    FileStatus = "pending",
                    UploadDate = DateTime.Now
                };
                //do it here;
                await _service.SaveFileToFolder(folderPath, file);           
                await _service.PersistToDb(fileRecord); 

                return Results.Ok(new { Message = "File uploaded successfully"});
            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred while uploading the file: {ex.Message}");
            }
        });
    }
}

