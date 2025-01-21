using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using CTBX.ImportHolidays.Shared;
using NCommandBus.Core.Abstractions;
using System.Collections.Immutable;



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
        GetHolidaysEndpoint(app);
    }
 
    private void AddImportHolidaysFilesEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(BackendRoutes.HOLIDAYSFILES, async (
            [FromServices] ImportService service,
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
 
    private void AddGetFileRecordsEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(BackendRoutes.HOLIDAYSFILES, async (
            [FromServices] ImportService service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.Handle<OperationResult<IImmutableList<FileRecord>>>(new GetAllFileRecords(), cancellationToken);

            
            return result switch
            {
                _ when result.IsSuccess => Results.Ok(result.Value),
                _ when result.IsFailure => Results.BadRequest(new { result.Message }),
                _ => Results.NotFound()
            };
        });
    }


    private void SaveHolidaysToDBEndPoint(IEndpointRouteBuilder app)
    {
        app.MapPost(BackendRoutes.HOLIDAYS, async (
           [FromServices] ImportService service,
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

    private void GetHolidaysEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(BackendRoutes.HOLIDAYS, async (
            [FromServices] ImportService service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.Handle<OperationResult<IImmutableList<Holiday>>>(new GetHolidaysData(), cancellationToken);

            return result switch
            {
                _ when result.IsSuccess => Results.Ok(result.Value ),
                _ when result.IsFailure => Results.BadRequest(new { result.Message }),
                _ => Results.NotFound()
            };
        });
    }
    
}

