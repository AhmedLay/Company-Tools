using Carter;
using CTBX.EmployeesImport.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
namespace CTBX.EmployeesImport.Backend;

public class Endpoints : CarterModule
{
    private readonly EndpointServices Services;

    public Endpoints(IConfiguration configuration)
    {
        Services = new EndpointServices(configuration);
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        AddUploadEmployeesFilesEndpoint(app);
    }

    public void AddUploadEmployeesFilesEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(BackendRoutes.FILEUPLOAD, async (FileData file) =>
        {
            Console.WriteLine("POST-Endpoint got reached");

            //configure the folderpath here 
            var folderPath = @"C:\Users\User\Desktop\TEST FOLDER";
            try
            {
                var fileRecord = Services.CreateRecord(file, folderPath);
                await Services.SaveFileToFolder(folderPath, file);           
                await Services.InsertFileRecordAsync(fileRecord);

                return Results.Ok(new { Message = "File uploaded successfully"});
            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred while uploading the file: {ex.Message}");
            }
        });
    }
}

