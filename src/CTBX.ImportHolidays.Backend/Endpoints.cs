using Carter;
using CTBX.CommonUtils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using CTBX.ImportHolidays.Shared;
using NCommandBus.Core.Abstractions;
using System.Collections.Immutable;
using System.Threading;


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

        SaveHolidaysToDBEndPoint(app);

        AddGetHolidaysEndpoint(app);
    }
    // Working: Upload Holidays
    private void AddImportHolidaysFilesEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(BackendRoutes.HOLIDAYSFILES, async (
            [FromServices] HolidaysImporter service,
            FileData file,
            CancellationToken cancellationToken) =>
        {

            var result1 = await service.Handle<OperationResult>(new UploadHolidayFile(file.FileName, file.FileContent), cancellationToken);
            return result1 switch
            {
                _ when result1.IsSuccess => Results.Ok(new { result1.Message }),
                _ when result1.IsFailure => Results.BadRequest(new { result1.Message }),
                _ => Results.NotFound()
            };

        }); 
    }
    //Working: Get file records
    private void AddGetFileRecordsEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(BackendRoutes.HOLIDAYSFILES, async (
            [FromServices] FileUploadCommandHandler service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.Handle<OperationResult<IImmutableList<FileRecord>>>(new GetAllFileRecordsQuery(), cancellationToken);

            
            return result switch
            {
                _ when result.IsSuccess => Results.Ok(result.Value),
                _ when result.IsFailure => Results.BadRequest(new { result.Message }),
                _ => Results.NotFound()
            };
        });
    }


    // using this one currently
    private void SaveHolidaysToDBEndPoint(IEndpointRouteBuilder app)
    {
        app.MapPost(BackendRoutes.HOLIDAYS, async (
           [FromServices] HolidaysImporter service,
           FileData file,
           CancellationToken cancellationToken) =>
        {

            var result = await service.Handle<OperationResult>(new PersistHolidaysFromFile(file.FileName, file.FileContent), cancellationToken);
            return result switch
            {
                _ when result.IsSuccess => Results.Ok(new { result.Message }),
                _ when result.IsFailure => Results.BadRequest(new { result.Message }),
                _ => Results.NotFound()
            };

        });
    }




    private void AddGetHolidaysEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(BackendRoutes.HOLIDAYS, async (
            [FromServices] FileUploadCommandHandler service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.Handle<OperationResult>(new GetHolidaysDataQuery(), cancellationToken);

            return result switch
            {
                _ when result.IsSuccess => Results.Ok(new { result.Message }),
                _ when result.IsFailure => Results.BadRequest(new { result.Message }),
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

    
}

