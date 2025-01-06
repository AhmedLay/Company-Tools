using CTBX.CommonMudComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using FluentValidation;
using FluentValidation.Results;

namespace CTBX.EmployeesImport.UI;

public class AbsenceManagementFormValidator : AbstractValidator<VacationRequest>
{
    public AbsenceManagementFormValidator()
    {
        RuleFor(form => form.From)
            .NotEmpty().WithMessage("The 'From' date cannot be empty.");

        RuleFor(form => form.To)
            .NotEmpty().WithMessage("The 'To' date cannot be empty.")
            .GreaterThan(form => form.From)
            .WithMessage("'To' must be greater than 'From'.");
    }

}

