using Carter;
using CTBX.AbsenceManagement.Shared.AbsenceManagerCommands;
using CTBX.AbsenceManagement.Shared.DTOs;
using DnsClient.Protocol;
using Eventuous;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MongoDB.Driver;

namespace MinimalApiArchitecture.Application;
public class AbsenceManagementEndpoints : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/absences", () => "Absence list here");
        AddVacationsEndpoints(app);
        RequestSickLeaveEndpoint(app);
        GetSickLeaveRequestsEndpoint(app);
    }

    private static void AddVacationsEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost(BackendRoutes.VacationScheduleURL, async (
        VacationScheduled command,
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

    private static void GetSickLeaveRequestsEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(BackendRoutes.SICKLEAVEDATA, async(
            [FromServices] AbsenceManagementService service) =>
        {
            //var sickLeaveData = await service.GetSickLeaveData();
            //return sickLeaveData.Any() ? Results.Ok(sickLeaveData) : Results.NotFound("No sick leave requests found.");
            await Task.Delay(1);
        });

    }
}
