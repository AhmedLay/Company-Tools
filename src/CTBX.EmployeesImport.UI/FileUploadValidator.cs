using CTBX.CommonMudComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using FluentValidation;
using FluentValidation.Results;

namespace CTBX.EmployeesImport.UI;

public class FileUploadValidator : AbstractValidator<IBrowserFile>
{
    public FileUploadValidator()
    {
        RuleFor(file => file.Name)
            .NotEmpty().WithMessage("File name cannot be empty.");

        RuleFor(file => file.Size)
            .GreaterThan(0).WithMessage("The file you are trying to upload is empty. Please try again.")
            .LessThanOrEqualTo(1_024_000).WithMessage("The file you are trying to upload is too big. Please try again.");

        RuleFor(file => file)
            .MustAsync(ValidateFileContent).WithMessage("The file's content does not meet the required format. Each line must contain 6 columns separated by semicolons (;).");
    }

    private async Task<bool> ValidateFileContent(IBrowserFile file, CancellationToken cancellationToken)
    {
        try
        {

            using var stream = file.OpenReadStream(maxAllowedSize: 10_000_000);
            using var reader = new StreamReader(stream);

            var lines = await reader.ReadToEndAsync();

            var dataRows = lines.Split(Environment.NewLine)
                                 .Where(line => !string.IsNullOrWhiteSpace(line))
                                 .ToList();

            // Jede Zeile überprüfen
            foreach (var line in dataRows)
            {
                var columns = line.Split(';');
                if (columns.Length != 6)
                {
                    return false; 
                }
            }

            return true; 
        }
        catch
        {
            return false; 
        }
    }


}

