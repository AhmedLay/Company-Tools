using CTBX.AbsenceManagement.Shared;
using CTBX.CommonMudComponents;
using FluentValidation;
using Microsoft.AspNetCore.Components;

namespace CTBX.AbsenceManagement.UI
{
    public class ScheduleFileBase : BaseMudComponent
    {
        [Inject]
        public required AbsenceManagementService Service { get; set; }
        [Inject]
        public required IValidator<RequestModel> RequestValidator { get; set; }
        public record Request(int Id, string Draftname, DateTimeOffset From, DateTimeOffset To, string AbsenceType);
        public List<Request> Requests { get; set; } = new();
        public bool _open = false;
        public RequestModel CurrentRequest { get; set; } = new();
        public bool _visible = false;
        public List<DateTime> MarkedDates { get; set; } = new();
        public List<DraftsItems> _events = new();
        public bool _isEditMode = false;
        public bool _isVacationRequest = true;

        public void OpenDrawer()
        {
            _open = true;
        }
        public void OpenSickLeaveDrawer()
        {
            _isVacationRequest = false;
            _open = true;
        }
        public async Task SaveDraft()
        {
            if (CurrentRequest.From == null || CurrentRequest.To == null)
            return;
            var from = new DateTimeOffset(CurrentRequest.From.Value, TimeSpan.Zero);
            var to = new DateTimeOffset(CurrentRequest.To.Value, TimeSpan.Zero);
            var scheduledat = DateTimeOffset.UtcNow;

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
            var command = new SchedulingVacation(id, 123, from, to, CurrentRequest.Comment, scheduledat);
            await OnHandleOperation(
                operation: async () => await Service.SetSchedule(command),
                successMssage: "Vacation Draft Saved",
                errMessage: "Something went wrong!"
                );
                _open = false;
                 ResetForm();
                 await LoadVacationData();
        }
        public async Task SaveSickLeaveDraft()
        {
            if (CurrentRequest.From == null || CurrentRequest.To == null)
            {
                return;
            }
            var from = new DateTimeOffset(CurrentRequest.From.Value, TimeSpan.Zero);
            var to = new DateTimeOffset(CurrentRequest.To.Value, TimeSpan.Zero);
            var scheduledat = DateTimeOffset.UtcNow;

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
            var command = new RequestingSickLeave(id, 123, from, to, CurrentRequest.Comment, DateTimeOffset.UtcNow);
            var response = await Service.SendCommandSL(command);
            await OnHandleOperation(
                    operation: async () => await Service.SendCommandSL(command),
                    successMssage: "Sick Leave Draft Saved",
                    errMessage: "Something went wrong!"
                );

            _open = false;
            ResetForm();
            await LoadVacationData();
        }

        public void SubmitRequest()
        {
            _open = false;
            NotifySuccess("Vacation Request sent");
            ResetForm();
        }
        protected override async Task OnInitializedAsync()
        {
            await LoadVacationData();
        }
        public async Task LoadVacationData()
        {
            _visible = true;
            _events = await Service.GetVacationSchedulesCalenderAsync();
            _visible = false;
        }
        private void ResetForm()
        {

            CurrentRequest = new RequestModel
            {
                EmployeeId = 0,
                From = null,
                To = null,
                Scheduledat = DateTimeOffset.MinValue,
                Comment = string.Empty,
            };
            _isEditMode = false;
        }
        public void EditDraft(DraftsItems draft)
        {
            _isEditMode = true;

            CurrentRequest = new RequestModel
            {
                Id = draft.id,
                EmployeeId = 123,
                From = draft.Start,
                To = draft.End,
                Comment = draft.Text,
            };
            _open = true;
        }
        public async Task UpdateDraft()
        {
            if (CurrentRequest.From == null || CurrentRequest.To == null)
            return;
            var from = new DateTimeOffset(CurrentRequest.From.Value, TimeSpan.Zero);
            var to = new DateTimeOffset(CurrentRequest.To.Value, TimeSpan.Zero);
            var editAt = DateTimeOffset.UtcNow;
            var validationResult = await RequestValidator.ValidateAsync(CurrentRequest);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    await NotifyError(error.ErrorMessage);
                }
                return;
            }
            var id = CurrentRequest.Id;
            var command = new ChangingVacationSchedule(id, 123, from, to, CurrentRequest.Comment, editAt);
            await OnHandleOperation(
               operation: async () => await Service.EditVacation(command),
               successMssage: "Vacation Draft is Edited",
               errMessage: "Something went wrong!"
               );
            _open = false;
            ResetForm();
            await LoadVacationData();

        }
        public async Task ConfirmRequest(DraftsItems items)
        {
            if (items == null)
                return;

            var id = items.id;
            var requestedAt = DateTimeOffset.UtcNow;
            var from = new DateTimeOffset(items.Start, TimeSpan.Zero);
            var to = new DateTimeOffset(items.End ?? DateTime.UtcNow, TimeSpan.Zero);

            var command = new RequestingVacation(id, 123, 234, from, to, items.Text, requestedAt);

            await OnHandleOperation(
                operation: async () => await Service.RequestVacation(command),
                successMssage: "Vacation Draft is requested",
                errMessage: "Something went wrong!"
            );
        }

    }
}
