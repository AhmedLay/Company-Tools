using Carter;
using CTBX.Backend;
using Dapper;
using Microsoft.Data.SqlClient;



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
            //just for testing, for now we have temp as folder 
            var folderpath = Path.GetTempPath();  

            string connectionstring = "filedbconnectstring";

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

                //were using dapper to put it to to database 
                using (var connection = new SqlConnection(connectionstring))
                { // for build the connection with the db 
                    var fileUpload = new FileRecord
                    {
                        FilePath = filePath,
                        Status = "pending"
                    };
                    var insertquery = "INSERT INTO FileUploads (FilePath, Status) VALUES (@FilePath, @Status)";
                    // queryasync / executeasync ? 
                    await connection.ExecuteAsync(insertquery, fileUpload);
                }

                return Results.Ok(new { Message = "File uploaded successfully ", FilePath = filePath });

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

    public class FileRecord
    {
        public int Id { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }


}
