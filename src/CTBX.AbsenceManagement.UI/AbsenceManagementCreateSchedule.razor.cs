using System.Security.Claims;
using CTBX.AbsenceManagement.Shared;
using CTBX.CommonMudComponents;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace CTBX.AbsenceManagement.UI
{
    public class ScheduleFileBase : BaseMudComponent
    {
        [Inject]
        private AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
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
        public List<ReadModel> _events = new();
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

            var authState = await AuthenticationStateProvider!.GetAuthenticationStateAsync();
            var user = authState.User;
            var email = user.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            Console.WriteLine(email);

            if (string.IsNullOrEmpty(email))
            {
                await NotifyError("User email not found!");
                return;
            }
            var employeeId = await Service.GetEmployeeID(email);

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
            var command = new SchedulingVacation(id, employeeId, from, to, CurrentRequest.Comment, scheduledat);

            _visible = true;

            await OnHandleOperation(
                operation: async () => {
                    await Service.SetSchedule(command);
                    await Task.Delay(750);
                    await LoadVacationData();
                },
                successMssage: "Vacation Draft Saved",
                errMessage: "Something went wrong!"
            );
            _open = false;
            ResetForm();
            _visible = false;

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
                    operation: async () =>
                    await Service.SendCommandSL(command),

                    successMssage: "Sick Leave Draft Saved",
                    errMessage: "Something went wrong!"
                );

            _open = false;
            ResetForm();
            await LoadVacationData();
        }
        public async Task LoadVacationData()
        {
            try
            {
                _events = await Service.GetData();
                StateHasChanged(); 
            }
            catch (Exception ex)
            {
                await NotifyError("Failed to load vacation data: " + ex.Message);
            }
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
        public void EditDraft(ReadModel draft)
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
                operation: async () => {
                    await Service.EditVacation(command);
                    await Task.Delay(700);
                    await LoadVacationData();
                },
               successMssage: "Vacation Draft is Edited",
               errMessage: "Something went wrong!"
               );
            _open = false;
            ResetForm();

        }
        public async Task ConfirmRequest(ReadModel items)
        {
            if (items == null)
                return;

            var id = items.id;
            var requestedAt = DateTimeOffset.UtcNow;

            var command = new RequestingVacation(id, 234, requestedAt);

            await OnHandleOperation(
                operation: async () =>
                {
                    await Service.RequestVacation(command);
                    await Task.Delay(700);
                    await LoadVacationData();
                },
               successMssage: "Vacation Requested is sended!",
               errMessage: "Something went wrong!"
               );
        }
        public async Task AbondonRequest(ReadModel items)
        {
            if (items == null)
                return;

            var id = items.id;
            var abondonAt = DateTimeOffset.UtcNow;

            var command = new AbdoningRequest(id, abondonAt);

            await OnHandleOperation(
                 operation: async () =>
                 {
                     await Service.AbondonRequest(command);
                     await Task.Delay(700);
                     await LoadVacationData();
                 },
                successMssage: "Vacation Requested got Abondon!",
                errMessage: "Something went wrong!"
                );
        }
        protected override async Task OnInitializedAsync()
        {
            await LoadVacationData();
        }
    }
}
