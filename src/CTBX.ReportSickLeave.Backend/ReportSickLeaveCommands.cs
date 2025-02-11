using CTBX.ReportSickLeave.Shared;

namespace CTBX.ReportSickLeave.Backend;

public class ReportSickLeaveCommands
{
    public record RequestSickLeave(int EmployeeId, DateTime From, DateTime Until, SickLeaveStatus Status, DateTimeOffset ReportedAt);
    public record ApproveSickLeave(int Id, SickLeaveStatus Status);
    public record RejectSickLeave(int Id, SickLeaveStatus Status);
    public record DeleteSickLeave(int Id);
}
