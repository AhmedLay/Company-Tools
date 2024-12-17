using Carter;
using CTBX.CommonUtils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using CTBX.ImportHolidays.Shared;


namespace CTBX.ImportHolidays.Backend;
public class FileUploadOptions
{
    public string UploadDirectory { get; set; } = string.Empty;
}

public class Endpoints : CarterModule
    {
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        AddImportHolidaysFilesEndpoint(app);
    }

    public void AddImportHolidaysFilesEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(BackendRoutes.FILEUPLOAD, async (
            [FromServices] IFileUploadHandler service,
            [FromServices] IOptions<FileUploadOptions> options,
            [FromServices] IDateTimeProvider dateTimeProvider,
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

            return Results.Ok(new { Message = "File uploaded successfully" });

        });

    }

}

