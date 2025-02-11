using Eventuous;

namespace CTBX.ReportSickLeave.Backend.Events;

[EventType("V1.SickLeaveDeleted")]
public record SickLeaveDeleted
{
    public int Id { get; init; }
    public DateTime DeletedAt { get; init; }
}
