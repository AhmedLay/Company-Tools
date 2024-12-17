using CTBX.CommonMudComponents;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace CTBX.ImportHoliday.UI;

    public class ImportHolidaysFileBase : BaseMudComponent
    {
        [Inject]
        public required ImportHolidaysService Service { get; set; }

        [Inject]
        public new required ISnackbar Snackbar { get; set; }
        [Inject]
        public required IValidator<IBrowserFile> FileUploadValidator { get; set; }

        public List<IBrowserFile> UploadedFiles { get; set; } = new();
        public bool _visible = false;

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
            UploadedFiles.Clear();
            _visible = false;
        }
    }

