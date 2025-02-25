using Carter;
using CTBX.AbsenceManagement.Shared;
using Eventuous;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace MinimalApiArchitecture.Application;
public class AbsenceManagementEndpoints : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/absences", () => "Absence list here");
        AddVacationsEndpoints(app);
        RequestSickLeaveEndpoint(app);
    }

    private static void AddVacationsEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost(BackendRoutes.VacationScheduleURL, async (
        SchedulingVacation command,
        CancellationToken token,
        [FromServices] AbsenceManagementApplicationService service) =>
        {
            var result = await service.Handle(command, token);
            return Results.Ok(result);
        });

        app.MapPost(BackendRoutes.EditVacation, async (
        ChangingVacationSchedule command,
        CancellationToken token,
        [FromServices] AbsenceManagementApplicationService service) =>
        {
            var result = await service.Handle(command, token);
            return Results.Ok(result);

        });

        app.MapGet(BackendRoutes.VacationCalenderViewURL, async (
        [FromServices] AbsenceManagementService service) =>
        {
            var vacationSchedules = await service.GetCalenderData();
            return Results.Ok(vacationSchedules);
        });

    }

    private static void RequestSickLeaveEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(BackendRoutes.SICKLEAVEREQUEST, async (
            RequestingSickLeave command,
            CancellationToken token,
            [FromServices] AbsenceManagementApplicationService service) =>
        {
            var result = await service.Handle(command, token);
            return Results.Ok(result);
        });
    }
}
