using CTBX.CommonMudComponents;
using CTBX.ReportSickLeave.Shared;
//using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using static MudBlazor.CategoryTypes;

namespace CTBX.ReportSickLeave.UI;

    public class ReportSickLeaveBase : BaseMudComponent
{
        [Inject]
        public required ReportSickLeaveService Service { get; set; }
        
    //public required IValidator<SickLeaveReport> RequestValidator { get; set; }
        
        public List<SickLeaveReport> List { get; set; } = new();
   
        public bool _open;
        public SickLeaveReport SickRequest { get; set; } = new();
        public bool _visible;
        public List<DraftItems> _events = new();


    public void OpenDrawer()
    {
            _open = true;
    }
    public async Task SubmitRequest()
    {
        var id = Guid.NewGuid().ToString();
        var scheduledat = DateTimeOffset.UtcNow;
        var command = new Shared.ReportSickLeave(id, SickRequest.EmployeeId, SickRequest.From, SickRequest.Until, scheduledat);
        //var response = await Service.SendCommand(command);

        await OnHandleOperation(
                operation: () => Service.SendCommand(command),
                successMssage: $"Sick Leave request Submitted",
                errMessage: $"Sick Leave request failed!!!!"
            );

        
            _visible = true;
            await LoadData();
            ResetForm();
            _open = false;
            _visible = false;

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

        SickRequest = new SickLeaveReport
        {
            EmployeeId = 0,
            From = DateTime.Now,
            Until = DateTime.Now,
            ReportedAt = DateTime.Now,
        };
    }

}
