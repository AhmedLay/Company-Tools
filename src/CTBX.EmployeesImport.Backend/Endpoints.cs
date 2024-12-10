using Carter;
using CTBX.EmployeesImport.Shared;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
namespace CTBX.EmployeesImport.Backend;


public class FileUploadOptions
{
    public string UploadDirectory { get; set; } = string.Empty;
}

public class Endpoints : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        AddUploadEmployeesFilesEndpoint(app);
    }

    public void AddUploadEmployeesFilesEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(BackendRoutes.FILEUPLOAD, async ([FromServices] IFileUploadHandler service, [FromServices] IOptions<FileUploadOptions> options, FileData file) =>
        {
            service.GuardAgainstNull(nameof(service));
            var filename = file.FileName;
            var folderpath = options.GuardAgainstNull(nameof(options))
                                    .Value.UploadDirectory;
            var fileRecord = new FileRecord
            {
                FileName = filename,
                FilePath = Path.Combine(folderpath, filename),
                FileStatus = "Pending",
                UploadDate = DateTimeOffset.Now
            };

            await service.SaveFileToFolder(folderpath, file);
            await service.PersistToDb(fileRecord);

            return Results.Ok(new { Message = "File uploaded successfully" });
            
        });
    }
}

