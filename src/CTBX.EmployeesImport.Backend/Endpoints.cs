using Carter;
using CTBX.EmployeesImport.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
namespace CTBX.EmployeesImport.Backend;

public class Endpoints : CarterModule
{
    private readonly EmployeeQueryService _QueryService;

    public Endpoints(IConfiguration configuration)
    {
        _QueryService = new EmployeeQueryService(configuration);
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

            var folderPath = @"C:\Users\User\Desktop\TEST FOLDER";

            try
            {
                // Ensure the folder exists
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Set the name of the file and create the final path
                var fileName = file.FileName!.GuardAgainstNullOrEmpty("fileName");
                var filePath = Path.Combine(folderPath, fileName);

                // Check if the file is empty
                if (file.FileContent == null || file.FileContent.Length == 0)
                {
                    return Results.BadRequest(new { Message = "File content is empty." });
                }

                // Save the file to the final path
                await File.WriteAllBytesAsync(filePath, file.FileContent);

                // Create a file record and delegate DB interaction to service clas 
                var fileRecord = new FileRecord
                {
                    FileName = fileName,
                    FilePath = filePath,
                    FileStatus = "Pending",
                    UploadDate = DateTime.Now
                };

                await _QueryService.InsertFileRecordAsync(fileRecord);

                return Results.Ok(new { Message = "File uploaded successfully", FilePath = filePath });
            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred while uploading the file: {ex.Message}");
            }
        });
    }
}

