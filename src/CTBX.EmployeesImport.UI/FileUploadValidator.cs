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
            .LessThanOrEqualTo(1000000).WithMessage("The file you are trying to upload is too big. Please try again.");
    }
}

