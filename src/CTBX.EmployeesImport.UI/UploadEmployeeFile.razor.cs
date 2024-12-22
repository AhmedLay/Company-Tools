using CTBX.CommonMudComponents;
using CTBX.EmployeesImport.Shared;
using FluentValidation;
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
    [Inject]
    public required IValidator<IBrowserFile> FileUploadValidator { get; set; }

    public List<IBrowserFile> UploadedFiles { get; set; } = new();
    public bool _visible = false;
    public List<FileRecord> fileRecodsList = new();
    public required string _width, _height;
    public bool _open;

    public void OpenDrawer()
    {
    _open = true;
    }
    protected async Task LoadFiles(IReadOnlyList<IBrowserFile> files)
    {
        _visible = true;
        foreach (var file in files)
        {
           
            var validationResult = await FileUploadValidator.ValidateAsync(file);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    await NotifyError(error.ErrorMessage);
                }
                continue;
            }
            UploadedFiles.Add(file);
            await NotifySuccess($"File {file.Name} is registered an is ready to be uploaded!");
            
        }
        _visible = false;
    }
    public void RemoveFile(IBrowserFile file)
    {
        UploadedFiles.Remove(file);
    }

    protected async Task SubmitFiles()
    {
        _visible = true;
        foreach (var file in UploadedFiles)
        {
            await OnHandleOperation(
                operation: () => Service.UploadFile(file),
                successMssage: $"Upload succeeded for {file.Name}",
                errMessage: $"Something went wrong with!"
            );
        }
        await ReloadData();
        UploadedFiles.Clear();
         _visible = false;
    }

    protected override async Task OnInitializedAsync()
    {
        await ReloadData();
    }

    public async Task ReloadData()
    {
        _visible = true;
        fileRecodsList = await Service.GetFileRecordsAsync() ?? new List<FileRecord>();
        _visible = false;
    }

}
