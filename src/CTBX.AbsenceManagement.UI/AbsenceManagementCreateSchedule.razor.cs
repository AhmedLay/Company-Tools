using System.Reflection.Metadata.Ecma335;
using CTBX.AbsenceManagement.Shared.DTOs;
using CTBX.CommonMudComponents;
using FluentValidation;
using Heron.MudCalendar;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MinimalApiArchitecture.Application.Commands;
using MudBlazor;

namespace CTBX.AbsenceManagement.UI
{
    public class VacationScheduleFileBase : BaseMudComponent
    {
        [Inject]
        public required AbsenceManagementService Service { get; set; }
        [Inject]
        public required IValidator<VacationRequest> RequestValidator { get; set; }
        public record Request(int Id, string Draftname, DateTimeOffset From, DateTimeOffset To, string AbsenceType);
        public List<VacationScheduleDTO> List { get; set; } = new();
        public List<Request> Requests { get; set; } = new();
        public bool _open = false;
        public VacationRequest CurrentRequest { get; set; } = new();
        public bool _visible = false;
        public List<DateTime> MarkedDates { get; set; } = new();
        public List<DraftsItems> _events = new();
        public void OpenDrawer()
        {
            _open = true;
        }
        public async Task SaveDraft()
        {
            if (CurrentRequest.From == null || CurrentRequest.To == null)
            {
                return;
            }
            var from = new DateTimeOffset(CurrentRequest.From.Value, TimeSpan.Zero);
            var to = new DateTimeOffset(CurrentRequest.To.Value, TimeSpan.Zero);
            var scheduledat = DateTimeOffset.UtcNow;

            if (CurrentRequest.RequestType == true)
            {
                var validationResult = await RequestValidator.ValidateAsync(CurrentRequest);
                if (!validationResult.IsValid)
                {
                    foreach (var error in validationResult.Errors)
                    {
                        await NotifyError(error.ErrorMessage);
                    }
                    return;
                }
                var id = Guid.NewGuid().ToString();
                var command = new VacationScheduleCommand(id, CurrentRequest.EmployeeId, from, to, CurrentRequest.Comment, scheduledat);
                var response = await Service.SendCommand(command);
                if (response.IsSuccessStatusCode)
                {
                    _visible = true;
                    await LoadData();
                    await NotifySuccess("Draft Saved");
                    ResetForm();
                    _open = false;
                    _visible = false;
                }
            }
        }
        public void SubmitRequest()
        {
            _open = false;
            NotifySuccess("Vacation Request sent");
            ResetForm();
        }
        protected override async Task OnInitializedAsync()
        {           
            await LoadData();
        }
        public async Task LoadData()
        {
            _visible = true;
            List = await Service.GetVacationSchedulesAsync();
            _events = await Service.GetVacationSchedulesCalenderAsync();
            _visible = false;
        }

        private void ResetForm()
        {

            CurrentRequest = new VacationRequest
            {
                EmployeeId = 0,
                From = null,
                To = null,
                Scheduledat = DateTimeOffset.MinValue,
                Comment = string.Empty,
                RequestType = true
            };
        }
        public void DeleteItem()
        {

        }
    }
}
