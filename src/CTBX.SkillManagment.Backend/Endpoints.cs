using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace CTBX.SkillManagment.Backend;

public class Endpoints : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/ctbx/skills", ([FromServices] SkillsManager service) =>
        {
            var result = service.GetSkills();

            return Results.Ok(result);
        }
        );

        app.MapPost("api/ctbx/skills", ([FromServices] SkillsManager service, CreateSkill command) =>
        {
            service.AddSkill(command);

            return Results.Ok();
        }
        );

    }
}
public record CreateSkill(string Name, string Description, string Type);
public record Skill(int id, string Name, string Description, string Type);

