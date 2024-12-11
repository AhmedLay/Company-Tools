using CTBX.CommonMudComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace CTBX.EmployeesImport.UI;

public class UploadEmployeeFileBase : BaseMudComponent
{
    [Inject]
    public required UploadEmployeesService Service { get; set; }

    [Inject]
    public new required ISnackbar Snackbar { get; set; }

    private readonly FileUploadValidator _validator = new();

    public List<IBrowserFile> UploadedFiles { get; set; } = new();

    protected Task LoadFiles(IReadOnlyList<IBrowserFile> files)
    {
        foreach (var file in files)
        {
            var validationResult = _validator.Validate(file);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    NotifyError(error.ErrorMessage);
                }
                continue;
            }

            UploadedFiles.Add(file);
            NotifySuccess($"File {file.Name} is registered an is ready to be uploaded!");
        }

        return Task.CompletedTask;
    }
    protected async Task SubmitFiles()
    {
        foreach (var file in UploadedFiles)
        {
            await OnHandleOperation(
                operation: () => Service.UploadFile(file),
                successMssage: $"Upload succeeded for {file.Name}",
                errMessage: $"Something went wrong with {file.Name}! make sure to have to right Formation."
            );
        }
        UploadedFiles.Clear();
    }
}
