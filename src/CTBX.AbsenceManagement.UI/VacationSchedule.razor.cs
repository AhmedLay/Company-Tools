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
        public List<Request> Requests { get; set; } = new();
        public bool _open = false;
        public VacationRequest CurrentRequest { get; set; } = new();

        public void OpenDrawer()
        {
            _open = true;
        }
        public async Task SaveDraft()
        {
            if (CurrentRequest.From == null || CurrentRequest.To == null || string.IsNullOrWhiteSpace(CurrentRequest.Draftname))
            {
                return;
            }
            var from = new DateTimeOffset(CurrentRequest.From.Value, TimeSpan.Zero);
            var to = new DateTimeOffset(CurrentRequest.To.Value, TimeSpan.Zero);
            var scheduledat = DateTimeOffset.UtcNow;

            if (CurrentRequest.RequestType == true)
            {
                var id = Guid.NewGuid().ToString();
                var command = new VacationScheduleCommand(id, CurrentRequest.EmployeeId, from, to, CurrentRequest.Comment, scheduledat);
                _open = false;
                await Service.SendCommand(command);
                await NotifySuccess("Draft Saved");
            }
        }
        public void SubmitRequest()
        {
            _open = false;
            NotifySuccess("Vacation Request sent");
        }
    }

    public class VacationRequest
    {
        public string Draftname { get; set; } = string.Empty;
        public int EmployeeId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public DateTimeOffset Scheduledat { get; set; }
        public string Comment { get; set; } = string.Empty;
        public bool RequestType { get; set; } = true;
    }

}
