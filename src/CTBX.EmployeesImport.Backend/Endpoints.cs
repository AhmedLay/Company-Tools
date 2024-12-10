using Carter;
using CTBX.EmployeesImport.Shared;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
namespace CTBX.EmployeesImport.Backend;

public class Endpoints : CarterModule
{
    private readonly FileUploadService _service;
    private readonly string _folderPath;

    public Endpoints(IConfiguration configuration)
    {
        _service = new FileUploadService(configuration);
        //not working rn 
        _folderPath = configuration["FileUploadConfiguration:FolderPath"] ?? "default/path";
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        AddUploadEmployeesFilesEndpoint(app);
    }
    public void AddUploadEmployeesFilesEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(BackendRoutes.FILEUPLOAD, async (FileData file) =>
        {        
                var filename = file.FileName;
                var folderpath = _folderPath;
                var fileRecord = new FileRecord
                {
                    FileName = filename,
                    FilePath = Path.Combine(folderpath, filename),
                    FileStatus = "Pending",
                    UploadDate = DateTimeOffset.Now
                };
                await _service.SaveFileToFolder(folderpath, file);
                await _service.PersistToDb(fileRecord);
                return Results.Ok(new { Message = "File uploaded successfully" });
            
        });
    }
}

