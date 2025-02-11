using CTBX.ReportSickLeave.Shared;
using Eventuous;

namespace CTBX.ReportSickLeave.Backend.Events;

[EventType("V1.SickLeaveRequested")]
public record SickLeaveRequested
{
    public int EmployeeId { get; init; }
    public DateTime From { get; init; }
    public DateTime Until { get; init; }
    public SickLeaveStatus Status { get; init; }
    public DateTimeOffset ReportedAt { get; init; }
}
