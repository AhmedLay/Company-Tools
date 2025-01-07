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

    protected List<IBrowserFile> UploadedFiles { get; set; } = new();
    protected bool Visible { get; set; }
    protected List<FileRecord> FileRecodsList { get; set; } = new();

    protected string Width { get; set; } = string.Empty;
    protected string Height { get; set; } = string.Empty;

    public bool Open { get; set; } // Updated naming

    public void OpenDrawer()
    {
        Open = true;
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

            if (!UploadedFiles.Contains(file)) 
            {
                UploadedFiles.Add(file);
                await NotifySuccess($"File {file.Name} is registered and is ready to be uploaded!");
            }
        }

        Visible = false;
    }

    public void RemoveFile(IBrowserFile file)
    {
        UploadedFiles.Remove(file);
    }

    protected async Task SubmitFiles()
    {
        if (!UploadedFiles.Any())
        {
            await NotifyError("No files to submit.");
            return;
        }

        Visible = true;

        foreach (var file in UploadedFiles)
        {
            await OnHandleOperation(
                operation: () => Service.UploadFile(file),
                successMssage: $"Upload succeeded for {file.Name}",
                errMessage: $"Upload failed for {file.Name}."
            );
        }

        UploadedFiles.Clear(); // Clear only after successful uploads
        await ReloadData();
        Visible = false;
    }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await ReloadData();
        }
        catch (Exception ex)
        {
            await NotifyError($"Failed to load file records: {ex.Message}");
        }
    }

    public async Task ReloadData()
    {
        Visible = true;

        try
        {
            //var fileRecords = await Service.GetFileRecordsAsync();
            //FileRecodsList = fileRecords.ToList(); // Convert ImmutableList to List


            FileRecodsList = await Service.GetFileRecordsAsync() ?? new List<FileRecord>();
           
        }
        catch (Exception ex)
        {
            await NotifyError($"Failed to reload data: {ex.Message}");
        }
        finally
        {
            Visible = false;
        }
    }
}
