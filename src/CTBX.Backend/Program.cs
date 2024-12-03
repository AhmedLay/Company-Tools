using System.Collections.Immutable;
using System.Security.Principal;
using Carter;
using CTBX.Backend;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCarter();
builder.AddMongoDBClient("ctbx-read-db");
builder.AddServiceDefaults();
builder.Services.AddCors(opts=>opts.AddPolicy("all",p=> p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
builder.Services.RegisterJWTBearerAuthNService(builder.Configuration);
builder.Services.RegisterEventuousStores(builder.Configuration);
var app = builder.Build();
app.UseCors("all");
app.MapDefaultEndpoints();

app.MapCarter();
app.Run();

public static class Endpoints
{
    public static void MapUploadEmployeesFiles(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/ctbx/fileupload", async (FileData file) =>
        {
            var folderpath = @"C:\Users\User\Desktop\CompanyToolbox\src\CTBX.EmployeesImport.Shared\uploadedFiles\";
            var filePath = Path.Combine(folderpath, file.FileName);

            if (System.IO.File.Exists(filePath))
            {
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                return Results.File(fileBytes, "text/plain", file.FileName);
            }

            return Results.NotFound();
        });

    }

}
public class FileData
{
    public string FileName { get; set; } = string.Empty;
    public byte[]? FileContent { get; set; }
    public string Id { get; set; } = "";
}
