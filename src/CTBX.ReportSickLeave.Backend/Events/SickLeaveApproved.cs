using CTBX.ReportSickLeave.Shared;
using Eventuous;

namespace CTBX.ReportSickLeave.Backend.Events;

[EventType("V1.SickLeaveApproved")]
public record SickLeaveApproved
{
    public int Id { get; init; }
    public SickLeaveStatus Status { get; init; }
    public DateTime ApprovedAt { get; init; }
}
