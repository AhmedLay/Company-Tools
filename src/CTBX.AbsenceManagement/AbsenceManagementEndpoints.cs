using Carter;
using Eventuous;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MinimalApiArchitecture.Application.Commands;
using MongoDB.Driver;

namespace MinimalApiArchitecture.Application;
public class AbsenceManagementEndpoints : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/absences", () => "Absence list here");
        AddVacationsEndpoints(app);
        AddSickLeavesEndpoints(app);
    }

    private static void AddVacationsEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost(BackendRoutes.VacationScheduleURL, async (
        VacationScheduleCommand command,
        CancellationToken token,
        [FromServices] AbsenceManagementApplicationService service) =>
        {
            Console.WriteLine("test");
            var result = await service.Handle(command, token);
            return result.Success ? Results.Ok("Sucess") : Results.BadRequest("Something went wrong");
            
        });
    }
    private static void AddSickLeavesEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/sick-leaves/", () => "Hello from Carter!");
    }
}
