using Carter;
using CTBX.EmployeesImport.Shared;
using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Npgsql;


namespace CTBX.EmployeesImport.Backend;

public class Endpoints : CarterModule
{
    
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        AddUploadEmployeesFilesEndpoint(app);
        AddTestEndpoint(app);
    }
    public static void AddUploadEmployeesFilesEndpoint(IEndpointRouteBuilder app)
    {

        app.MapPost(BackendRoutes.FILEUPLOAD, async (FileData file) =>
        {
            //just for testing, for now we have temp as folder 
            var folderpath = @"C:\Users\User\Desktop\TEST FOLDER";
            Console.WriteLine("hello world");

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

                ////were using dapper to put it to to database
                //string connectionstring = "filedbconnectstring";
                //using (var connection = new NpgsqlConnection(connectionstring))
                //{ // for build the connection with the db 
                //    var fileUpload = new FileRecord
                //    {
                //        FilePath = filePath,
                //        Status = "pending"
                //    };
                //    var insertquery = "INSERT INTO FileUploads (FilePath, Status) VALUES (@FilePath, @Status)";
                //    // queryasync / executeasync ? 
                //    await connection.ExecuteAsync(insertquery, fileUpload);
                //}

                return Results.Ok(new { Message = "File uploaded successfully ", FilePath = filePath });

            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred while uploading the file: {ex.Message}");
            }
        });

        



    }

    public static void AddTestEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(BackendRoutes.FILEUPLOAD, () => "HI from Employee Registration module.");
    }
}
