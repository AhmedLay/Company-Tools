using CTBX.ReportSickLeave.Shared;
using Eventuous;

namespace CTBX.ReportSickLeave.Backend.Events;

[EventType("V1.SickLeaveRejected")]
public record SickLeaveRejected
{
    public int Id { get; init; }
    public SickLeaveStatus Status { get; init; }
    public DateTime RejectedAt { get; init; }
}
