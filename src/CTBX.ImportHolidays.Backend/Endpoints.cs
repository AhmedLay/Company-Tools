using Carter;
using CTBX.CommonUtils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using CTBX.ImportHolidays.Shared;
using NCommandBus.Core.Abstractions;


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
        AddGetFileRecordsEndpoint(app);
        AddGetHolidaysEndpoint(app);
    }

    private void AddImportHolidaysFilesEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(BackendRoutes.HOLIDAYSFILES, async (
            [FromServices] HolidaysImporter service,
            FileData file,
            CancellationToken cancellationToken) =>
        {
            var result = await service.Handle<OperationResult>(new UploadHolidayFile(file.FileName, file.FileContent),cancellationToken);

            return result switch
            {
                _ when result.IsSuccess => Results.Ok(new { result.Message }),
                _ when result.IsFailure => Results.BadRequest(new { result.Message }),
                _ => Results.NotFound()
            };
        });
        //app.MapPost(BackendRoutes.HOLIDAYSFILES, async (
        //    [FromServices] IFileUploadHandler service,
        //    [FromServices] IOptions<FileUploadOptions> options,
        //    [FromServices] IDateTimeProvider dateTimeProvider,
        //    FileData file) =>
        //{
        //    service.GuardAgainstNull(nameof(service));

        //    var filename = file.FileName;
        //    var folderpath = options.GuardAgainstNull(nameof(options))
        //                            .Value.UploadDirectory;
        //    var fileRecord = new FileRecord
        //    {
        //        FileName = filename,
        //        FilePath = Path.Combine(folderpath, filename),
        //        FileStatus = "Pending",
        //        UploadDate = dateTimeProvider.UtcNow
        //    };

        //    await service.SaveFileToFolder(folderpath, file);
        //    await service.PersistToDb(fileRecord);

        //    return Results.Ok(new { Message = "File uploaded successfully" });

        //});

    }



    private void AddGetHolidaysEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(BackendRoutes.HOLIDAYS, async (
            [FromServices] FileUploadCommandHandler service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.Handle(new GetHolidaysDataQuery(), cancellationToken);

            return result switch
            {
                _ when result.IsSuccess => Results.Ok(result.Value),
                _ when result.IsFailure => Results.BadRequest(new { result.ErrorMessage }),
                _ => Results.NotFound()
            };
        });
    }



    //private void AddGetHolidaysEndpoint(IEndpointRouteBuilder app)
    //{
    //    app.MapGet(BackendRoutes.HOLIDAYS, async([FromServices] IFileUploadHandler service) =>
    //    {
    //        service.GuardAgainstNull(nameof(service));
    //        var records = await service.GetHolidaysDataAsync();
    //        return Results.Ok(records);
    //    });

    //}

    private void AddGetFileRecordsEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(BackendRoutes.HOLIDAY, async (
            [FromServices] FileUploadCommandHandler service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.Handle(new GetAllFileRecordsQuery(), cancellationToken);

            return result switch
            {
                _ when result.IsSuccess => Results.Ok(result.Value),
                _ when result.IsFailure => Results.BadRequest(new { result.ErrorMessage }),
                _ => Results.NotFound()
            };
        });
    }
}

