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

            try
            {
                // Ensure the folder exists
                if (!Directory.Exists(folderpath))
                {
                    // if it doenst it gets created
                    Directory.CreateDirectory(folderpath);
                }
                //sets the name of the file and the creates the final path
                var fileName = file.FileName;
                var filePath = Path.Combine(folderpath, fileName);

                //checks if the file is empty, if yes returns a bad request
                if (file.FileContent == null || file.FileContent.Length == 0)
                {
                    return Results.BadRequest(new { Message = "File content is empty." });
                }
                // saves the file to the final path
                await File.WriteAllBytesAsync(filePath, file.FileContent);

                return Results.Ok(new { Message = "File uploaded successfully to ", FilePath = filePath });

            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred while uploading the file: {ex.Message}");
            }
        });

    }




    public class FileData
    {
        public string FileName { get; set; } = string.Empty;
        public byte[]? FileContent { get; set; }
        public string Id { get; set; } = "";
    }

}
