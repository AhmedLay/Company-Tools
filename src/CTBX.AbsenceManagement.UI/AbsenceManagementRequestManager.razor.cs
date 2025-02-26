using CTBX.AbsenceManagement.Shared;
using CTBX.CommonMudComponents;
using FluentValidation;
using Microsoft.AspNetCore.Components;

namespace CTBX.AbsenceManagement.UI
{
    public class RequestManagementFileBase : BaseMudComponent
    {
        [Inject]
        public required AbsenceManagementService Service { get; set; }
        public bool _visible = false;
        public List<DraftsItems> _events = new();

        public async Task LoadVacationData()
        {
            _visible = true;
            _events = await Service.GetDataSV();
            _visible = false;
        }
        protected override async Task OnInitializedAsync()
        {
            await LoadVacationData();
        }

        public async Task ApproveRequest(DraftsItems items)
        {
            var approvedAt = DateTimeOffset.UtcNow;
            var id = items.id;
            var command = new ApprovingVacation(id,123,approvedAt);
            await OnHandleOperation(
                    operation: async () => await Service.ApproveVacation(command),
                    successMssage: "Vacation is Approved!",
                    errMessage: "Something went wrong!"
                );

            await LoadVacationData();
        }
        public async Task RejectRequest(DraftsItems items)
        {
            var rejectedAt = DateTimeOffset.UtcNow;
            var id = items.id;
            var command = new RejectingRequest(id, 123, rejectedAt);
            await OnHandleOperation(
                    operation: async () => await Service.RejectRequest(command),
                    successMssage: "Vacation got rejected!",
                    errMessage: "Something went wrong!"
                );

            await LoadVacationData();
        }

    }
}
