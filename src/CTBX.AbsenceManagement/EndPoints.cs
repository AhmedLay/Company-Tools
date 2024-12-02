using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MinimalApiArchitecture.Application;

public class EndPoints : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        // fetch absences (with filter query stirngs)
        app.MapGet("/absences", () => "Absence list here");
        AddVacationsEndpoints(app);
        AddSickLeavesEndpoints(app);
    }

    private static void AddVacationsEndpoints(IEndpointRouteBuilder app)
    {
        // add vacation schedule
        app.MapPost("/vacations/", () => "Hello from Carter!");
        // change vacation schedule
        app.MapPut("/vacations/id/schedule", () => "Hello from Carter!");
        app.MapPut("/vacations/id/submition", () => "Hello from Carter!");
        app.MapPut("/vacations/id/approval", () => "Hello from Carter!");
        app.MapPut("/vacations/id/rejection", () => "Hello from Carter!");

        // abondon vacation schedule
        app.MapDelete("/vacation/id/", () => "Hello from Carter!");
    }

    private static void AddSickLeavesEndpoints(IEndpointRouteBuilder app)
    {
        // add vacation schedule
        app.MapPost("/sick-leaves/", () => "Hello from Carter!");
    }


}
