using System.Drawing;
using CTBX.AbsenceManagement.Shared;
using CTBX.CommonMudComponents;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CTBX.AbsenceManagement.UI
{
    public class RequestManagementFileBase : BaseMudComponent
    {
        [Inject]
        public required AbsenceManagementService Service { get; set; }
        public bool _visible = false;
        public List<ReadModel> _events = new();
        public string GetColor(MudBlazor.Color color) => $"var(--mud-palette-{color.ToDescriptionString()})";
        public async Task LoadVacationData()
        {
            _visible = true;
            _events = await Service.GetData();
            _visible = false;
        }
        protected override async Task OnInitializedAsync()
        {
            await LoadVacationData();
        }

        public async Task ApproveRequest(ReadModel items)
        {
            var approvedAt = DateTimeOffset.UtcNow;
            var id = items.id;
            var employeeid = items.EmployeeID;
            var command = new ApprovingVacation(id, employeeid,123, approvedAt);
            await OnHandleOperation(
                operation: async () => {
                    await Service.ApproveVacation(command);
                    await Task.Delay(750);
                    await LoadVacationData();
                },
                successMssage: $"Request from {items.LastName} is Approved",
                errMessage: "Something went wrong!"
            );
        }
        public async Task RejectRequest(ReadModel items)
        {
            var rejectedAt = DateTimeOffset.UtcNow;
            var id = items.id;
            var command = new RejectingRequest(id, 123, rejectedAt);
            await OnHandleOperation(
                 operation: async () => {
                  await Service.RejectRequest(command);
                  await Task.Delay(750);
                  await LoadVacationData();
                  },
                 successMssage: $"Vacation Request from {items.LastName} got rejected",
                 errMessage: "Something went wrong!"
                );

            await LoadVacationData();
        }

    }
}
