using CTBX.ImportHolidays.Shared;
using FluentValidation;

namespace CTBX.ImportHolidays.Backend
{
    class FluentValidator : AbstractValidator<FileData>
    {
        public FluentValidator()
        {
            RuleFor(file => file.FileName)
                .NotEmpty().WithMessage("File name must not be empty.")
                .Must(fileName => fileName.EndsWith(".csv") || fileName.EndsWith(".txt"))
                .WithMessage("File name must have a valid type");

            RuleFor(file => file.FileContent)
                .NotNull().WithMessage("File content must not be null.")
                .Must(content => content.Length > 0)
                .WithMessage("File content must not be empty.");

            RuleFor(file => file.FileContent.Length)
                .LessThanOrEqualTo(1_024_000) // 1 MB limit
                .WithMessage("File size must not exceed 1 MB.");
        }
    }
}
