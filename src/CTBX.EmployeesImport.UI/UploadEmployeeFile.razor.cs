using CTBX.CommonMudComponents;
using Microsoft.AspNetCore.Components.Forms;

namespace CTBX.EmployeesImport.UI;

public class UploadEmployeeFileBase : BaseMudComponent
{
    private readonly UploadEmployeesService _service;

    public UploadEmployeeFileBase(UploadEmployeesService Service)
    {
        _service = Service;
    }

    protected async Task UploadFiles(IReadOnlyList<IBrowserFile> files)
    {
        foreach (var file in files)
        {
            // TODO: use fluent validation
            //if (uploadedFileNames.Contains(file.Name))
            //{
            //    Console.WriteLine($"File {file.Name} has already been uploaded.");
            //    Snackbar.Add("This file has already been uploaded. Please try a different file.", Severity.Error);
            //    continue;
            //}
            //if (file.Size == 0)
            //{
            //    Snackbar.Add("The file you are trying to upload is empty. Please try again.", Severity.Error);
            //    continue;
            //}
            //if (file.Size > 1000000)
            //{
            //    Snackbar.Add("The file you are trying to upload is too big. Please try again.", Severity.Error);
            //    continue;
            //}

            await OnHandleOperation(operation    : ()=> _service.UploadFile(file),
                                    successMssage: "Upload succeeded",
                                    errMessage   : "File upload failed");
        }
    }
}
