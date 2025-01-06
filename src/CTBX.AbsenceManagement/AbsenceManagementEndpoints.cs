using Carter;
using DnsClient.Protocol;
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
    }

    private static void AddVacationsEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost(BackendRoutes.VacationScheduleURL, async (
        VacationScheduleCommand command,
        CancellationToken token,
        [FromServices] AbsenceManagementApplicationService service) =>
        {
            var result = await service.Handle(command, token);
            return Results.Ok(result);

        });

        app.MapGet(BackendRoutes.VacationDatagridURL, async (
        [FromServices] AbsenceManagementService service) =>
        {
            var vacationSchedules = await service.GetData();
            return Results.Ok(vacationSchedules);
        });

        app.MapGet(BackendRoutes.VacationCalenderViewURL, async (
        [FromServices] AbsenceManagementService service) =>
        {
            var vacationSchedules = await service.GetCalenderData();
            return Results.Ok(vacationSchedules);
        });

    }
}
