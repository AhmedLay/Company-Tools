using CTBX.CommonMudComponents;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using CTBX.ImportHolidays.Shared;

namespace CTBX.ImportHoliday.UI;

public class ImportHolidayFileBase : BaseMudComponent
{
    [Inject]
    public required ImportHolidaysService Service { get; set; }

    [Inject]
    public required IValidator<IBrowserFile> FileUploadValidator { get; set; }

    protected List<IBrowserFile> UploadedFiles { get; set; } = [];
    protected bool Visible { get; set; }
    protected List<FileRecord> FileRecodsList { get; set; } = [];

    protected string Width { get; set; } = string.Empty;

    protected string Height { get; set; } = string.Empty;


    public bool _open; // fix the naming

    public void OpenDrawer()
    {
        _open = true;
    }
    protected async Task LoadFiles(IReadOnlyList<IBrowserFile> files)
    {
        Visible = true;
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
        Visible = false;
    }
    public void RemoveFile(IBrowserFile file)
    {
        UploadedFiles.Remove(file);
    }

    protected async Task SubmitFiles()
    {
        Visible = true;
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
        Visible = false;
    }

    protected override async Task OnInitializedAsync()
    {
        await ReloadData();
    }

    public async Task ReloadData()
    {
        Visible = true;
        var result = await Service.GetFileRecordsAsync();
        FileRecodsList = [.. result];
        Visible = false;
    }

}

