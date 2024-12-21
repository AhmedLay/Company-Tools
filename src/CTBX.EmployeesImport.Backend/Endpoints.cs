using Carter;
using CTBX.CommonUtils;
using CTBX.EmployeesImport.Shared;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
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
        AddGetFileRecordsEndpoint(app);
    }

    public void AddUploadEmployeesFilesEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(BackendRoutes.FILEUPLOAD, async (
            [FromServices] IFileUploadHandler service,
            [FromServices] IOptions<FileUploadOptions> options,
            [FromServices] IDateTimeProvider dateTimeProvider,
            [FromServices] FileImporter fileImporter,
            FileData file) =>
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
                UploadDate = dateTimeProvider.UtcNow
            };

            await service.SaveFileToFolder(folderpath, file);
            await service.PersistToDb(fileRecord);



            BackgroundJob.Enqueue(() => fileImporter.ImportEmployeeFromFile());
            return Results.Ok(new { Message = "File uploaded successfully" });
            
        });
    }

    public void AddGetFileRecordsEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(BackendRoutes.GETFILERECORDS, async (
            [FromServices] IFileUploadHandler service) =>
        {
            service.GuardAgainstNull(nameof(service));
            var records = await service.GetAllFileRecordsAsync();
            return Results.Ok(records);
        });
    }


}

