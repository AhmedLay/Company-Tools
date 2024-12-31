using System.ComponentModel.Design;
using CTBX.CommonMudComponents;
using Microsoft.AspNetCore.Components;
using MinimalApiArchitecture.Application.Commands;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CTBX.AbsenceManagement.UI
{
    public class VacationScheduleFileBase : BaseMudComponent
    {
        [Inject]
        public required AbsenceManagementService Service { get; set; }
        public record Request(int Id, string Draftname, DateTimeOffset From, DateTimeOffset To, string AbsenceType);
        public List<VacationScheduleCommand> List { get; set; } = new();
        public List<Request> Requests { get; set; } = new();
        public bool _open = false;
        public VacationRequest CurrentRequest { get; set; } = new();
        public void OpenDrawer()
        {
            _open = true;
        }
        public async Task SaveDraft()
        {
            if (CurrentRequest.From == null || CurrentRequest.To == null )
            {
                return;
            }
            var from = new DateTimeOffset(CurrentRequest.From.Value, TimeSpan.Zero);
            var to = new DateTimeOffset(CurrentRequest.To.Value, TimeSpan.Zero);
            //later timspan configuable ? 
            var scheduledat = DateTimeOffset.UtcNow;

            if (CurrentRequest.RequestType == true)
            {
                var id = Guid.NewGuid().ToString();
                var command = new VacationScheduleCommand(id, CurrentRequest.EmployeeId, from, to, CurrentRequest.Comment, scheduledat);
                _open = false;
                await LoadData();
                await Service.SendCommand(command);
                await NotifySuccess("Draft Saved");
            }
        }
        public void SubmitRequest()
        {
            _open = false;
            NotifySuccess("Vacation Request sent");
        }
        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }
        public async Task LoadData()
        {
            List = await Service.GetVacationSchedulesAsync() ?? new List<VacationScheduleCommand>();
        }
    }
    public class VacationRequest
    {
        public int EmployeeId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public DateTimeOffset Scheduledat { get; set; }
        public string Comment { get; set; } = string.Empty;
        public bool RequestType { get; set; } = true;
    }
}
