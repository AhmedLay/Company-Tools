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

    protected async Task LoadFiles(IReadOnlyList<IBrowserFile> files)
    {
        foreach (var file in files)
        {
            var validationResult = _validator.Validate(file);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    await NotifyError(error.ErrorMessage);
                }
                continue;
            }

            bool isValid = await Service.ValidateFileContent(file);
            if (!isValid)
            {
                await NotifyError("The file's content does not meet the required format. Each line must contain 6 columns separated by semicolons (;).");
                continue;
            }

            UploadedFiles.Add(file);
            await NotifySuccess($"File {file.Name} is registered an is ready to be uploaded!");
        }

    }
    public void RemoveFile(IBrowserFile file)
    {
        UploadedFiles.Remove(file);
    }

    protected async Task SubmitFiles()
    {
        foreach (var file in UploadedFiles)
        {
            await OnHandleOperation(
                operation: () => Service.UploadFile(file),
                successMssage: $"Upload succeeded for {file.Name}",
                errMessage: $"Something went wrong with {file.Name}!"
            );
        }
        UploadedFiles.Clear();
    }



}
