using Carter;
using CTBX.EmployeesImport.Shared;
using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Npgsql;


namespace CTBX.EmployeesImport.Backend;

public class Endpoints : CarterModule
{
    private readonly IConfiguration _configuration;

    // Constructor to accept IConfiguration
    public Endpoints(IConfiguration configuration)
    {
        _configuration = configuration;
    }


    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        AddUploadEmployeesFilesEndpoint(app);

    }
    public void AddUploadEmployeesFilesEndpoint(IEndpointRouteBuilder app)
    {

        app.MapPost(BackendRoutes.FILEUPLOAD, async (FileData file) =>
        {
            Console.WriteLine("POST-Endpoint got reached");
            //just for testing, for now we have temp as folder 
            var folderpath = @"C:\Users\User\Desktop\TEST FOLDER";
            try
            {
                // Ensure the folder exists
                if (!Directory.Exists(folderpath))
                {
                    // if it doenst it gets created
                    Directory.CreateDirectory(folderpath);
                }
                //sets the name of the file and the creates the final path
                var fileName = file.FileName!.GuardAgainstNullOrEmpty("fileName");
                var filePath = Path.Combine(folderpath, fileName);

                //checks if the file is empty, if yes returns a bad request
                if (file.FileContent == null || file.FileContent.Length == 0)
                {
                    return Results.BadRequest(new { Message = "File content is empty." });
                }
                // saves the file to the final path
                await File.WriteAllBytesAsync(filePath, file.FileContent);

                //were using dapper to put it to to database
                var connectionString = _configuration.GetConnectionString("employee-db");

                using (var connection = new NpgsqlConnection(connectionString))
                { // for build the connection with the db 
                    var fileUpload = new FileRecord
                    {
                        FileName = fileName,
                        FilePath = filePath,
                        FileStatus = "Pending"
                    };
                    var insertquery = "INSERT INTO EmployeeFile (FileName, FilePath, FileStatus) VALUES (@FileName, @FilePath, @FileStatus)";
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

}
