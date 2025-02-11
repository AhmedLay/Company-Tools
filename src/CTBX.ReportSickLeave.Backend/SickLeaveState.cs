
using CTBX.ReportSickLeave.Shared;
using Eventuous;

namespace CTBX.ReportSickLeave.Backend;

public record SickLeaveState : State<SickLeaveState>
{
    public int Id { get; private set; }
    public int EmployeeId { get; private set; }
    public DateTime From { get; private set; }
    public DateTime Until { get; private set; }
    public SickLeaveStatus Status { get; private set; }
    public DateTime ReportedAt { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public DateTime? RejectedAt { get; private set; }

}
